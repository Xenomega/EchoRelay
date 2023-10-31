using EchoRelay.Core.Server.Storage.Resources;
using EchoRelay.Core.Server.Storage.Types;
using EchoRelay.Core.Server.Storage.Types.DocumentTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EchoRelay.Core.Server.Storage
{
    public abstract class InitialDeployment
    {
        private static void DeployAccessControlList(ServerStorage storage)
        {
            // Update the access control list.
            storage.AccessControlList.Set(new AccessControlListResource(allowRules: new string[] {"*"}, disallowRules: Array.Empty<string>()));
        }
        private static void DeploySymbolCache(ServerStorage storage, string? gameDirectory = null)
        {
            // Obtain our symbol cache
            SymbolCache symbolCache = new SymbolCache();

            // Add standard config symbols
            symbolCache.Add("main_menu", 1516004601999793531);
            symbolCache.Add("active_battle_pass_season", 8740945458790516606);
            symbolCache.Add("active_store_entry", 6474864185678376393);
            symbolCache.Add("active_store_featured_entry", 6145481310444124465);

            // Add document symbols
            symbolCache.Add("eula", -3980269165643165007);

            // Add level symbols
            symbolCache.Add("mpl_lobby_b2", -3415139097788326908);
            symbolCache.Add("mpl_tutorial_lobby", 4363271643694206015);

            symbolCache.Add("mpl_arena_a", 6300205991959903307);
            symbolCache.Add("mpl_tutorial_arena", 4363271690485661735);

            symbolCache.Add("mpl_combat_combustion", 4784809810443202620);
            symbolCache.Add("mpl_combat_dyson", 4891712358845785604);
            symbolCache.Add("mpl_combat_fission", -2351820497221352492);
            symbolCache.Add("mpl_combat_gauss", 4891712363006409241);

            // Add gametype symbols
            symbolCache.Add("social_2.0", 301069346851901302);
            symbolCache.Add("social_2.0_private", 3485062872400698437);
            symbolCache.Add("social_2.0_npe", 1601406692177864215);

            symbolCache.Add("echo_arena", -3791849610740453517);
            symbolCache.Add("echo_arena_private", 691594351282457603);
            symbolCache.Add("echo_arena_tournament", -3081978974147786912);
            symbolCache.Add("echo_arena_public_ai", -3076694376331427079);
            symbolCache.Add("echo_arena_practice_ai", -8607855738967935905);
            symbolCache.Add("echo_arena_private_ai", -2341211041644966243);
            symbolCache.Add("echo_arena_first_match", -1545408622389224342);
            symbolCache.Add("echo_arena_npe", -2840452043221058453);

            symbolCache.Add("echo_combat", 4421472114608583194);
            symbolCache.Add("echo_combat_private", 3727844164146657855);
            symbolCache.Add("echo_combat_tournament", 7729563559975407548);
            symbolCache.Add("echo_combat_public_ai", 4832867265306071705);
            symbolCache.Add("echo_combat_practice_ai", 2720675696233281171);
            symbolCache.Add("echo_combat_private_ai", 7060564080080586305);
            symbolCache.Add("echo_combat_first_match", 5171983837792427686);

            symbolCache.Add("echo_demo", 5603003217554343217);
            symbolCache.Add("echo_demo_public", 3718950499098277919);

            // If a game directory was provided, extract symbols from it.
            if (gameDirectory != null)
                symbolCache.Add(gameDirectory, SearchOption.TopDirectoryOnly, false);

            // Update the symbol cache.
            storage.SymbolCache.Set(symbolCache);
        }
        private static void DeployLoginSettings(ServerStorage storage)
        {
            // Create new login settings. The default values will be sufficient for most of this.
            // We create a config data entry pointing to each of our config resources.
            // The default values for an entry have a start/end range that will be sufficient for years.
            LoginSettingsResource loginSettings = new LoginSettingsResource()
            {
                ConfigData = new LoginSettingsResource.LoginConfigSettings()
            };

            // Update our login settings in storage
            storage.LoginSettings.Set(loginSettings);
        }
        private static void DeployChannelInfo(ServerStorage storage)
        {
            // Create a new channel info
            ChannelInfoResource channelInfo = new ChannelInfoResource(
                new ChannelInfoResource.Channel()
                {
                    ChannelUUID = "90DD4DB5-B5DD-4655-839E-FDBE5F4BC0BF",
                    Name = "THE PLAYGROUND",
                    Description = "Classic Echo VR social lobbies.",
                    Rules = "1. Only use this channel for testing.\n2. Act responsibly.\n3. Act legally.",
                    RulesVersion = 1,
                    Link = "https://en.wikipedia.org/wiki/Lone_Echo",
                    Priority = 0,
                    Rad = true,
                },
                new ChannelInfoResource.Channel()
                {
                    ChannelUUID = "DD9C48DF-C495-4EF3-B317-4FD6364F329D",
                    Name = "CASUAL MATURE GAMERS",
                    Description = "Casual lobbies for less competitive players.",
                    Rules = "1. Only use this channel for testing.\n2. Act responsibly.\n3. Act legally.",
                    RulesVersion = 1,
                    Link = "https://en.wikipedia.org/wiki/Lone_Echo",
                    Priority = 1,
                    Rad = true,
                },
                new ChannelInfoResource.Channel()
                {
                    ChannelUUID = "937CE604-5DC7-431F-812B-C7C25B4B37B6",
                    Name = "COMPETITIVE GAMERS",
                    Description = "Competitive lobbies for competitive players.",
                    Rules = "1. Only use this channel for testing.\n2. Act responsibly.\n3. Act legally.",
                    RulesVersion = 1,
                    Link = "https://en.wikipedia.org/wiki/Lone_Echo",
                    Priority = 2,
                    Rad = true,
                },
                new ChannelInfoResource.Channel()
                {
                    ChannelUUID = "EF663D3F-D947-484A-BA7E-8C5ED7FED1A6",
                    Name = "COMBAT PLAYERS",
                    Description = "Casual and competitive lobbies for Echo Combat players.",
                    Rules = "1. Only use this channel for testing.\n2. Act responsibly.\n3. Act legally.",
                    RulesVersion = 1,
                    Link = "https://en.wikipedia.org/wiki/Lone_Echo",
                    Priority = 3,
                    Rad = true,
                }
                );

            // Update our channel info in storage.
            storage.ChannelInfo.Set(channelInfo);
        }
        private static void DeployConfigs(ServerStorage storage)
        {
            // Add all relevant resources.

            #region Main Menu
            storage.Configs.Set(JsonConvert.DeserializeObject<ConfigResource>(@"
{
   ""type"":""main_menu"",
   ""id"":""main_menu"",
   ""_ts"":0,
   ""news"":{
      ""offseason"":{
         ""texture"":""None"",
         ""link"":""https://en.wikipedia.org/wiki/Lone_Echo""
      },
      ""sentiment"":{
         ""texture"":""ui_mnu_news_latest"",
         ""link"":""https://en.wikipedia.org/wiki/Lone_Echo""
      }
   },
   ""splash"":{
      ""offseason"":{
         ""texture"":""ui_menu_splash_screen_poster_a_shutdown_clr"", // can replace with ui_menu_splash_screen_poster_a_s7_clr for season 7 ending message, etc.
         ""link"":""https://en.wikipedia.org/wiki/Lone_Echo""
      }
   },
   ""splash_version"":1, // the version of the splash seen, so it doesn't show again. should monotonically increase.
   ""help_link"":""https://en.wikipedia.org/wiki/Lone_Echo"",
   ""news_link"":""https://en.wikipedia.org/wiki/Lone_Echo"",
   ""discord_link"":""https://en.wikipedia.org/wiki/Lone_Echo""
}
")!);
            #endregion
        }
        private static void DeployDocuments(ServerStorage storage)
        {
            // Create our document resources.
            EulaDocumentResource eula = new EulaDocumentResource
            {
                Type = "eula",
                Language = "en",
                Version = 1,
                VersionGameAdmin = 1,
                Text = "Warning: This is an unofficial server. By continuing, you indicate your connection is intentional, for legal personal research purposes.",
                TextGameAdmin = "Unofficial server operators have visibility into game traffic, while players with authorized Game Admin roles may act as spectators or moderators to observe player interactions.",
                MarkAsReadProfileKey = "legal|eula_version",
                MarkAsReadProfileKeyGameAdmin = "legal|game_admin_version",
                PrivacyPolicyLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                CodeOfConductLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                CodeOfConductVRLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                EchoCombatLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                EchoArenaLink = "https://en.wikipedia.org/wiki/Lone_Echo", 
                EchoVREulaLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                GameAdminsLink = "https://en.wikipedia.org/wiki/Lone_Echo",
                TermsAndConditionsLink = "https://en.wikipedia.org/wiki/Lone_Echo",
            };

            // Convert the object to a generic document resource and store it.
            DocumentResource documentResource = JObject.FromObject(eula)?.ToObject<DocumentResource>()!;
            storage.Documents.Set(documentResource);
        }
        public static void PerformInitialDeployment(ServerStorage storage, string? gameDirectory = null, bool clearExistingAccounts = false)
        {
            DeployAccessControlList(storage);
            DeploySymbolCache(storage, gameDirectory);
            DeployLoginSettings(storage);
            DeployChannelInfo(storage);
            DeployConfigs(storage);
            DeployDocuments(storage);
        }
    }
}
