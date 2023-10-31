using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, indicating a game server registration request had failed.
    /// </summary>
    public class LobbyRegistrationFailure : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -5373034290044534839;

        private byte _result;
        /// <summary>
        /// The failure code for the lobby registration.
        /// </summary>
        public FailureCode Result
        {
            get { return (FailureCode)_result; }
            set { _result = (byte)value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyRegistrationFailure"/> message.
        /// </summary>
        public LobbyRegistrationFailure()
        {
        
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyRegistrationFailure"/> with the provided arguments.
        /// </summary>
        /// <param name="result">The failure code for the lobby registration.</param>
        public LobbyRegistrationFailure(FailureCode result)
        {
            Result = result;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref _result);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(result={Result})";
        }
        #endregion

        #region Enums
        /// <summary>
        /// Indicates the type of game server registration failure that occurred.
        /// </summary>
        public enum FailureCode : int
        {
            InvalidRequest = 0,
            Timeout = 1,
            CryptographyError = 2,
            DatabaseError = 3,
            AccountDoesNotExist = 4,
            ConnectionFailed = 5,
            ConnectionLost = 6,
            ProviderError = 7,
            Restricted = 8,
            Unknown = 9,
            Failure = 10,
            Success = 11,
        }
        #endregion
    }
}
