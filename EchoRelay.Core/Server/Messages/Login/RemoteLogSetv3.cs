using EchoRelay.Core.Game;
using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Server.Messages.Login
{
    /// <summary>
    /// A message from client to the server logging client-side data, as established by login profile data that tells the client how verbosely to log.
    /// It contains arbitrary log data about informational state changes, warnings, and errors.
    /// </summary>
    public class RemoteLogSetv3 : Message
    {
        #region Fields
        /// <summary>
        /// The unique 64-bit symbol denoting the type of message.
        /// </summary>
        public override long MessageTypeSymbol => 2615262521988737761;

        /// <summary>
        /// The user identifier associated with the request.
        /// </summary>
        public XPlatformId UserId;

        // TODO: Unknown
        public ulong Unk0;
        public ulong Unk1;
        public ulong Unk2;
        public ulong Unk3;

        private ulong _logLevel;
        /// <summary>
        /// The verbosity level which the log is targeting.
        /// </summary>
        public LoggingLevel LogLevel
        {
            get
            {
                return (LoggingLevel)_logLevel;
            }
            set
            {
                _logLevel = (ulong)value;
            }
        }

        /// <summary>
        /// The client-side logs provided to the server.
        /// </summary>
        public string[] Logs;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="RemoteLogSetv3"/> message.
        /// </summary>
        public RemoteLogSetv3()
        {
            UserId = new XPlatformId();
            Logs = Array.Empty<string>();
        }
        /// <summary>
        /// Initializes a new <see cref="RemoteLogSetv3"/> message with the provided arguments.
        /// </summary>
        /// <param name="userId">The user identifier associated with the request.</param>
        /// <param name="unk0">Unknown.</param>
        /// <param name="unk1">Unknown.</param>
        /// <param name="unk2">Unknown.</param>
        /// <param name="unk3">Unknown.</param>
        /// <param name="logLevel">The verbosity level which the log is targeting.</param>
        /// <param name="logs">The underlying client-side logs.</param>
        public RemoteLogSetv3(XPlatformId userId, ulong unk0, ulong unk1, ulong unk2, ulong unk3, LoggingLevel logLevel, string[] logs)
        {
            UserId = userId;
            Unk0 = unk0;
            Unk1 = unk1;
            Unk2 = unk2;
            Unk3 = unk3;
            LogLevel = logLevel;
            Logs = logs;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Streams the message data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public override void Stream(StreamIO io)
        {
            // Stream the first simple values
            UserId.Stream(io);
            io.Stream(ref Unk0);
            io.Stream(ref Unk1);
            io.Stream(ref Unk2);
            io.Stream(ref Unk3);
            io.Stream(ref _logLevel);

            // Next switch on our stream mode for more complex serialization.
            if (io.StreamMode == StreamMode.Read)
            {
                // Read the log count
                ulong logCount = io.ReadUInt64();

                // Read the list of offsets. These are offsets relative to the end of this offset table.
                // The offsets are only included for items after the first, as its location is trivially known.
                // We allocate an extra uint here to account for that, but start reading into the second position of our offset array.
                uint[] offsets = new uint[logCount];
                for (uint i = 1; i < offsets.Length; i++)
                    offsets[i] = io.ReadUInt32();

                // Define the start of the encoded JSON buffer messages
                long jsonBufferStart = io.Position;

                // Loop for each offset and read the JSON data there.
                Logs = new string[logCount];
                for (int i = 0; i < offsets.Length; i++)
                {
                    io.Position = jsonBufferStart + offsets[i];
                    Logs[i] = io.ReadString(true);
                }
            } 
            else
            {
                // Write our count of logs
                io.Write((ulong)Logs.Length);

                // The next part has (1) a list of offsets to JSON data, and (2) the JSON data for each log, encoded null-terminated, one after another.
                // We encode the JSON data here in a separate buffer, while writing the offsets to every log (except the first, it is trivial/known, so it is intuitively not part of this message format).
                StreamIO encodedBufferIO = new StreamIO(io.DefaultByteOrder, StreamMode.Write);
                for (int i = 0; i < Logs.Length; i++)
                {
                    // Write into our internal buffer
                    encodedBufferIO.Write(Logs[i], true);

                    // If this isn't the end of the stream (last item), we write the pointer to the next item.
                    io.Write((uint)encodedBufferIO.Position);
                }

                // Write the entire buffer out.
                io.Write(encodedBufferIO.ToArray());
                encodedBufferIO.Close();
            }
        }

        public override string ToString()
        {
            return $"{GetType().Name}(user_id={UserId}, unk0={Unk0}, unk1={Unk1}, unk2={Unk2}, unk3={Unk3}, log_level={LogLevel}, logs=[{string.Join(", ", Logs.Select(x => $"\"{x}\"").AsEnumerable())}])";
        }
        #endregion

        #region Enums
        /// <summary>
        /// The verbosity level which a log or logger is targeting.
        /// </summary>
        public enum LoggingLevel : int
        {
            Debug = 0x1,
            Info = 0x2,
            Warning = 0x4,
            Error = 0x8,
            Default = 0xE,
            Any = 0xF,
        };
        #endregion
    }
}
