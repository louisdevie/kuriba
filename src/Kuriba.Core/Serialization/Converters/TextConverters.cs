using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
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
                throw new NotImplementedException();
            }

            protected override void WriteValue(string value, IMessageWriter output)
            {
                Encoder textEncoder = Encoding.UTF8.GetEncoder();
                byte[] encoded = new byte[textEncoder.GetByteCount(value, false)];
                textEncoder.GetBytes(value, encoded, false);
                output.WriteVarField(encoded);
            }
        }
    }
}
