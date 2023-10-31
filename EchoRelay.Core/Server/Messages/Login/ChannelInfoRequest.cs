using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server, requesting information about the various in-game channels.
    /// </summary>
    public class ChannelInfoRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -8037361449999850272;

        /// <summary>
        /// An unused byte sent with the message.
        /// </summary>
        public byte Unused;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ChannelInfoRequest"/> message.
        /// </summary>
        public ChannelInfoRequest()
        {
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
