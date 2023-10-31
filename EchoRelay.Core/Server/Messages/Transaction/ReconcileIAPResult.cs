using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Transaction
{
    /// <summary>
    /// TODO: In-app purchase related response
    /// </summary>
    public class ReconcileIAPResult : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 985094533933992578;

        /// <summary>
        /// The user identifier associated with the logged-in profile.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The in-app purchase related data.
        /// </summary>
        public JObject IAPData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ReconcileIAPResult"/> message.
        /// </summary>
        public ReconcileIAPResult()
        {
            UserId = new XPlatformId();
            IAPData = new JObject();
        }
        /// <summary>
        /// Initializes a new <see cref="ReconcileIAPResult"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The user identifier associated with the request.</param>
        /// <param name="iapData">The requested in-app purchase related data.</param>
        public ReconcileIAPResult(XPlatformId userId, JObject iapData)
        {
            UserId = userId;
            IAPData = iapData;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            UserId.Stream(io);
            io.StreamJSON(ref IAPData, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, iap_data={IAPData.ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
