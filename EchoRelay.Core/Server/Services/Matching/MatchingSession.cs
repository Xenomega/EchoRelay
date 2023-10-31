using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Messages.ServerDB;
using EchoRelay.Core.Server.Services.ServerDB;
using static EchoRelay.Core.Server.Messages.ServerDB.ERGameServerStartSession;

namespace EchoRelay.Core.Server.Services.Matching
{
    public class MatchingSession
    {
        public XPlatformId UserId { get; private set; }
        public Guid? LobbyId { get; private set; }
        public Guid? Channel { get; private set; }
        public long? GameTypeSymbol { get; private set; }
        public long? LevelSymbol { get; private set; }
        public LobbyType NewSessionLobbyType { get; private set; }
        public LobbyType[] SearchLobbyTypes
        {
            get
            {
                return (NewSessionLobbyType == LobbyType.Private) ? new LobbyType[] { LobbyType.Unassigned } : new LobbyType[] { LobbyType.Unassigned, LobbyType.Public };
            }
        }
        public ERGameServerStartSession.SessionSettings SessionSettings { get; private set; }
        public TeamIndex TeamIndex { get; private set; }

        public RegisteredGameServer? MatchedGameServer { get; set; }
        public Guid? MatchedSessionId { get; set; }
        private MatchingSession(XPlatformId userId, Guid? lobbyId, Guid? channel, long? gameTypeSymbol, long? levelSymbol, LobbyType newSessionLobbyType, TeamIndex teamIndex, ERGameServerStartSession.SessionSettings sessionSettings)
        {
            UserId = userId;
            LobbyId = lobbyId;
            Channel = channel;
            GameTypeSymbol = gameTypeSymbol;
            LevelSymbol = levelSymbol;
            NewSessionLobbyType = newSessionLobbyType;
            TeamIndex = teamIndex;
            SessionSettings = sessionSettings;
        }

        public static MatchingSession FromCreateSessionCriteria(XPlatformId userId, Guid? channel, long? gameTypeSymbol, long? levelSymbol, LobbyType lobbyType, TeamIndex teamIndex, ERGameServerStartSession.SessionSettings sessionSettings)
        {
            return new MatchingSession(userId, null, channel, gameTypeSymbol, levelSymbol, lobbyType, teamIndex, sessionSettings);
        }
        public static MatchingSession FromFindSessionCriteria(XPlatformId userId, Guid? channel, long? gameTypeSymbol, TeamIndex teamIndex, ERGameServerStartSession.SessionSettings sessionSettings)
        {
            return new MatchingSession(userId, null, channel, gameTypeSymbol, null, LobbyType.Public, teamIndex, sessionSettings);
        }

        public static MatchingSession FromJoinSpecificSessionCriteria(XPlatformId userId, Guid? lobbyId, TeamIndex teamIndex, ERGameServerStartSession.SessionSettings sessionSettings)
        {
            return new MatchingSession(userId, lobbyId, null, null, null, LobbyType.Public, teamIndex, sessionSettings);
        }
    }
}
