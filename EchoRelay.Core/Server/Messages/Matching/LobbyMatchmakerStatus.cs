using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to the client, providing the status of a previously sent <see cref="LobbyMatchmakerStatusRequest"/>.
    /// </summary>
    public class LobbyMatchmakerStatus : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -8131021305597149493;

        /// <summary>
        /// The status code representing the matchmaker's status creating/finding/joining a session.
        /// </summary>
        public uint StatusCode;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyMatchmakerStatus"/> message.
        /// </summary>
        public LobbyMatchmakerStatus()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyMatchmakerStatus"/> message with the provided arguments.
        /// </summary>
        /// <param name="statusCode">The status code associated with the matchmaker status.</param>
        public LobbyMatchmakerStatus(uint statusCode)
        {
            StatusCode = statusCode;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref StatusCode);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(status={StatusCode})";
        }
        #endregion
    }
}
