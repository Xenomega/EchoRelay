using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to the client indicating a <see cref="DocumentRequestv2"/> succeeded.
    /// It contains information about the requested document resource.
    /// </summary>
    public class DocumentSuccess : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -3422738499139816183;

        /// <summary>
        /// The symbol associated with the requested document name.
        /// </summary>
        public long DocumentNameSymbol;
        /// <summary>
        /// The requested document resource data to return to the client.
        /// </summary>
        public DocumentResource Document;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="DocumentSuccess"/> message.
        /// </summary>
        public DocumentSuccess()
        {
            Document = new DocumentResource();
        }
        /// <summary>
        /// Initializes a new <see cref="DocumentSuccess"/> message with the provided arguments.
        /// </summary>
        /// <param name="documentNameSymbol">The symbol associated with the requested document resource name.</param>
        /// <param name="document">The requested document resource data to return to the client.</param>
        public DocumentSuccess(long documentNameSymbol, DocumentResource document)
        {
            DocumentNameSymbol = documentNameSymbol;
            Document = document;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref DocumentNameSymbol);
            io.StreamJSON(ref Document, true, JSONCompressionMode.Zstd);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(document_symbol={DocumentNameSymbol}, data={JObject.FromObject(Document).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
