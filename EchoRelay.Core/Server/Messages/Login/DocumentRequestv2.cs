using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting a document resource.
    /// </summary>
    public class DocumentRequestv2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -230010198603715656;

        /// <summary>
        /// The language of the document being requested.
        /// </summary>
        public string Language;
        /// <summary>
        /// The name of the document being requested.
        /// </summary>
        public string Name;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="DocumentRequestv2"/> message.
        /// </summary>
        public DocumentRequestv2()
        {
            Language = Game.Language.English;
            Name = "";
        }
        /// <summary>
        /// Initializes a new <see cref="DocumentRequestv2"/> message with the provided arguments.
        /// </summary>
        /// <param name="language">The language of the document being requested.</param>
        /// <param name="name">The name of the document being requested.</param>
        public DocumentRequestv2(string language, string name)
        {
            Language = language;
            Name = name;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Language, true);
            io.Stream(ref Name, true);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(language=\"{Language}\", name=\"{Name}\")";
        }
        #endregion
    }
}
