using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating a list of players should be accepted by the game server.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerPlayersAccepted : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770600;

        public byte Unk0;

        /// <summary>
        /// The players which should be accepted by the game server.
        /// </summary>
        public Guid[] PlayerSessions;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerPlayersAccepted"/> message.
        /// </summary>
        public ERGameServerPlayersAccepted()
        {
            PlayerSessions = Array.Empty<Guid>();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerPlayersAccepted"/> with the provided arguments.
        /// </summary>
        /// <param name="playerSessions">The players to be accepted by the game server.</param>
        public ERGameServerPlayersAccepted(Guid[] playerSessions)
        {
            PlayerSessions = playerSessions;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Unk0);

            // Read/write our array size
            if (io.StreamMode == StreamMode.Read)
            {
                int count = (int)(io.Length - io.Position) / 16;
                PlayerSessions = new Guid[count];
            }

            // Stream all player sessions
            for (int i = 0; i < PlayerSessions.Length; i++)
            {
                // Stream the player session data in/out.
                io.Stream(ref PlayerSessions[i]);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(player_sessions=[{string.Join(", ", PlayerSessions.AsEnumerable())}])";
        }
        #endregion
    }
}
