using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client, indicating a <see cref="LobbyPlayerSessionsRequestv5"/> for sessions for given user identifiers succeeded.
    /// It contains the sessions for a given list of user identifiers.
    /// </summary>
    public class LobbyPlayerSessionsSuccessv2 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -6793175491028678296;

        // TODO
        public byte Unk0;

        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The player session token obtained for the requested player user identifier.
        /// </summary>
        public Guid PlayerSession;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessv2"/> message.
        /// </summary>
        public LobbyPlayerSessionsSuccessv2()
        {
            UserId = new XPlatformId();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessv2"/> message.
        /// </summary>
        /// <param name="unk0">TODO: Unknown.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="playerSession">The player session token obtained for the requested player user identifier.</param>
        public LobbyPlayerSessionsSuccessv2(byte unk0, XPlatformId userId, Guid playerSession)
        {
            Unk0 = unk0;
            UserId = userId;
            PlayerSession = playerSession;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref Unk0);
            UserId.Stream(io);
            io.Stream(ref PlayerSession);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"unk0={Unk0}, " +
                $"user_id={UserId}, " +
                $"player_session={PlayerSession}" +
                $")";
        }
        #endregion
    }
}
