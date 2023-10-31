using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages
{
    /// <summary>
    /// One of many websocket messages sent between the client and server in a <see cref="Packet"/>.
    /// </summary>
    public abstract class Message : IStreamable
    {
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public abstract long MessageTypeSymbol { get; }

        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public abstract void Stream(StreamIO io);

        /// <summary>
        /// Encodes the message into bytes.
        /// </summary>
        /// <returns>Returns the encoded message bytes.</returns>
        public byte[] Encode()
        {
            // Create a new stream and write our data to it.
            StreamIO io = new StreamIO(ByteOrder.LittleEndian, StreamMode.Write);
            Stream(io);
            io.Close();

            // Return the streamed data.
            return io.ToArray();
        }

        /// <summary>
        /// Decodes the message from provided bytes.
        /// </summary>
        /// <param name="data">The data to decode the message from.</param>
        public void Decode(byte[] data)
        {
            // Create a stream for this data.
            StreamIO io = new StreamIO(data, ByteOrder.LittleEndian, StreamMode.Read);

            // Stream the data. If we're debugging, assert we read ALL the data (flagging incorrect implementation).
            Stream(io);

            #if DEBUG
            if (io.Position != io.Length)
            {
                //throw new IOException($"[DEBUGGING] Message decoding did not read all data for message type: {GetType().Name}");
            }
            #endif

            // Close the stream.
            io.Close();
        }
    }

    /// <summary>
    /// An unimplemented message. This stores the message identifier and data in a raw form.
    /// </summary>
    public class UnimplementedMessage : Message
    {
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol { get; }

        /// <summary>
        /// The data contained within the unimplemented message.
        /// </summary>
        public byte[] Data;


        /// <summary>
        /// Initializes a new <see cref="UnimplementedMessage"/> message.
        /// </summary>
        public UnimplementedMessage(long messageId)
        {
            MessageTypeSymbol = messageId;
            Data = new byte[0];
        }

        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            // If we're reading, allocate enough space to read in the rest of the stream.
            if (io.StreamMode == StreamMode.Read)
            {
                Data = new byte[io.Length - io.Position];
            }

            // Stream our data in/out.
            io.Stream(ref Data);
        }

        public override string ToString()
        {
            return $"{GetType().Name}(message_type_symbol={MessageTypeSymbol}, data={Convert.ToHexString(Data)})";
        }

    }
}