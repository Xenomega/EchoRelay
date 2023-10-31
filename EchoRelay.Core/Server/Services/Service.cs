using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Server.Storage;
using EchoRelay.Core.Server.Storage.Resources;
using System.Collections.Concurrent;
using System.Net;
using System.Net.WebSockets;

namespace EchoRelay.Core.Server.Services
{
    /// <summary>
    /// An Echo VR websocket service that handles client connections.
    /// </summary>
    public abstract class Service
    {
        #region Properties
        /// <summary>
        /// The name of the service.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The <see cref="EchoRelay.Core.Server.Server"/> which the service is operating on.
        /// </summary>
        public Server Server { get; }
        /// <summary>
        /// The persistent storage layer for the parent <see cref="Server"/>.
        /// </summary>
        public ServerStorage Storage
        {
            get
            {
                return Server.Storage;
            }
        }
        /// <summary>
        /// The <see cref="Server"/>'s <see cref="SymbolCache"/>, used to resolve symbols for various objects.
        /// </summary>
        public SymbolCache SymbolCache
        {
            get
            {
                return Server.SymbolCache;
            }
        }

        /// <summary>
        /// A list of connected peers in the service.
        /// </summary>
        public List<Peer> Peers { get; }
        /// <summary>
        /// A lock used to access or update <see cref="Peers"/>, to avoid concurrent access issues.
        /// </summary>
        public readonly object PeersLock = new object();
        /// <summary>
        /// A lookup of peer endpoints (ip address, port) -> peer.
        /// </summary>
        public ConcurrentDictionary<(IPAddress,int), Peer> PeersByEndpoint { get; }
        #endregion

        #region Events
        /// <summary>
        /// Event for a <see cref="Peer"/> connecting to a <see cref="Service"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> the <see cref="Peer"/> connected to.</param>
        /// <param name="peer">The <see cref="Peer"/> which connected.</param>
        public delegate void PeerConnectedEventHandler(Service service, Peer peer);
        /// <summary>
        /// Event for a <see cref="Peer"/> connecting to a <see cref="Service"/>.
        /// </summary>
        public event PeerConnectedEventHandler? OnPeerConnected;

        /// <summary>
        /// Event for a <see cref="Peer"/> disconnecting from a <see cref="Service"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> the <see cref="Peer"/> disconnected from.</param>
        /// <param name="peer">The <see cref="Peer"/> which disconnected.</param>
        public delegate void PeerDisconnectedEventHandler(Service service, Peer peer);
        /// <summary>
        /// Event for a <see cref="Peer"/> disconnecting from a <see cref="Service"/>.
        /// </summary>
        public event PeerDisconnectedEventHandler? OnPeerDisconnected;

        /// <summary>
        /// Event for a <see cref="Peer"/> authenticating within a <see cref="Service"/>, with a given <see cref="XPlatformId"/>.
        /// </summary>
        public event Peer.AuthenticatedEventHandler? OnPeerAuthenticated;

        /// <summary>
        /// Event for a packet being received from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        public event Peer.PacketReceivedEventHandler? OnPacketReceived;

        /// <summary>
        /// Event for a packet being sent from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        public event Peer.PacketSentEventHandler? OnPacketSent;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes the new <see cref="Service"/> with the provided arguments.
        /// </summary>
        /// <param name="name">The name of the service.</param>
        public Service(Server server, string name)
        {
            // Set the provided arguments.
            Server = server;
            Name = name;
            Peers = new List<Peer>();
            PeersByEndpoint = new ConcurrentDictionary<(IPAddress, int), Peer>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Enters a communication loop with the accepted websocket connection.
        /// </summary>
        /// <param name="webSocket">The websocket to process messages for.</param>
        internal async Task HandleConnection(HttpListenerContext context, WebSocket webSocket)
        {
            // Create a new peer from this connection and add it to our list of peers.
            Peer peer = new Peer(Server, this, context, webSocket);
            var endpoint = (peer.Address, peer.Port);
            lock (PeersLock)
            {
                Peers.Add(peer);
                PeersByEndpoint[endpoint] = peer;
            }

            // Fire the peer connected event
            OnPeerConnected?.Invoke(this, peer);

            // Subscribe to peer event handlers to forward events.
            peer.OnPeerAuthenticated += Peer_OnPeerAuthenticated;
            peer.OnPacketSent += Peer_OnPacketSent;
            peer.OnPacketReceived += Peer_OnPacketReceived;

            // Create a buffer to receive our packet data in.
            byte[] receiveBuffer = new byte[Packet.MAX_SIZE];

            // While the connection is open, continuously try to receive messages.
            try
            {
                while (webSocket.State == WebSocketState.Open)
                {
                    // Receive data from the web socket until we hit our end of message.
                    Memory<byte> receiveBufferAtPosition = receiveBuffer;
                    int totalSize = 0;
                    WebSocketMessageType messageType = WebSocketMessageType.Close;
                    while(true)
                    {
                        var receiveResult = await webSocket.ReceiveAsync(receiveBufferAtPosition, CancellationToken.None);
                        messageType = receiveResult.MessageType;
                        receiveBufferAtPosition = receiveBufferAtPosition.Slice(receiveResult.Count);
                        totalSize += receiveResult.Count;
                        if (receiveResult.EndOfMessage)
                            break;
                    }

                    // Obtain the packet buffer without the trailing unused space.
                    byte[] packetBuffer = receiveBuffer.Take(totalSize).ToArray();
                    switch (messageType)
                    {
                        case WebSocketMessageType.Binary:
                            // Parse a packet out of this message.
                            Packet packet = Packet.Decode(packetBuffer);

                            // Fire the packet received event for this service.
                            peer.InvokeReceiveEventHandler(packet);;

                            // Handle the packet
                            await HandlePacket(peer, packet);
                            break;
                        case WebSocketMessageType.Close:
                            // Close the connection gracefully.
                            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                            return;
                        default:
                            // Close the connection with an invalid message type status.
                            await webSocket.CloseAsync(WebSocketCloseStatus.InvalidMessageType, "", CancellationToken.None);
                            throw new WebSocketException("Received an unexpected websocket message type");
                    }
                }
            }
            finally
            {
                // Whether we exit gracefully or encounter an exception, remove the peer from our list of peers.
                lock (PeersLock)
                {
                    Peers.Remove(peer);
                    PeersByEndpoint.Remove(endpoint, out _);
                }

                // Fire the peer disconnected event
                OnPeerDisconnected?.Invoke(this, peer);
            }
        }

        /// <summary>
        /// Handles a packet being received by a peer.
        /// This is called after all event handlers have been fired for <see cref="OnPacketReceived"/>.
        /// </summary>
        /// <param name="sender">The peer which sent the packet.</param>
        /// <param name="packet">The packet sent by the peer.</param>
        protected abstract Task HandlePacket(Peer sender, Packet packet);
        #endregion

        #region Event Handlers
        private void Peer_OnPacketSent(Service service, Peer peer, Packet packet)
        {
            // Fire the sent event for the service.
            OnPacketSent?.Invoke(service, peer, packet);
        }

        private void Peer_OnPeerAuthenticated(Service service, Peer peer, XPlatformId userId)
        {
            // Fire the sent event for the service.
            OnPeerAuthenticated?.Invoke(service, peer, userId);
        }

        private void Peer_OnPacketReceived(Service service, Peer peer, Packet packet)
        {
            // Fire the sent event for the service.
            OnPacketReceived?.Invoke(service, peer, packet);
        }
        #endregion
    }
}
