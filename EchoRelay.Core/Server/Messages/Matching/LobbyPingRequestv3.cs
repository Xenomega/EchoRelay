using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client, requesting the client ping a set of endpoints to determine
    /// the optimal game server to connect to.
    /// </summary>
    public class LobbyPingRequestv3 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -378478809818600461;

        // TODO
        public ushort Unk0;
        public ushort Unk1;
        public uint Unk2;

        /// <summary>
        /// The endpoints which the client should be asked to ping.
        /// </summary>
        public EndpointData[] Endpoints;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPingRequestv3"/> message.
        /// </summary>
        public LobbyPingRequestv3()
        {
            Unk0 = 0;
            Unk1 = 0;
            Unk2 = 0;
            Endpoints = Array.Empty<EndpointData>();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyPingRequestv3"/> with the provided arguments.
        /// </summary>
        /// <param name="unk0">TODO: Unknown.</param>
        /// <param name="unk1">TODO: Unknown.</param>
        /// <param name="unk2">TODO: Unknown.</param>
        /// <param name="endpoints">The endpoints to provide to the client for the ping request.</param>
        public LobbyPingRequestv3(ushort unk0, ushort unk1, uint unk2, EndpointData[] endpoints)
        {
            Unk0 = unk0;
            Unk1 = unk1;
            Unk2 = unk2;
            Endpoints = endpoints;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Unk0);
            io.Stream(ref Unk1);
            io.Stream(ref Unk2);

            // If we're reading, initialize our buffer to the correct size.
            if (io.StreamMode == StreamMode.Read)
            {
                int endpointCount = (int)(io.Length - io.Position) / 12; // 10 byte struct + 2 byte padding
                Endpoints = new EndpointData[endpointCount];
            }
          
            // Stream all endpoints
            for (int i = 0; i < Endpoints.Length; i++)
            {
                // Stream data in/out. If the endpoint isn't initialized, we're likely reading, so we initialize it first.
                if (Endpoints[i] == null)
                    Endpoints[i] = new EndpointData();
                Endpoints[i].Stream(io);

                // Stream 2 bytes of padding
                ushort unused = 0;
                io.Stream(ref unused);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"unk0={Unk0}, " +
                $"unk1={Unk1}, " +
                $"unk2={Unk2}, " +
                $"endpoints=[{string.Join(", ", Endpoints.AsEnumerable())}]" +
                $")";
        }
        #endregion

        #region Classes
        /// <summary>
        /// Endpoint-related data, describing a game server peer.
        /// </summary>
        public class EndpointData : IStreamable
        {
            #region Fields
            /// <summary>
            /// The internal/private address of the endpoint.
            /// </summary>
            public IPAddress InternalAddress;
            /// <summary>
            /// The external/public address of the endpoint.
            /// </summary>
            public IPAddress ExternalAddress;
            /// <summary>
            /// The port used by the endpoint.
            /// </summary>
            public ushort Port;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new <see cref="EndpointData"/>.
            /// </summary>
            public EndpointData()
            {
                InternalAddress = new IPAddress(0);
                ExternalAddress = new IPAddress(0);
                Port = 0;
            }
            /// <summary>
            /// Initializes a new <see cref="EndpointData"/> with the provided arguments.
            /// </summary>
            /// <param name="internalAddress">The internal/private address of the endpoint.</param>
            /// <param name="externalAddress">The external/public address of the endpoint.</param>
            /// <param name="port">The port used by the endpoint.</param>
            public EndpointData(IPAddress internalAddress, IPAddress externalAddress, ushort port)
            {
                InternalAddress = internalAddress;
                ExternalAddress = externalAddress;
                Port = port;
            }
            #endregion

            #region Functions
            public void Stream(StreamIO io)
            {
                // Network byte order is used here (big endian).
                io.Stream(ref InternalAddress, ByteOrder.BigEndian);
                io.Stream(ref ExternalAddress, ByteOrder.BigEndian);
                io.Stream(ref Port, ByteOrder.BigEndian);
            }

            public override string ToString()
            {
                return $"{GetType().Name}(int_ip={InternalAddress}, ext_ip={ExternalAddress}, port={Port})";
            }
            #endregion
        }
        #endregion
    }
}
