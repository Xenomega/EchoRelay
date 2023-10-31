using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating a game server registration request had succeeded.
    /// </summary>
    public class LobbyRegistrationSuccess : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -5369924845641990433;

        /// <summary>
        /// The identifier of the game server.
        /// </summary>
        public ulong ServerId;
        /// <summary>
        /// The external/public IP address of the game server.
        /// </summary>
        public IPAddress ExternalAddress;

        public ulong Unk0;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyRegistrationSuccess"/> message.
        /// </summary>
        public LobbyRegistrationSuccess()
        {
            ExternalAddress = new IPAddress(0);
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyRegistrationSuccess"/> with the provided arguments.
        /// </summary>
        /// <param name="serverId">The identifier of the game server.</param>
        /// <param name="externalAddr">The external/public IP address of the game server.</param>
        public LobbyRegistrationSuccess(ulong serverId, IPAddress externalAddr)
        {
            ServerId = serverId;
            ExternalAddress = externalAddr;
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
            io.Stream(ref ExternalAddress);
            io.Stream(ref Unk0);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(server_id={ServerId}, external_ip={ExternalAddress})";
        }
        #endregion
    }
}
