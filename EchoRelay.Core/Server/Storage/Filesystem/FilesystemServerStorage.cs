using EchoRelay.Core.Game;
using EchoRelay.Core.Server.Storage.Resources;
using EchoRelay.Core.Server.Storage.Types;

namespace EchoRelay.Core.Server.Storage.Filesystem
{
    public class FilesystemServerStorage : ServerStorage
    {
        /// <summary>
        /// The root directory to be used for file system server storage.
        /// </summary>
        public string RootDirectory { get; }

        public override ResourceProvider<AccessControlListResource> AccessControlList => _accessControlList;
        private FilesystemResourceProvider<AccessControlListResource> _accessControlList;

        public override ResourceCollectionProvider<XPlatformId, AccountResource> Accounts => _accounts;
        private FilesystemResourceCollectionProvider<XPlatformId, AccountResource> _accounts;

        public override ResourceProvider<ChannelInfoResource> ChannelInfo => _channelInfo;
        private FilesystemResourceProvider<ChannelInfoResource> _channelInfo;

        public override ResourceCollectionProvider<(string type, string identifier), ConfigResource> Configs => _configs;
        private FilesystemResourceCollectionProvider<(string type, string identifier), ConfigResource> _configs;

        public override ResourceCollectionProvider<(string type, string language), DocumentResource> Documents => _documents;
        private FilesystemResourceCollectionProvider<(string type, string language), DocumentResource> _documents;

        public override ResourceProvider<LoginSettingsResource> LoginSettings => _loginSettings;
        private FilesystemResourceProvider<LoginSettingsResource> _loginSettings;

        public override ResourceProvider<SymbolCache> SymbolCache => _symbolCache;
        private FilesystemResourceProvider<SymbolCache> _symbolCache;


        private readonly object _symbolCacheLock = new object();

        public FilesystemServerStorage(string rootDirectory)
        {
            // Verify our root directory exists
            Directory.CreateDirectory(rootDirectory);

            // Set our properties
            RootDirectory = rootDirectory;
            var accountsDirectory = Path.Join(RootDirectory, "accounts");
            var configResourceDirectory = Path.Join(RootDirectory, "configs");
            var documentResourceDirectory = Path.Join(RootDirectory, "documents");
            var accessControlListFilePath = Path.Join(RootDirectory, "access_control_list.json");
            var channelInfoFilePath = Path.Join(RootDirectory, "channel_info.json");
            var loginSettingsFilePath = Path.Join(RootDirectory, "login_settings.json");
            var symbolCacheFilePath = Path.Join(RootDirectory, "symbols.json");

            // Create our resource containers
            _accessControlList = new FilesystemResourceProvider<AccessControlListResource>(this, accessControlListFilePath);
            _accounts = new FilesystemResourceCollectionProvider<XPlatformId, AccountResource>(this, accountsDirectory, "*.json", x => $"{x}.json", 0x100);
            _channelInfo = new FilesystemResourceProvider<ChannelInfoResource>(this, channelInfoFilePath);
            _configs = new FilesystemResourceCollectionProvider<(string Type, string Identifier), ConfigResource>(this, configResourceDirectory, "*.json", x => $"{x.Identifier}.json", 0x100);
            _documents = new FilesystemResourceCollectionProvider<(string Type, string Language), DocumentResource>(this, documentResourceDirectory, "*.json", x => $"{x.Type}_{x.Language}.json", 0x100);
            _loginSettings = new FilesystemResourceProvider<LoginSettingsResource>(this, loginSettingsFilePath);
            _symbolCache = new FilesystemResourceProvider<SymbolCache>(this, symbolCacheFilePath);
        }
    }
}
