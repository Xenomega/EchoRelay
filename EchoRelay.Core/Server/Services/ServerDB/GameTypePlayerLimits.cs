using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoRelay.Core.Server.Services.ServerDB
{
    /// <summary>
    /// Obtains non-spectator and total player limits for a given gametype.
    /// </summary>
    public abstract class GameTypePlayerLimits
    {
        private static ConcurrentDictionary<string, PlayerLimits> _playerLimits;
        public static PlayerLimits DefaultLimits { get; }
        static GameTypePlayerLimits()
        {
            // Create a lookup for each gametype.
            _playerLimits = new ConcurrentDictionary<string, PlayerLimits>(StringComparer.OrdinalIgnoreCase);
            _playerLimits["echo_arena"] = new PlayerLimits(16, 8); // 4 vs 4
            _playerLimits["echo_combat"] = new PlayerLimits(16, 8); // 4 vs 4

            // Note: Private DOES NOT need to be enforced, since you can only join with -lobbyid, and players
            // are already locked out at 5v5 (but spectators are allowed), we won't matchmake you to a private.
            // So this is unnecessary and would only cause more issues (because players could change between
            // spectator and non, and central server would not know).
            //_playerLimits["echo_arena_private"] = new PlayerLimits(16, 10); // 5 vs 5
            //_playerLimits["echo_combat_private"] = new PlayerLimits(16, 10); // 5 vs 5

            // Note: AI, echo_demo, and other gametypes are not adjusted as AI is currently unsupported,
            // and player counts including bots would differ, especially as bots are added by game server.

            // Note: Other gametypes don't need to enforce a reserved player count.
            DefaultLimits = new PlayerLimits(16);
        }
        /// <summary>
        /// Obtain the player limits for a given gametype.
        /// </summary>
        /// <param name="server">The server which is serving requests.</param>
        /// <param name="gameTypeSymbol">The symbol denoting the game type for the session.</param>
        /// <returns>The orange + blue team, and total player limits for the game type.</returns>
        public static PlayerLimits GetPlayerLimits(Server server, long gameTypeSymbol)
        {
            // Obtain the name of the gametype.
            string? gameTypeName = server.SymbolCache.GetName(gameTypeSymbol);

            // Return the player limits for the given gametype, if they are defined.
            if (gameTypeName != null && _playerLimits.TryGetValue(gameTypeName, out PlayerLimits? playerLimits))
                    return playerLimits;

            // Otherwise fall back onto default limits.
            return DefaultLimits;
        }

        #region Classes
        public class PlayerLimits
        {
            /// <summary>
            /// The amount of slots reserved for active game participants (participants matching with orange/blue team, or any team, which we assume 
            /// </summary>
            public int? FixedActiveGameParticipantTarget { get; }
            /// <summary>
            /// The total amount of players allowed in the game, of any team or role, including spectator and moderator.
            /// </summary>
            public int TotalPlayerLimit { get; }


            public PlayerLimits(int totalPlayerLimit, int? fixedActiveGameParticipantTarget = null)
            {
                // Check the bounds of limits
                if (totalPlayerLimit > 255)
                    throw new ArgumentException("Game type total player limit cannot exceed 255, as it is represented by a single byte type.");
                if (fixedActiveGameParticipantTarget != null && fixedActiveGameParticipantTarget > totalPlayerLimit)
                    throw new ArgumentException("Game type fixed active participant target cannot exceed total player limit.");

                // Set our limit values.
                TotalPlayerLimit = totalPlayerLimit;
                FixedActiveGameParticipantTarget = fixedActiveGameParticipantTarget;
            }

            public bool CheckTeamAvailability(TeamIndex[] peerRequestedTeams, TeamIndex requestedTeam)
            {
                // Check if the total player limit was hit.
                if (peerRequestedTeams.Length >= TotalPlayerLimit)
                    return false;

                // If there is no fixed participant target, there is availability then.
                if (FixedActiveGameParticipantTarget == null)
                    return true;

                // Check active game participant count
                int activeGameParticipants = 0;
                foreach(TeamIndex peerRequestedTeam in peerRequestedTeams)
                {
                    // When requesting "any" team, you get assigned to blue/orange in a real match, or to the social participant team in a social lobby.
                    // This logic below doesn't support social lobbies, but we don't enforce fixed active game participants for them.
                    if (peerRequestedTeam == TeamIndex.Any || peerRequestedTeam == TeamIndex.Blue || peerRequestedTeam == TeamIndex.Orange)
                        activeGameParticipants++;
                }
                int nonActiveGameParticipants = peerRequestedTeams.Length - activeGameParticipants;

                // Check if we're requesting an active team, or a non active one, then return if there is availability.
                if (requestedTeam == TeamIndex.Any || requestedTeam == TeamIndex.Blue || requestedTeam == TeamIndex.Orange)
                    return FixedActiveGameParticipantTarget - activeGameParticipants > 0;
                else
                    return (TotalPlayerLimit - FixedActiveGameParticipantTarget) - nonActiveGameParticipants > 0;
            }
        }
        #endregion
    }
}
