using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client indicating a lobby session request failed.
    /// </summary>
    public class LobbySessionFailurev1 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -5071315040643272207;

        private byte _errorCode;
        /// <summary>
        /// The error code to return with the failure.
        /// </summary>
        public LobbySessionFailureErrorCode ErrorCode
        {
            get
            {
                return (LobbySessionFailureErrorCode)_errorCode;
            }
            set
            {
                _errorCode = (byte)value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev1"/> message.
        /// </summary>
        public LobbySessionFailurev1()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev1"/> with the provided arguments.
        /// </summary>
        /// <param name="errorCode">The error code to send with the failure.</param>
        public LobbySessionFailurev1(LobbySessionFailureErrorCode errorCode)
        {
            ErrorCode = errorCode;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref _errorCode);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(error_code={ErrorCode})";
        }
        #endregion
    }
}
