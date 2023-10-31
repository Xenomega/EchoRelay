using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting the server update the user's client profile.
    /// </summary>
    public class UserServerProfileUpdateRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -3271750463532589966;

        /// <summary>
        /// The user identifier for the user to update the server profile for.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The server profile update information.
        /// </summary>
        public ServerProfileUpdateInfo UpdateInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="UserServerProfileUpdateRequest"/> message.
        /// </summary>
        public UserServerProfileUpdateRequest()
        {
            UserId = new XPlatformId();
            UpdateInfo = new ServerProfileUpdateInfo();
        }
        /// <summary>
        /// Initializes a new <see cref="UserServerProfileUpdateRequest"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The identifier of the logged-in user requesting their profile.</param>
        /// <param name="serverUpdateInfo">The profile data requesting to be updated on the server.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the session token is not the correct length.</exception>
        public UserServerProfileUpdateRequest(XPlatformId userId, ServerProfileUpdateInfo serverUpdateInfo)
        {
            UserId = userId;
            UpdateInfo = serverUpdateInfo;
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
            io.StreamJSON(ref UpdateInfo, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, update_info={JObject.FromObject(UpdateInfo).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion

        #region Classes
        /// <summary>
        /// A structure containing information about the server profile information to be updated.
        /// </summary>
        public class ServerProfileUpdateInfo
        {
            /// <summary>
            /// The identifier of the current session from which the update was requested.
            /// </summary>
            [JsonProperty("sessionid")]
            public string? SessionId { get; set; }

            /// <summary>
            /// The game type associated with the session.
            /// </summary>
            [JsonProperty("matchtype")]
            public string? MatchType { get; set; }

            /// <summary>
            /// The server profile components to be updated (merged into the full server profile).
            /// </summary>
            [JsonProperty("update")]
            public JObject? Update { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion
    }
}
