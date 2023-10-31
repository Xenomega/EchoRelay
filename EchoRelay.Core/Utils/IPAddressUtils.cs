using System.Net;

namespace EchoRelay.Core.Utils
{
    /// <summary>
    /// Provides extensions for <see cref="IPAddress"/>s.
    /// </summary>
    public static class IPAddressUtils
    {
        /// <summary>
        /// Converts an <see cref="IPAddress"/> into a big endian 32-bit unsigned integer.
        /// </summary>
        /// <param name="address">The address to convert into an integer.</param>
        /// <returns>The integer which represents the provided address.</returns>
        public static uint ToUInt32(this IPAddress address)
        {
            byte[] bytes = address.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// Converts a big endian 32-bit unsigned integer into an <see cref="IPAddress"/>.
        /// </summary>
        /// <param name="addressUint">The integer to convert into an address.</param>
        /// <returns>Returns an <see cref="IPAddress"/> that represents the integer.</returns>
        public static IPAddress ToIPAddress(this uint addressUint)
        {
            byte[] bytes = BitConverter.GetBytes(addressUint);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        /// <summary>
        /// Checks whether the IP address is an IPv4 address that falls within the private address range.
        /// </summary>
        /// <param name="address">The IPv4 address to check.</param>
        /// <returns>Returns true if the address is a knownprivate address, false otherwise.</returns>
        public static bool IsPrivate(this IPAddress address)
        {
            // Check if the address is an IPv4 that falls in the private IP range.
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                // Obtain the bytes and check if they fall in the private range.
                byte[] ipBytes = address.GetAddressBytes();

                // 10.x.x.x
                if (ipBytes[0] == 10)
                {
                    return true;
                }
                // 172.16.x.x
                else if (ipBytes[0] == 172 && ipBytes[1] == 16)
                {
                    return true;
                }
                // 192.168.x.x
                else if (ipBytes[0] == 192 && ipBytes[1] == 168)
                {
                    return true;
                }
                // 169.254.x.x
                else if (ipBytes[0] == 169 && ipBytes[1] == 254)
                {
                    return true;
                }
                // 127.0.0.1
                else if (ipBytes[0] == 127 && ipBytes[1] == 0 && ipBytes[2] == 0 && ipBytes[3] == 1)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Obtains the public/external IP address of the current machine.
        /// </summary>
        /// <returns>Returns the public IP address of the current machine, or null if it could not be obtained.</returns>
        public static async Task<IPAddress?> GetExternalIPAddress()
        {
            try
            {
                // Request the IP from a server, and sanitize the response.
                string? externalIP = (await new HttpClient().GetStringAsync("https://ipinfo.io/ip"))
                    .Replace("\\r\\n", "").Replace("\\n", "").Trim();

                // Try to parse an IP address from the sanitized response data.
                if (IPAddress.TryParse(externalIP, out IPAddress? address))
                    return address;
            }
            catch { }

            return null;
        }
    }
}
