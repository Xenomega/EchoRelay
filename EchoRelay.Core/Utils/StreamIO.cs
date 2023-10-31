using Newtonsoft.Json;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;

namespace EchoRelay.Core.Utils
{
    /// <summary>
    /// Describes the byte order of a given architecture or value.
    /// </summary>
    public enum ByteOrder
    {
        LittleEndian,
        BigEndian,
    }

    /// <summary>
    /// Describes whether <see cref="StreamIO"/> is reading or writing data.
    /// </summary>
    public enum StreamMode
    {
        Read,
        Write,
    }

    /// <summary>
    /// Describes the compression type for a JSON value written with <see cref="StreamIO"/>
    /// </summary>
    public enum JSONCompressionMode
    {
        None,
        Zlib,
        Zstd,
    }

    /// <summary>
    /// An interface that entails an object being streamable with <see cref="StreamIO"/>.
    /// </summary>
    public interface IStreamable
    {
        /// <summary>
        /// Streams the data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        void Stream(StreamIO io);
    }

    /// <summary>
    /// A stream wrapper that provides read/write functionality for various types.
    /// </summary>
    public class StreamIO
    {
        #region Fields/Properties
        /// <summary>
        /// JSON serializer settings that prevent the "id" key from being lost when serializing our data.
        /// </summary>
        public static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings { 
            PreserveReferencesHandling = PreserveReferencesHandling.None,
            NullValueHandling = NullValueHandling.Ignore,
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
        };
        /// <summary>
        /// The underlying stream which this provider wraps.
        /// </summary>
        private MemoryStream _stream;
        /// <summary>
        /// The byte order of the current executing machine.
        /// </summary>
        private ByteOrder _machineByteOrder;

        /// <summary>
        /// The default byte order to use for streaming operations.
        /// </summary>
        public ByteOrder DefaultByteOrder { get; }
        /// <summary>
        /// The read/write mode to be using when streaming data.
        /// </summary>
        public StreamMode StreamMode { get; set; }
        /// <summary>
        /// Indicates the byte position within the stream.
        /// </summary>
        public long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }
        /// <summary>
        /// Indicates the length of the stream.
        /// </summary>
        public long Length
        {
            get
            {
                return _stream.Length;
            }
        }
        #endregion

        #region Constructors
        public StreamIO(ByteOrder defaultByteOrder = ByteOrder.LittleEndian, StreamMode streamMode = StreamMode.Write)
        {
            // Set our stream parameters;
            _stream = new MemoryStream();
            _machineByteOrder = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            DefaultByteOrder = defaultByteOrder;
            StreamMode = streamMode;
        }
        public StreamIO(byte[] data, ByteOrder defaultByteOrder = ByteOrder.LittleEndian, StreamMode streamMode = StreamMode.Read)
        {
            // Set our stream parameters
            _stream = new MemoryStream(data);
            _machineByteOrder = BitConverter.IsLittleEndian ? ByteOrder.LittleEndian : ByteOrder.BigEndian;
            DefaultByteOrder = defaultByteOrder;
            StreamMode = streamMode;
        }
        #endregion

        #region Methods
        #region General Methods
        public void Close()
        {
            _stream.Close();
        }
        /// <summary>
        /// Returns a byte array of the stream data.
        /// </summary>
        /// <returns>Returns a byte array of the current stream data.</returns>
        public byte[] ToArray()
        {
            return _stream.ToArray();
        }
        #endregion

