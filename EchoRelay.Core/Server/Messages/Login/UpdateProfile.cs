using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to server requesting the server update the user's client profile.
    /// </summary>
    public class UpdateProfile : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 7878099332047717397;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The profile data requesting to be updated on the server-side.
        /// </summary>
        public AccountResource.AccountClientProfile ClientProfile;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="UpdateProfile"/> message.
        /// </summary>
        public UpdateProfile()
        {
            Session = new Guid();
            UserId = new XPlatformId();
            ClientProfile = new AccountResource.AccountClientProfile();
        }
        /// <summary>
        /// Initializes a new <see cref="UpdateProfile"/> message with the provided arguments.
        /// </summary>
        /// <param name="session">A session token which the user may have from an existing session.</param>
        /// <param name="userId">The identifier of the logged-in user requesting their profile.</param>
        /// <param name="clientProfile">The profile data requesting to be updated on the server.</param>
        /// <exception cref="ArgumentException">An exception is thrown if the session token is not the correct length.</exception>
        public UpdateProfile(Guid session, XPlatformId userId, AccountResource.AccountClientProfile clientProfile)
        {
            Session = session;
            UserId = userId;
            ClientProfile = clientProfile;
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
            io.StreamJSON(ref ClientProfile, true, JSONCompressionMode.None);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(session={Session}, user_id={UserId}, profile_request={JObject.FromObject(ClientProfile).ToString(Newtonsoft.Json.Formatting.None)})";
        }
        #endregion
    }
}
