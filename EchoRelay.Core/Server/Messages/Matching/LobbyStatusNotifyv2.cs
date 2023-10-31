using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client notifying them of some status (e.g. the reason they were kicked).
    /// </summary>
    public class LobbyStatusNotifyv2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -1965344278184031864;

        /// <summary>
        /// The channel which the status notification applies to.
        /// </summary>
        public Guid Channel;

        /// <summary>
        /// A message describing the status update. This is a maximum of 64 bytes, UTF-8 encoded.
        /// </summary>
        public string Message;

        // TODO: Create a DateTime property for ExpiryTime.

        /// <summary>
        /// The time the status change takes effect until.
        /// </summary>
        private ulong _expiryTime64;

        private ulong _reason;
        /// <summary>
        /// The reason for the status notification.
        /// </summary>
        public StatusUpdateReason Reason
        {
            get
            {
                return (StatusUpdateReason)_reason;
            }
            set
            {
                _reason = (ulong)value;
            }
        }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyStatusNotifyv2"/> message.
        /// </summary>
        public LobbyStatusNotifyv2()
        {
            Channel = new Guid();
            Message = "";
            _expiryTime64 = 0;
            _reason = 0;
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyStatusNotifyv2"/> with the provided arguments.
        /// </summary>
        /// <param name="channel">The channel which the status notification applies to.</param>
        /// <param name="message"> A message describing the status update.</param>
        /// <param name="expiryTime">The time at which the status change expires.</param>
        /// <param name="reason">The reason for the status notification.</param>
        public LobbyStatusNotifyv2(Guid channel, string message, ulong expiryTime, StatusUpdateReason reason)
        {
            Channel = channel;
            Message = message;
            _expiryTime64 = expiryTime;
            Reason = reason;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Channel);
            io.Stream(ref Message, 64);
            io.Stream(ref _expiryTime64);
            io.Stream(ref _reason);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"channel={Channel}, " +
                $"message={Message}, " +
                $"until={_expiryTime64}, " +
                $"reason={Reason}" + 
                $")";
        }
        #endregion

        #region Enums
        /// <summary>
        /// Describes the reason for the status update in a <see cref="LobbyStatusNotifyv2"/> message.
        /// </summary>
        public enum StatusUpdateReason
        {
            Banned,
            Kicked,
            Demoted,
            Unknown
        }
        #endregion
    }
}
