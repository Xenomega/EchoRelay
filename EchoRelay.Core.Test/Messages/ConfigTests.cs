using EchoRelay.Core.Server.Messages.Config;

namespace EchoRelay.Core.Test.Messages
{
    public class ConfigTests
    {
        [Fact]
        public void TestConfigFailurev2()
        {
            ConfigFailurev2 message = new ConfigFailurev2();
            message.Decode(Convert.FromHexString("000000000000000000000000000000007b2274797065223a20225465737454797065222c20226964223a2022546573744964656e746966696572222c20226572726f72636f6465223a20372c20226572726f72223a202248656c6c6f21227d00"));
            Assert.Equal("TestType", message.Info.Type);
            Assert.Equal("TestIdentifier", message.Info.Identifier);
            Assert.Equal(7, message.Info.ErrorCode);
            Assert.Equal("Hello!", message.Info.Error);
        }

        [Fact]
        public void TestConfigSuccess()
        {
            ConfigSuccessv2 message = new ConfigSuccessv2();
            message.Decode(Convert.FromHexString("7b0000000000000041010000000000000d00000028b52ffd200d6900007b226e756d223a203838387d00"));
            Assert.Equal(123, message.TypeSymbol);
            Assert.Equal(321, message.IdentifierSymbol);
            Assert.Equal(888, message.Resource.AdditionalData["num"]);
        }
    }
}
