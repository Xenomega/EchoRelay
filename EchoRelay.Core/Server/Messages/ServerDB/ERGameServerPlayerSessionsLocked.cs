using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from game server to server, indicating player sessions have been locked on the game server.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// </summary>
    public class ERGameServerPlayerSessionsLocked : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770300;

        /// <summary>
        /// An unused byte sent with the packet.
        /// </summary>
        public byte Unused;
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
