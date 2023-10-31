using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to server, providing the results of a ping request.
    /// This tells the server which game servers are optimal for the client.
    /// </summary>
    public class LobbyPingResponse : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 6937742467394678351;

        /// <summary>
        /// The endpoints which the client should be asked to ping.
        /// </summary>
        public EndpointPingResult[] Results;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPingResponse"/> message.
        /// </summary>
        public LobbyPingResponse()
        {
            Results = Array.Empty<EndpointPingResult>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            // If we're reading, initialize our buffer to the correct size.
            if (io.StreamMode == StreamMode.Read)
            {
                ulong resultCount = io.ReadUInt64();
                Results = new EndpointPingResult[resultCount];
            }
            else
            {
                io.Write((ulong)Results.Length);
            }

            // Stream all results
            for (int i = 0; i < Results.Length; i++)
            {
                // Stream data in/out. If the endpoint isn't initialized, we're likely reading, so we initialize it first.
                if (Results[i] == null)
                    Results[i] = new EndpointPingResult();
                Results[i].Stream(io);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(results=[{string.Join(", ", Results.AsEnumerable())}])";
        }
        #endregion

        #region Classes
        /// <summary>
        /// The result of a <see cref="LobbyPingRequestv3"/> to an individual game server.
        /// </summary>
        public class EndpointPingResult : IStreamable
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
            /// The ping time the client took to reach the game server, in milliseconds.
            /// </summary>
            public uint PingMilliseconds;
            #endregion

            #region Constructor
            /// <summary>
            /// Initializes a new <see cref="EndpointPingResult"/>.
            /// </summary>
            public EndpointPingResult()
            {
                InternalAddress = new IPAddress(0);
                ExternalAddress = new IPAddress(0);
                PingMilliseconds = 0;
            }
            #endregion

            #region Functions
            public void Stream(StreamIO io)
            {
                // Network byte order is used here (big endian).
                io.Stream(ref InternalAddress, ByteOrder.BigEndian);
                io.Stream(ref ExternalAddress, ByteOrder.BigEndian);
                io.Stream(ref PingMilliseconds);
            }

            public override string ToString()
            {
                return $"{GetType().Name}(int_ip={InternalAddress}, ext_ip={ExternalAddress}, ping_ms={PingMilliseconds})";
            }
            #endregion
        }
        #endregion
    }
}
