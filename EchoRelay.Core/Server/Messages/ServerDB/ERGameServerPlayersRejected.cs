using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating a list of players should be kicked/rejected by the game server.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerPlayersRejected : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770700;

        private byte _errorCode;
        /// <summary>
        /// The error code indicates the underlying reason for the player session rejection.
        /// </summary>
        public PlayerSessionError ErrorCode
        {
            get { return (PlayerSessionError)_errorCode; }
            set { _errorCode = (byte)value;  }
        }

        /// <summary>
        /// The players which were rejected.
        /// </summary>
        public Guid[] PlayerSessions;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerPlayersRejected"/> message.
        /// </summary>
        public ERGameServerPlayersRejected()
        {
            PlayerSessions = Array.Empty<Guid>();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerPlayersRejected"/> with the provided arguments.
        /// </summary>
        /// <param name="errorCode">The error code indicates the underlying reason for the player session rejection.</param>
        /// <param name="playerSessions">The players which were rejected.</param>
        public ERGameServerPlayersRejected(PlayerSessionError errorCode, Guid[] playerSessions)
        {
            ErrorCode = errorCode;
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
            // Read/write our error code
            io.Stream(ref _errorCode);

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
            return $"{GetType().Name}(error_code={ErrorCode}, player_sessions=[{string.Join(", ", PlayerSessions.AsEnumerable())}])";
        }
        #endregion

        #region Enums
        /// <summary>
        /// Describes the error reason for rejecting a player session.
        /// </summary>
        public enum PlayerSessionError : int
        {
            Internal = 0x0,
            BadRequest = 0x1,
            Timeout = 0x2,
            Duplicate = 0x3,
            LobbyLocked = 0x4,
            LobbyFull = 0x5,
            LobbyEnding = 0x6,
            KickedFromServer = 0x7,
            Disconnected = 0x8,
            Inactive = 0x9,
        }
        #endregion
    }
}
