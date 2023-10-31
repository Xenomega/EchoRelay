using EchoRelay.Core.Utils;
using Newtonsoft.Json;

namespace EchoRelay.Core.Server.Storage.Filesystem
{
    /// <summary>
    /// A filesystem operation read/write/delete cache for JSON objects compatible with <see cref="JsonConvert"/>. 
    /// It caches items to minimize reads from disk, and caches items for some time to minimize the amount of subsequent writes to the same file.
    /// </summary>
    /// <typeparam name="T">The type of JSON objects the file cache will <see cref="JsonConvert"/> when reading/writing.</typeparam>
    internal class LRUFileCache<T>
    {
        #region Fields
        private int _capacity;
        private Dictionary<string, LRUFileCacheItem> _cache;
        private LinkedList<LRUFileCacheItem> _accessLRU;
        private LinkedList<LRUFileCacheItem> _writeLRU;
        private TimeSpan _flushDelay;
        private object _cacheLock = new object();
        #endregion

        #region Constructor
        public LRUFileCache(int capacity, TimeSpan? flushDelay = null)
        {
            _capacity = capacity;
            _cache = new Dictionary<string, LRUFileCacheItem>();
            _accessLRU = new LinkedList<LRUFileCacheItem>();
            _writeLRU = new LinkedList<LRUFileCacheItem>();
            _flushDelay = flushDelay ?? TimeSpan.Zero;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Checks if a given file exists at the path, factoring in changes made in the cache.
        /// </summary>
        /// <param name="path">The file path to check for existence.</param>
        /// <returns>Returns a boolean indicating if the file currently exists (considering in changes in cache).</returns>
        public bool Exists(string path)
        {
            // Normalize the path
            path = PathUtils.NormalizedPath(path);

            lock (_cacheLock)
            {
                // Determine if the path is in our cache or we can find the file.
                return _cache.ContainsKey(path) || File.Exists(path);
            }
        }
        /// <summary>
        /// Obtains the contents of a file at a given path, factoring in changes from the cache.
        /// </summary>
        /// <param name="path">The file path to obtain contents for.</param>
        /// <returns>Returns the file contents, factoring in changes from the cache.</returns>
        public T? Read(string path)
        {
            // Normalize the path
            path = PathUtils.NormalizedPath(path);

            lock (_cacheLock)
            {
                // If the file is in our cache, move it to the end of LRU (most recently used now) and return it.
                if (_cache.TryGetValue(path, out var cachedFile) && cachedFile != null)
                {
                    _accessLRU.Remove(cachedFile.AccessLRUNode!);
                    _accessLRU.AddLast(cachedFile.AccessLRUNode!);
                    return cachedFile.Content;
                }

                // Read the content and deserialize it
                T? content = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));

                // If we got a result, cache it.
                if (content != null && _capacity > 0)
                {
                    // If we are at capacity for our cache, remove an item.
                    if (_cache.Count >= _capacity)
                    {
                        RemoveItem(_accessLRU.First!.Value);
                    }

                    // Add a new cache item to our list.
                    cachedFile = new LRUFileCacheItem(path, content);
                    cachedFile.AccessLRUNode = new LinkedListNode<LRUFileCacheItem>(cachedFile);
                    _accessLRU.AddLast(cachedFile.AccessLRUNode);
                    _cache[path] = cachedFile;
                }
                return content;
            }
        }
        /// <summary>
        /// Updates the contents of a file at a given path. If the cache capacity is 0, the file is flushed immediately.
        /// Otherwise, the write changes are cached, and will later be flushed by <see cref="Update"/> or <see cref="Flush"/>.
        /// </summary>
        /// <param name="path">The path of the file to update contents for.</param>
        /// <param name="content">The file contents to update with.</param>
        public void Write(string path, T? content)
        {
            // Normalize the path
            path = PathUtils.NormalizedPath(path);

            lock (_cacheLock)
            {
                // If we are using a cache, add the item to it to be written at a later time.
                if (_capacity > 0)
                {
                    // If the item is in our cache, we update it, otherwise, we create a new cache item.
                    if (_cache.TryGetValue(path, out var cachedFile))
                    {
                        // Update the existing item
                        cachedFile.Content = content;
                        _accessLRU.Remove(cachedFile.AccessLRUNode!);
                        _accessLRU.AddLast(cachedFile.AccessLRUNode!);
                        if (cachedFile.ChangedTime == null)
                            cachedFile.ChangedTime = DateTime.UtcNow;
                    }
                    else
                    {
                        // If we are at capacity for our cache, remove an item.
                        if (_cache.Count >= _capacity)
                        {
                            RemoveItem(_accessLRU.First!.Value);
                        }

                        // Add a new cache item to our list.
                        cachedFile = new LRUFileCacheItem(path, content, DateTime.UtcNow);
                        cachedFile.AccessLRUNode = new LinkedListNode<LRUFileCacheItem>(cachedFile);
                        _accessLRU.AddLast(cachedFile.AccessLRUNode);
                        _cache[path] = cachedFile;
                    }

                    // If we didn't attach this to our write LRU yet, do so now.
                    if (cachedFile.WriteLRUNode == null)
                    {
                        cachedFile.WriteLRUNode = new LinkedListNode<LRUFileCacheItem>(cachedFile);
                        _writeLRU.AddLast(cachedFile.WriteLRUNode);
                    }

                    // If the flush delay is zero, flush immediately. Otherwise, ensure we have a flush request already for this node.
                    if (_flushDelay == TimeSpan.Zero)
                        FlushItem(cachedFile);
                }
                else
                {
                    // If we have file contents, write them. Otherwise, we perform a deletion operation.
                    if (content != null)
                    {
                        string serializedData = JsonConvert.SerializeObject(content, Formatting.Indented, StreamIO.JsonSerializerSettings);
                        File.WriteAllText(path, serializedData);
                    } else
                    {
                        File.Delete(path);
                    }
                }
            }
        }
        /// <summary>
        /// Deletes a file at a given path. If the cache capacity is 0, the file is deleted immediately.
        /// Otherwise, the delete changes are cached, and will later be flushed by <see cref="Update"/> or <see cref="Flush"/>.
        /// </summary>
        /// <param name="path"></param>
        public void Delete(string path)
        {
            // Write default value (null), which signals to delete a file.
            Write(path, default);
        }
        /// <summary>
        /// Updates the cache, checking for any file flushes which should be made.
        /// </summary>
        public void Update()
        {
            // Loop through the write/flush LRU to determine if any write flushes are overdue.
            while (_writeLRU.Count > 0)
            {
                // Obtain the first item.
                var writeLRUNode = _writeLRU.First!;
                var cacheItem = _writeLRU.First!.Value;

                // If it is somehow unchanged (not needing flushing, simply remove it from the write LRU cache).
                if (cacheItem.ChangedTime == null)
                {
                    cacheItem.WriteLRUNode = null;
                    _writeLRU.Remove(writeLRUNode);
                    continue;
                }

                // If our oldest item to flush isn't old enough, we can stop now.
                TimeSpan timeDiff = DateTime.UtcNow - (DateTime)cacheItem.ChangedTime;
                if (timeDiff < _flushDelay)
                    return;

                // Otherwise we flush the item and move onto the next.
                FlushItem(cacheItem);
            }
        }
        /// <summary>
        /// Flushes all pending write/delete operations to disk.
        /// This retains the items within the cache for reads.
        /// </summary>
        public void Flush()
        {
            lock (_cacheLock)
            {
                while (_writeLRU.Count > 0)
                {
                    FlushItem(_writeLRU.First());
                }
            }
        }
        /// <summary>
        /// Clears the entire cache after flushing all pending/write delete operations to disk.
        /// </summary>
        public void Clear()
        {
            lock (_cacheLock)
            {
                while (_accessLRU.Count > 0)
                {
                    RemoveItem(_accessLRU.First());
                }
            }
        }

