using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.ServerDB
{
    /// <summary>
    /// A message from server to game server, providing a challenge to the game server to complete prior to registration.
    /// NOTE: This is an unofficial message created for Echo Relay.
    /// TODO: This is unused in favor of lazy SERVERDB API key authentication.
    /// </summary>
    public class ERGameServerChallengeRequest : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 0x7777777777770900;

        /// <summary>
        /// The data to be sent to the client, which they will be expected to transform accordingly.
        /// </summary>
        public byte[] InputPayload;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="ERGameServerChallengeRequest"/>.
        /// </summary>
        public ERGameServerChallengeRequest()
        {
            InputPayload = Array.Empty<byte>();
        }
        /// <summary>
        /// Initializes a new <see cref="ERGameServerChallengeRequest"/> with the provided arguments.
        /// </summary>
        /// <param name="inputPayload">The data to be sent to the client, which they will be expected to transform accordingly.</param>
        public ERGameServerChallengeRequest(byte[] inputPayload)
        {
            InputPayload = inputPayload;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            if (io.StreamMode == StreamMode.Read)
                InputPayload = new byte[io.Length - io.Position]; 
            io.Stream(ref InputPayload);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(input_payload={Convert.ToHexString(InputPayload)})";
        }
        #endregion
    }
}
