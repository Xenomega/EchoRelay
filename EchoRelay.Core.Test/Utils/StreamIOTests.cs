using EchoRelay.Core.Utils;
using System.Net;

namespace EchoRelay.Core.Test.Utils
{
    public class StreamIOTests
    {
        [Fact]
        public void TestValueStreamingDefaultEndian()
        {
            // Try both byte orders
            ByteOrder[] byteOrders = { ByteOrder.LittleEndian, ByteOrder.BigEndian };
            foreach (ByteOrder byteOrder in byteOrders)
            {
                // Create a new IO
                StreamIO io = new StreamIO(byteOrder);

                // Create data to be streamed
                byte b = 0x77;
                byte[] bArr = new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 };
                short i16 = 0x1234;
                ushort ui16 = 0xFEDC;
                int i32 = 0x12345678;
                uint ui32 = 0x87654321;
                long i64 = 0x1122334455667788;
                ulong ui64 = 0x8877665544332211;
                Int128 i128 = Int128.Parse("-85070591730234615865843651857942052864");
                UInt128 ui128 = UInt128.Parse("85070591730234615865843651857942052864");
                float f32 = 0.1234567f;
                double f64 = 0.1234567891234;
                string str = "testUTF8Яα⾀";

                // Write the data, then read it back using stream methods
                StreamMode[] streamModes = { StreamMode.Write, StreamMode.Read };
                foreach (StreamMode streamMode in streamModes)
                {
                    // Set the stream mode, reset our IO position, and stream the data.
                    io.Position = 0;
                    io.StreamMode = streamMode;
                    io.Stream(ref b);
                    io.Stream(ref bArr);
                    io.Stream(ref i16);
                    io.Stream(ref ui16);
                    io.Stream(ref i32);
                    io.Stream(ref ui32);
                    io.Stream(ref i64);
                    io.Stream(ref ui64);
                    io.Stream(ref f32);
                    io.Stream(ref f64);
                    io.Stream(ref str, true);
                    io.Stream(ref i128);
                    io.Stream(ref ui128);

                    // If this is the write operation, reset values in preparation for the read, to be sure our code did stream it in properly.
                    if (streamMode == StreamMode.Write)
                    {
                        b = 0;
                        bArr = new byte[bArr.Length];
                        i16 = 0;
                        ui16 = 0;
                        i32 = 0;
                        ui32 = 0;
                        i64 = 0;
                        ui64 = 0;
                        f32 = 0;
                        f64 = 0;
                        str = "";
                        i128 = 0;
                        ui128 = 0;
                    }

                    Assert.Equal(io.Length, io.Position);
                }

                Assert.Equal((byte)0x77, b);
                Assert.Equal(new byte[] { 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99 }, bArr);
                Assert.Equal((short)0x1234, i16);
                Assert.Equal((ushort)0xFEDC, ui16);
                Assert.Equal(0x12345678, i32);
                Assert.Equal(0x87654321, ui32);
                Assert.Equal(0x1122334455667788, i64);
                Assert.Equal(0x8877665544332211, ui64);
                Assert.Equal(0.1234567f, f32);
                Assert.Equal(0.1234567891234, f64);
                Assert.Equal("testUTF8Яα⾀", str);
                Assert.Equal(Int128.Parse("-85070591730234615865843651857942052864"), i128);
                Assert.Equal(UInt128.Parse("85070591730234615865843651857942052864"), ui128);
                io.Close();
            }
        }

        [Fact]
        public void TestGuidStreaming()
        {
            // Create a new IO
            StreamIO io = new StreamIO(ByteOrder.LittleEndian);

            // Write a guid
            Guid guid = Guid.Parse("90dd4db5-b5dd-4655-839e-fdbe5f4bc0bf");


            // Write the data, then read it back using stream methods
            StreamMode[] streamModes = { StreamMode.Write, StreamMode.Read };
            foreach (StreamMode streamMode in streamModes)
            {
                // Set the stream mode, reset our IO position, and stream the data.
                io.Position = 0;
                io.StreamMode = streamMode;
                io.Stream(ref guid);

                // If this is the write operation, reset values in preparation for the read, to be sure our code did stream it in properly.
                if (streamMode == StreamMode.Write)
                {
                    guid = Guid.Parse("00000000-0000-0000-0000-000000000000");

                    // Since this is the only value we're writing, lets obtain the byte buffer and verify it.
                    byte[] guidBytes = io.ToArray();
                    Assert.Equal(Convert.FromHexString("b54ddd90ddb55546839efdbe5f4bc0bf"), guidBytes);
                }

                Assert.Equal(io.Length, io.Position);
            }

            Assert.Equal(Guid.Parse("90dd4db5-b5dd-4655-839e-fdbe5f4bc0bf"), guid);

            io.Close();
        }

        [Fact]
        public void TestIPAddress()
        {
            // Define our address bytes
            byte[] expectedBytes = new byte[]{ 0x11, 0x22, 0x33, 0x44 };
            IPAddress expectedAddr = IPAddress.Parse("17.34.51.68");

            // Read an IP address from bytes.
            StreamIO io = new StreamIO(expectedBytes, ByteOrder.LittleEndian, StreamMode.Read);
            IPAddress addr = io.ReadIPv4Address(ByteOrder.BigEndian);
            io.Close();
            Assert.Equal(expectedAddr, addr);

            // Write the IP address to bytes.
            io = new StreamIO(ByteOrder.BigEndian, StreamMode.Write);
            io.Write(addr);
            byte[] addrBytes = io.ToArray();
            io.Close();
            Assert.Equal(expectedBytes, addrBytes);
        }
    }
}
