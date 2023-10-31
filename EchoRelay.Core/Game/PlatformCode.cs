namespace EchoRelay.Core.Game
{
    /// <summary>
    /// Describes the platforms which a client may be operating on.
    /// </summary>
    public enum PlatformCode : int
    {
        /// <summary>
        /// Steam
        /// </summary>
        STM = 1,

        /// <summary>
        /// Playstation
        /// </summary>
        PSN = 2,

        /// <summary>
        /// Xbox
        /// </summary>
        XBX = 3,

        /// <summary>
        /// Oculus VR user
        /// </summary>
        OVR_ORG = 4,

        /// <summary>
        /// Oculus VR
        /// </summary>
        OVR = 5,

        /// <summary>
        /// Bot/AI
        /// </summary>
        BOT = 6,

        /// <summary>
        /// Demo (no ovr)
        /// </summary>
        DMO = 7,

        /// <summary>
        /// TODO: Tencent?
        /// </summary>
        TEN = 8
    }

    /// <summary>
    /// Describes extension methods for a given <see cref="PlatformCode"/>.
    /// </summary>
    public static class PlatformCodeExtensions
    {
        /// <summary>
        /// Obtains a platform prefix string for a given <see cref="PlatformCode"/>.
        /// </summary>
        /// <param name="code">The <see cref="PlatformCode"/> to obtain an platform prefix string for.</param>
        /// <returns>Returns the platform prefix string for the provided <see cref="PlatformCode"/>.</returns>
        public static string GetPrefix(this PlatformCode code)
        {
            // Try to obtain a name for this platform code.
            string? name = Enum.GetName(typeof(PlatformCode), code);

            // If we could obtain one, the prefix should just be the same as the name, but with underscores represented as dashes.
            if (name != null)
                return name.Replace("_", "-");

            // An unknown/invalid platform is denoted with the value returned below.
            return "???";
        }

        /// <summary>
        /// Obtains a display name for a given <see cref="PlatformCode"/>.
        /// </summary>
        /// <param name="code">The <see cref="PlatformCode"/> to obtain a display name for.</param>
        /// <returns>Returns the display name string for the provided <see cref="PlatformCode"/>.</returns>
        public static string GetDisplayName(this PlatformCode code)
        {
            // Switch on the provided platform code and return a display name.
            switch (code)
            {
                case PlatformCode.STM:
                    return "Steam";
                case PlatformCode.PSN:
                    return "Playstation";
                case PlatformCode.XBX:
                    return "Xbox";
                case PlatformCode.OVR_ORG:
                    return "Oculus VR (ORG)";
                case PlatformCode.OVR:
                    return "Oculus VR";
                case PlatformCode.BOT:
                    return "Bot";
                case PlatformCode.DMO:
                    return "Demo";
                case PlatformCode.TEN:
                    return "Tencent"; // TODO: Verify, this is only suspected to be the target of "TEN".
                default:
                    return "Unknown";

            }
        }

        /// <summary>
        /// Parses a string generated from <see cref="PlatformCode"/>'s ToString() method back into a <see cref="PlatformCode"/>."/>
        /// </summary>
        /// <param name="s">The string to parse.</param>
        /// <returns>The <see cref="PlatformCode"/> parsed from the string, or 0 (invalid option) otherwise.</returns>
        public static PlatformCode Parse(string s)
        {
            // Convert any underscores in the string to dashes.
            s = s.Replace("-", "_");

            // Get the enum option to represent this.
            try
            {
                return (PlatformCode)Enum.Parse(typeof(PlatformCode), s);
            }
            catch
            {
                return 0;
            }
        }
    }
}
