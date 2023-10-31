using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from game server to server, requesting game server registration so clients can match with it.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerRegistrationRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777777777;

        /// <summary>
        /// The identifier of the game server.
        /// </summary>
        public ulong ServerId;
        /// <summary>
        /// The internal/private IP address of the game server.
        /// </summary>
        public IPAddress InternalAddress;
        /// <summary>
        /// The UDP port that the game server is broadcasting on.
        /// </summary>
        public ushort Port;
        /// <summary>
        /// A symbol indicating the region of the server.
        /// </summary>
        public long RegionSymbol;
        /// <summary>
        /// The version of the server, prevents mismatches in matching.
        /// </summary>
        public long VersionLock;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerRegistrationRequest"/> message.
        /// </summary>
        public ERGameServerRegistrationRequest()
        {
            InternalAddress = new IPAddress(0);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref ServerId);
            io.Stream(ref InternalAddress, ByteOrder.BigEndian);
            io.Stream(ref Port);
            ushort pad2 = 0; // 2 bytes of padding
            io.Stream(ref pad2);
            io.Stream(ref RegionSymbol);
            io.Stream(ref VersionLock);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"server_id={ServerId}, " +
                $"internal_ip={InternalAddress}, " +
                $"port={Port}, " +
                $"region={RegionSymbol}, " +
                $"version_lock={VersionLock}" +
                $")";
        }
        #endregion
    }
}
