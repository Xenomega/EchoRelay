using EchoRelay.Core.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace EchoRelay.Core.Server.Storage.Types
{
    /// <summary>
    /// Access control lists dictating allow and disallow rules for IPs, supporting wildcard ("*") matching.
    /// The allow rules are checked prior to the disallow rules.
    /// </summary>
    public class AccessControlListResource
    {
        #region Properties
        /// <summary>
        /// IPv4 string filters that dictate which connections can be established for a <see cref="Server"/>.
        /// This is applied before the <see cref="BannedList"/>.
        /// </summary>
        [JsonProperty("allow_rules")]
        [JsonConverter(typeof(JsonUtils.HashSetConverter<string>))]
        public HashSet<string> AllowRules { get; set; }

        /// <summary>
        /// IPv4 string filters that dictate which connections can not be established for a <see cref="Server"/>.
        /// This is applied after the <see cref="AllowRules"/>.
        /// </summary>
        [JsonProperty("disallow_rules")]
        [JsonConverter(typeof(JsonUtils.HashSetConverter<string>))]
        public HashSet<string> DisallowRules { get; set; }

        /// <summary>
        /// Additional fields which are not caught explicitly are retained here.
        /// </summary>
        [JsonExtensionData]
        public IDictionary<string, JToken> AdditionalData = new Dictionary<string, JToken>();
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new <see cref="AccessControlListResource"/>.
        /// </summary>
        public AccessControlListResource()
        {
            AllowRules = new HashSet<string>();
            DisallowRules = new HashSet<string>();
        }
        /// <summary>
        /// Initializes a new <see cref="AccessControlListResource"/> with the provided arguments.
        /// </summary>
        public AccessControlListResource(IEnumerable<string> allowRules, IEnumerable<string> disallowRules)
        {
            AllowRules = new HashSet<string>(allowRules);
            DisallowRules = new HashSet<string>(disallowRules);
        }
        #endregion

        #region Functions
        /// <summary>
        /// Checks if an address string matches an array of rules.
        /// </summary>
        /// <param name="address">The IP address string to match.</param>
        /// <param name="rules">The rules to match against.</param>
        /// <returns>Returns true if any rule matched, false otherwise.</returns>
        private bool MatchAddressToRules(string address, string[] rules)
        {
            // Try to match any rule to this address string.
            foreach (string rule in rules)
            {
                // Create a regex by converting wildcard expressions.
                // Note: Other regex expressions would be retained here. It is the caller's responsibility to provide a string only containing numerics, '.' and '*' characters.
                string pattern = rule.ToLower().Replace(".", "\0").Replace("*", ".*").Replace("\0", "\\.");
                Regex regex = new Regex(pattern);

                // If we have a match, report it immediately
                if (regex.IsMatch(address.ToLower()))
                    return true;
            }

            // No rule matched.
            return false;
        }

        /// <summary>
        /// Checks whether a given IP address passes the allow list/disallow list.
        /// </summary>
        /// <param name="address">The address to check is allowed through the access controls list.</param>
        /// <returns>Returns true if the IP address should be allowed, false otherwise.</returns>
        public bool CheckAuthorized(IPAddress address)
        {
            // Obtain the IP address as a string
            string ipAddress = address.ToString();

            // Verify at least one allow rule matches the address.
            if (!MatchAddressToRules(ipAddress, AllowRules.ToArray()))
                return false;

            // Verify no disallow rule matches the address.
            return !MatchAddressToRules(ipAddress, DisallowRules.ToArray());
        }
        #endregion
    }
}



