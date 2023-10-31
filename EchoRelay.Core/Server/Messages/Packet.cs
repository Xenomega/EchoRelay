using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages
{
    /// <summary>
    /// Describes a packet sent across a websocket service connection. It wraps a set of underlying messages.
    /// </summary>
    public class Packet : List<Message>
    {
        /// <summary>
        /// The 64-bit header identifier for any <see cref="Packet"/>.
        /// </summary>
        public const ulong HEADER_ID = 0xbb8ce7a278bb40f6;

        /// <summary>
        /// The max size that any <see cref="Packet"/> may be.
        /// </summary>
        public const int MAX_SIZE = 0x8000;

        /// <summary>
        /// Creates a <see cref="Packet"/> with any provided messages initially added to it.
        /// </summary>
        /// <param name="messages">The messages to add to the <see cref="Packet"/> on construction.</param>
        public Packet(params Message[] messages)
        {
            // Add any provided messages to the packet.
            AddRange(messages);
        }


        /// <summary>
        /// Encodes a packet into bytes.
        /// </summary>
        /// <returns>Returns the encoded packet data.</returns>
        public byte[] Encode()
        {
            // Create a new stream
            StreamIO io = new StreamIO(ByteOrder.LittleEndian);

            // Write every message in our packet
            foreach (Message message in this)
            {
                // Write the message out.
                io.Write(HEADER_ID);
                io.Write(message.MessageTypeSymbol);

                // Encode the message, write the length, then the encoded message data.
                byte[] encodedMessage = message.Encode();
                io.Write((ulong)encodedMessage.Length);
                io.Write(encodedMessage);
            }

            // Obtain the bytes from our stream
            byte[] result = io.ToArray();
            return result;
        }

        /// <summary>
        /// Decodes a packet from the provided byte data.
        /// </summary>
        /// <param name="data">The data to decode a packet from.</param>
        /// <returns>Returns the decoded packet.</returns>
        /// <exception cref="IOException">This exception may occur if the packet failed to be decoded from the stream due to the packet format being malformed.</exception>
        public static Packet Decode(byte[] data)
        {
            // Create a stream out of the data, to read underlying messages.
            StreamIO io = new StreamIO(data, ByteOrder.LittleEndian);

            // Create a packet to store our messages.
            Packet packet = new Packet();

            // Read messages until we are at the end of our stream.
            try
            {
                while (io.Position != io.Length)
                {
                    // Verify the header identifier
                    if (io.ReadUInt64() != HEADER_ID)
                    {
                        throw new IOException("Invalid websocket packet header identifier");
                    }

                    // Read the message type and data length
                    long messageId = io.ReadInt64();
                    ulong messageDataLength = io.ReadUInt64();

                    // Verify the message data can be read from the rest of the stream.
                    if ((ulong)(io.Length - io.Position) < messageDataLength)
                    {
                        throw new IOException("Invalid websocket packet message length");
                    }

                    // Read the underlying data.
                    byte[] messageData = io.ReadBytes((int)messageDataLength);

                    // Create an instance of this message type and parse it.
                    Message message = MessageTypes.CreateMessage(messageId, false);
                    message.Decode(messageData);

                    // Add the successfully parsed message to our packet
                    packet.Add(message);
                }
            }
            finally
            {
                // Whether we encounter an exception or not, close our IO prior to returning.
                io.Close();
            }

            return packet;
        }
    }
}
