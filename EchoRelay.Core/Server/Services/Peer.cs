using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages;
using EchoRelay.Core.Utils;
using System.Net;
using System.Net.WebSockets;

namespace EchoRelay.Core.Server.Services
{
    /// <summary>
    /// A peer which has established a connection with the websocket server and begun engaging with a <see cref="Service"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Peer
    {
        #region Properties
        /// <summary>
        /// The <see cref="EchoRelay.Core.Server.Server"/> which the peer is connected to.
        /// </summary>
        public Server Server { get; }
        /// <summary>
        /// The <see cref="Services.Service"/> which the peer is connected to.
        /// </summary>
        public Service Service { get; }
        /// <summary>
        /// The websocket connection with the peer.
        /// </summary>
        internal WebSocket Connection { get; }
        /// <summary>
        /// Indicates whether the peer is still connected.
        /// </summary>
        public bool Connected
        {
            get
            {
                return Connection.State == WebSocketState.Open;
            }
        }
        /// <summary>
        /// Session data associated with a given <see cref="Service"/>.
        /// </summary>
        private object? _sessionData;

        /// <summary>
        /// The remote address of the connected peer.
        /// </summary>
        public IPAddress Address { get; }
        /// <summary>
        /// The remote TCP port of the connected peer.
        /// </summary>
        public ushort Port { get; }
        /// <summary>
        /// The URI of the request made to connect to the <see cref="Service"/>.
        /// </summary>
        public Uri RequestUri { get; }
        /// <summary>
        /// A unique identifier for the peer on this service.
        /// </summary>
        public string Id { get; }
        /// <summary>
        /// Thread synchronization to avoid multiple sends at the same time, which will throw an exception.
        /// </summary>
        private AsyncLock _sendLock;

        /// <summary>
        /// The user identifier for the peer. This is set when the peer authenticates to a
        /// user account. Otherwise, it is null.
        /// </summary>
        public XPlatformId? UserId { get; private set; }
        /// <summary>
        /// The user display name for the peer. This is set when the peer authenticates to a
        /// user account. Otherwise, it is null.
        /// </summary>
        public string? UserDisplayName { get; private set; }
        #endregion

        #region Events
        /// <summary>
        /// Event for a <see cref="Peer"/> authenticating within a <see cref="Service"/>, with a given <see cref="XPlatformId"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> the <see cref="Peer"/> authenticated to.</param>
        /// <param name="peer">The <see cref="Peer"/> which authenticated itself.</param>
        public delegate void AuthenticatedEventHandler(Service service, Peer peer, XPlatformId userId);
        /// <summary>
        /// Event for a <see cref="Peer"/> authenticating within a <see cref="Service"/>, with a given <see cref="XPlatformId"/>.
        /// </summary>
        public event AuthenticatedEventHandler? OnPeerAuthenticated;

        /// <summary>
        /// Event for a packet being received from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> the <see cref="Peer"/> connected to.</param>
        /// <param name="peer">The <see cref="Peer"/> which sent the <see cref="Packet"/>.</param>
        /// <param name="packet">The <see cref="Packet"/> set by the <see cref="Peer"/>.</param>
        public delegate void PacketReceivedEventHandler(Service service, Peer peer, Packet packet);
        /// <summary>
        /// Event for a packet being received from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        public event PacketReceivedEventHandler? OnPacketReceived;
        /// <summary>
        /// Event for a packet being sent from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        /// <param name="service">The <see cref="Service"/> the <see cref="Peer"/> connected to.</param>
        /// <param name="peer">The <see cref="Peer"/> which is being sent the <see cref="Packet"/>.</param>
        /// <param name="packet">The <see cref="Packet"/> set by the <see cref="Peer"/>.</param>
        public delegate void PacketSentEventHandler(Service service, Peer peer, Packet packet);
        /// <summary>
        /// Event for a packet being sent from a <see cref="Peer"/> connected to the <see cref="Service"/>.
        /// </summary>
        public event PacketSentEventHandler? OnPacketSent;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a <see cref="Peer"/> with the provided arguments.
        /// </summary>
        /// <param name="context">The <see cref="HttpListenerContext"/> used to accept the connection request.</param>
        public Peer(Server server, Service service, HttpListenerContext context, WebSocket connection)
        {
            // Set our provided arguments.
            Server = server;
            Service = service;
            Address = context.Request.RemoteEndPoint.Address;
            Port = (ushort)context.Request.RemoteEndPoint.Port;
            RequestUri = context.Request.Url!;
            Connection = connection;
            Id = $"{service.Name}:{Address}:{Port}";
            _sessionData = null;
            _sendLock = new AsyncLock();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Obtains the session data of the provided type for the peer. It must match the type of data used in <see cref="SetSessionData{T}(T)"/>.
        /// </summary>
        /// <typeparam name="T">The type of the session data to obtain.</typeparam>
        /// <returns>Returns the session data as the requested type.</returns>
        public T? GetSessionData<T>()
        {
            // Obtain the potentially null session data.
            if (_sessionData == null)
                return default;
            return (T?)_sessionData;
        }
        /// <summary>
        /// Sets session data for the provided peer.
        /// </summary>
        /// <typeparam name="T">The type of the session data to set.</typeparam>
        /// <param name="sessionData">The session data to set.</param>
        public void SetSessionData<T>(T? sessionData)
        {
            // Set session data.
            _sessionData = sessionData;
        }
        /// <summary>
        /// Clear session data for the provided peer.
        /// </summary>
        /// <typeparam name="T">The type of the session data to clear.</typeparam>
        /// <param name="sessionData">The session data to clear.</param>
        public void ClearSessionData()
        {
            // Clear session data.
            _sessionData = null;
        }

        /// <summary>
        /// Updates the peer with a user identifier which they were authenticated to, on their current service.
        /// This triggers relevant event handlers in the peer and service if the user identifier was changed by this method.
        /// </summary>
        /// <param name="userId">The user identifier which the peer was authenticated to.</param>
        /// <param name="userDisplayName">The display name for the user account authenticated to.</param>
        public void UpdateUserAuthentication(XPlatformId userId, string userDisplayName)
        {
            // Determine if we're changing the state of the user id.
            bool changed = false;
            if (UserId != userId || UserDisplayName != userDisplayName)
                changed = true;

            // Set the user id and display name.
            UserId = userId;
            UserDisplayName = userDisplayName;

            // If the platform id changed
            if (changed)
                OnPeerAuthenticated?.Invoke(Service, this, UserId);
        }

        /// <summary>
        /// Sends a provided packet to the peer through the websocket.
        /// </summary>
        /// <param name="messages">The messages to wrap in a packet to send to the peer.</param>
        /// <returns>A task representing the send operation state.</returns>
        public async Task Send(params Message[] messages)
        {
            // Wrap the messages in a packet and send it.
            await Send(new Packet(messages));
        }
        /// <summary>
        /// Sends a provided packet to the peer through the websocket.
        /// </summary>
        /// <param name="packet">The packet to send to the peer.</param>
        /// <returns>A task representing the send operation state.</returns>
        public async Task Send(Packet packet)
        {
            // Send the provided packet through the client websocket connection.
            await _sendLock.ExecuteLocked(async() => {
                await Connection.SendAsync(new ArraySegment<byte>(packet.Encode()), WebSocketMessageType.Binary, true, CancellationToken.None);
            });

            // Fire the packet sent event.
            OnPacketSent?.Invoke(Service, this, packet);
        }
        /// <summary>
        /// Receives a provided packet and fires relevant event handlers.
        /// </summary>
        /// <param name="packet">The packet received from the peer.</param>
        internal void InvokeReceiveEventHandler(Packet packet)
        {
            // Fire the packet received event.
            OnPacketReceived?.Invoke(Service, this, packet);
        }
        #endregion
    }
}
