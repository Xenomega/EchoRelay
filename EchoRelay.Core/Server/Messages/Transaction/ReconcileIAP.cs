using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages.Login;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Transaction
{
    /// <summary>
    /// TODO: In-app purchase related request
    /// </summary>
    public class ReconcileIAP : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 2004379208746620732;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        /// <summary>
        /// The identifier of the associated user.
        /// </summary>
        public XPlatformId UserId;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoginSuccess"/> message.
        /// </summary>
        public ReconcileIAP()
        {
            UserId = new XPlatformId();
            Session = new Guid();
        }
        /// <summary>
        /// Initializes a new <see cref="ReconcileIAP"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The identifier of the associated user.</param>
        /// <param name="session">The session token granted for the user.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the session token is not the correct length.</exception>
        public ReconcileIAP(XPlatformId userId, Guid session)
        {
            UserId = userId;
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
            UserId.Stream(io);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, session={Session})";
        }
        #endregion
    }
}
