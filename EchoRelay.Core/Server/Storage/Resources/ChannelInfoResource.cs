using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// Provides information to clients regarding in-game channels (e.g. "COMPETITIVE GAMERS", "ECHO COMBAT PLAYERS", "PLAYGROUND", etc).
    /// </summary>
    public class ChannelInfoResource
    {
        #region Properties
        /// <summary>
        /// The list of channels and their underlying information.
        /// </summary>
        [JsonProperty("group")]
        public Channel[] Group { get; set; } = Array.Empty<Channel>();

        /// <summary>
        /// Additional fields which are not caught explicitly are retained here.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ChannelInfoResource"/> with the optionally provided channels.
        /// </summary>
        /// <param name="group">The optionally provided channels to set.</param>
        public ChannelInfoResource(params Channel[] group)
        {
            Group = group;
        }

        #endregion

        #region Classes
        /// <summary>
        /// Provides information on a single channel in a <see cref="Group"/>.
        /// </summary>
        public class Channel
        {
            /// <summary>
            /// The UUID representing the channel.
            /// </summary>
            [JsonProperty("channeluuid")]
            public string ChannelUUID { get; set; } = "77777777-7777-7777-7777-777777777777";

            /// <summary>
            /// The name of the channel, e.g. "COMPETITIVE GAMERS", "ECHO COMBAT PLAYERS", "PLAYGROUND".
            /// </summary>
            [JsonProperty("name")]
            public string Name { get; set; } = "";

            /// <summary>
            /// The description text shown to players. It may have new line characters in it.
            /// </summary>
            [JsonProperty("description")]
            public string Description { get; set; } = "";

            /// <summary>
            /// The rules text shown to players. It may have new line characters in it.
            /// See <see cref="RulesVersion"/>'s documentation for information regarding user rule acceptance.
            /// </summary>
            [JsonProperty("rules")]
            public string Rules { get; set; } = "";

            /// <summary>
            /// The version number of the rules. Each client accepts the rules and updates their profile with this rules version
            /// to know if they have acknowledged the most recent rules. Thus, increasing this number forces users to re-accept rules.
            /// This should be increased monotonically.
            /// </summary>
            [JsonProperty("rules_version")]
            public ulong RulesVersion { get; set; } = 1;

            /// <summary>
            /// The description text shown to players. It may have new line characters in it.
            /// </summary>
            [JsonProperty("link")]
            public string Link { get; set; } = "https://discord.com/";

            /// <summary>
            /// The priority (index from 0) of the channel in its parent <see cref="Group"/> list.
            /// The order of a <see cref="Channel"/> in the <see cref="Group"/> list does not have to match
            /// priority, as long as there are no gaps or duplicates in priority numbers.
            /// </summary>
            [JsonProperty("priority")]
            public ulong Priority { get; set; }

            /// <summary>
            /// TODO: Unknown.
            /// </summary>
            [JsonProperty("_rad")]
            public bool Rad { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion
    }
}



