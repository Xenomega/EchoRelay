namespace EchoRelay.Core.Game
{
    /// <summary>
    /// Describes packet encoding settings for one party in a game server <-> client connection.
    /// </summary>
    public class PacketEncoderSettings
    {
        #region Properties
        /// <summary>
        /// Indicates whether encryption should be used for each packet to ensure confidentiality.
        /// </summary>
        public bool EncryptionEnabled { get; }
        /// <summary>
        /// Indicates whether MACs should be attached to each packet to verify their integrity.
        /// </summary>
        public bool MacEnabled { get; }
        /// <summary>
        /// The byte size of the MAC output packets should use. It must not exceed 64 (512 bit), 
        /// as the MAC is cut from the front of the HMAC-SHA512 digest.
        /// </summary>
        public int MacDigestSize { get; }
        /// <summary>
        /// The iteration count, if set to zero, dictates MAC should be plain HMAC-SHA512.
        /// TODO: (Unverified) If iteration count is greater than zero, PBKDF2 HMAC-SHA512 will be 
        /// used with the given number of iterations.
        /// </summary>
        public int MacPBKDF2IterationCount { get; }
        /// <summary>
        /// The byte size of the HMAC-SHA512 key that should be used.
        /// </summary>
        public int MacKeySize { get; }
        /// <summary>
        /// The byte size of the AES-CBC key to be used. Default is 32 (AES-256-CBC).
        /// </summary>
        public int EncryptionKeySize { get; }
        /// <summary>
        /// The byte size of the random key which the sponge/duplex construction  Keccak-F permutation (1600-bit)
        /// random number generator (RNG) should ingest to seed itself. Both parties are provided eachother's packet encoding
        /// settings. Each packet sent should be encrypted/decrypted using the party's encryption key, where the 16-byte initialization
        /// vector (IV) is generated newly by the RNG for every step in sequence id.
        /// </summary>
        public int RandomKeySize { get; }
        #endregion

        #region Constructors
        public PacketEncoderSettings(bool encryptionEnabled = true, bool macEnabled = true, int macDigestSize = 64, int macPBKDF2IterationCounter = 0, int macKeySize = 32, int encryptionKeySize = 32, int randomKeySize = 32)
        {
            // Set our provided fields
            EncryptionEnabled = encryptionEnabled;
            MacEnabled = macEnabled;
            MacDigestSize = macDigestSize;
            MacPBKDF2IterationCount = macPBKDF2IterationCounter;
            MacKeySize = macKeySize;
            EncryptionKeySize = encryptionKeySize;
            RandomKeySize = randomKeySize;
        }
        public PacketEncoderSettings(ulong flags)
        {
            // Decode our information.
            EncryptionEnabled = (flags & 1) != 0;
            MacEnabled = (flags & 2) != 0;
            MacDigestSize = (int)(flags >> 2) & 0xFFF;
            MacPBKDF2IterationCount = (int)(flags >> 14) & 0xFFF;
            MacKeySize = (int)(flags >> 26) & 0xFFF;
            EncryptionKeySize = (int)(flags >> 38) & 0xFFF;
            RandomKeySize = (int)(flags >> 50) & 0xFFF;
        }
        #endregion

        #region Operators
        public static explicit operator PacketEncoderSettings(ulong flags) => new PacketEncoderSettings(flags);
        public static explicit operator ulong(PacketEncoderSettings packetEncoderSettings)
        {
            ulong flags = (uint)(packetEncoderSettings.EncryptionEnabled ? 1 : 0);
            flags |= (uint)(packetEncoderSettings.MacEnabled ? 1 << 1 : 0);
            flags |= (ulong)packetEncoderSettings.MacDigestSize << 2;
            flags |= (ulong)packetEncoderSettings.MacPBKDF2IterationCount << 14;
            flags |= (ulong)packetEncoderSettings.MacKeySize << 26;
            flags |= (ulong)packetEncoderSettings.EncryptionKeySize << 38;
            flags |= (ulong)packetEncoderSettings.RandomKeySize << 50;
            return flags;
        }
        #endregion
    }
}
