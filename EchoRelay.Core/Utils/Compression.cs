using Ionic.Zlib;
using ZstdSharp;

namespace EchoRelay.Core.Utils
{
    /// <summary>
    /// Provides zlib compression/decompression methods.
    /// </summary>
    public abstract class Compression
    {
        /// <summary>
        /// Compresses a buffer with zlib compression.
        /// </summary>
        /// <param name="data">The buffer to compress with zlib.</param>
        /// <returns>Returns the zlib compressed buffer.</returns>
        public static byte[] CompressZlib(byte[] data)
        {
            return ZlibStream.CompressBuffer(data);
        }

        /// <summary>
        /// Decompresses a zlib compressed buffer.
        /// </summary>
        /// <param name="data">The zlib compressed buffer to decompress.</param>
        /// <returns>Returns the decompressed buffer.</returns>
        public static byte[] DecompressZlib(byte[] data)
        {
            return ZlibStream.UncompressBuffer(data);
        }

        /// <summary>
        /// Compresses a buffer with zstd compression.
        /// </summary>
        /// <param name="data">The buffer to compress with zstd.</param>
        /// <returns>Returns the zstd compressed buffer.</returns>
        public static byte[] CompressZstd(byte[] data)
        {
            return new Compressor().Wrap(data).ToArray();
        }

        /// <summary>
        /// Decompresses a zstd compressed buffer.
        /// </summary>
        /// <param name="data">The zstd compressed buffer to decompress.</param>
        /// <returns>Returns the decompressed buffer.</returns>
        public static byte[] DecompressZstd(byte[] data)
        {
            return new Decompressor().Unwrap(data).ToArray();
        }
    }
}
