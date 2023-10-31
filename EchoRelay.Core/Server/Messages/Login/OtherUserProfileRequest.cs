using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting the user profile for another user.
    /// </summary>
    public class OtherUserProfileRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 1310854393570331826;

        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The request data for the underlying profile, indicating fields of interest.
        /// </summary>
        public JObject ProfileRequestData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="OtherUserProfileRequest"/> message.
        /// </summary>
        public OtherUserProfileRequest()
        {
            UserId = new XPlatformId();
            ProfileRequestData = new JObject();
        }
        /// <summary>
        /// Initializes a new <see cref="OtherUserProfileRequest"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The identifier of the user requesting another user's profile.</param>
        /// <param name="profileRequestData">The request data for the underlying profile, indicating fields of interest.</param>
        public OtherUserProfileRequest(XPlatformId userId, JObject profileRequestData)
        {
            UserId = userId;
            ProfileRequestData = profileRequestData;
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
            io.StreamJSON(ref ProfileRequestData, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, profile_request={ProfileRequestData.ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
