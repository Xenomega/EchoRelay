namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// A storage resource which is indexed by a particular key.
    /// </summary>
    /// <typeparam name="K">The type of key the resource is indexed by.</typeparam>
    public interface IKeyedResource<K>
    {
        /// <summary>
        /// Obtains the key which the storage resource is indexed by.
        /// </summary>
        /// <returns>Returns the key which the resource is indexed by.</returns>
        public K Key();
    }
}
