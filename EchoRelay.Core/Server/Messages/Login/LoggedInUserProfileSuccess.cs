using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to the client indicating a <see cref="LoggedInUserProfileRequest"/> succeeded.
    /// It contains profile information about the logged-in user.
    /// </summary>
    public class LoggedInUserProfileSuccess : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -327009806726689417;

        /// <summary>
        /// The user identifier associated with the logged-in profile.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The requested logged-in user profile data to return to the client.
        /// </summary>
        public AccountResource.AccountProfile Profile;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoggedInUserProfileSuccess"/> message.
        /// </summary>
        public LoggedInUserProfileSuccess()
        {
            UserId = new XPlatformId();
            Profile = new AccountResource.AccountProfile();
        }
        /// <summary>
        /// Initializes a new <see cref="LoggedInUserProfileSuccess"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The user identifier associated with the logged-in profile.</param>
        /// <param name="profile">The requested logged-in user profile data to return to the client.</param>
        public LoggedInUserProfileSuccess(XPlatformId userId, AccountResource.AccountProfile profile)
        {
            UserId = userId;
            Profile = profile;
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
            io.StreamJSON(ref Profile, true, JSONCompressionMode.Zstd);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, profile={JObject.FromObject(Profile).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