        #region Read Methods
        /// <summary>
        /// Reads a single byte from the stream.
        /// </summary>
        /// <returns>Returns the read byte.</returns>
        /// <exception cref="IOException">An exception thrown if the end of stream has been reached.</exception>
        public byte ReadByte()
        {
            // Read a byte, if it's -1, it failed to read. Otherwise, it succeeded.
            int value = _stream.ReadByte();
            if (value == -1)
            {
                throw new IOException($"StreamIO failed to read byte");
            }
            return (byte)value;
        }
        /// <summary>
        /// Reads a span of bytes from the stream.
        /// </summary>
        /// <param name="size">The amount of bytes to read.</param>
        /// <returns>Returns the read bytes.</returns>
        /// <exception cref="IOException">An exception thrown if the end of stream has been reached.</exception>
        public Span<byte> ReadBytesSpan(int size)
        {
            // Create a span of the requested size and read the data into it.
            Span<byte> value = new Span<byte>(new byte[size]);
            int readSize = _stream.Read(value);
            if (readSize != size)
            {
                throw new IOException($"StreamIO failed to read {size} bytes, obtained {readSize}");
            }
            return value;
        }
        private T ReadValue<T>(ByteOrder byteOrder) where T : struct, IComparable
        {
            // Read the value as bytes
            Span<byte> valueBytes = ReadBytesSpan(Marshal.SizeOf(typeof(T)));

            // Determine if we should reverse the byte order
            if (byteOrder != _machineByteOrder)
            {
                valueBytes.Reverse();
            }

            // Cast the memory to the type we are interested in.
            Span<T> value = MemoryMarshal.Cast<byte, T>(valueBytes);

            // Return the value
            return value[0];
        }

