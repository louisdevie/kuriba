using System;
using System.Buffers;
using System.Text;
using Kuriba.Core.Exceptions;

namespace Kuriba.Core.Serialization.Converters
{
    internal static class TextConverters
    {
        public static void AddAll(ConverterFactory factory)
        {
            factory.AddConverter(new CharConverter());
            factory.AddConverter(new StringConverter());
#if NET5_0_OR_GREATER
            factory.AddConverter(new RuneConverter());
#endif
        }
        public class CharConverter : Converter<char>
        {
            protected override char ReadValue(IMessageReader input)
            {
                byte[] encoded = input.ReadVarField();
                Decoder textDecoder = Encoding.UTF8.GetDecoder();
                char[] charAsArray = new char[textDecoder.GetCharCount(encoded, false)];
                textDecoder.GetChars(encoded, charAsArray, false);
                return charAsArray.Length == 1 ? charAsArray[0] : throw new UnreadableException($"Expected to read one and only one char, got {charAsArray.Length}");
            }

            protected override void WriteValue(char value, IMessageWriter output)
            {
                char[] charAsArray = new char[1] { value };
                Encoder textEncoder = Encoding.UTF8.GetEncoder();
                byte[] encoded = new byte[textEncoder.GetByteCount(charAsArray, false)];
                textEncoder.GetBytes(charAsArray, encoded, false);
                output.WriteVarField(encoded);
            }
        }

#if NET5_0_OR_GREATER
        public class RuneConverter : Converter<Rune>
        {
            protected override Rune ReadValue(IMessageReader input)
            {
                byte[] encoded = input.ReadVarField();
                var status = Rune.DecodeFromUtf8(encoded, out Rune result, out _);
                if (status != OperationStatus.Done) throw new UnreadableException("Cannot read full Rune.");
                return result;
            }

            protected override void WriteValue(Rune value, IMessageWriter output)
            {
                byte[] encoded = new byte[value.Utf8SequenceLength];
                value.EncodeToUtf8(encoded);
                output.WriteVarField(encoded);
            }
        }
#endif

        public class StringConverter : Converter<string>
        {
            protected override string ReadValue(IMessageReader input)
            {
                byte[] encoded = input.ReadVarField();
                return Encoding.UTF8.GetString(encoded);
            }

            protected override void WriteValue(string value, IMessageWriter output)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(value);
                output.WriteVarField(encoded);
            }
        }
    }
}
