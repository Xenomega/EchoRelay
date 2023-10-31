using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to the client providing information on servers available in different regions.
    /// This is not necessary for a client to operate.
    /// </summary>
    public class FindServerRegionInfo : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -8261057723629147028;

        // TODO: Unknown
        public ushort Unk0;
        public ushort Unk1;
        public ushort Unk2;

        /// <summary>
        /// The region information for various servers.
        /// </summary>
        public JObject RegionInfo;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="FindServerRegionInfo"/> message.
        /// </summary>
        public FindServerRegionInfo()
        {
            RegionInfo = new JObject();
        }
        /// <summary>
        /// Initializes a new <see cref="FindServerRegionInfo"/> message with the provided arguments.
        /// </summary>
        /// <param name="unk0">Unknown.</param>
        /// <param name="unk1">Unknown.</param>
        /// <param name="unk2">Unknown.</param>
        /// <param name="regionInfo">The region-based information for servers.</param>
        public FindServerRegionInfo(ushort unk0, ushort unk1, ushort unk2, JObject regionInfo)
        {
            Unk0 = unk0;
            Unk1 = unk1;    
            Unk2 = unk2;
            RegionInfo = regionInfo;
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
            io.Stream(ref Unk2);
            io.StreamJSON(ref RegionInfo, false, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(unk0={Unk0}, unk1={Unk1}, unk2={Unk2}, region_info={JObject.FromObject(RegionInfo).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
