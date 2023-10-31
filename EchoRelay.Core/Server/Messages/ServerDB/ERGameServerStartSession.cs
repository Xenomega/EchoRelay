using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating it should start a session on its end.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerStartSession : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770000;

        /// <summary>
        /// The identifier for the game server session to start.
        /// </summary>
        public Guid SessionId;

        /// <summary>
        /// TODO: Unknown. In offline play, this is zero.
        /// </summary>
        public Guid Unk0;

        /// <summary>
        /// The maximum amount of players allowed to join the lobby.
        /// </summary>
        public byte PlayerLimit;

        /// <summary>
        /// TODO: Unknown, this is some count for a struct that follows the JSON. The size per each is 0x24.
        /// </summary>
        public byte Unk2;

        private byte _lobbyType;
        /// <summary>
        /// The type of lobby
        /// </summary>
        public LobbyType Type
        {
            get
            {
                return (LobbyType)_lobbyType;
            }
            set
            {
                _lobbyType = (byte)value;
            }
        }

        /// <summary>
        /// The JSON settings associated with the session.
        /// </summary>
        public SessionSettings Settings;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerStartSession"/> message.
        /// </summary>
        public ERGameServerStartSession()
        {
            Settings = new SessionSettings();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerStartSession"/> with the provided arguments.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="playerLimit"></param>
        /// <param name="lobbyType"></param>
        /// <param name="settings"></param>
        public ERGameServerStartSession(Guid sessionId, Guid unk0, byte playerLimit, LobbyType type, SessionSettings settings)
        {
            SessionId = sessionId;
            Unk0 = unk0;
            PlayerLimit = playerLimit;
            Type = type;
            Settings = settings;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            byte pad1 = 0;

            io.Stream(ref SessionId);
            io.Stream(ref Unk0);
            io.Stream(ref PlayerLimit);
            io.Stream(ref Unk2);
            io.Stream(ref _lobbyType);
            io.Stream(ref pad1);
            io.StreamJSON(ref Settings, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(session_id={SessionId}, player_limit={PlayerLimit}, lobby_type={Type}, settings={Settings})";
        }
        #endregion

        #region Classes
        /// <summary>
        /// The type of lobby for the created session.
        /// </summary>
        public enum LobbyType : int
        {
            Public = 0x0,
            Private = 0x1,
            Unassigned = 0x2,
        } 

        /// <summary>
        /// Session settings for the session to be started by a <see cref="ERGameServerStartSession"/> message.
        /// </summary>
        public class SessionSettings
        {
            /// <summary>
            /// Identifier of the application.
            /// </summary>
            [JsonProperty("appid")]
            public string? AppId { get; set; }

            /// <summary>
            /// A symbol representing the gametype of the session to create.
            /// </summary>
            [JsonProperty("gametype")]
            public long? GameType { get; set; }

            /// <summary>
            /// A symbol representing the level of the session to create.
            /// </summary>
            [JsonProperty("level")]
            public long? Level { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            public SessionSettings()
            {

            }
            public SessionSettings(string? appId, long? gametype, long? level, IDictionary<string, JToken>? additionalData = null)
            {
                AppId = appId;
                GameType = gametype;
                Level = level;
                AdditionalData = additionalData ?? AdditionalData;
            }

            public override string ToString()
            {
                return $"<appid={AppId}, gametype={GameType}, level={Level}>";
            }
        }
        #endregion
    }
}
