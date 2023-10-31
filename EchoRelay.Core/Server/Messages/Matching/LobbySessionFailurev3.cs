using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client indicating a lobby session request failed.
    /// </summary>
    public class LobbySessionFailurev3 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 5397623933917067627;

        /// <summary>
        /// A symbol representing the gametype requested for the session.
        /// </summary>
        public long GameTypeSymbol;
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

        // TODO 
        public uint Unk0;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev3"/> message.
        /// </summary>
        public LobbySessionFailurev3()
        {
            ChannelUUID = new Guid();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbySessionFailurev3"/> with the provided arguments.
        /// </summary>
        /// <param name="gameTypeSymbol">The gametype that the matching failed for.</param>
        /// <param name="channel">The channel that the matching failed for.</param>
        /// <param name="errorCode">The error code to send with the failure.</param>
        /// <param name="unk0">Unknown.</param>
        public LobbySessionFailurev3(long gameTypeSymbol, Guid channel, LobbySessionFailureErrorCode errorCode, uint unk0)
        {
            GameTypeSymbol = gameTypeSymbol;
            ChannelUUID = channel;
            ErrorCode = errorCode;
            Unk0 = unk0;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref GameTypeSymbol);
            io.Stream(ref ChannelUUID);
            io.Stream(ref _errorCode);
            io.Stream(ref Unk0);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"game_type={GameTypeSymbol}, " +
                $"channel={ChannelUUID}, " +
                $"error_code={ErrorCode}, " +
                $"unk0={Unk0}" +
                $")";
        }
        #endregion
    }
}
