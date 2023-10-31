using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client, indicating a <see cref="LobbyPlayerSessionsRequestv5"/> for sessions for given user identifiers succeeded.
    /// It contains the sessions for a given list of user identifiers.
    /// </summary>
    public class LobbyPlayerSessionsSuccessUnk1 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => -40104227197879335;

        /// <summary>
        /// The matching-related session token for the current matchmaker operation.
        /// </summary>
        public Guid MatchingSession;

        /// <summary>
        /// The player session token obtained for the requested player user identifier.
        /// </summary>
        public Guid[] PlayerSessions;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessUnk1"/> message.
        /// </summary>
        public LobbyPlayerSessionsSuccessUnk1()
        {
            PlayerSessions = Array.Empty<Guid>();
        }
        /// <summary>
        /// Initializes a new <see cref="LobbyPlayerSessionsSuccessUnk1"/> with the provided arguments.
        /// </summary>
        /// <param name="matchingSession">The matching-related session token for the current matchmaker operation.</param>
        /// <param name="playerSessions">The player session token obtained for the requested player user identifier.</param>
        public LobbyPlayerSessionsSuccessUnk1(Guid matchingSession, Guid[] playerSessions)
        {
            MatchingSession = matchingSession;
            PlayerSessions = playerSessions;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            // TODO: This was never verified to be a count, so if its wrong, it may produce a crash.
            // This may just be an unrelated integer and the size of the structure may not change normally.
            if (io.StreamMode == StreamMode.Read)
            {
                ulong count = io.ReadUInt64();
                PlayerSessions = new Guid[count];
            } 
            else
            {
                io.Write((ulong)PlayerSessions.Length);
            }

            // Stream the match session
            io.Stream(ref MatchingSession);

            // Stream all player sessions
            for (int i = 0; i < PlayerSessions.Length; i++)
                io.Stream(ref PlayerSessions[i]);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"matching_session={MatchingSession}, " +
                $"player_sessions=[{string.Join(", ", PlayerSessions.AsEnumerable())}]" +
                $")";
        }
        #endregion
    }
}
