using Newtonsoft.Json;

namespace EchoRelay.Core.Server.Storage.Types.DocumentTypes
{
    /// <summary>
    /// A JSON resource used to represent the End-User License Agreement (EULA) <see cref="DocumentResource"/>.
    /// It is served through the <see cref="Services.Login.LoginService"/> to clients, which must accept it and advance 
    /// their profile's "accepted version" to avoid seeing it again.
    /// 
    /// NOTE: This type is dynamic and accepts extra fields. As such, despite it inheriting from <see cref="DocumentResource"/>, it should
    /// not be passed into functions that deserialize a <see cref="DocumentResource"/> in case the difference in included fields makes some fields be lost.
    /// </summary>
    public class EulaDocumentResource : DocumentResource
    {
        #region Properties
        /// <summary>
        /// The version of the EULA. Clients track the version they've accepted in their profiles.
        /// Increasing the version forces users to accept the new version.
        /// </summary>
        [JsonProperty("version")]
        public long? Version { get; set; }

        /// <summary>
        /// The version of the EULA for game admins. See <see cref="Version"/>'s documentation for more information
        /// about versioning.
        /// </summary>
        [JsonProperty("version_ga")]
        public long? VersionGameAdmin { get; set; }

        /// <summary>
        /// The EULA text for normal users.
        /// </summary>
        [JsonProperty("text")]
        public string? Text { get; set; }

        /// <summary>
        /// The EULA text for game admins.
        /// </summary>
        [JsonProperty("text_ga")]
        public string? TextGameAdmin { get; set; }

        /// <summary>
        /// The JSON key to use to mark the document as being read in the user's profile.
        /// </summary>
        [JsonProperty("mark_as_read_profile_key")]
        public string? MarkAsReadProfileKey { get; set; }

        /// <summary>
        /// The JSON key to use to mark the document as being read in the user's profile.
        /// </summary>
        [JsonProperty("mark_as_read_profile_key_ga")]
        public string? MarkAsReadProfileKeyGameAdmin { get; set; }


        /// <summary>
        /// The URI to the code of conduct and reporting page.
        /// </summary>
        [JsonProperty("link_cc")]
        public string? CodeOfConductLink { get; set; }

        /// <summary>
        /// The URI to the privacy policy page.
        /// </summary>
        [JsonProperty("link_pp")]
        public string? PrivacyPolicyLink { get; set; }

        /// <summary>
        /// The URI to the full Echo VR EULA page.
        /// </summary>
        [JsonProperty("link_vr")]
        public string? EchoVREulaLink { get; set; }

        /// <summary>
        /// The URI to the code of conduct for virtual experiences.
        /// </summary>
        [JsonProperty("link_cp")]
        public string? CodeOfConductVRLink { get; set; }

        /// <summary>
        /// The URI to the Echo Combat page.
        /// </summary>
        [JsonProperty("link_ec")]
        public string? EchoCombatLink { get; set; }

        /// <summary>
        /// The URI to the Echo Arena page.
        /// </summary>
        [JsonProperty("link_ea")]
        public string? EchoArenaLink { get; set; }

        /// <summary>
        /// The URI to the Game Admins page.
        /// </summary>
        [JsonProperty("link_ga")]
        public string? GameAdminsLink { get; set; }

        /// <summary>
        /// The URI to the Terms and Conditions page.
        /// </summary>
        [JsonProperty("link_tc")]
        public string? TermsAndConditionsLink { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="EulaDocumentResource"/>.
        /// </summary>
        public EulaDocumentResource() : base()
        {
        }
        #endregion
    }
}
