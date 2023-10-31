using EchoRelay.Core.Game;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// An account registered with the server.
    /// </summary>
    public class AccountResource : IKeyedResource<XPlatformId>
    {
        #region Properties
        /// <summary>
        /// The client and server side profile for the account.
        /// </summary>
        [JsonProperty("profile")]
        public AccountProfile Profile { get; set; }

        /// <summary>
        /// Indicates whether the account can enter game servers as a moderator (using the `-moderator` CLI command).
        /// This enables a free flycam mode where you are invisible to players.
        /// </summary>
        [JsonProperty("is_moderator")]
        public bool IsModerator { get; set; }

        /// <summary>
        /// Indicates when the account is banned until. If the user is not banned, this is null.
        /// </summary>
        [JsonProperty("banned_until")]
        public DateTime? BannedUntil { get; set; }

        /// <summary>
        /// A password-based account lock hash. This is computed as hash(UTF-8(account_lock) . <see cref="AccountLockSalt"/>).
        /// </summary>
        [JsonProperty("account_lock_hash")]
        public byte[]? AccountLockHash { get; private set; }

        /// <summary>
        /// The salt to be appended to the password-based account lock before being hashed 
        /// and stored in the <see cref="AccountLockHash"/> property.
        /// </summary>
        [JsonProperty("account_lock_salt")]
        public byte[]? AccountLockSalt { get; private set; }

        /// <summary>
        /// Indicates whether the account is currently banned.
        /// </summary>
        [JsonIgnore]
        public bool Banned => BannedUntil != null && BannedUntil > DateTime.UtcNow;

        /// <summary>
        /// The platform account identifier (unique user identifier) for the account.
        /// This is provided from the <see cref="ServerProfile"/>. It may be invalid if it
        /// has not yet been set.
        /// </summary>
        [JsonIgnore]
        public XPlatformId AccountIdentifier
        {
            get
            {
                return XPlatformId.Parse(Profile.Server.XPlatformId) ?? new XPlatformId(0, 0);
            }
        }
        #endregion

        #region Constructors
        public AccountResource()
        {
            Profile = new AccountProfile();
        }
        public AccountResource(XPlatformId userId, string? displayName = null, bool completedNPE = false, bool completedCommunityGuidelines = false, bool disableAfkTimeout = false) : this()
        {
            // Verify the user id provided.
            if (!userId.Valid())
                throw new ArgumentException("Could not create user with invalid platform id supplied");

            // Set the display name and platform id for client and server profiles.
            Profile.Server.XPlatformId = userId.ToString();
            Profile.Client.XPlatformId = Profile.Server.XPlatformId;
            Profile.SetDisplayName(displayName ?? userId.ToString());

            // Set onboarding tutorials as completed, if requested.
            if (completedNPE)
            {
                Profile.Client.NPE = new AccountClientProfile.NPESettings();
                Profile.Client.NPE.Lobby.Completed = true;
                Profile.Client.NPE.Movement.Completed = true;
                Profile.Client.NPE.FirstMatch.Completed = true;
                Profile.Client.NPE.ArenaBasics.Completed = true;
            }

            // Set acceptance of community guidelines, if requested.
            if (completedCommunityGuidelines)
            {
                Profile.Client.Social = new AccountClientProfile.SocialSettings();
                Profile.Client.Social.SetupVersion = 1;
                Profile.Client.Social.CommunityValuesVersion = 1;
            }

            // Set developer settings
            if (disableAfkTimeout)
            {
                Profile.Server.Developer = new AccountServerProfile.DeveloperSettings();
                Profile.Server.Developer.DisableAfkTimeout = true; // prevent kicking of "no ovr" (demo) users.
                Profile.Server.Developer.XPlatformId = userId.ToString(); // enables developer mode to allow other options
            }
        }
        #endregion

        #region Functions
        /// <summary>
        /// Obtains the key which the storage resource is indexed by.
        /// </summary>
        /// <returns>Returns the key which the resource is indexed by.</returns>
        public XPlatformId Key()
        {
            return AccountIdentifier;
        }

        /// <summary>
        /// Authenticates the user with the provided account lock.
        /// If no account lock was set, authentication always passes.
        /// If a account lock is provided when no account lock is set, it is then set to the provided account lock.
        /// </summary>
        /// <param name="accountLock">The account lock to authenticate to the account with. If the account has no account lock, it is set to this for the future.</param>
        /// <returns>Returns true if the user was authenticated, false otherwise.</returns>
        public bool Authenticate(string? accountLock)
        {
            // If our account hash or salt is null, set the password and return true.
            if (AccountLockSalt == null || AccountLockHash == null)
            {
                SetAccountLock(accountLock);
                return true;
            }
            else if (accountLock == null)
            {
                // If the provided password is null, but the account has one, authentication fails.
                return false;
            }
            else
            {
                // Otherwise, a password exists, so we calculate the hash for this password and compare.
                byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(accountLock).Concat(AccountLockSalt).ToArray());
                return hash.SequenceEqual(AccountLockHash);
            }
        }

        /// <summary>
        /// Sets the account lock to the provided account lock.
        /// </summary>
        /// <param name="accountLock">The account lock to use. If null/empty, this clears the account lock.</param>
        public void SetAccountLock(string? accountLock)
        {
            if (string.IsNullOrEmpty(accountLock))
            {
                ClearAccountLock();
            } 
            else
            {
                // Generate a new salt
                AccountLockSalt = RandomNumberGenerator.GetBytes(0x16);

                // Hash the password with the salt.
                // NOTE: Ideally, we'd use argon2id, bcrypt, or something more intensive here.
                // For simplicity's sake and lack of control over client code (supporting unmodified clients),
                // we'll just use a less secure scheme here with SHA256.
                AccountLockHash = SHA256.HashData(Encoding.UTF8.GetBytes(accountLock).Concat(AccountLockSalt).ToArray());
            }
        }

        /// <summary>
        /// Clears the account lock on the account, unlocking it for any to use.
        /// The account lock may be set by the client again with one provided.
        /// </summary>
        public void ClearAccountLock()
        {
            AccountLockHash = null;
            AccountLockSalt = null;
        }
        #endregion

        /// <summary>
        /// The client and server-side profiles for a given account.
        /// </summary>
        public class AccountProfile
        {
            /// <summary>
            /// The client-side profile for the account.
            /// </summary>
            [JsonProperty("client")]
            public AccountClientProfile Client { get; set; } = new AccountClientProfile();

            /// <summary>
            /// The server-side profile for the account.
            /// </summary>
            [JsonProperty("server")]
            public AccountServerProfile Server { get; set; } = new AccountServerProfile();


            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            /// <summary>
            /// Sets the display name for both client and server profiles.
            /// </summary>
            /// <param name="displayName">The display name to set.</param>
            public void SetDisplayName(string displayName)
            {
                Client.DisplayName = displayName;
                Server.DisplayName = displayName;
            }
        }

        /// <summary>
        /// The client-side profile for a given account.
        /// It does not contain any custom fields and reflects the structure used by the game.
        /// </summary>
        public class AccountClientProfile
        {
            /// <summary>
            /// The display name of the account.
            /// </summary>
            [JsonProperty("displayname")]
            public string DisplayName { get; set; } = "";

            /// <summary>
            /// The platform account identifier (unique user identifier) for the account.
            /// </summary>
            [JsonProperty("xplatformid")]
            public string XPlatformId { get; set; } = "";

            /// <summary>
            /// The team name for the user.
            /// </summary>
            [JsonProperty("teamname")]
            public string? TeamName { get; set; } = null;

            /// <summary>
            /// The default weapon type used by this player.
            /// </summary>
            [JsonProperty("weapon")]
            public string? Weapon { get; set; } = "scout"; // "scout", etc

            /// <summary>
            /// The default grenade type used by this player.
            /// </summary>
            [JsonProperty("grenade")]
            public string? Grenade { get; set; } = "det"; // "det", etc

            /// <summary>
            /// The default arm used by this player.
            /// </summary>
            [JsonProperty("weaponarm")]
            public int? WeaponArm { get; set; } = 1;

            /// <summary>
            /// The last time this profile was modified.
            /// </summary>
            [JsonProperty("modifytime")]
            public ulong? ModifyTime { get; set; }

            /// <summary>
            /// The default ability for this player.
            /// </summary>
            [JsonProperty("ability")]
            public string? Ability { get; set; } = null; // "heal", "sensor", etc

            /// <summary>
            /// Acceptance of terms of service, EULA, etc.
            /// </summary>
            [JsonProperty("legal")]
            public JObject? Legal { get; set; } = null;

            /// <summary>
            /// TODO: 
            /// </summary>
            [JsonProperty("temp")]
            public JObject? Temp { get; set; } = null;

            /// <summary>
            /// A structure containing player ids which were muted by this player.
            /// </summary>
            [JsonProperty("mute")]
            public JObject? Mute { get; set; } = null;

            /// <summary>
            /// A structure indicating progress in the new-player-experience/tutorials completion status.
            /// </summary>
            [JsonProperty("npe")]
            public NPESettings? NPE { get; set; } = null;

            /// <summary>
            /// A structure indicating versions of customizations/configs the user is on.
            /// </summary>
            [JsonProperty("customization")]
            public CustomizationSettings? Customization { get; set; } = new CustomizationSettings();

            /// <summary>
            /// A structure specifying various information such as the user's default channel, setup version, community values acceptance, etc.
            /// </summary>
            [JsonProperty("social")]
            public SocialSettings? Social { get; set; } = new SocialSettings();

            /// <summary>
            /// A structure containing symbol ids for unlockables.
            /// </summary>
            [JsonProperty("newunlocks")]
            public long[] NewUnlocks { get; set; } = Array.Empty<long>();

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            #region Classes
            /// <summary>
            /// The social settings for a <see cref="AccountClientProfile"/>.
            /// Specifies various settubgs such as the user's default channel, setup version, 
            /// community values acceptance, etc.
            /// </summary>
            public class SocialSettings
            {
                /// <summary>
                /// The latest version of community values guidelines which has been accepted.
                /// </summary>
                [JsonProperty("community_values_version")]
                public long? CommunityValuesVersion { get; set; }

                /// <summary>
                /// The latest setup version which has been accepted.
                /// </summary>
                [JsonProperty("setup_version")]
                public long? SetupVersion { get; set; }

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
            }
            /// <summary>
            /// The customizations settings for a <see cref="AccountClientProfile"/>.
            /// Specifies the versions of unlocks the client profile has most recently encountered.
            /// </summary>
            public class CustomizationSettings
            {
                /// <summary>
                /// The latest version of battle pass season the client profile encountered.
                /// </summary>
                [JsonProperty("battlepass_season_poi_version")]
                public long? BattlePassSeasonPoiVersion { get; set; } = 0;

                /// <summary>
                /// The latest version of battle pass season the client profile encountered.
                /// </summary>
                [JsonProperty("new_unlocks_poi_version")]
                public long? NewUnlocksPoiVersion { get; set; } = 0;

                /// <summary>
                /// The latest version of battle pass season the client profile encountered.
                /// </summary>
                [JsonProperty("store_entry_poi_version")]
                public long? StoreEntryPoiVersion { get; set; } = 0;

                /// <summary>
                /// The latest version of battle pass season the client profile encountered.
                /// </summary>
                [JsonProperty("clear_new_unlocks_version")]
                public long? ClearNewUnlocksVersion { get; set; } = 0;

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
            }
            /// <summary>
            /// The "New Player Experience" settings for a <see cref="AccountClientProfile"/>.
            /// Specifies which onboarding/tutorial sequences the client has completed.
            /// </summary>
            public class NPESettings
            {
                /// <summary>
                /// Indicates whether the onboarding tutorial for the lobby has been completed.
                /// </summary>
                [JsonProperty("lobby")]
                public Status Lobby { get; set; } = new Status();

                /// <summary>
                /// Indicates whether the onboarding tutorial for the first match has been completed.
                /// </summary>
                [JsonProperty("firstmatch")]
                public Status FirstMatch { get; set; } = new Status();

                /// <summary>
                /// Indicates whether the onboarding tutorial for player movement has been completed.
                /// </summary>
                [JsonProperty("movement")]
                public Status Movement { get; set; } = new Status();

                /// <summary>
                /// Indicates whether the onboarding tutorial for arena has been completed.
                /// </summary>
                [JsonProperty("arenabasics")]
                public Status ArenaBasics { get; set; } = new Status();

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                /// <summary>
                /// Indicates the completion status of an NPE entry.
                /// </summary>
                public class Status
                {
                    /// <summary>
                    /// Indicates whether the NPE entry has been marked completed by the user, or whether they must still undergo it.
                    /// </summary>
                    [JsonProperty("completed")]
                    public bool Completed { get; set; }

                    /// <summary>
                    /// Additional fields which are not caught explicitly are retained here.
                    /// </summary>
                    [JsonExtensionData]
                    public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                    public Status(bool completed = false)
                    {
                        Completed = completed;
                    }
                }
            }
            #endregion

        }

        /// <summary>
        /// The server-side profile for a given account.
        /// It does not contain any custom fields and reflects the structure used by the game.
        /// </summary>
        public class AccountServerProfile
        {
            /// <summary>
            /// The display name of the account.
            /// </summary>
            [JsonProperty("displayname")]
            public string DisplayName { get; set; } = "";

            /// <summary>
            /// The platform account identifier (unique user identifier) for the account.
            /// </summary>
            [JsonProperty("xplatformid")]
            public string XPlatformId { get; set; } = "";

            /// <summary>
            /// TODO: Some client version
            /// </summary>
            [JsonProperty("_version")]
            public long Version { get; set; } = 5;

            /// <summary>
            /// An environment lock for different sandboxes.
            /// </summary>
            [JsonProperty("publisher_lock")]
            public string? PublisherLock { get; set; } = "rad15_live";

            /// <summary>
            /// The date echo combat was purchased.
            /// </summary>
            [JsonProperty("purchasedcombat")]
            public ulong? PurchasedCombatDate { get; set; } = 0;

            /// <summary>
            /// The lobby build timestamp.
            /// </summary>
            [JsonProperty("lobbyversion")]
            public ulong? LobbyVersion { get; set; }

            /// <summary>
            /// The last time this profile was modified.
            /// </summary>
            [JsonProperty("modifytime")]
            public ulong? ModifyTime { get; set; }

            /// <summary>
            /// The last time this profile was logged into.
            /// </summary>
            [JsonProperty("logintime")]
            public ulong? LoginTime { get; set; }

            /// <summary>
            /// The last time this profile was updated.
            /// </summary>
            [JsonProperty("updatetime")]
            public ulong? UpdateTime { get; set; }

            /// <summary>
            /// The time this profile was created.
            /// </summary>
            [JsonProperty("createtime")]
            public ulong? CreateTime { get; set; }

            /// <summary>
            /// Indicates this profile data may have gone stale.
            /// </summary>
            [JsonProperty("maybestale")]
            public bool? MaybeStale { get; set; }

            /// <summary>
            /// A structure detailing account statistics.
            /// </summary>
            [JsonProperty("stats")]
            public StatsSettings? Stats { get; set; } = new StatsSettings();

            /// <summary>
            /// In-game unlockables, denoted by symbols.
            /// </summary>
            [JsonProperty("unlocks")]
            public UnlocksSettings? Unlocks { get; set; } = new UnlocksSettings();

            /// <summary>
            /// A structure detailing loadout information for the player.
            /// </summary>
            [JsonProperty("loadout")]
            public LoadoutSettings? Loadout { get; set; } = new LoadoutSettings();

            /// <summary>
            /// Social information for the player, akin to <see cref="AccountClientProfile"/>'s.
            /// </summary>
            [JsonProperty("social")]
            public JObject? Social { get; set; }

            /// <summary>
            /// A structure tracking achievement data.
            /// </summary>
            [JsonProperty("achievements")]
            public JObject? Achievements { get; set; }

            /// <summary>
            /// TODO: 
            /// </summary>
            [JsonProperty("reward_state")]
            public JObject? RewardState { get; set; }

            /// <summary>
            /// A structure specifying various information such as the user's default channel, setup version, community values acceptance, etc.
            /// </summary>
            [JsonProperty("dev")]
            public DeveloperSettings? Developer { get; set; }

            /// <summary>
            /// Additional fields which are not caught explicitly are retained here.
            /// </summary>
            [JsonExtensionData]
            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

            #region Classes
            /// <summary>
            /// The game stats settings for a <see cref="AccountServerProfile"/>.
            /// </summary>
            public class StatsSettings
            {
                /// <summary>
                /// Game stats for Echo Arena.
                /// </summary>
                [JsonProperty("arena")]
                public GameStats? Arena { get; set; } = new GameStats();

                /// <summary>
                /// Game stats for Echo Combat.
                /// </summary>
                [JsonProperty("combat")]
                public GameStats? Combat { get; set; } = new GameStats();

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                /// <summary>
                /// A structure containing gameplay stats for a given game type.
                /// </summary>
                public class GameStats
                {
                    /// <summary>
                    /// The player's level in the given game.
                    /// </summary>
                    [JsonProperty("Level")]
                    public Stat? Level { get; set; } = new Stat(1, "add", 1);

                    /// <summary>
                    /// Additional fields which are not caught explicitly are retained here.
                    /// </summary>
                    [JsonExtensionData]
                    public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                    public class Stat
                    {
                        [JsonProperty("cnt")]
                        public long? Count { get; set; }

                        [JsonProperty("op")]
                        public string? Operation { get; set; }

                        [JsonProperty("val")]
                        public long? Value { get; set; }

                        /// <summary>
                        /// Additional fields which are not caught explicitly are retained here.
                        /// </summary>
                        [JsonExtensionData]
                        public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                        public Stat(long? count = null, string? operation = null, long? value = null)
                        {
                            Count = count;
                            Operation = operation;
                            Value = value;
                        }
                    }
                }
            }

            /// <summary>
            /// The current profile loadout settings for a <see cref="AccountServerProfile"/>.
            /// e.g. armor types, color, profile icon, etc.
            /// </summary>
            public class LoadoutSettings
            {
                /// <summary>
                /// The underlying loadout sets.
                /// </summary>
                [JsonProperty("instances")]
                public LoadoutInstances? Instances { get; set; } = new LoadoutInstances();

                /// <summary>
                /// The number displayed alongside the user display.
                /// </summary>
                [JsonProperty("number")]
                public long? Number { get; set; } = 1;

                /// <summary>
                /// The number displayed on the body of the user.
                /// </summary>
                [JsonProperty("number_body")]
                public long? NumberBody { get; set; } = null;

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                /// <summary>
                /// A structure containing the sets of loadouts for the game.
                /// </summary>
                public class LoadoutInstances
                {
                    /// <summary>
                    /// A structure containing a unified loadout.
                    /// </summary>
                    [JsonProperty("unified")]
                    public Loadout? Unified { get; set; } = new Loadout();

                    /// <summary>
                    /// Additional fields which are not caught explicitly are retained here.
                    /// </summary>
                    [JsonExtensionData]
                    public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                    /// <summary>
                    /// A structure containing a unified loadout.
                    /// Note: Presumably this means unified across gametypes.
                    /// </summary>
                    public class Loadout
                    {
                        /// <summary>
                        /// A structure containing the actual armor/decal/style assignments for the loadout.
                        /// </summary>
                        [JsonProperty("slots")]
                        public LoadoutSlots? Slots { get; set; } = new LoadoutSlots();

                        /// <summary>
                        /// Additional fields which are not caught explicitly are retained here.
                        /// </summary>
                        [JsonExtensionData]
                        public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                        /// <summary>
                        /// A structure containing the actual armor/decal/style assignments for the loadout.
                        /// </summary>
                        public class LoadoutSlots
                        {
                            // Set the default loadout the game normally sets for any players.
                            // For reference, see `sourcedb\rad15\json\r14\defaultserverprofile.json`.

                            [JsonProperty("decal")]
                            public string? Decal { get; set; } = "decal_default";

                            [JsonProperty("decal_body")]
                            public string? DecalBody { get; set; } = "decal_default";

                            [JsonProperty("emote")]
                            public string? Emote { get; set; } = "emote_blink_smiley_a";

                            [JsonProperty("secondemote")]
                            public string? SecondEmote { get; set; } = "emote_blink_smiley_a";

                            [JsonProperty("tint")]
                            public string? Tint { get; set; } = "tint_neutral_a_default";

                            [JsonProperty("tint_body")]
                            public string? TintBody { get; set; } = "tint_neutral_a_default";

                            [JsonProperty("tint_alignment_a")]
                            public string? TintAlignmentA { get; set; } = "tint_blue_a_default";

                            [JsonProperty("tint_alignment_b")]
                            public string? TintAlignmentB { get; set; } = "tint_orange_a_default";

                            [JsonProperty("pattern")]
                            public string? Pattern { get; set; } = "pattern_default";

                            [JsonProperty("pattern_body")]
                            public string? PatternBody { get; set; } = "pattern_default";

                            [JsonProperty("pip")]
                            public string? Pip { get; set; } = "rwd_decalback_default";

                            [JsonProperty("chassis")]
                            public string? Chassis { get; set; } = "rwd_chassis_body_s11_a";

                            [JsonProperty("bracer")]
                            public string? Bracer { get; set; } = "rwd_bracer_default";

                            [JsonProperty("booster")]
                            public string? Booster { get; set; } = "rwd_booster_default";

                            [JsonProperty("title")]
                            public string? Title { get; set; } = "rwd_title_title_default";

                            [JsonProperty("tag")]
                            public string? Tag { get; set; } = "rwd_tag_s1_a_secondary";

                            [JsonProperty("banner")]
                            public string? Banner { get; set; } = "rwd_banner_s1_default";

                            [JsonProperty("medal")]
                            public string? Medal { get; set; } = "rwd_medal_default";

                            [JsonProperty("goal_fx")]
                            public string? GoalFX { get; set; } = "rwd_goal_fx_default";

                            [JsonProperty("emissive")]
                            public string? Emissive { get; set; } = "emissive_default";

                            /// <summary>
                            /// Additional fields which are not caught explicitly are retained here.
                            /// </summary>
                            [JsonExtensionData]
                            public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
                        }
                    }
                }
            }

            /// <summary>
            /// The unlockable player rewards for a <see cref="AccountServerProfile"/>.
            /// </summary>
            public class UnlocksSettings 
            { 
                /// <summary>
                /// Game stats for Echo Arena.
                /// </summary>
                [JsonProperty("arena")]
                public Dictionary<string, bool> Arena { get; set; } = new Dictionary<string, bool>();

                /// <summary>
                /// Game stats for Echo Combat.
                /// </summary>
                [JsonProperty("combat")]
                public Dictionary<string, bool> Combat { get; set; } = new Dictionary<string, bool>();

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                public UnlocksSettings()
                {
                    // Add the default unlockables the game normally adds for any players.
                    // For reference, see `sourcedb\rad15\json\r14\defaultserverprofile.json`.
                    Arena["decal_combat_flamingo_a"] = true;
                    Arena["decal_combat_logo_a"] = true;
                    Arena["decal_default"] = true;
                    Arena["decal_sheldon_a"] = true;
                    Arena["emote_blink_smiley_a"] = true;
                    Arena["emote_default"] = true;
                    Arena["emote_dizzy_eyes_a"] = true;
                    Arena["loadout_number"] = true;
                    Arena["pattern_default"] = true;
                    Arena["pattern_lightning_a"] = true;
                    Arena["rwd_banner_s1_default"] = true;
                    Arena["rwd_booster_default"] = true;
                    Arena["rwd_bracer_default"] = true;
                    Arena["rwd_chassis_body_s11_a"] = true;
                    Arena["rwd_decalback_default"] = true;
                    Arena["rwd_decalborder_default"] = true;
                    Arena["rwd_medal_default"] = true;
                    Arena["rwd_tag_default"] = true;
                    Arena["rwd_tag_s1_a_secondary"] = true;
                    Arena["rwd_title_title_default"] = true;
                    Arena["tint_blue_a_default"] = true;
                    Arena["tint_neutral_a_default"] = true;
                    Arena["tint_neutral_a_s10_default"] = true;
                    Arena["tint_orange_a_default"] = true;
                    Arena["rwd_goal_fx_default"] = true;
                    Arena["emissive_default"] = true;

                    Combat["rwd_booster_s10"] = true;
                    Combat["rwd_chassis_body_s10_a"] = true;
                }
            }

            /// <summary>
            /// The development settings for a <see cref="AccountServerProfile"/>.
            /// </summary>
            public class DeveloperSettings
            {
                /// <summary>
                /// Disables AFK timeout.
                /// </summary>
                [JsonProperty("disable_afk_timeout")]
                public bool? DisableAfkTimeout { get; set; }

                /// <summary>
                /// The platform identifier for the account. If set, this shows the account as a developer.
                /// </summary>
                [JsonProperty("xplatformid")]
                public string? XPlatformId { get; set; }

                /// <summary>
                /// Additional fields which are not caught explicitly are retained here.
                /// </summary>
                [JsonExtensionData]
                public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();

                public DeveloperSettings(XPlatformId? accountId = null, bool disableAfkTimeout=false)
                {
                    XPlatformId = accountId?.ToString();
                    DisableAfkTimeout = disableAfkTimeout;
                }
            }
            #endregion

        }
    }
}
