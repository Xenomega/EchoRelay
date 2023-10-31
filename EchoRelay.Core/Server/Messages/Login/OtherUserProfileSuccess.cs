using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to the client indicating a <see cref="OtherUserProfileRequest"/> succeeded.
    /// It contains profile information about the requested user.
    /// </summary>
    public class OtherUserProfileSuccess : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 1310555403549215925;

        /// <summary>
        /// The user identifier associated with the logged-in profile.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The requested other user profile data to return to the client.
        /// </summary>
        public AccountResource.AccountServerProfile Profile;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="OtherUserProfileSuccess"/> message.
        /// </summary>
        public OtherUserProfileSuccess()
        {
            UserId = new XPlatformId();
            Profile = new AccountResource.AccountServerProfile();
        }
        /// <summary>
        /// Initializes a new <see cref="OtherUserProfileSuccess"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The user identifier associated with the requested profile.</param>
        /// <param name="profile">The requested other user profile data to return to the client.</param>
        public OtherUserProfileSuccess(XPlatformId userId, AccountResource.AccountServerProfile profile)
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
