using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Matching
{
    /// <summary>
    /// A message from server to client indicating that a request to create/join/find a game server
    /// session succeeded.
    /// </summary>
    public class LobbySessionSuccessv4 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 7876201346521829646;

        /// <summary>
        /// A symbol representing the gametype requested for the session.
        /// </summary>
        public long GameTypeSymbol;
        /// <summary>
        /// The matching-related session token for the current matchmaker operation.
        /// </summary>
        public Guid MatchingSession;
        /// <summary>
        /// The server-selected game server endpoint that the client should connect to.
        /// </summary>
        public LobbyPingRequestv3.EndpointData Endpoint;


        /// <summary>
        /// The team index of the player. -1 for the server to assign a team.
        /// </summary>
        public short TeamIndex;

        // TODO
        public uint Unk1;

        /// <summary>
        /// Flags indicating the parameters for packet encoding for the server.
        /// </summary>
        public ulong ServerEncoderFlags;
        /// <summary>
        /// Flags indicating the parameters for packet encoding for the server.
        /// </summary>
        public ulong ClientEncoderFlags;
        /// <summary>
        /// The sequence id that the server should start with when a connection is established.
        /// </summary>
        public ulong ServerSequenceId;
        /// <summary>
        /// The HMAC key that should be used by the server.
        /// </summary>
        public byte[] ServerMacKey;
        /// <summary>
        /// The AES key that should be used by the server.
        /// </summary>
        public byte[] ServerEncKey;
        /// <summary>
        /// The random key used to seed the Keccak1600-F sponge construction RNG, which steps for each packet
        /// to generate AES initialization vectors to encrypt it.
        /// </summary>
        public byte[] ServerRandomKey;
        /// <summary>
        /// The sequence id that the client should start with when a connection is established.
        /// </summary>
        public ulong ClientSequenceId;
        /// <summary>
        /// The HMAC key that should be used by the client.
        /// </summary>
        public byte[] ClientMacKey;
        /// <summary>
        /// The AES key that should be used by the server.
        /// </summary>
        public byte[] ClientEncKey;
        /// <summary>
        /// The random key used to seed the Keccak1600-F sponge construction RNG, which steps for each packet
        /// to generate AES initialization vectors to encrypt it.
        /// </summary>
        public byte[] ClientRandomKey;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="LobbySessionSuccessv5"/> message.
        /// </summary>
        public LobbySessionSuccessv4()
        {
            Endpoint = new LobbyPingRequestv3.EndpointData();
            ServerMacKey = new byte[0x20];
            ServerEncKey = new byte[0x20];
            ServerRandomKey = new byte[0x20];
            ClientMacKey = new byte[0x20];
            ClientEncKey = new byte[0x20];
            ClientRandomKey = new byte[0x20];
        }
        public LobbySessionSuccessv4(long gameTypeSymbol, Guid matchingSession, LobbyPingRequestv3.EndpointData endpoint, short teamIndex, uint unk1, ulong serverEncoderFlags, ulong clientEncoderFlags, ulong serverSequenceId, byte[] serverMacKey, byte[] serverEncKey, byte[] serverRandomKey, ulong clientSequenceId, byte[] clientMacKey, byte[] clientEncKey, byte[] clientRandomKey)
        {
            GameTypeSymbol = gameTypeSymbol;
            MatchingSession = matchingSession;
            Endpoint = endpoint;
            TeamIndex = teamIndex;
            Unk1 = unk1;
            ServerEncoderFlags = serverEncoderFlags;
            ClientEncoderFlags = clientEncoderFlags;
            ServerSequenceId = serverSequenceId;
            ServerMacKey = serverMacKey;
            ServerEncKey = serverEncKey;
            ServerRandomKey = serverRandomKey;
            ClientSequenceId = clientSequenceId;
            ClientMacKey = clientMacKey;
            ClientEncKey = clientEncKey;
            ClientRandomKey = clientRandomKey;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            io.Stream(ref GameTypeSymbol);
            io.Stream(ref MatchingSession);
            Endpoint.Stream(io);
            io.Stream(ref TeamIndex);
            io.Stream(ref Unk1);
            io.Stream(ref ServerEncoderFlags);
            io.Stream(ref ClientEncoderFlags);
            io.Stream(ref ServerSequenceId);
            io.Stream(ref ServerMacKey);
            io.Stream(ref ServerEncKey);
            io.Stream(ref ServerRandomKey);
            io.Stream(ref ClientSequenceId);
            io.Stream(ref ClientMacKey);
            io.Stream(ref ClientEncKey);
            io.Stream(ref ClientRandomKey);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(" +
                $"game_type={GameTypeSymbol}, " +
                $"matching_session={MatchingSession}, " +
                $"endpoint={Endpoint}, " +
                $"team_index={TeamIndex}, " +
                $"unk1={Unk1}, " +
                $"server_encoder_flags={ServerEncoderFlags}, " +
                $"client_encoder_flags={ClientEncoderFlags}, " +
                $"server_seq_id={ServerSequenceId}, " +
                $"server_mac_key={Convert.ToHexString(ServerMacKey)}, " +
                $"server_enc_key={Convert.ToHexString(ServerEncKey)}, " +
                $"server_random_key={Convert.ToHexString(ServerRandomKey)}, " +
                $"client_seq_id={ClientSequenceId}, " +
                $"client_mac_key={Convert.ToHexString(ClientMacKey)}, " +
                $"client_enc_key={Convert.ToHexString(ClientEncKey)}, " +
                $"client_random_key={Convert.ToHexString(ClientRandomKey)}" +
                $")";
        }
        #endregion
    }
}
