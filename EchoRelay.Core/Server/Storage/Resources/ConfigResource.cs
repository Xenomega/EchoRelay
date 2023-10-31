using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// A JSON resource used to represent a game/server configuration, served through the <see cref="Services.Config.ConfigService"/> to clients.
    /// </summary>
    public class ConfigResource : IKeyedResource<(string Type, string Identifier)>
    {
        #region Properties
        /// <summary>
        /// The type of the config resource. This must correspond to a symbol in the <see cref="Resources.SymbolCache"/>.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        /// <summary>
        /// The name of the config resource. This must correspond to a symbol in the <see cref="Resources.SymbolCache"/>.
        /// </summary>
        [JsonProperty("id")]
        public string Identifier { get; set; } = "";

        /// <summary>
        /// Additional properties of the resource. Each resource defines its own set of fields.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ConfigResource"/>.
        /// </summary>
        public ConfigResource()
        {
            // Initialize our additional tokens.
            AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Validates the JSON fields and throws an exception if they are invalid.
        /// </summary>
        /// <exception cref="JsonException">The exception to throw.</exception>
        public virtual void ValidateJSON()
        {
            // If our type or identifier are are not set, throw an exception.
            if (string.IsNullOrEmpty(Type) || string.IsNullOrEmpty(Identifier))
                throw new JsonException("Cannot have a blank 'type' or 'id' key");
        }

        /// <summary>
        /// Obtains the key which the storage resource is indexed by.
        /// </summary>
        /// <returns>Returns the key which the resource is indexed by.</returns>
        public (string Type, string Identifier) Key()
        {
            return (Type, Identifier);
        }
        #endregion
    }
}
