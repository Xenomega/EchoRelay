using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from client to server, asking it to obtain game server sessions for a given list of user identifiers.
    /// </summary>
    public class LobbyPlayerSessionsRequestv5 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -7281482002396079611;

        /// <summary>
        /// The user's session token.
        /// </summary>
        public Guid Session;
        /// <summary>
        /// The user identifier.
        /// </summary>
        public XPlatformId UserId;
        /// <summary>
        /// The matching-related session token for the current matchmaker operation.
        /// </summary>
        public Guid MatchingSession;
        /// <summary>
        /// A symbol representing the platform requested for the session.
        /// </summary>
        public long PlatformSymbol;
        /// <summary>
        /// The user identifiers for the players to obtain sessions for.
        /// </summary>
        public XPlatformId[] PlayerUserIds;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsRequestv5"/> message.
        /// </summary>
        public LobbyPlayerSessionsRequestv5()
        {
            Session = new Guid();
            UserId = new XPlatformId();
            PlayerUserIds = Array.Empty<XPlatformId>();
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
            io.Stream(ref MatchingSession);
            io.Stream(ref PlatformSymbol);

            // Read/write our count and ensure the array is prepared to stream its data.
            if (io.StreamMode == StreamMode.Read)
            {
                ulong playerUserIds = io.ReadUInt64();
                PlayerUserIds = new XPlatformId[playerUserIds];
            }
            else
            {
                io.Write((ulong)PlayerUserIds.Length);
            }

            // Stream all user id data.
            for (int i = 0; i < PlayerUserIds.Length; i++)
            {
                if (PlayerUserIds[i] == null)
                    PlayerUserIds[i] = new XPlatformId();
                PlayerUserIds[i].Stream(io);
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"session={Session}, " +
                $"user_id={UserId}, " +
                $"matching_session={MatchingSession}, " +
                $"platform={PlatformSymbol}, " +
                $"player_user_ids=[{string.Join(", ", PlayerUserIds.AsEnumerable())}]" +
                $")";
        }
        #endregion
    }
}
