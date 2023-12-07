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

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(sbyte), messageReader);

            Assert.IsType<sbyte>(value);
            Assert.Equal(-47, (sbyte)value);
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

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(byte), messageReader);

            Assert.IsType<byte>(value);
            Assert.Equal(47, (byte)value);
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

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(short), messageReader);

            Assert.IsType<short>(value);
            Assert.Equal(-1036, (short)value);
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

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(ushort), messageReader);

            Assert.IsType<ushort>(value);
            Assert.Equal(1036, (ushort)value);
        }

        [Fact]
        public void Int32()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int32Converter();

            converter.Write(-831467035, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0xe5, 0xd1, 0x70, 0xce,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(int), messageReader);

            Assert.IsType<int>(value);
            Assert.Equal(-831467035, value);
        }

        [Fact]
        public void UInt32()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt32Converter();

            converter.Write(831467035u, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0x1b, 0x2e, 0x8f, 0x31,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(uint), messageReader);

            Assert.IsType<uint>(value);
            Assert.Equal(831467035u, value);
        }

        [Fact]
        public void Int64()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int64Converter();

            converter.Write(-831467035L, messageWriter);

            Assert.Equal(
                new byte[9]
                {
                    0x08, 0xe5, 0xd1, 0x70, 0xce, 0xff, 0xff, 0xff, 0xff,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(long), messageReader);

            Assert.IsType<long>(value);
            Assert.Equal(-831467035L, value);
        }

        [Fact]
        public void UInt64()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt64Converter();

            converter.Write(831467035ul, messageWriter);

            Assert.Equal(
                new byte[9]
                {
                    0x08, 0x1b, 0x2e, 0x8f, 0x31, 0x00, 0x00, 0x00, 0x00,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(ulong), messageReader);

            Assert.IsType<ulong>(value);
            Assert.Equal(831467035ul, value);
        }

        [Fact]
        public void Int128()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.Int128Converter();

            converter.Write((Int128)(-831467035), messageWriter);

            Assert.Equal(
                new byte[17]
                {
                    0x10, 0xe5, 0xd1, 0x70, 0xce, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff, 0xff,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(long), messageReader);

            Assert.IsType<Int128>(value);
            Assert.Equal(-831467035, (Int128)value);
        }

        [Fact]
        public void UInt128()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new IntegerConverters.UInt128Converter();

            converter.Write((UInt128)831467035, messageWriter);

            Assert.Equal(
                new byte[17]
                {
                    0x10, 0x1b, 0x2e, 0x8f, 0x31, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(UInt128), messageReader);

            Assert.IsType<UInt128>(value);
            Assert.Equal((UInt128)831467035, (UInt128)value);
        }
    }
}