        /// <summary>
        /// Reads an array of bytes from the stream.
        /// </summary>
        /// <param name="size">The amount of bytes to read.</param>
        /// <returns>Returns the read bytes.</returns>
        /// <exception cref="IOException">An exception thrown if the end of stream has been reached.</exception>
        public byte[] ReadBytes(int size)
        {
            // Read bytes as an array.
            return ReadBytesSpan(size).ToArray();
        }
        public short ReadInt16()
        {
            return ReadInt16(DefaultByteOrder);
        }
        public short ReadInt16(ByteOrder byteOrder)
        {
            return ReadValue<short>(byteOrder);
        }
        public ushort ReadUInt16()
        {
            return ReadUInt16(DefaultByteOrder);
        }
        public ushort ReadUInt16(ByteOrder byteOrder)
        {
            return ReadValue<ushort>(byteOrder);
        }
        public int ReadInt32()
        {
            return ReadInt32(DefaultByteOrder);
        }
        public int ReadInt32(ByteOrder byteOrder)
        {
            return ReadValue<int>(byteOrder);
        }
        public uint ReadUInt32()
        {
            return ReadUInt32(DefaultByteOrder);
        }
        public uint ReadUInt32(ByteOrder byteOrder)
        {
            return ReadValue<uint>(byteOrder);
        }
        public long ReadInt64()
        {
            return ReadInt64(DefaultByteOrder);
        }
        public long ReadInt64(ByteOrder byteOrder)
        {
            return ReadValue<long>(byteOrder);
        }
        public ulong ReadUInt64()
        {
            return ReadUInt64(DefaultByteOrder);
        }
        public ulong ReadUInt64(ByteOrder byteOrder)
        {
            return ReadValue<ulong>(byteOrder);
        }
        public Int128 ReadInt128()
        {
            return ReadInt128(DefaultByteOrder);
        }
        public Int128 ReadInt128(ByteOrder byteOrder)
        {
            return ReadValue<Int128>(byteOrder);
        }
        public UInt128 ReadUInt128()
        {
            return ReadUInt128(DefaultByteOrder);
        }
        public UInt128 ReadUInt128(ByteOrder byteOrder)
        {
            return ReadValue<UInt128>(byteOrder);
        }
        public float ReadFloat()
        {
            return ReadFloat(DefaultByteOrder);
        }
        public float ReadFloat(ByteOrder byteOrder)
        {
            return ReadValue<float>(byteOrder);
        }
        public double ReadDouble()
        {
            return ReadDouble(DefaultByteOrder);
        }
        public double ReadDouble(ByteOrder byteOrder)
        {
            return ReadValue<double>(byteOrder);
        }
        public string ReadString(bool nullTerminated = true)
        {
            // Read bytes until end of file or null termination.
            List<byte> valueBytes = new List<byte>();
            while (true)
            {
                // If we're at the end of the stream and not reading a null terminated string, stop
                if (!nullTerminated && Position >= Length)
                    break;

                // Read a byte
                byte valueByte = ReadByte();

                // If the byte is a null terminator and this is a null terminated string, stop.
                if (nullTerminated && valueByte == 0)
                    break;

                // Add the byte to our list
                valueBytes.Add(valueByte);
            }

            // Convert the bytes to a string and return them
            string value = Encoding.UTF8.GetString(valueBytes.ToArray());
            return value;
        }
        public string ReadString(int size)
        {
            // Read the amount of bytes requested
            byte[] valueBytes = ReadBytes(size);

            // Convert the bytes to a string and return them
            string value = Encoding.UTF8.GetString(valueBytes.ToArray());
            return value.TrimEnd('\0');
        }
        public Guid ReadGuid()
        {
            return ReadGuid(DefaultByteOrder);
        }
        public Guid ReadGuid(ByteOrder byteOrder)
        {
            // TODO: Support this later.
            if (byteOrder == ByteOrder.BigEndian)
                throw new NotImplementedException("Reading GUID in big endian byte order is unsupported");

            return ReadValue<Guid>(byteOrder);
        }
        public IPAddress ReadIPv4Address()
        {
            return ReadIPv4Address(DefaultByteOrder);
        }
        public IPAddress ReadIPv4Address(ByteOrder byteOrder)
        {
            // Read four bytes
            Span<byte> bytes = ReadBytesSpan(4);

            // If the byte order is not big endian, swap it, as the IPAddress type expects this.
            if (byteOrder != ByteOrder.BigEndian)
                bytes.Reverse();

            return new IPAddress(bytes);
        }
        public T ReadJSON<T>(bool nullTerminated = true, JSONCompressionMode compressionMode = JSONCompressionMode.None, ByteOrder? byteOrder = null)
        {
            // If there is no compression, we simply read the underlying value.
            if (compressionMode == JSONCompressionMode.None)
            {
                // Read and deserialize the JSON encoded value.
                string encodedJson = ReadString(nullTerminated);
                T? value = JsonConvert.DeserializeObject<T>(encodedJson, JsonSerializerSettings);

                // If we successfully deserialized the value of our desired type, set it. Otherwise throw an exception. 
                if (value == null)
                {
                    throw new InvalidDataException($"Could not stream null terminated JSON string, failed to decode type '{typeof(T).Name}'");
                }
                return value;
            }
            else if (compressionMode == JSONCompressionMode.Zlib)
            {
                // Zlib mode writes a 64-bit decompressed length, followed by the compressed buffer (until end of file).
                ulong decompressedLength = ReadUInt64(byteOrder ?? DefaultByteOrder);
                byte[] data = ReadBytes((int)(Length - Position));

                // Decompress the data
                data = Compression.DecompressZlib(data);

                // Read the uncompressed data.
                StreamIO uncompressedIO = new StreamIO(data, DefaultByteOrder, StreamMode.Read);
                T value = uncompressedIO.ReadJSON<T>(nullTerminated, JSONCompressionMode.None, byteOrder);
                uncompressedIO.Close();
                return value;
            }
            else if (compressionMode == JSONCompressionMode.Zstd)
            {
                // Zstd mode writes a 32-bit decompressed length, followed by the compressed buffer (until end of file).
                uint decompressedLength = ReadUInt32(byteOrder ?? DefaultByteOrder);
                byte[] data = ReadBytes((int)(Length - Position));

                // Decompress the data
                data = Compression.DecompressZstd(data);

                // Read the uncompressed data.
                StreamIO uncompressedIO = new StreamIO(data, DefaultByteOrder, StreamMode.Read);
                T value = uncompressedIO.ReadJSON<T>(nullTerminated, JSONCompressionMode.None, byteOrder);
                uncompressedIO.Close();
                return value;
            }

            // Throw an exception if an unsupported compression mode was provided.
            throw new ArgumentException($"Invalid {nameof(JSONCompressionMode)} ({compressionMode}) provided to {nameof(StreamIO)}");
        }
        #endregion

