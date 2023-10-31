using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to server, requesting the status of a pending matchmaking operation.
    /// </summary>
    public class LobbyMatchmakerStatusRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 1336293084088743504;

        /// <summary>
        /// An unused byte sent with the message.
        /// </summary>
        public byte Unused;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyMatchmakerStatusRequest"/> message.
        /// </summary>
        public LobbyMatchmakerStatusRequest()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyMatchmakerStatusRequest"/> message with the provided arguments.
        /// </summary>
        /// <param name="unused">An unused byte in the request</param>
        public LobbyMatchmakerStatusRequest(byte unused)
        {
            Unused = unused;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Unused);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(unused={Unused})";
        }
        #endregion
    }
}
