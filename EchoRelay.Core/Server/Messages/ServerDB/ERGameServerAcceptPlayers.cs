using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from game server to server, indicating a number of player sessions had been accepted by the game server.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerAcceptPlayers : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770500;

        /// <summary>
        /// The players sessions which were accepted by the game server.
        /// </summary>
        public Guid[] PlayerSessions;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerAcceptPlayers"/> message.
        /// </summary>
        public ERGameServerAcceptPlayers()
        {
            PlayerSessions = Array.Empty<Guid>();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerAcceptPlayers"/> with the provided arguments.
        /// </summary>
        /// <param name="playerIds">The player sessions which were accepted by the game server.</param>
        public ERGameServerAcceptPlayers(Guid[] playerSessions)
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
            // Read/write our array size
            if (io.StreamMode == StreamMode.Read)
            {
                int count = (int)(io.Length - io.Position) / 16;
                PlayerSessions = new Guid[count];
            }

            // Stream all player sessions
            for (int i = 0; i < PlayerSessions.Length; i++)
            {
                // Stream the player id data in/out.
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
