using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Game
{
    /// <summary>
    /// The JSON service-related config used by Echo VR to indicate service endpoints and other configuration information.
    /// </summary>
    public class ServiceConfig
    {
        #region Properties
        /// <summary>
        /// An unofficial JSON key used to denote the endpoint for the HTTP(S) API server.
        /// This is only supported with EchoRelay patches.
        /// </summary>
        [JsonProperty("apiservice_host")]
        public string? ApiServiceHost { get; set; }

        /// <summary>
        /// The endpoint to be used for the config service.
        /// </summary>
        [JsonProperty("configservice_host")]
        public string ConfigServiceHost { get; set; }

        /// <summary>
        /// The endpoint to be used for the login service.
        /// </summary>
        [JsonProperty("loginservice_host")]
        public string LoginServiceHost { get; set; }

        /// <summary>
        /// The endpoint to be used for the matching service.
        /// </summary>
        [JsonProperty("matchingservice_host")]
        public string MatchingServiceHost { get; set; }

        /// <summary>
        /// The endpoint to be used for the serverdb service.
        /// </summary>
        [JsonProperty("serverdb_host")]
        public string? ServerDBServiceHost { get; set; }

        /// <summary>
        /// The endpoint to be used for the transaction service.
        /// </summary>
        [JsonProperty("transactionservice_host")]
        public string TransactionServiceHost { get; set; }

        /// <summary>
        /// The filename of the server plugin (minus ".dll").
        /// </summary>
        [JsonProperty("server_plugin")]
        public string? ServerPlugin { get; set; }

        /// <summary>
        /// The environment/publisher lock to be used.
        /// </summary>
        [JsonProperty("publisher_lock")]
        public string? PublisherLock { get; set; }

        /// <summary>
        /// Additional properties of the resource. Each resource defines its own set of fields.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ServiceConfig"/>.
        /// </summary>
        public ServiceConfig() : this(null, "", "", "", "", "", null, null)
        {
            // Initialize our additional tokens.
            AdditionalData = new Dictionary<string, JToken>();
        }

        /// <summary>
        /// Initializes a new <see cref="ServiceConfig"/> with the provided arguments.
        /// </summary>
        public ServiceConfig(string? apiServiceHost, string configServiceHost, string loginServiceHost, string matchingServiceHost, string? serverdbServiceHost, string transactionServiceHost, string? publisherLock, string? serverPlugin)
        {
            // Set our provided arguments.
            ApiServiceHost = apiServiceHost;
            ConfigServiceHost = configServiceHost;
            LoginServiceHost = loginServiceHost;
            MatchingServiceHost = matchingServiceHost;
            ServerDBServiceHost = serverdbServiceHost;
            TransactionServiceHost = transactionServiceHost;
            ServerPlugin = serverPlugin;
            PublisherLock = publisherLock;

            // Initialize our additional tokens.
            AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion
    }
}
