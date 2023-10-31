using EchoRelay.Core.Utils;

namespace EchoRelay.Core.Test.Utils
{
    public class CompressionTests
    {
        [Fact]
        public void TestZlibCompression()
        {
            // Create a map of compressed hex strings to decompressed results
            var testCases = new Dictionary<string, string>
            {
                { 
                    // Compressed original value
                    "789c2bafa8040002d10169", 

                    // Expected decompressed value
                    "777879"
                },
                {
                     // Compressed original value
                    "789c2bafa86c98307112000dd9039c",

                    // Expected decompressed value
                    "77787980909192"
                }
            };

            // Loop for each test case
            foreach (var testCase in testCases)
            {
                // Obtain the test case values as bytes
                byte[] compressed = Convert.FromHexString(testCase.Key);
                byte[] expectedUncompressed = Convert.FromHexString(testCase.Value);

                // Decompress the compressed data and ensure it matches our expected result.
                Assert.Equal(expectedUncompressed, Compression.DecompressZlib(compressed));
            }
        }

        [Fact]
        public void TestZstdCompression()
        {
            // Create a map of compressed hex strings to decompressed results
            var testCases = new Dictionary<string, string>
            {
                { 
                    // Compressed original value
                    "28b52ffd2003190000777879", 

                    // Expected decompressed value
                    "777879"
                },
                {
                     // Compressed original value
                    "28b52ffd200739000077787980909192",

                    // Expected decompressed value
                    "77787980909192"
                }
            };

            // Loop for each test case
            foreach (var testCase in testCases)
            {
                // Obtain the test case values as bytes
                byte[] compressed = Convert.FromHexString(testCase.Key);
                byte[] expectedUncompressed = Convert.FromHexString(testCase.Value);

                // Decompress the compressed data and ensure it matches our expected result.
                Assert.Equal(expectedUncompressed, Compression.DecompressZstd(compressed));
            }
        }
    }
}
