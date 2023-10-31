using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Resources;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.Core.Server.Storage
{
    public abstract class ServerStorage
    {
        #region Properties
        public bool Opened { get; private set; }

        public abstract ResourceProvider<AccessControlListResource> AccessControlList { get; }
        public abstract ResourceCollectionProvider<XPlatformId, AccountResource> Accounts { get; }
        public abstract ResourceProvider<ChannelInfoResource> ChannelInfo { get; }
        public abstract ResourceCollectionProvider<(string type, string identifier), ConfigResource> Configs { get; }
        public abstract ResourceCollectionProvider<(string type, string language), DocumentResource> Documents { get; }
        public abstract ResourceProvider<LoginSettingsResource> LoginSettings { get; }
        public abstract ResourceProvider<SymbolCache> SymbolCache { get; }

        private readonly object _openCloseLock = new object();
        #endregion

        #region Events
        /// <summary>
        /// Event for storage being opened or closed.
        /// </summary>
        /// <param name="storage">The storage which was opened or closed.</param>
        public delegate void StorageOpenedClosedEventHandler(ServerStorage storage);

        /// <summary>
        /// Event for storage being opened. This indicates resources may have been fully re-loaded.
        /// </summary>
        public event StorageOpenedClosedEventHandler? OnStorageOpened;
        /// <summary>
        /// Event for storage being opened. This indicates resources may no longer be available.
        /// </summary>
        public event StorageOpenedClosedEventHandler? OnStorageClosed;
        #endregion

        #region General
        public void Open()
        {
            // If this is already opened, exit early.
            if (Opened)
                return;

            // Open the storage
            lock (_openCloseLock)
            {
                Opened = true;
                OpenInternal();
            }

            // Fire the relevant event
            OnStorageOpened?.Invoke(this);
        }
        protected virtual void OpenInternal()
        {
            // Signal opening to all resource providers.
            AccessControlList.Open();
            Accounts.Open();
            ChannelInfo.Open();
            Configs.Open();
            Documents.Open();
            LoginSettings.Open();
            SymbolCache.Open();
        }
        public void Close()
        {
            // Close storage
            lock (_openCloseLock)
            {
                CloseInternal();
                Opened = false;
            }

            // Fire the relevant event.
            OnStorageClosed?.Invoke(this);
        }
        protected virtual void CloseInternal()
        {
            // Signal closing to all resource providers.
            AccessControlList.Close();
            Accounts.Close();
            ChannelInfo.Close();
            Configs.Close();
            Documents.Close();
            LoginSettings.Close();
            SymbolCache.Close();
        }
        public void Clear(bool accessControlList = true, bool accounts = true, bool channelInfo = true, bool configs = true, bool documents = true, bool loginSettings = true, bool symbolCache = true)
        {
            if (accessControlList)
                AccessControlList.Delete();

            if (channelInfo)
                ChannelInfo.Delete();

            if (loginSettings)
                LoginSettings.Delete();

            if (configs)
                foreach (var configInfo in Configs.Keys())
                    Configs.Delete((configInfo.type, configInfo.identifier));

            if (documents)
                foreach (var documentInfo in Documents.Keys())
                    Documents.Delete((documentInfo.type, documentInfo.language));

            if (accounts)
                foreach (var userId in Accounts.Keys())
                    Accounts.Delete(userId);

            if (symbolCache)
                SymbolCache.Delete();
        }
        #endregion
    }

}
