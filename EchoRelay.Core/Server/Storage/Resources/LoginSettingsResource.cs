using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// The environment settings provided to users when they sign in, detailing metrics/reporting settings for client-to-server, in-app purchase/matchmaker settings, and active config data.
    /// </summary>
    public class LoginSettingsResource
    {
        #region Properties
        /// <summary>
        /// Indicates whether the client has in-app purchases unlocked.
        /// </summary>
        [JsonProperty("iap_unlocked")]
        public bool IAPUnlocked { get; set; } = false;

        /// <summary>
        /// Indicates whether the client should log social events.
        /// </summary>
        [JsonProperty("remote_log_social")]
        public bool RemoteLogSocial { get; set; }

        /// <summary>
        /// Indicates whether the client should log warning events.
        /// </summary>
        [JsonProperty("remote_log_warnings")]
        public bool RemoteLogWarnings { get; set; }

        /// <summary>
        /// Indicates whether the client should log error events.
        /// </summary>
        [JsonProperty("remote_log_errors")]
        public bool RemoteLogErrors { get; set; }

        /// <summary>
        /// Indicates whether the client should log rich presence events.
        /// </summary>
        [JsonProperty("remote_log_rich_presence")]
        public bool RemoteLogRichPresence { get; set; }

        /// <summary>
        /// Indicates whether the client should log metrics events.
        /// </summary>
        [JsonProperty("remote_log_metrics")]
        public bool RemoteLogMetrics { get; set; } = true;

        /// <summary>
        /// The current environment of the network (similar to publisher_lock).
        /// </summary>
        [JsonProperty("env")]
        public string Environment { get; set; } = "live";

        /// <summary>
        /// Authentication-related nonce.
        /// </summary>
        [JsonProperty("matchmaker_queue_mode")]
        public string MatchmakerQueueMode { get; set; } = "disabled";


        /// <summary>
        /// Configuration related data.
        /// </summary>
        [JsonProperty("config_data")]
        public LoginConfigSettings? ConfigData { get; set; }

        /// <summary>
        /// Additional fields which are not caught explicitly are retained here.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        #endregion

        #region Classes
        /// <summary>
        /// References <see cref="ConfigResource"/> which are/will become available.
        /// </summary>
        public class LoginConfigSettings
        {
            /// <summary>
            /// Configuration related data.
            /// </summary>
            [JsonProperty("active_battle_pass_season")]
            public Entry? ActiveBattlePassSeason { get; set; }

            /// <summary>
            /// Configuration related data.
            /// </summary>
            [JsonProperty("active_store_entry")]
            public Entry? ActiveStoreEntry { get; set; }

            /// <summary>
            /// Configuration related data.
            /// </summary>
            [JsonProperty("active_store_featured_entry")]
            public Entry? ActiveStoreFeatureEntry { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            /// <summary>
            /// A single entry in <see cref="LoginConfigData"/>.
            /// </summary>
            public class Entry
            {
                /// <summary>
                /// A <see cref="ConfigResource.Identifier"/>, indicating which config resource to use.
                /// </summary>
                [JsonProperty("id")]
                public string Identifier { get; set; } = "";

                /// <summary>
                /// The time at which the <see cref="ConfigResource"/> referenced will become available.
                /// </summary>
                [JsonProperty("starttime")]
                public ulong StartTime { get; set; }

                /// <summary>
                /// The time at which the <see cref="ConfigResource"/> referenced will expire.
                /// </summary>
                [JsonProperty("endtime")]
                public ulong EndTime { get; set; }

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                /// <summary>
                /// Initializes a new <see cref="Entry"/>.
                /// </summary>
                public Entry()
                {
                    StartTime = 0; // default = a date that started already
                    EndTime = 3392363227; // default = a date that won't expire soon
                }
                /// <summary>
                /// Initializes a new <see cref="Entry"/> with the provided arguments.
                /// </summary>
                /// <param name="identifier">The <see cref="ConfigResource.Identifier"/>, indicating which config resource to use.</param>
                /// <param name="startTime">The time at which the <see cref="ConfigResource"/> referenced will become available.</param>
                /// <param name="endTime">The time at which the <see cref="ConfigResource"/> referenced will expire.</param>
                public Entry(string identifier, ulong? startTime = null, ulong? endTime = null) : this()
                {
                    Identifier = identifier;
                    if (startTime != null)
                        StartTime = startTime.Value;
                    if (endTime != null)
                        EndTime = endTime.Value;
                }
            }
        }
        #endregion
    }
}



