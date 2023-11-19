using Kuriba.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    public class ConverterFactoryTests
    {
        [Fact]
        public void Default()
        {
            ConverterFactory defaultFactory = ConverterFactory.Default;

            Assert.NotNull(defaultFactory);

            Assert.IsType<IntegerConverters.SByteConverter>(defaultFactory.GetConverterFor(typeof(sbyte)));
            Assert.IsType<IntegerConverters.ByteConverter>(defaultFactory.GetConverterFor(typeof(byte)));

            Assert.IsType<IntegerConverters.Int16Converter>(defaultFactory.GetConverterFor(typeof(short)));
            Assert.IsType<IntegerConverters.UInt16Converter>(defaultFactory.GetConverterFor(typeof(ushort)));

            Assert.IsType<IntegerConverters.Int32Converter>(defaultFactory.GetConverterFor(typeof(int)));
            Assert.IsType<IntegerConverters.UInt32Converter>(defaultFactory.GetConverterFor(typeof(uint)));

            Assert.IsType<IntegerConverters.Int64Converter>(defaultFactory.GetConverterFor(typeof(long)));
            Assert.IsType<IntegerConverters.UInt64Converter>(defaultFactory.GetConverterFor(typeof(ulong)));

            Assert.IsType<IntegerConverters.Int128Converter>(defaultFactory.GetConverterFor(typeof(Int128)));
            Assert.IsType<IntegerConverters.UInt128Converter>(defaultFactory.GetConverterFor(typeof(UInt128)));

            Assert.IsType<NumberConverters.HalfConverter>(defaultFactory.GetConverterFor(typeof(Half)));
            Assert.IsType<NumberConverters.FloatConverter>(defaultFactory.GetConverterFor(typeof(float)));
            Assert.IsType<NumberConverters.DoubleConverter>(defaultFactory.GetConverterFor(typeof(double)));
            Assert.IsType<NumberConverters.DecimalConverter>(defaultFactory.GetConverterFor(typeof(decimal)));

            Assert.IsType<TextConverters.CharConverter>(defaultFactory.GetConverterFor(typeof(char)));
            Assert.IsType<TextConverters.RuneConverter>(defaultFactory.GetConverterFor(typeof(Rune)));
            Assert.IsType<TextConverters.StringConverter>(defaultFactory.GetConverterFor(typeof(string)));
        }

        private class MockIntConverter : Converter
        {
            public override bool CanConvert(Type type, ConverterFactory factory) => type == typeof(int);

            public override void Write(object? value, IMessageWriter output) => throw new NotImplementedException();
        }

        private class MockAnyConverter : Converter
        {
            public override bool CanConvert(Type type, ConverterFactory factory) => true;

            public override void Write(object? value, IMessageWriter output) => throw new NotImplementedException();
        }

        [Fact]
        public void AddAndRemove()
        {
            ConverterFactory converterFactory = new();
            Converter converter = new MockIntConverter();

            Assert.Throws<UnwriteableTypeException>(() => converterFactory.GetConverterFor(typeof(int)));

            converterFactory.AddConverter(converter);

            Assert.IsType<MockIntConverter>(converterFactory.GetConverterFor(typeof(int)));
            Assert.Same(converter, converterFactory.GetConverterFor(typeof(int)));

            converterFactory.RemoveConverter(converter);

            Assert.Throws<UnwriteableTypeException>(() => converterFactory.GetConverterFor(typeof(int)));
        }

        [Fact]
        public void GetConverterFor()
        {
            ConverterFactory converterFactory = new();
            Converter intConverter = new MockIntConverter();
            Converter anyConverter = new MockAnyConverter();

            converterFactory.AddConverter(anyConverter);
            converterFactory.AddConverter(intConverter);

            // the intConverter was added last
            Assert.IsType<MockIntConverter>(converterFactory.GetConverterFor(typeof(int)));
            Assert.Same(intConverter, converterFactory.GetConverterFor(typeof(int)));

            // the other types are catched by the anyConverter
            Assert.IsType<MockAnyConverter>(converterFactory.GetConverterFor(typeof(string)));
            Assert.Same(anyConverter, converterFactory.GetConverterFor(typeof(string)));

            // bring the anyConverter forwards
            converterFactory.AddConverter(anyConverter);

            // now the anyConverter catches all types
            Assert.IsType<MockAnyConverter>(converterFactory.GetConverterFor(typeof(int)));
            Assert.Same(anyConverter, converterFactory.GetConverterFor(typeof(int)));

            Assert.IsType<MockAnyConverter>(converterFactory.GetConverterFor(typeof(string)));
            Assert.Same(anyConverter, converterFactory.GetConverterFor(typeof(string)));
        }
    }
}
