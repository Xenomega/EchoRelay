using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages.ServerDB;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to server requesting finding of an existing game session that
    /// matches the message's underlying arguments.
    /// </summary>
    public class LobbyFindSessionRequestv11 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 3543253192791466997;

        /// <summary>
        /// The version of the client, prevents mismatches in matching.
        /// </summary>
        public ulong VersionLock;
        /// <summary>
        /// A symbol representing the gametype requested for the session.
        /// </summary>
        public long GameTypeSymbol;
        /// <summary>
        /// A symbol representing the level requested for the session.
        /// </summary>
        public long LevelSymbol;
        /// <summary>
        /// A symbol representing the platform requested for the session.
        /// </summary>
        public long PlatformSymbol;
        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;

        // TODO
        public ulong Unk1;
        public UInt128 Unk2;

        /// <summary>
        /// The channel requested for the session.
        /// </summary>
        public Guid ChannelUUID;
        /// <summary>
        /// The session information supplied for the request.
        /// </summary>
        public ERGameServerStartSession.SessionSettings SessionSettings;
        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;

        /// <summary>
        /// The team index the player is requesting to join.
        /// </summary>
        public short TeamIndex;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyFindSessionRequestv11"/> message.
        /// </summary>
        public LobbyFindSessionRequestv11()
        {
            VersionLock = 0;
            GameTypeSymbol = -1;
            LevelSymbol = -1;
            PlatformSymbol = -1;
            Session = new Guid();
            Unk1 = 0;
            Unk2 = 0;
            ChannelUUID = new Guid();
            SessionSettings = new ERGameServerStartSession.SessionSettings();
            UserId = new XPlatformId();
            TeamIndex = -1;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref VersionLock);
            io.Stream(ref GameTypeSymbol);
            io.Stream(ref LevelSymbol);
            io.Stream(ref PlatformSymbol);
            io.Stream(ref Session);
            io.Stream(ref Unk1);
            io.Stream(ref Unk2);
            io.Stream(ref ChannelUUID);
            io.StreamJSON(ref SessionSettings, true, JSONCompressionMode.None);
            UserId.Stream(io);

            // TODO: Figure this out properly
            if (io.StreamMode == StreamMode.Read)
            {
                if (io.Length - io.Position >= 2)
                    TeamIndex = io.ReadInt16();
            }
            else
            {
                if (TeamIndex != -1)
                    io.Write(TeamIndex);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"version_lock={VersionLock}, " +
                $"game_type={GameTypeSymbol}, " +
                $"level={LevelSymbol}, " +
                $"platform={PlatformSymbol}, " +
                $"session={Session}, " +
                $"unk1={Unk1}, " +
                $"unk2={Unk2}, " +
                $"channel={ChannelUUID}, " +
                $"session_settings={JObject.FromObject(SessionSettings).ToString(Newtonsoft.Json.Formatting.None)}, " +
                $"user_id={UserId}, " +
                $"team_index={TeamIndex}" +
                $")";
        }
        #endregion
    }
}
