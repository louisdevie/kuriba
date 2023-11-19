using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    public class IntegerConvertersTests
    {
        [Fact]
        public void SByte()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.SByteConverter();

            converter.Write((sbyte)(-47), messageWriter);

            Assert.Equal(
                new byte[2]
                {
                    0x01, 0xd1,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Byte()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.ByteConverter();

            converter.Write((byte)47, messageWriter);

            Assert.Equal(
                new byte[2]
                {
                    0x01, 0x2f,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Int16()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int16Converter();

            converter.Write((short)(-1036), messageWriter);

            Assert.Equal(
                new byte[3]
                {
                    0x02, 0xf4, 0xfb,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void UInt16()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt16Converter();

            converter.Write((ushort)1036, messageWriter);

            Assert.Equal(
                new byte[3]
                {
                    0x02, 0x0c, 0x04,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Int32()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int32Converter();

            converter.Write(-1036, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0xf4, 0xfb, 0xff, 0xff,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void UInt32()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt32Converter();

            converter.Write(1036u, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0x0c, 0x04, 0x00, 0x00,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Int64()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int64Converter();

            converter.Write(-1036L, messageWriter);

            Assert.Equal(
                new byte[9]
                {
                    0x08, 0xf4, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void UInt64()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt64Converter();

            converter.Write(1036ul, messageWriter);

            Assert.Equal(
                new byte[9]
                {
                    0x08, 0x0c, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Int128()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int128Converter();

            converter.Write((Int128)(-1036), messageWriter);

            Assert.Equal(
                new byte[17]
                {
                    0x10, 0xf4, 0xfb, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void UInt128()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt128Converter();

            converter.Write((UInt128)1036, messageWriter);

            Assert.Equal(
                new byte[17]
                {
                    0x10, 0x0c, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                },
                output.ToArray()
            );
        }
    }
}
