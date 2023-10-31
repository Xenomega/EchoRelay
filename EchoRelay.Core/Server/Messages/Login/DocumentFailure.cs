using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to client indicating a <see cref="DocumentRequestv2"/> failed.
    /// </summary>
    public class DocumentFailure : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -7032236248796415888;

        // TODO: Figure this out
        public ulong Unk0;
        public ulong Unk1;

        /// <summary>
        /// The message to return with the failure.
        /// </summary>
        public string Message;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="DocumentFailure"/> message.
        /// </summary>
        public DocumentFailure()
        {
            Message = "";
        }

        /// <summary>
        /// Initializes a new <see cref="DocumentFailure"/> message with the provided arguments.
        /// </summary>
        /// <param name="unk0">Unknown.</param>
        /// <param name="unk1">Unknown.</param>
        /// <param name="message">The message to send with the failure.</param>
        public DocumentFailure(ulong unk0, ulong unk1, string message)
        {
            Unk0 = unk0;
            Unk1 = unk1;
            Message = message;
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
            io.Stream(ref Unk1);
            io.Stream(ref Message, true);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(unk0={Unk0}, unk1={Unk1}, msg=\"{Message}\")";
        }
        #endregion
    }
}
