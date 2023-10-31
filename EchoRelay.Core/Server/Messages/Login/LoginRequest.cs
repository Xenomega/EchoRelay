using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting for a user sign-in.
    /// </summary>
    public class LoginRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -4777159589668118518;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The client account information supplied for the sign-in request.
        /// </summary>
        public LoginAccountInfo AccountInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoginRequest"/> message.
        /// </summary>
        public LoginRequest()
        {
            AccountInfo = new LoginAccountInfo();
            Session = new Guid();
            UserId = new XPlatformId();
        }
        /// <summary>
        /// Initializes a new <see cref="LoginRequest"/> message with the provided arguments.
        /// </summary>
        /// <param name="session">A session token which the user may have from an existing session.</param>
        /// <param name="userId">The identifier of the user requesting log-in.</param>
        /// <param name="accountData">The account data supplied with the sign-in request.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the session token is not the correct length.</exception>
        public LoginRequest(Guid session, XPlatformId userId, LoginAccountInfo accountData)
        {
            Session = session;
            UserId = userId;
            AccountInfo = accountData;
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
            io.StreamJSON(ref AccountInfo, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(session={Session}, user_id={UserId}, account_data={JObject.FromObject(AccountInfo).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion

        #region Classes
        /// <summary>
        /// A structure containing information about the client, provided for authentication.
        /// </summary>
        public class LoginAccountInfo
        {
            /// <summary>
            /// The account identifier.
            /// </summary>
            [JsonProperty("accountid")]
            public ulong AccountId { get; set; }

            /// <summary>
            /// The user's display name.
            /// </summary>
            [JsonProperty("displayname")]
            public string? DisplayName { get; set; }

            /// <summary>
            /// TODO: Unknown
            /// </summary>
            [JsonProperty("bypassauth")]
            public bool? BypassAuth { get; set; } = false;

            /// <summary>
            /// Oculus-related access token for authentication.
            /// </summary>
            [JsonProperty("access_token")]
            public string? AccessToken { get; set; }

            /// <summary>
            /// Authentication-related nonce.
            /// </summary>
            [JsonProperty("nonce")]
            public string? Nonce { get; set; }

            /// <summary>
            /// Client build version.
            /// </summary>
            [JsonProperty("buildversion")]
            public long BuildVersion { get; set; }

            /// <summary>
            /// The lobby build timestamp.
            /// </summary>
            [JsonProperty("lobbyversion")]
            public ulong? LobbyVersion { get; set; }

            /// <summary>
            /// The identifier for the application.
            /// </summary>
            [JsonProperty("appid")]
            public ulong? AppId { get; set; }

            /// <summary>
            /// An environment lock for different sandboxes.
            /// </summary>
            [JsonProperty("publisher_lock")]
            public string? PublisherLock { get; set; } = "rad15_live";

            /// <summary>
            /// Headset serial number
            /// </summary>
            [JsonProperty("hmdserialnumber")]
            public string? HMDSerialNumber { get; set; }

            /// <summary>
            /// Requested version for clients to receive their profile in.
            /// </summary>
            [JsonProperty("desiredclientprofileversion")]
            public long? DesiredClientProfileVersion { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion
    }
}
