using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from server to client, providing the settings for the user after a <see cref="LoginRequest"/>.
    /// </summary>
    public class LoginSettings : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -1343230735030331919;

        /// <summary>
        /// The settings data supplied for the user.
        /// </summary>
        public LoginSettingsResource Resource;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoginSettings"/> message.
        /// </summary>
        public LoginSettings()
        {
            Resource = new LoginSettingsResource();
        }
        /// <summary>
        /// Initializes a new <see cref="LoginSettings"/> message with the provided arguments.
        /// </summary>
        /// <param name="resource">The user settings to send to the user post-login.</param>
        public LoginSettings(LoginSettingsResource resource)
        {
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
            io.StreamJSON(ref Resource, false, JSONCompressionMode.Zlib);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(settings={JObject.FromObject(Resource).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
