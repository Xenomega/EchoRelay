using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Config
{
    /// <summary>
    /// A message from client to server requesting a specific configuration resource.
    /// </summary>
    public class ConfigRequestv2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -9041364331368070280;

        /// <summary>
        /// The tail byte of the <see cref="ConfigRequestv2Info.Identifier"/> symbol.
        /// </summary>
        public byte ConfigTypeSymbolTail;
        /// <summary>
        /// Information which is provided for the type and identifier of resource being requested.
        /// </summary>
        public ConfigInfo Info;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ConfigRequestv2"/> message.
        /// </summary>
        public ConfigRequestv2()
        {
            Info = new ConfigInfo();
        }
        /// <summary>
        /// Initializes a new <see cref="ConfigRequestv2"/> message with the provided arguments.
        /// </summary>
        /// <param name="configTypeSymbolTail">The last byte of the config resource's symbol.</param>
        /// <param name="info">The actual underlying information associated with the config request.</param>
        public ConfigRequestv2(byte configTypeSymbolTail, ConfigInfo info)
        {
            ConfigTypeSymbolTail = configTypeSymbolTail;
            Info = info;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref ConfigTypeSymbolTail);
            io.StreamJSON(ref Info, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(type_symbol_tail={ConfigTypeSymbolTail}, type_info={Info})";
        }
        #endregion

        #region Classes
        /// <summary>
        /// The JSON encoded config request information.
        /// </summary>
        public class ConfigInfo
        {
            /// <summary>
            /// The type (group) of resource being requested.
            /// </summary>
            [JsonProperty("type")]
            public string Type { get; set; } = "";

            /// <summary>
            /// The identifier (name) of the resource being requested.
            /// </summary>
            [JsonProperty("id")]
            public string Identifier { get; set; } = "";

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            public override string ToString()
            {
                return $"<type={Type}, id={Identifier}>";
            }
        }
        #endregion
    }
}
