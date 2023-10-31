using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to the server, indicating intent to cancel pending matchmaker operations.
    /// </summary>
    public class LobbyPendingSessionCancel : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -8238795091130540074;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPendingSessionCancel"/> message.
        /// </summary>
        public LobbyPendingSessionCancel()
        {
            Session = new Guid();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyPendingSessionCancel"/> message with the provided arguments.
        /// </summary>
        /// <param name="session">The user's session token.</param>
        public LobbyPendingSessionCancel(Guid session)
        {
            Session = session;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Session);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(session={Session})";
        }
        #endregion
    }
}
