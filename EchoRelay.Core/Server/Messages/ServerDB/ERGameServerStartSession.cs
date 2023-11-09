using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating it should start a session on its end.
    /// NOTE: This is an unofficial message created for Echo Relay. It mimicks the LobbyStartSessionv4 game server broadcaster message.
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
        /// TODO: Unverified, suspected to be channel UUID.
        /// </summary>
        public Guid Channel;

        /// <summary>
        /// The maximum amount of players allowed to join the lobby.
        /// </summary>
        public byte PlayerLimit;

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

        /// <summary>
        /// Information regarding entrants (e.g. including offline/local player ids, or AI bot platform ids).
        /// </summary>
        public EntrantDescriptor[] EntrantDescriptors;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerStartSession"/> message.
        /// </summary>
        public ERGameServerStartSession()
        {
            Settings = new SessionSettings();
            EntrantDescriptors = Array.Empty<EntrantDescriptor>();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerStartSession"/> with the provided arguments.
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="playerLimit"></param>
        /// <param name="lobbyType"></param>
        /// <param name="settings"></param>
        public ERGameServerStartSession(Guid sessionId, Guid channel, byte playerLimit, LobbyType type, SessionSettings settings, EntrantDescriptor[] entrantDescriptors = null)
        {
            SessionId = sessionId;
            Channel = channel;
            PlayerLimit = playerLimit;
            Type = type;
            Settings = settings;
            EntrantDescriptors = entrantDescriptors ?? Array.Empty<EntrantDescriptor>();
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            byte finalStructCount = (byte)EntrantDescriptors.Length;
            byte pad1 = 0;

            // Read our data until the end of the session JSON.
            io.Stream(ref SessionId);
            io.Stream(ref Channel);
            io.Stream(ref PlayerLimit);
            io.Stream(ref finalStructCount);
            io.Stream(ref _lobbyType);
            io.Stream(ref pad1);
            io.StreamJSON(ref Settings, true, JSONCompressionMode.None);

            // If we're reading, make sure our array is the correct size to stream into.
            if (io.StreamMode == StreamMode.Read)
                EntrantDescriptors = new EntrantDescriptor[finalStructCount];

            // Stream the remaining data
            for(int i = 0; i < EntrantDescriptors.Length; i++)
            {
                // If we're reading, make sure we have a valid item.
                if (io.StreamMode == StreamMode.Read)
                    EntrantDescriptors[i] = new EntrantDescriptor();
                EntrantDescriptors[i].Stream(io);
            }
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

        public class EntrantDescriptor : IStreamable
        {
            /// <summary>
            /// TODO: Unknown, maybe a player session UUID?
            /// </summary>
            public Guid Unk0;
            /// <summary>
            /// The player identifier which the entrant descriptor describes.
            /// </summary>
            public XPlatformId PlayerId;
            /// <summary>
            /// TODO: Unknown flags.
            /// Observed to be 0x0144BB8000 for player, 0x0044BB8000 for bot player id in public AI match.
            /// </summary>
            public ulong Flags;

            /// <summary>
            /// Creates a random bot entrant descriptor.
            /// </summary>
            public EntrantDescriptor()
            {
                Unk0 = SecureGuidGenerator.Generate();
                PlayerId = new XPlatformId(PlatformCode.BOT, BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8)));
                Flags = 0x0044BB8000;
            }

            /// <summary>
            /// Creates an entrant desciptor with the provided arguments.
            /// </summary>
            /// <param name="unk0">TODO: Unknown</param>
            /// <param name="playerId">The player identifier which the entrant descriptor corresponds to.</param>
            /// <param name="flags">TODO: Some unknown flags</param>
            public EntrantDescriptor(Guid unk0, XPlatformId playerId, ulong flags)
            {
                Unk0 = unk0;
                PlayerId = playerId;
                Flags = flags;
            }

            public void Stream(StreamIO io)
            {
                io.Stream(ref Unk0);
                PlayerId.Stream(io);
                io.Stream(ref Flags);
            }
        }
        #endregion
    }
}
