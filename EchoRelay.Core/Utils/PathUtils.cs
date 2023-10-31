namespace EchoRelay.Core.Utils
{
    public static class PathUtils
    {
        /// <summary>
        /// Normalizes a path so it is consistent and deterministically resolved within the cache.
        /// </summary>
        /// <param name="path">The path to normalize.</param>
        /// <returns>Returns a normalized path.</returns>
        public static string NormalizedPath(string path)
        {
            return Path.GetFullPath(new Uri(path).LocalPath)
                       .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
                       .ToLowerInvariant();
        }
    }
}
