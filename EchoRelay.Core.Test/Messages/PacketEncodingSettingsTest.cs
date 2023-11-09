using EchoRelay.Core.Game;

namespace EchoRelay.Core.Test.Messages
{
    public class PacketEncodingSettingsTest
    {
        [Fact]
        public void TestPacketEncoderSettings()
        {
            // Test decoding and re-encoding of common packet encoder settings configs
            // One 32 byte, and one with 64 byte MAC digest sizes.
            PacketEncoderSettings e = new PacketEncoderSettings(0x80080080000083);
            Assert.True(e.EncryptionEnabled);
            Assert.True(e.MacEnabled);
            Assert.Equal(0x20, e.MacDigestSize);
            Assert.Equal(0x20, e.MacKeySize);
            Assert.Equal(0, e.MacPBKDF2IterationCount);
            Assert.Equal(0x20, e.EncryptionKeySize);
            Assert.Equal(0x20, e.RandomKeySize);
            Assert.Equal((ulong)0x80080080000083, (ulong)e);

            e = new PacketEncoderSettings(0x80080080000103);
            Assert.True(e.EncryptionEnabled);
            Assert.True(e.MacEnabled);
            Assert.Equal(0x40, e.MacDigestSize);
            Assert.Equal(0x20, e.MacKeySize);
            Assert.Equal(0, e.MacPBKDF2IterationCount);
            Assert.Equal(0x20, e.EncryptionKeySize);
            Assert.Equal(0x20, e.RandomKeySize);
            Assert.Equal((ulong)0x80080080000103, (ulong)e);
        }
    }
}
