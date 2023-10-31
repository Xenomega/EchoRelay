using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// A JSON resource used to represent a document, served through the <see cref="Services.Login.LoginService"/> to clients.
    /// </summary>
    public class DocumentResource : IKeyedResource<(string Type, string Language)>
    {
        #region Properties
        /// <summary>
        /// The type of the resource identifier. This must correspond to a symbol in the <see cref="Resources.SymbolCache"/>.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; } = "";

        /// <summary>
        /// The language of the resource identifier. This must correspond to a <see cref="Game.Language"/>.
        /// </summary>
        [JsonProperty("lang")]
        public string Language { get; set; } = "";

        /// <summary>
        /// Additional properties of the resource. Each resource defines its own set of fields.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="DocumentResource"/>.
        /// </summary>
        public DocumentResource()
        {
            // Initialize our additional tokens.
            AdditionalData = new Dictionary<string, JToken>();
        }
        #endregion

        #region Functions
        /// <summary>
        /// Obtains the key which the storage resource is indexed by.
        /// </summary>
        /// <returns>Returns the key which the resource is indexed by.</returns>
        public (string Type, string Language) Key()
        {
            return (Type, Language);
        }
        #endregion
    }
}
