using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    internal static class NumberConverters
    {
        public static void AddAll(ConverterFactory factory)
        {
            factory.AddConverter(new FloatConverter());
            factory.AddConverter(new DoubleConverter());
            factory.AddConverter(new DecimalConverter());
#if NET5_0_OR_GREATER
            factory.AddConverter(new HalfConverter());
#endif
        }

        public class FloatConverter : Converter<float>
        {
            protected override float ReadValue(IMessageReader input)
            {
                byte[] bytes = input
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToSingle(bytes);
            }

            protected override void WriteValue(float value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write32BitsField(bytes);
            }
        }

        public class DoubleConverter : Converter<double>
        {
            protected override double ReadValue(IMessageReader input)
            {
                throw new NotImplementedException();
            }

            protected override void WriteValue(double value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write64BitsField(bytes);
            }
        }

        public class DecimalConverter : Converter<decimal>
        {
            protected override decimal ReadValue(IMessageReader input)
            {
                throw new NotImplementedException();
            }

            protected override void WriteValue(decimal value, IMessageWriter output)
            {
                int[] parts = decimal.GetBits(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(parts);
                byte[] bytes = new byte[16];
                for (int i = 0; i < 4; i++)
                {
                    BitConverter.GetBytes(parts[i]).CopyTo(bytes, 4*i);
                }
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write128BitsField(bytes);
            }
        }

#if NET5_0_OR_GREATER
        public class HalfConverter : Converter<Half>
        {
            protected override Half ReadValue(IMessageReader input)
            {
                throw new NotImplementedException();
            }

            protected override void WriteValue(Half value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write16BitsField(bytes);
            }
        } 
#endif
    }
}
