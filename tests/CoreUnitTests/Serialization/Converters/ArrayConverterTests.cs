using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    public class ArrayConverterTests
    {
        private class Anything { }

        [Fact]
        public void CanConvert()
        {
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new IntegerConverters.Int32Converter());
            factory.AddConverter(new TextConverters.StringConverter());

            // cannot convert any type
            Assert.False(genericArrayConverter.CanConvert(typeof(Anything), factory));

            // can convert arrays
            Assert.True(genericArrayConverter.CanConvert(typeof(int[]), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(string[]), factory));
            // except if there is no converter for the element type
            Assert.False(genericArrayConverter.CanConvert(typeof(Anything[]), factory));

            // can convert lists
            Assert.True(genericArrayConverter.CanConvert(typeof(List<int>), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(List<string>), factory));
            // except if there is no converter for the element type, or if it is the generic type
            Assert.False(genericArrayConverter.CanConvert(typeof(List<Anything>), factory));
            Assert.False(genericArrayConverter.CanConvert(typeof(List<>), factory));

            // can convert nested arrays and lists
            Assert.True(genericArrayConverter.CanConvert(typeof(List<List<int>>), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(List<int[]>), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(List<int>[]), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(int[][]), factory));
            Assert.True(genericArrayConverter.CanConvert(typeof(int[,,]), factory));
        }

        [Fact]
        public void Specialize()
        {
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new IntegerConverters.Int32Converter());
            factory.AddConverter(new TextConverters.StringConverter());

            Converter intArrayConverter = genericArrayConverter.SpecializeFor(typeof(int[]), factory);

            Assert.IsType<SpecializedArrayConverter>(intArrayConverter);
            Assert.True(intArrayConverter.CanConvert(typeof(int[]), factory));
            Assert.False(intArrayConverter.CanConvert(typeof(string[]), factory));
            Assert.False(intArrayConverter.CanConvert(typeof(List<int>), factory));
            Assert.False(intArrayConverter.CanConvert(typeof(List<string>), factory));

            Converter stringArrayConverter = genericArrayConverter.SpecializeFor(typeof(string[]), factory);

            Assert.IsType<SpecializedArrayConverter>(stringArrayConverter);
            Assert.False(stringArrayConverter.CanConvert(typeof(int[]), factory));
            Assert.True(stringArrayConverter.CanConvert(typeof(string[]), factory));
            Assert.False(stringArrayConverter.CanConvert(typeof(List<int>), factory));
            Assert.False(stringArrayConverter.CanConvert(typeof(List<string>), factory));

            Converter intListConverter = genericArrayConverter.SpecializeFor(typeof(List<int>), factory);

            Assert.IsType<SpecializedArrayConverter>(intArrayConverter);
            Assert.False(intListConverter.CanConvert(typeof(int[]), factory));
            Assert.False(intListConverter.CanConvert(typeof(string[]), factory));
            Assert.True(intListConverter.CanConvert(typeof(List<int>), factory));
            Assert.False(intListConverter.CanConvert(typeof(List<string>), factory));

            Converter stringListConverter = genericArrayConverter.SpecializeFor(typeof(List<string>), factory);

            Assert.IsType<SpecializedArrayConverter>(stringArrayConverter);
            Assert.False(stringListConverter.CanConvert(typeof(int[]), factory));
            Assert.False(stringListConverter.CanConvert(typeof(string[]), factory));
            Assert.False(stringListConverter.CanConvert(typeof(List<int>), factory));
            Assert.True(stringListConverter.CanConvert(typeof(List<string>), factory));
        }

        [Fact]
        public void Int32ArrayAndList()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new IntegerConverters.Int32Converter());
            Converter arrayConverter = genericArrayConverter.SpecializeFor(typeof(int[]), factory);
            Converter listConverter = genericArrayConverter.SpecializeFor(typeof(List<int>), factory);

            byte[] expectedResult =
                new byte[17]
                {
                    0x80, // array field  | header
                    0x03, // three values |
                    0x04, 23, 0, 0, 0, // first value  |
                    0x04, 45, 0, 0, 0, // second value | payload
                    0x04, 67, 0, 0, 0, // third value  |
                };

            arrayConverter.Write(new int[] { 23, 45, 67 }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            listConverter.Write(new List<int> { 23, 45, 67 }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());
        }

        [Fact]
        public void StringArrayAndList()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new TextConverters.StringConverter());
            Converter arrayConverter = genericArrayConverter.SpecializeFor(typeof(string[]), factory);
            Converter listConverter = genericArrayConverter.SpecializeFor(typeof(List<string>), factory);

            byte[] expectedResult =
                new byte[16]
                {
                    0x80, // array field | header
                    0x02, // two values  |
                    0x1f, 0x05, 0x48, 0x65, 0x6c, 0x6c, 0x6f, // first value  | payload
                    0x1f, 0x05, 0x57, 0x6F, 0x72, 0x6C, 0x64, // second value |
                };

            arrayConverter.Write(new string[] { "Hello", "World" }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            listConverter.Write(new List<string> { "Hello", "World" }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());
        }

        [Fact]
        public void NestedArrayAndList()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new IntegerConverters.Int32Converter());
            Converter arrayOfArrayConverter = genericArrayConverter.SpecializeFor(typeof(int[][]), factory);
            Converter arrayOfListConverter = genericArrayConverter.SpecializeFor(typeof(List<int>[]), factory);
            Converter listOfArrayConverter = genericArrayConverter.SpecializeFor(typeof(List<int[]>), factory);
            Converter listOfListConverter = genericArrayConverter.SpecializeFor(typeof(List<List<int>>), factory);

            byte[] expectedResult =
                new byte[]
                {
                    0x80, // array field | header                            |
                    0x02, // two values  |                                   |
                    //                                                       |
                    0x80, // array field  | header     |                     |
                    0x03, // three values |            |                     |
                    0x04, 23, 0, 0, 0, // first value  | first nested array  |
                    0x04, 45, 0, 0, 0, // second value |                     | array of arrays
                    0x04, 67, 0, 0, 0, // third value  |                     |
                    //                                                       |
                    0x80, // array field  | header     |                     |
                    0x02, // two values   |            |                     |
                    0x04, 89, 0, 0, 0, // first value  | second nested array |
                    0x04, 01, 0, 0, 0, // second value |                     |
                };

            arrayOfArrayConverter.Write(new int[][] { new int[] { 23, 45, 67 }, new int[] { 89, 1 } }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            arrayOfListConverter.Write(new List<int>[] { new() { 23, 45, 67 }, new() { 89, 1 } }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            listOfArrayConverter.Write(new List<int[]> { new int[] { 23, 45, 67 }, new int[] { 89, 1 } }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            listOfListConverter.Write(new List<List<int>> { new() { 23, 45, 67 }, new() { 89, 1 } }, messageWriter);

            Assert.Equal(expectedResult, output.ToArray());
        }

        [Fact]
        public void MultiDimensionalArrays()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            ConverterFactory factory = new ConverterFactory();
            GenericConverter genericArrayConverter = new ArrayConverter();
            factory.AddConverter(genericArrayConverter);
            factory.AddConverter(new IntegerConverters.ByteConverter());

            Converter array2DConverter = genericArrayConverter.SpecializeFor(typeof(byte[,]), factory);

            array2DConverter.Write(new byte[2, 4] { { 11, 22, 33, 44 }, { 15, 30, 45, 60 } }, messageWriter);

            Assert.Equal(
                new byte[]
                {
                    0x80, // array field
                    0x02, // first dimension

                    0x80, // nested array field
                    0x04, // second dimension
                    0x01, 11, 0x01, 22, 0x01, 33, 0x01, 44, // first data slice

                    0x80, // | repeated header
                    0x04, // |
                    0x01, 15, 0x01, 30, 0x01, 45, 0x01, 60, // second data slice
                },
                output.ToArray()
            );

            // reset memory stream without reallocating
            output.Position = 0;
            output.SetLength(0);

            Converter array3DConverter = genericArrayConverter.SpecializeFor(typeof(byte[,,]), factory);

            array3DConverter.Write(new byte[2, 2, 2] { { { 0, 1 }, { 2, 3 } }, { { 4, 5 }, { 6, 7 } } }, messageWriter);

            Assert.Equal(
                new byte[30]
                {
                    0x80, // array field
                    0x02, // first dimension

                    0x80, // nested array field
                    0x02, // second dimension

                    0x80, // nested array field
                    0x02, // third dimension
                    0x01, 0b000, 0x01, 0b001, // first data slice
                    0x80, // |
                    0x02, // | repeated thrid dimension header
                    0x01, 0b010, 0x01, 0b011, // second data slice

                    0x80, // |
                    0x02, // | repeated second dimension header

                    0x80, // | 
                    0x02, // | repeated thrid dimension header
                    0x01, 0b100, 0x01, 0b101, // third data slice
                    0x80, // |
                    0x02, // | repeated thrid dimension header
                    0x01, 0b110, 0x01, 0b111, // fourth data slice
                },
                output.ToArray()
            );
        }
    }
}
