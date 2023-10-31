using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Config
{
    /// <summary>
    /// A message from server to client indicating a <see cref="ConfigRequestv2"/> resulted in a failure.
    /// </summary>
    public class ConfigFailurev2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -7032236248796415888;

        /// <summary>
        /// TODO: Unknown, 16 bytes.
        /// </summary>
        public UInt128 Unk0;

        /// <summary>
        /// The config failure error information.
        /// </summary>
        public ErrorInfo Info;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ConfigFailurev2"/> message.
        /// </summary>
        public ConfigFailurev2()
        {
            Unk0 = 0;
            Info = new ErrorInfo();
        }

        /// <summary>
        /// Initializes a new <see cref="ConfigFailurev2"/> message with the provided parameters.
        /// </summary>
        /// <param name="type">The type of the config resource that was requested.</param>
        /// <param name="identifier">The identifier of the config resource that was requested.</param>
        /// <param name="errorCode">The error code returned as a result of the failure.</param>
        /// <param name="error">The error message returned as a result of the failure.</param>
        public ConfigFailurev2(string type, string identifier, long errorCode, string error) : this()
        {
            Info.Type = type;
            Info.Identifier = identifier;
            Info.ErrorCode = errorCode;
            Info.Error = error;
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
            io.StreamJSON(ref Info, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(unk0={Unk0}, info={Info})";
        }
        #endregion

        #region Classes
        /// <summary>
        /// The JSON encoded config failure error information.
        /// </summary>
        public class ErrorInfo
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
            /// The error code returned for the failure.
            /// </summary>
            [JsonProperty("errorcode")]
            public long ErrorCode { get; set; }

            /// <summary>
            /// The error string returned for the failure.
            /// </summary>
            [JsonProperty("error")]
            public string Error { get; set; } = "";

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            public override string ToString()
            {
                return $"<type={Type}, id={Identifier}, errorcode={ErrorCode}, error=\"{Error}\">";
            }

        }
        #endregion
    }
}
