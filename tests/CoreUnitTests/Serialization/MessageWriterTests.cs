using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Kuriba.Core.Serialization
{
    public class MessageWriterTests
    {
        private Random random = new();

        private byte[] RandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)random.Next(256);
            }
            return bytes;
        }

        [Fact]
        public void Write8BitsField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            messageWriter.Write8BitsField(0x47);

            Assert.Equal(
                new byte[2]
                {
                    0x01, // header indicating a 8 bits field
                    0x47, // value
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Write16BitsField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            byte[] data = RandomBytes(2);

            messageWriter.Write16BitsField(data);

            byte[] expectedResult = new byte[3];
            expectedResult[0] = 0x02; // header indicating a 16 bits field
            Array.Copy(data, 0, expectedResult, 1, 2); // then the value

            Assert.Equal(expectedResult, output.ToArray());

            Assert.Throws<ArgumentException>(() => messageWriter.Write16BitsField(RandomBytes(1)));
            Assert.Throws<ArgumentException>(() => messageWriter.Write16BitsField(RandomBytes(7)));
        }

        [Fact]
        public void Write32BitsField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            byte[] data = RandomBytes(4);

            messageWriter.Write32BitsField(data);

            byte[] expectedResult = new byte[5];
            expectedResult[0] = 0x04; // header indicating a 32 bits field
            Array.Copy(data, 0, expectedResult, 1, 4); // then the value

            Assert.Equal(expectedResult, output.ToArray());

            Assert.Throws<ArgumentException>(() => messageWriter.Write32BitsField(RandomBytes(3)));
            Assert.Throws<ArgumentException>(() => messageWriter.Write32BitsField(RandomBytes(11)));
        }

        [Fact]
        public void Write64BitsField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            byte[] data = RandomBytes(8);

            messageWriter.Write64BitsField(data);

            byte[] expectedResult = new byte[9];
            expectedResult[0] = 0x08; // header indicating a 64 bits field
            Array.Copy(data, 0, expectedResult, 1, 8); // then the value

            Assert.Equal(expectedResult, output.ToArray());

            Assert.Throws<ArgumentException>(() => messageWriter.Write64BitsField(RandomBytes(5)));
            Assert.Throws<ArgumentException>(() => messageWriter.Write64BitsField(RandomBytes(15)));
        }

        [Fact]
        public void Write128BitsField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            byte[] data = RandomBytes(16);

            messageWriter.Write128BitsField(data);

            byte[] expectedResult = new byte[17];
            expectedResult[0] = 0x10; // header indicating a 128 bits field
            Array.Copy(data, 0, expectedResult, 1, 16); // then the value

            Assert.Equal(expectedResult, output.ToArray());

            Assert.Throws<ArgumentException>(() => messageWriter.Write128BitsField(RandomBytes(7)));
            Assert.Throws<ArgumentException>(() => messageWriter.Write128BitsField(RandomBytes(19)));
        }

        [Fact]
        public void WriteVarField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            byte[] data, expectedResult;

            //==================== NO DATA ====================//

            messageWriter.WriteVarField(Array.Empty<byte>());

            Assert.Equal(
                new byte[2] { 0x1f, 0 }, // header indicating empty field
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 1-BYTE SIZE MARKER ===============//

            data = RandomBytes(107);

            messageWriter.WriteVarField(data);

            expectedResult = new byte[109];
            expectedResult[0] = 0x1f; // variable-length field           | header
            expectedResult[1] = 0b0110_1011; // actual size is 107 bytes | 
            Array.Copy(data, 0, expectedResult, 2, 107); // then the data

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 2-BYTE SIZE MARKER ===============//

            data = RandomBytes(1234);

            messageWriter.WriteVarField(data);

            expectedResult = new byte[1237];
            expectedResult[0] = 0x1f; // variable-length field         | 
            expectedResult[1] = 0b1000_1001; // | actual size (1234)   | header
            expectedResult[2] = 0b0101_0010; // | split over two bytes |
            Array.Copy(data, 0, expectedResult, 3, 1234); // then the data

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 3-BYTE SIZE MARKER ===============//

            data = RandomBytes(23456);

            messageWriter.WriteVarField(data);

            expectedResult = new byte[23460];
            expectedResult[0] = 0x1f; // variable-length field           |
            expectedResult[1] = 0b1000_0001; // | actual size (23460)    | header
            expectedResult[2] = 0b1011_0111; // | split over three bytes | 
            expectedResult[3] = 0b0010_0000; // |                        |
            Array.Copy(data, 0, expectedResult, 4, 23456); // then the data

            Assert.Equal(expectedResult, output.ToArray());
        }

        [Fact]
        public void WriteArrayHeader()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            //================== EMPTY ARRAY ==================//

            messageWriter.WriteArrayHeader(0);

            Assert.Equal(
                new byte[2] { 0x80, 0 }, // header indicating empty array
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 1-BYTE SIZE MARKER ===============//

            messageWriter.WriteArrayHeader(107);

            Assert.Equal(
                new byte[2] { 0x80, 107 }, // header indicating an array of 107 values
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 2-BYTE SIZE MARKER ===============//

            messageWriter.WriteArrayHeader(1234);

            Assert.Equal(
                new byte[3] {
                    0x80, // array field                   |
                    0b1000_1001, // | actual size (1234)   | header
                    0b0101_0010, // | split over two bytes |
                },
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            //============== 3-BYTE SIZE MARKER ===============//

            messageWriter.WriteArrayHeader(23456);

            Assert.Equal(
                new byte[4] {
                    0x80, // array field                     |
                    0b1000_0001, // | actual size (23460)    | header
                    0b1011_0111, // | split over three bytes | 
                    0b0010_0000, // |                        |
                },
                output.ToArray()
            );
        }

        [Fact]
        public void WriteEmptyField()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));

            messageWriter.WriteEmptyField();

            Assert.Equal(
                new byte[1] { 0xff }, // header indicating there is no value
                output.ToArray()
            );
        }
    }
}
