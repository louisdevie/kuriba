using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    public class NumberConvertersTests
    {
        [Fact]
        public void Half()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new NumberConverters.HalfConverter();

            Half oneThird = (Half)(1.0 / 3.0);
            converter.Write(oneThird, messageWriter);

            Assert.Equal(
                new byte[3]
                {
                    0x02, 0x55, 0x35,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(Half), messageReader);

            Assert.IsType<Half>(value);
            Assert.Equal(oneThird, (Half)value);
        }

        [Fact]
        public void Float()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new NumberConverters.FloatConverter();

            converter.Write(12.34f, messageWriter);

            Assert.Equal(
                new byte[5]
                {
                    0x04, 0xa4, 0x70, 0x45, 0x41
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(float), messageReader);

            Assert.IsType<float>(value);
            Assert.Equal(12.34f, (float)value);
        }

        [Fact]
        public void Double()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new NumberConverters.DoubleConverter();

            converter.Write(1234.5678, messageWriter);

            Assert.Equal(
                new byte[9]
                {
                    0x08, 0xad, 0xfa, 0x5c, 0x6d, 0x45, 0x4a, 0x93, 0x40,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(double), messageReader);

            Assert.IsType<double>(value);
            Assert.Equal(1234.5678, (double)value);
        }

        [Fact]
        public void Decimal()
        {
            MemoryStream output = new();
            MessageWriter messageWriter = new(new BinaryWriter(output));
            Converter converter = new NumberConverters.DecimalConverter();

            converter.Write(-0.123456789m, messageWriter);

            Assert.Equal(
                new byte[17]
                {
                    //    ,------ lower -------,  ,------ middle ------,  ,------ higher ------,        point --,     ,-- sign
                    0x10, 0x15, 0xcd, 0x5b, 0x07, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x09, 0x80,
                },
                output.ToArray()
            );

            output.Position = 0;
            MessageReader messageReader = new(new BinaryReader(output));

            var value = converter.Read(typeof(decimal), messageReader);

            Assert.IsType<decimal>(value);
            Assert.Equal(-0.123456789m, (decimal)value);
        }
    }
}
