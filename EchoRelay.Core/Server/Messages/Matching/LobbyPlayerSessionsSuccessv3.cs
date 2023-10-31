using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client, indicating a <see cref="LobbyPlayerSessionsRequestv5"/> for sessions for given user identifiers succeeded.
    /// It contains the sessions for a given list of user identifiers.
    /// </summary>
    public class LobbyPlayerSessionsSuccessv3 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -6793175491028678295;

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

        /// <summary>
        /// The team index the player is being assigned on initial matching.
        /// </summary>
        public short TeamIndex;

        // TODO
        public ushort Unk1;
        public uint Unk2;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessv3"/> message.
        /// </summary>
        public LobbyPlayerSessionsSuccessv3()
        {
            UserId = new XPlatformId();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessv3"/> with the provided arguments.
        /// </summary>
        /// <param name="unk0">TODO: Unknown.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="playerSession">The player session token obtained for the requested player user identifier.</param>
        /// <param name="unk1">TODO: Unknown.</param>
        public LobbyPlayerSessionsSuccessv3(byte unk0, XPlatformId userId, Guid playerSession, short teamIndex, ushort unk1, uint unk2)
        {
            Unk0 = unk0;
            UserId = userId;
            PlayerSession = playerSession;
            TeamIndex = teamIndex;
            Unk1 = unk1;
            Unk2 = unk2;
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
            io.Stream(ref TeamIndex);
            io.Stream(ref Unk1);
            io.Stream(ref Unk2);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"unk0={Unk0}, " +
                $"user_id={UserId}, " +
                $"player_session={PlayerSession}, " +
                $"team_index={TeamIndex}, " +
                $"unk1={Unk1}, " +
                $"unk2={Unk2}" +
                $")";
        }
        #endregion
    }
}