        #region Write Methods
        public void Write(byte value)
        {
            _stream.WriteByte(value);
        }
        public void Write(Span<byte> value)
        {
            _stream.Write(value);
        }
        public void Write(Span<byte> value, int size)
        {
            _stream.Write(value.Slice(0, size));
        }
        public void Write(short value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(short value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(ushort value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(ushort value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(int value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(int value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(uint value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(uint value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(long value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(long value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(ulong value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(ulong value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(Int128 value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(Int128 value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(UInt128 value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(UInt128 value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(float value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(float value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(double value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(double value, ByteOrder byteOrder)
        {
            WriteValue(value, byteOrder);
        }
        public void Write(string value, bool nullTerminated = true)
        {
            Write(Encoding.UTF8.GetBytes(value));
            if (nullTerminated)
            {
                Write((byte)0x00);
            }
        }
        public void Write(string value, int fixedSize)
        {
            // Ensure our string doesn't exceed the provided size, and convert it to bytes.
            byte[] valueBytes = Encoding.UTF8.GetBytes(value.Substring(0, Math.Min(fixedSize, value.Length)));

            // Write the string data.
            Write(valueBytes);
            
            // If there is remaining bytes to write to reach our requested size, fill with zero padding.
            if (valueBytes.Length < fixedSize)
                Write(new byte[fixedSize - valueBytes.Length]);
        }
        public void Write(Guid value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(Guid value, ByteOrder byteOrder)
        {
            // TODO: Support this later.
            if (byteOrder == ByteOrder.BigEndian)
                throw new NotImplementedException("Writing GUID in big endian byte order is unsupported");

            WriteValue(value, byteOrder);
        }
        public void Write(IPAddress value)
        {
            Write(value, DefaultByteOrder);
        }
        public void Write(IPAddress value, ByteOrder byteOrder)
        {
            // Obtain the address bytes, which are always in big endian.
            byte[] addrBytes = value.GetAddressBytes();
            if (byteOrder != ByteOrder.BigEndian)
                addrBytes.Reverse();

            // Obtain the bytes for this IP address and write them
            Write(addrBytes, 4);
        }
        public void WriteJSON<T>(T value, bool nullTerminated = true, JSONCompressionMode compressionMode = JSONCompressionMode.None, ByteOrder? byteOrder = null)
        {
            // If there is no compression, we simply write the underlying value.
            if (compressionMode == JSONCompressionMode.None)
            {
                // JSON encode the value
                string encodedJson = JsonConvert.SerializeObject(value, JsonSerializerSettings);

                // Write it as a null terminated string
                Write(encodedJson, nullTerminated);
                return;
            }
            else if (compressionMode == JSONCompressionMode.Zlib)
            {
                // Obtain the uncompressed data.
                StreamIO uncompressedIO = new StreamIO(DefaultByteOrder, StreamMode.Write);
                uncompressedIO.WriteJSON(value, nullTerminated, JSONCompressionMode.None, byteOrder);
                byte[] data = uncompressedIO.ToArray();
                uncompressedIO.Close();

                // Zlib mode writes a 64-bit decompressed length, followed by the compressed buffer (until end of file).
                // Compress the data and write it.
                ulong decompressedLength = (ulong)data.Length;
                data = Compression.CompressZlib(data);
                Write(decompressedLength, byteOrder ?? DefaultByteOrder);
                Write(data);
                return;
            }
            else if (compressionMode == JSONCompressionMode.Zstd)
            {
                // Obtain the uncompressed data.
                StreamIO uncompressedIO = new StreamIO(DefaultByteOrder, StreamMode.Write);
                uncompressedIO.WriteJSON(value, nullTerminated, JSONCompressionMode.None, byteOrder);
                byte[] data = uncompressedIO.ToArray();
                uncompressedIO.Close();

                // Zstd mode writes a 32-bit decompressed length, followed by the compressed buffer (until end of file).
                // Compress the data and write it.
                uint decompressedLength = (uint)data.Length;
                data = Compression.CompressZstd(data);
                Write(decompressedLength, byteOrder ?? DefaultByteOrder);
                Write(data);
                return;
            }

            // Throw an exception if an unsupported compression mode was provided.
            throw new ArgumentException($"Invalid {nameof(JSONCompressionMode)} ({compressionMode}) provided to {nameof(StreamIO)}");
        }
        private void WriteValue<T>(T value, ByteOrder byteOrder) where T : struct, IComparable
        {
            // Read the value as bytes
            Span<T> valueSpan = MemoryMarshal.CreateSpan(ref value, 1);
            Span<byte> valueSpanBytes = MemoryMarshal.Cast<T, byte>(valueSpan);

            // Determine if we should reverse the byte order
            if (byteOrder != _machineByteOrder)
            {
                // Copy to a new buffer so as not to alter the original value.
                Span<byte> valueSpanBytesReversed = new Span<byte>(new byte[valueSpanBytes.Length]);
                valueSpanBytes.CopyTo(valueSpanBytesReversed);
                valueSpanBytesReversed.Reverse();
                valueSpanBytes = valueSpanBytesReversed;
            }

            // Write the bytes
            Write(valueSpanBytes);
        }
        #endregion

        #region Stream Methods
        /// <summary>
        /// Streams into/from the provided value based on the <see cref="StreamMode"/> set.
        /// </summary>
        /// <param name="value">The value to stream.</param>
        public void Stream(ref byte value)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadByte();
            }
            else
            {
                Write(value);
            }
        }
        /// <summary>
        /// Streams into/from the provided value based on the <see cref="StreamMode"/> set.
        /// </summary>
        /// <param name="value">The value to stream.</param>
        public void Stream(ref byte[] value)
        {
            Stream(ref value, value.Length);
        }
        /// <summary>
        /// Streams into/from the provided value based on the <see cref="StreamMode"/> set.
        /// </summary>
        /// <param name="value">The value to stream.</param>
        public void Stream(ref byte[] value, int size)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadBytes(size);
            }
            else
            {
                Write(value, size);
            }
        }
        public void Stream(ref short value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref short value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadInt16(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref ushort value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref ushort value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadUInt16(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref int value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref int value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadInt32(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref uint value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref uint value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadUInt32(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref long value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref long value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadInt64(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref ulong value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref ulong value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadUInt64(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref Int128 value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref Int128 value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadInt128(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref UInt128 value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref UInt128 value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadUInt128(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref float value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref float value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadFloat(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref double value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref double value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadDouble(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref string value, bool nullTerminated = true)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadString(nullTerminated);
            }
            else
            {
                Write(value, nullTerminated);
            }
        }
        public void Stream(ref string value, int size)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadString(size);
            }
            else
            {
                Write(value, size);
            }
        }
        public void Stream(ref Guid value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref Guid value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadGuid(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void Stream(ref IPAddress value)
        {
            Stream(ref value, DefaultByteOrder);
        }
        public void Stream(ref IPAddress value, ByteOrder byteOrder)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadIPv4Address(byteOrder);
            }
            else
            {
                Write(value, byteOrder);
            }
        }
        public void StreamJSON<T>(ref T value, bool nullTerminated = true, JSONCompressionMode compressionMode = JSONCompressionMode.None, ByteOrder? byteOrder = null)
        {
            if (StreamMode == StreamMode.Read)
            {
                value = ReadJSON<T>(nullTerminated, compressionMode, byteOrder);
            }
            else
            {
                WriteJSON(value, nullTerminated, compressionMode, byteOrder);
            }
        }
        public void Stream(IStreamable value)
        {
            value.Stream(this);
        }
        #endregion

        #endregion

    }
}
