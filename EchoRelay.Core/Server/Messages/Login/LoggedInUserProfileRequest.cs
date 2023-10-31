using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting the user profile for their logged-in account.
    /// </summary>
    public class LoggedInUserProfileRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -326745984434664080;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The request data for the underlying profile, indicating fields of interest.
        /// </summary>
        public JObject ProfileRequestData;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LoggedInUserProfileRequest"/> message.
        /// </summary>
        public LoggedInUserProfileRequest()
        {
            Session = new Guid();
            UserId = new XPlatformId();
            ProfileRequestData = new JObject();
        }
        /// <summary>
        /// Initializes a new <see cref="LoggedInUserProfileRequest"/> message with the provided arguments.
        /// </summary>
        /// <param name="session">A session token which the user may have from an existing session.</param>
        /// <param name="userId">The identifier of the logged-in user requesting their profile.</param>
        /// <param name="profileRequestData">The request data for the underlying profile, indicating fields of interest.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the session token is not the correct length.</exception>
        public LoggedInUserProfileRequest(Guid session, XPlatformId userId, JObject profileRequestData)
        {
            Session = session;
            UserId = userId;
            ProfileRequestData = profileRequestData;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Session);
            UserId.Stream(io);
            io.StreamJSON(ref ProfileRequestData, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(session={Session}, user_id={UserId}, profile_request={ProfileRequestData.ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
