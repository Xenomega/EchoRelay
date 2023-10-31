using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Game
{
    /// <summary>
    /// An identifier for a user on the platform.
    /// </summary>
    public class XPlatformId : IStreamable
    {
        #region Constants
        /// <summary>
        /// The serialized/streamed size of this object.
        /// </summary>
        public const int SIZE = 16;
        #endregion

        #region Properties/Fields
        private ulong _platformCode;

        /// <summary>
        /// The platform code the identifier belongs to.
        /// </summary>
        public PlatformCode PlatformCode
        {
            get { return (PlatformCode)_platformCode; }
            set { _platformCode = (ulong)value; }
        }
        /// <summary>
        /// The identifier of the account on the given platform.
        /// </summary>
        public ulong AccountId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new <see cref="XPlatformId"/>.
        /// </summary>
        public XPlatformId()
        {
        }
        /// <summary>
        /// Initializes a new <see cref="XPlatformId"/> with the provided arguments.
        /// </summary>
        /// <param name="platformCode">The platform the account belongs to.</param>
        /// <param name="accountId">The unique identifier of the account.</param>
        public XPlatformId(PlatformCode platformCode, ulong accountId)
        {
            PlatformCode = platformCode;
            AccountId = accountId;
        }
        #endregion

        #region Functions
        /// <summary>
        /// Verifies that the <see cref="XPlatformId"/> is well structured by ensuring it has a valid <see cref="PlatformCode"/>.
        /// </summary>
        /// <returns>Returns true if valid, otherwise false.</returns>
        public bool Valid()
        {
            return Enum.IsDefined(typeof(PlatformCode), PlatformCode);
        }

        /// <summary>
        /// Parses a string into a given platform identifier.
        /// </summary>
        /// <param name="s">The string to attempt to parse as a platform id.</param>
        /// <returns>The platform identifier, or null if it could not be parsed.</returns>
        public static XPlatformId? Parse(string s)
        {
            // Obtain the position of the last dash.
            int dashIndex = s.LastIndexOf('-');
            if (dashIndex < 0)
                return null;

            // Split it there
            string platformCodeStr = s.Substring(0, dashIndex);
            string accountIdStr = s.Substring(dashIndex + 1);

            // Determine the platform code.
            PlatformCode code = PlatformCodeExtensions.Parse(platformCodeStr);

            // Try to parse the account identifier
            if (!ulong.TryParse(accountIdStr, out ulong accountId))
                return null;

            // Create the identifier
            XPlatformId platformId = new XPlatformId(code, accountId);
            return platformId;
        }

        /// <summary>
        /// Streams the data in/out based on the streaming mode set.
        /// </summary>
        /// <param name="io">The stream to read/write data from/to.</param>
        public void Stream(StreamIO io)
        {
            io.Stream(ref _platformCode);
            io.Stream(ref AccountId);
        }

        /// <summary>
        /// Overloads the equality operator to ensure equality matches if the underlying fields match.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator ==(XPlatformId? a, XPlatformId? b)
        {
            // If the references match, they match.
            if (ReferenceEquals(a, b))
                return true;

            // If only one is null, they do not match.
            if (((object?)a == null) || ((object?)b == null))
            {
                return false;
            }

            // If both fields match, they match.
            return a.PlatformCode == b.PlatformCode && a.AccountId == b.AccountId;
        }
        /// <summary>
        /// Overloads the equality operator to ensure equality matches if the underlying fields match.
        /// </summary>
        /// <param name="a">The first object to compare.</param>
        /// <param name="b">The second object to compare.</param>
        /// <returns>The result of the comparison.</returns>
        public static bool operator !=(XPlatformId? a, XPlatformId? b)
        {
            // If the references match, they match.
            if (!ReferenceEquals(a, b))
                return true;

            // If only one is null, they do not match.
            if (((object?)a == null) || ((object?)b == null))
            {
                return true;
            }

            // If both fields match, they match.
            return a.PlatformCode != b.PlatformCode || a.AccountId != b.AccountId;
        }

        /// <summary>
        /// Checks equality of this object against another.
        /// This ensures two platform identifiers match even if they are different instances.
        /// </summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>The result of the equality check.</returns>
        public override bool Equals(object? obj)
        {
            // Obtain the object as a platform id.
            XPlatformId? objP = obj as XPlatformId;
            if (objP == null)
                return false;

            return AccountId == objP.AccountId && PlatformCode == objP.PlatformCode;
        }

        /// <summary>
        /// Obtains a hash code for this item to be used in dictionaries, etc.
        /// </summary>
        /// <returns>Returns the hashcode for this platform identifier.</returns>
        public override int GetHashCode()
        {
            return AccountId.GetHashCode() ^ PlatformCode.GetHashCode();
        }

        /// <summary>
        /// Obtains a string representation of the platform identifier (e.g. DMO-XXXXXXXXXXXXXXXXXX or OVR-ORG-XXXXXXXXXXXXXX ).
        /// </summary>
        /// <returns>A string representation of the platform identifier.</returns>
        public override string ToString()
        {
            return $"{PlatformCode.GetPrefix()}-{AccountId}";
        }
        #endregion
    }
}
