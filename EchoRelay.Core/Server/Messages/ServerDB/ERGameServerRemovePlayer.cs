using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from game server to server, indicating a player was removed by the game server.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerRemovePlayer : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770800;

        /// <summary>
        /// The player session which was removed by the game server.
        /// </summary>
        public Guid PlayerSession;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerRemovePlayer"/> message.
        /// </summary>
        public ERGameServerRemovePlayer()
        {
            PlayerSession = new Guid();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerRemovePlayer"/> with the provided arguments.
        /// </summary>
        /// <param name="playerSession">The player session removed by the game server session.</param>
        public ERGameServerRemovePlayer(Guid playerSession)
        {
            PlayerSession = playerSession;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref PlayerSession);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(player_session={PlayerSession})";
        }
        #endregion
    }
}
