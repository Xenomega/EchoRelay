using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages.ServerDB;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to server requesting joining of a specified game session that
    /// matches the message's underlying arguments.
    /// </summary>
    public class LobbyJoinSessionRequestv7 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 3387628926720258577;

        /// <summary>
        /// The lobby requested for the session.
        /// </summary>
        public Guid LobbyUUID;
        /// <summary>
        /// The version of the client, prevents mismatches in matching.
        /// </summary>
        public long VersionLock;
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
        public ulong Unk2;

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
        /// Initializes a new <see cref="LobbyJoinSessionRequestv7"/> message.
        /// </summary>
        public LobbyJoinSessionRequestv7()
        {
            LobbyUUID = new Guid();
            VersionLock = 0;
            PlatformSymbol = -1;
            Session = new Guid();
            Unk1 = 0;
            Unk2 = 0;
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
            io.Stream(ref LobbyUUID);
            io.Stream(ref VersionLock);
            io.Stream(ref PlatformSymbol);
            io.Stream(ref Session);
            io.Stream(ref Unk1);
            io.Stream(ref Unk2);
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
                $"lobby={LobbyUUID}, " +
                $"version_lock={VersionLock}, " +
                $"platform={PlatformSymbol}, " +
                $"session={Session}, " +
                $"unk1={Unk1}, " +
                $"unk2={Unk2}, " +
                $"info={JObject.FromObject(SessionSettings).ToString(Newtonsoft.Json.Formatting.None)}, " +
                $"user_id={UserId}, " +
                $"team_index={TeamIndex}" +
                $")";
        }
        #endregion
    }
}
