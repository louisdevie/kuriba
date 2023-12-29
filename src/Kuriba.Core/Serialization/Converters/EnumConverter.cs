using System;

namespace Kuriba.Core.Serialization.Converters
{
    internal class EnumConverter : GenericConverter
    {
        public override bool CanConvert(Type type, ConverterFactory factory)
        {
            return type.IsEnum && factory.HasConverterFor(type.GetEnumUnderlyingType());
        }

        public override Converter SpecializeFor(Type type, ConverterFactory factory)
        {
            Type underlyingType = type.GetEnumUnderlyingType();
            return new SpecializedEnumConverter(factory.GetConverterFor(underlyingType), underlyingType);
        }
    }

    internal class SpecializedEnumConverter : Converter
    {
        private readonly Converter intConverter;
        private readonly Type intType;

        public SpecializedEnumConverter(Converter intConverter, Type intType)
        {
            this.intConverter = intConverter;
            this.intType = intType;
        }

        public override bool CanConvert(Type type, ConverterFactory factory)
        {
            return type.IsEnum && type.GetEnumUnderlyingType() == this.intType;
        }

        public override object? Read(Type typeToRead, IMessageReader input)
        {
            return Enum.ToObject(typeToRead, this.intConverter.Read(this.intType, input)!);
        }

        public override void Write(object? value, IMessageWriter output)
        {
            this.intConverter.Write(value, output);
        }
    }
}