        /// <summary>
        /// Removes an individual cache item from the <see cref="LRUFileCache{T}"/>.
        /// </summary>
        /// <param name="cachedFile">The cached item to remove.</param>
        private void RemoveItem(LRUFileCacheItem cachedFile)
        {
            // Ensure any changes are flushed to disk.
            FlushItem(cachedFile);

            // Remove it from the cache
            _accessLRU.Remove(cachedFile.AccessLRUNode!);
            _cache.Remove(cachedFile.Path, out _);
        }

        /// <summary>
        /// Flushes any pending data to write to disk for an individual cache item.
        /// </summary>
        /// <param name="cachedFile">The cached item to flush changes to disk for.</param>
        private void FlushItem(LRUFileCacheItem cachedFile)
        {
            // If the file is unchanged from the filesystem, do nothing.
            if (cachedFile.ChangedTime == null)
                return;

            // If we have file contents, write them. Otherwise, we perform a deletion operation.
            if (cachedFile.Content != null)
            {
                // Serialize the data
                string serializedData = JsonConvert.SerializeObject(cachedFile.Content, Formatting.Indented, StreamIO.JsonSerializerSettings);

                // Create the folder if it doesn't exist, and write it.
                string? parentDirectory = Path.GetDirectoryName(cachedFile.Path);
                if (parentDirectory != null)
                    Directory.CreateDirectory(parentDirectory);
                File.WriteAllText(cachedFile.Path, serializedData);
            } else
            {
                File.Delete(cachedFile.Path);
            }

            // Remove it from the LRU used to track write flushes.
            if (cachedFile.WriteLRUNode != null)
                _writeLRU.Remove(cachedFile.WriteLRUNode);
            cachedFile.WriteLRUNode = null;
            cachedFile.ChangedTime = null;
        }
        #endregion

        #region Classes
        /// <summary>
        /// An individual cached file item.
        /// </summary>
        private class LRUFileCacheItem
        {
            #region Fields
            public string Path;
            public T? Content;
            public DateTime? ChangedTime;
            public LinkedListNode<LRUFileCacheItem>? AccessLRUNode;
            public LinkedListNode<LRUFileCacheItem>? WriteLRUNode;
            #endregion

            #region Constructor
            public LRUFileCacheItem(string path, T? content, DateTime? changedTime = null)
            {
                Path = path;
                Content = content;
                ChangedTime = changedTime;
            }
            #endregion
        }
        #endregion
    }
}
