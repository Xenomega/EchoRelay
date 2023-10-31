using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Common
{
    /// <summary>
    /// A message originating from either party, indicating a TCP event is no longer required.
    /// </summary>
    public class TcpConnectionUnrequireEvent : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x43e6963ac76beee4;

        /// <summary>
        /// An unused byte sent with the message.
        /// </summary>
        public byte Unused;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="TcpConnectionUnrequireEvent"/> message.
        /// </summary>
        public TcpConnectionUnrequireEvent()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="TcpConnectionUnrequireEvent"/> message.
        /// </summary>
        public TcpConnectionUnrequireEvent(byte unused)
        {
            Unused = unused;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Unused);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(unused={Unused})";
        }
        #endregion
    }
}
