using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.Core.Server.Storage
{
    /// <summary>
    /// Event for a storage resource being loaded.
    /// </summary>
    /// <param name="storage">The storage that the resource was loaded from.</param>
    /// <param name="resource">The resource that was loaded.</param>
    public delegate void ResourceLoadedEventHandler<T>(ServerStorage storage, T resource);

    /// <summary>
    /// Event for a storage resource being changed.
    /// </summary>
    /// <param name="storage">The storage that the resource was updated in.</param>
    /// <param name="resource">The resource that was changed.</param>
    /// <param name="changeType">The type of change made to the resource.</param>
    public delegate void ResourceChangedEventHandler<T>(ServerStorage storage, T resource, StorageChangeType changeType);

    #region Classes
    /// <summary>
    /// A storage provider for a given type of resource, <see cref="V"/>.
    /// </summary>
    /// <typeparam name="V">The type of resource which should be managed by this provider.</typeparam>
    public abstract class ResourceProvider<V>
    {
        /// <summary>
        /// The parent <see cref="ServerStorage"/> which this provider was initialized for.
        /// </summary>
        public ServerStorage Storage { get; }

        /// <summary>
        /// Event for a <see cref="V"/>-type resource being loaded.
        /// </summary>
        public event ResourceLoadedEventHandler<V>? OnLoaded;

        /// <summary>
        /// Event for a <see cref="V"/>-type resource being changed.
        /// </summary>
        public event ResourceChangedEventHandler<V>? OnChanged;

        public ResourceProvider(ServerStorage storage)
        {
            Storage = storage;
        }

        public void Open()
        {
            OpenInternal();
        }
        protected abstract void OpenInternal();
        public void Close()
        {
            CloseInternal();
        }
        protected abstract void CloseInternal();
        public abstract bool Exists();
        public V? Get()
        {
            V? resource = GetInternal();
            if (resource != null)
                OnLoaded?.Invoke(Storage, resource);
            return resource;
        }
        protected abstract V? GetInternal();
        public void Set(V resource)
        {
            SetInternal(resource);
            OnChanged?.Invoke(Storage, resource, StorageChangeType.Set);
        }
        protected abstract void SetInternal(V resource);
        public void Delete()
        {
            V? removedResource = DeleteInternal();
            if (removedResource != null)
                OnChanged?.Invoke(Storage, removedResource, StorageChangeType.Deleted);
        }
        protected abstract V? DeleteInternal();
    }

    /// <summary>
    /// A storage provider for a given type of resource that exists in a collection of the same type, <see cref="V"/>.
    /// </summary>
    /// <typeparam name="K">The type of key which is used to index the resource.</typeparam>
    /// <typeparam name="V">The type of resources which should be managed by this provider.</typeparam>
    public abstract class ResourceCollectionProvider<K, V> 
        where K : notnull 
        where V : IKeyedResource<K>
    {
        /// <summary>
        /// The parent <see cref="ServerStorage"/> which this provider was initialized for.
        /// </summary>
        public ServerStorage Storage { get; }

        /// <summary>
        /// Indicates whether the resource provider is currently opened on storage.
        /// </summary>
        public bool Opened { get; private set; }

        /// <summary>
        /// Event for a <see cref="V"/>-type resource being loaded.
        /// </summary>
        public event ResourceLoadedEventHandler<V>? OnLoaded;

        /// <summary>
        /// Event for a <see cref="V"/>-type resource being changed.
        /// </summary>
        public event ResourceChangedEventHandler<V>? OnChanged;

        public ResourceCollectionProvider(ServerStorage storage)
        {
            Storage = storage;
        }

        public void Open()
        {
            OpenInternal();
            Opened = true;
        }
        protected abstract void OpenInternal();
        public void Close()
        {
            CloseInternal();
            Opened = false;
        }
        protected abstract void CloseInternal();
        public abstract K[] Keys();
        public abstract bool Exists(K key);
        public V? Get(K key)
        {
            V? resource = GetInternal(key);
            if (resource != null)
                OnLoaded?.Invoke(Storage, resource);
            return resource;
        }
        protected abstract V? GetInternal(K key);
        public void Set(V resource)
        {
            SetInternal(resource.Key(), resource);
            OnChanged?.Invoke(Storage, resource, StorageChangeType.Set);
        }
        protected abstract void SetInternal(K key, V resource);
        public void Delete(K key)
        {
            V? removedResource = DeleteInternal(key);
            if (removedResource != null)
                OnChanged?.Invoke(Storage, removedResource, StorageChangeType.Deleted);
        }
        protected abstract V? DeleteInternal(K key);
    }
    #endregion

    #region Enums
    /// <summary>
    /// Indicates the type of change that was made to a resource in storage.
    /// </summary>
    public enum StorageChangeType
    {
        /// <summary>
        /// An item was set in storage.
        /// </summary>
        Set,
        /// <summary>
        /// An item was deleted from storage.
        /// </summary>
        Deleted
    }
    #endregion
}
