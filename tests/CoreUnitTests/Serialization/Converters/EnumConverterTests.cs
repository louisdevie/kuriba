using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    public class EnumConverterTests
    {
        private enum Int32Enum { A, B, C }

        private enum OtherInt32Enum { X = -1, Y = -2, Z = -3 }

        private enum ByteEnum : byte { A = 0x33, B = 0x44, C = 0x55 };

        private enum Int16Enum : short { A, B, C };

        [Fact]
        public void CanConvert()
        {
            ConverterFactory factory = new ConverterFactory();
            factory.AddConverter(new IntegerConverters.Int32Converter());
            factory.AddConverter(new IntegerConverters.ByteConverter());
            GenericConverter genericConverter = new EnumConverter();

            // can convert enums with a supported underlying type
            Assert.True(genericConverter.CanConvert(typeof(Int32Enum), factory));
            Assert.True(genericConverter.CanConvert(typeof(ByteEnum), factory));

            // cannot convert enums otherwise
            Assert.False(genericConverter.CanConvert(typeof(Int16Enum), factory));
            // cannot convert random other types
            Assert.False(genericConverter.CanConvert(typeof(int), factory));
        }

        [Fact]
        public void Specialize()
        {
            ConverterFactory factory = new ConverterFactory();
            factory.AddConverter(new IntegerConverters.Int32Converter());
            factory.AddConverter(new IntegerConverters.ByteConverter());
            GenericConverter genericConverter = new EnumConverter();

            Converter int32EnumConverter = genericConverter.SpecializeFor(typeof(Int32Enum), factory);
            Converter byteEnumConverter = genericConverter.SpecializeFor(typeof(ByteEnum), factory);

            Assert.True(int32EnumConverter.CanConvert(typeof(Int32Enum), factory));
            Assert.True(int32EnumConverter.CanConvert(typeof(OtherInt32Enum), factory));
            Assert.False(int32EnumConverter.CanConvert(typeof(ByteEnum), factory));
            Assert.False(int32EnumConverter.CanConvert(typeof(int), factory));

            Assert.True(byteEnumConverter.CanConvert(typeof(ByteEnum), factory));
            Assert.False(byteEnumConverter.CanConvert(typeof(Int32Enum), factory));
            Assert.False(byteEnumConverter.CanConvert(typeof(byte), factory));

        }

        [Fact]
        public void Int32()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            factory.AddConverter(new IntegerConverters.Int32Converter());
            GenericConverter genericConverter = new EnumConverter();
            Converter int32EnumConverter = genericConverter.SpecializeFor(typeof(Int32Enum), factory);

            int32EnumConverter.Write(Int32Enum.B, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0x01, 0x00, 0x00, 0x00,
                },
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            int32EnumConverter.Write(OtherInt32Enum.Z, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0xfd, 0xff, 0xff, 0xff,
                },
                output.ToArray()
            );
        }

        [Fact]
        public void Byte()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            factory.AddConverter(new IntegerConverters.ByteConverter());
            GenericConverter genericConverter = new EnumConverter();
            Converter int32EnumConverter = genericConverter.SpecializeFor(typeof(ByteEnum), factory);

            int32EnumConverter.Write(ByteEnum.A, messageWriter);

            Assert.Equal(
                new byte[2]
                {
                    0x01, 0x33
                },
                output.ToArray()
            );
        }
    }
}
