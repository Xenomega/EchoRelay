using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client indicating a lobby session request failed.
    /// </summary>
    public class LobbySessionFailurev2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 5397623933917067626;

        /// <summary>
        /// The channel requested for the session.
        /// </summary>
        public Guid ChannelUUID;

        private uint _errorCode;
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
                _errorCode = (uint)value;
            }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev2"/> message.
        /// </summary>
        public LobbySessionFailurev2()
        {
            ChannelUUID = new Guid();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev2"/> with the provided arguments.
        /// </summary>
        /// <param name="channel">The channel that the matching failed for.</param>
        /// <param name="errorCode">The error code to send with the failure.</param>
        public LobbySessionFailurev2(Guid channel, LobbySessionFailureErrorCode errorCode)
        {
            ChannelUUID = channel;
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
            io.Stream(ref ChannelUUID);
            io.Stream(ref _errorCode);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"channel={ChannelUUID}, " +
                $"error_code={ErrorCode}" + 
                $")";
        }
        #endregion
    }
}
