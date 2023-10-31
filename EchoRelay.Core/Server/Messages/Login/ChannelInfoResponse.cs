using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to client, providing the in-game channel information requested by a previous <see cref="ChannelInfoRequest"/>.
    /// </summary>
    public class ChannelInfoResponse : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 7822496150166529221;

        /// <summary>
        /// The channel info data obtained for the user.
        /// </summary>
        public ChannelInfoResource ChannelInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ChannelInfoResponse"/> message.
        /// </summary>
        public ChannelInfoResponse()
        {
            ChannelInfo = new ChannelInfoResource();
        }
        /// <summary>
        /// Initializes a new <see cref="ChannelInfoResponse"/> message with the provided arguments.
        /// </summary>
        /// <param name="channelInfo">The in-game channel info to send to the user.</param>
        public ChannelInfoResponse(ChannelInfoResource channelInfo)
        {
            ChannelInfo = channelInfo;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.StreamJSON(ref ChannelInfo, false, JSONCompressionMode.Zlib);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(channel_info={JObject.FromObject(ChannelInfo).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
