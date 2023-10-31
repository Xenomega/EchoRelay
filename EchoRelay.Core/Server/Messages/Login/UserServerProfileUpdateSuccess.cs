using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to client indicating their <see cref="UserServerProfileUpdateRequest"/> request succeeded.
    /// </summary>
    public class UserServerUpdateProfileSuccess : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -3271451319295304587;

        /// <summary>
        /// The identifier of the associated user.
        /// </summary>
        public XPlatformId UserId;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="UserServerUpdateProfileSuccess"/> message.
        /// </summary>
        public UserServerUpdateProfileSuccess()
        {
            UserId = new XPlatformId();
        }
        /// <summary>
        /// Initializes a new <see cref="UserServerUpdateProfileSuccess"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The identifier of the associated user.</param>
        public UserServerUpdateProfileSuccess(XPlatformId userId)
        {
            UserId = userId;
        }


        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            UserId.Stream(io);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId})";
        }
        #endregion
    }
}
