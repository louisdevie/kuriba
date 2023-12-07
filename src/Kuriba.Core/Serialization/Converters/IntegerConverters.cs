using System;
using System.Numerics;

namespace Kuriba.Core.Serialization.Converters
{
    internal static class IntegerConverters
    {
        public static void AddAll(ConverterFactory factory)
        {
            factory.AddConverter(new SByteConverter());
            factory.AddConverter(new ByteConverter());
            factory.AddConverter(new Int16Converter());
            factory.AddConverter(new UInt16Converter());
            factory.AddConverter(new Int32Converter());
            factory.AddConverter(new UInt32Converter());
            factory.AddConverter(new Int64Converter());
            factory.AddConverter(new UInt64Converter());
#if NET7_0_OR_GREATER
            factory.AddConverter(new Int128Converter());
            factory.AddConverter(new UInt128Converter());
#endif
        }

        public class SByteConverter : Converter<sbyte>
        {
            protected override sbyte ReadValue(IMessageReader input)
                => unchecked((sbyte)input.Read8BitsField());

            protected override void WriteValue(sbyte value, IMessageWriter output)
                => output.Write8BitsField(unchecked((byte)value));
        }

        public class ByteConverter : Converter<byte>
        {
            protected override byte ReadValue(IMessageReader input)
                => input.Read8BitsField();

            protected override void WriteValue(byte value, IMessageWriter output)
                => output.Write8BitsField(value);
        }

        public class Int16Converter : Converter<short>
        {
            protected override short ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read16BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToInt16(bytes);
            }

            protected override void WriteValue(short value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write16BitsField(bytes);
            }
        }

        public class UInt16Converter : Converter<ushort>
        {
            protected override ushort ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read16BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToUInt16(bytes);
            }

            protected override void WriteValue(ushort value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write16BitsField(bytes);
            }
        }

        public class Int32Converter : Converter<int>
        {
            protected override int ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read32BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToInt32(bytes);
            }

            protected override void WriteValue(int value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write32BitsField(bytes);
            }
        }

        public class UInt32Converter : Converter<uint>
        {
            protected override uint ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read32BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes);
            }

            protected override void WriteValue(uint value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write32BitsField(bytes);
            }
        }

        public class Int64Converter : Converter<long>
        {
            protected override long ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read64BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToInt64(bytes);
            }

            protected override void WriteValue(long value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write64BitsField(bytes);
            }
        }

        public class UInt64Converter : Converter<ulong>
        {
            protected override ulong ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read64BitsField();
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                return BitConverter.ToUInt64(bytes);
            }

            protected override void WriteValue(ulong value, IMessageWriter output)
            {
                byte[] bytes = BitConverter.GetBytes(value);
                if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
                output.Write64BitsField(bytes);
            }
        }

#if NET7_0_OR_GREATER
        public class Int128Converter : Converter<Int128>
        {
            protected override Int128 ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read128BitsField();
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                Int128 result = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    result <<= 8;
                    result += bytes[i];
                }
                return result;
            }

            protected override void WriteValue(Int128 value, IMessageWriter output)
            {
                byte[] bytes = new byte[16];
                (value as IBinaryInteger<Int128>).TryWriteLittleEndian(bytes, out _);
                output.Write128BitsField(bytes);
            }
        }

        public class UInt128Converter : Converter<UInt128>
        {
            protected override UInt128 ReadValue(IMessageReader input)
            {
                byte[] bytes = input.Read128BitsField();
                if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
                UInt128 result = 0;
                for (int i = 0; i < bytes.Length; i++)
                {
                    result <<= 8;
                    result += bytes[i];
                }
                return result;
            }

            protected override void WriteValue(UInt128 value, IMessageWriter output)
            {
                byte[] bytes = new byte[16];
                (value as IBinaryInteger<UInt128>).TryWriteLittleEndian(bytes, out _);
                output.Write128BitsField(bytes);
            }
        }
#endif
    }
}
