using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Config
{
    /// <summary>
    /// A message from server to the client indicating a <see cref="ConfigSuccessv2"/> succeeded.
    /// It contains information about the requested config resource.
    /// </summary>
    public class ConfigSuccessv2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -5058194012104830958;

        /// <summary>
        /// The symbol associated with the requested <see cref="ConfigRequestv2"/> identifier.
        /// </summary>
        public long TypeSymbol;
        /// <summary>
        /// The symbol associated with the requested <see cref="ConfigRequestv2"/> identifier.
        /// </summary>
        public long IdentifierSymbol;
        /// <summary>
        /// The requested config resource data. This is a JSON-serializable object.
        /// </summary>
        public ConfigResource Resource;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ConfigSuccessv2"/> message.
        /// </summary>
        public ConfigSuccessv2()
        {
            Resource = new ConfigResource();
        }
        /// <summary>
        /// Initializes a new <see cref="ConfigSuccessv2"/> message with the provided arguments.
        /// </summary>
        /// <param name="typeSymbol">The symbol associated with the requested config resource type.</param>
        /// <param name="identifierSymbol">The symbol associated with the requested config resource identifier.</param>
        /// <param name="resource">The config resource data which was requested (must be JSON serializable).</param>
        public ConfigSuccessv2(long typeSymbol, long identifierSymbol, ConfigResource resource)
        {
            TypeSymbol = typeSymbol;
            IdentifierSymbol = identifierSymbol;
            Resource = resource;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref TypeSymbol);
            io.Stream(ref IdentifierSymbol);
            io.StreamJSON(ref Resource, true, JSONCompressionMode.Zstd);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(type_symbol={TypeSymbol}, id_symbol={IdentifierSymbol}, data={JObject.FromObject(Resource).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
