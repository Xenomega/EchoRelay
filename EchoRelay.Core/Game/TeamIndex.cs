namespace EchoRelay.Core.Game
{
    /// <summary>
    /// Indicates what a given team index in the game represents in terms of user roles.
    /// </summary>
    public enum TeamIndex : short
    {
        /// <summary>
        /// Indicates any team index should be assigned by the game server.
        /// </summary>
        Any = -1,

        /// <summary>
        /// The player is assigned or requesting to be assigned to the blue team.
        /// </summary>
        Blue = 0,

        /// <summary>
        /// The player is assigned or requesting to be assigned to the orange team.
        /// </summary>
        Orange = 1,

        /// <summary>
        /// The player is assigned or requesting to be assigned to a spectator team, invisible to other players.
        /// </summary>
        Spectator = 2,

        /// <summary>
        /// TODO: This is unverified. From loose memory, might've been used as the team everyone is assigned to for social lobbies?
        /// </summary>
        SocialLobbyParticipant = 3,

        /// <summary>
        /// The player is assigned or requesting to be assigned to a moderator team role, invisible to other players and able to fly around.
        /// </summary>
        Moderator = 4,
    }
}
