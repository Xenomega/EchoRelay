using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace EchoRelay.Core.Server.Storage.Filesystem
{
    /// <summary>
    /// A filesystem <see cref="ResourceProvider{V}"/> which storages a singular resource.
    /// </summary>
    /// <typeparam name="K">The type of key which is used to index the resource.</typeparam>
    /// <typeparam name="V">The type of resources which should be managed by this provider.</typeparam>
    internal class FilesystemResourceProvider<V> : ResourceProvider<V>
    {
        /// <summary>
        /// The filesystem path for the <see cref="_resource"/>.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// A cached copy of the resource populated when the item is read from disk or written to disk.
        /// </summary>
        private V? _resource;

        public FilesystemResourceProvider(ServerStorage storage, string filePath) : base(storage)
        {
            FilePath = filePath;
        }


        protected override void OpenInternal()
        {
        }

        protected override void CloseInternal()
        {
            // Clear cache
            _resource = default;
        }

        public override bool Exists()
        {
            // Check if the resource is loaded or if the file exists.
            return _resource != null || File.Exists(FilePath);
        }

        protected override V? GetInternal()
        {
            // Cache the resource from file if it hasn't been already, then return it.
            if (_resource == null)
            {
                string resourceJson = File.ReadAllText(FilePath);
                _resource = JsonConvert.DeserializeObject<V>(resourceJson);
            }
            return _resource;
        }
        protected override void SetInternal(V resource)
        {
            // Update the cached resource
            _resource = resource;

            // Serialize the resource and write to disk.
            string resourceJson = JsonConvert.SerializeObject(_resource, Formatting.Indented, StreamIO.JsonSerializerSettings);
            File.WriteAllText(FilePath, resourceJson);
        }
        protected override V? DeleteInternal()
        {
            // Store a reference to our cached resource
            V? resource = _resource;

            // Clear the cached resource.
            _resource = default;

            // Return the removed resource, if any.
            return resource;
        }
    }

    /// <summary>
    /// A filesystem <see cref="ResourceCollectionProvider{K, V}"/> which storages a given type of keyed resource in a collection.
    /// </summary>
    /// <typeparam name="K">The type of key which is used to index the resource.</typeparam>
    /// <typeparam name="V">The type of resources which should be managed by this provider.</typeparam>
    internal class FilesystemResourceCollectionProvider<K, V> : ResourceCollectionProvider<K, V> 
        where K : notnull
        where V : IKeyedResource<K>
    {
        /// <summary>
        /// The directory containing the resources.
        /// </summary>
        public string ResourceDirectory { get; set; }

        private string _listFilesPattern;
        private Func<K, string> _relativeFilePathSelectorFunc;


        private ConcurrentDictionary<K, (string Path, V Resource)> _resources;
        private LRUFileCache<V> _cachedFilesystem;

        private object _lookupsChangeLock = new object();

        public FilesystemResourceCollectionProvider(ServerStorage storage, string resourceDirectory, string listFilesPattern, Func<K, string> relativeFilePathSelectorFunc, int cacheSize = 0) : base(storage)
        {
            ResourceDirectory = resourceDirectory;
            _resources = new ConcurrentDictionary<K, (string path, V)>();
            _cachedFilesystem = new LRUFileCache<V>(cacheSize);
            _listFilesPattern = listFilesPattern;
            _relativeFilePathSelectorFunc = relativeFilePathSelectorFunc;
        }


        protected override void OpenInternal()
        {
            // If the directory doesn't exist, exit
            if (!Directory.Exists(ResourceDirectory))
                return;

            // Create a set of verified keys/paths, to ensure we don't have duplicate keys.
            Dictionary<K, string> verifiedPaths = new Dictionary<K, string>();

            // Enumerate all config files and read them in.
            string[] filePaths = Directory.GetFiles(ResourceDirectory, _listFilesPattern);
            foreach (string filePath in filePaths)
            {
                try
                {
                    // Load the config resource.
                    V? resource = _cachedFilesystem.Read(filePath);

                    // Verify the resource.
                    if (resource == null)
                        throw new FileNotFoundException();

                    // Obtain the key for the resource
                    K key = resource.Key();
                    if (verifiedPaths.ContainsKey(key))
                        throw new InvalidDataException($"Resource definition collides with file '{verifiedPaths[key]}'");

                    // Verify the filename of the file is as expected, they should all be deterministic.
                    // If they are not, we move them for the user.
                    string normalizedFilePath = PathUtils.NormalizedPath(filePath);
                    string normalizedExpectedPath = PathUtils.NormalizedPath(Path.Join(ResourceDirectory, _relativeFilePathSelectorFunc(key)));
                    if (normalizedFilePath != normalizedExpectedPath)
                    {
                        File.Move(normalizedFilePath, normalizedExpectedPath);
                    }

                    // Add it to our lookups
                    _resources[key] = (normalizedExpectedPath, resource);

                    // Add it to our verified keys/paths to ensure we don't have later collisions.
                    verifiedPaths[key] = normalizedExpectedPath;
                }
                catch (Exception ex)
                {
                    Close();
                    throw new Exception($"Could not load resource {typeof(V).Name}: '{filePath}'", ex);
                }
            }
        }

        protected override void CloseInternal()
        {
            // Clear cache
            _resources.Clear();
            _cachedFilesystem.Clear();
        }

        public override K[] Keys()
        {
            // Return all the keys as an array.
            return _resources.Keys.ToArray();
        }
        public override bool Exists(K key)
        {
            // Check if we indexed the provided key.
            return _resources.ContainsKey(key);
        }

        protected override V? GetInternal(K key)
        {
            // Try to obtain the file path from the key. If we can't, return null/default.
            if (!_resources.TryGetValue(key, out var resourceInfo))
            {
                return default;
            }

            // The item from cached filesystem.
            return _cachedFilesystem.Read(resourceInfo.Path);
        }
        protected override void SetInternal(K key, V resource)
        {
            // Obtain the file path for this key.
            string filePath = PathUtils.NormalizedPath(Path.Join(ResourceDirectory, _relativeFilePathSelectorFunc(key)));

            // Set it in our resource lookup and cache the contents.
            _resources[key] = (filePath, resource);
            _cachedFilesystem.Write(filePath, resource);
        }
        protected override V? DeleteInternal(K key)
        {
            // Remove the item.
            _cachedFilesystem.Delete(_resources[key].Path);
            _resources.Remove(key, out var removed);
            return removed.Resource;
        }
    }
}
