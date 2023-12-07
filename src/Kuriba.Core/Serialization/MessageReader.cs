using Kuriba.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization
{
    internal class MessageReader : IMessageReader
    {
        private BinaryReader input;

        public MessageReader(BinaryReader input)
        {
            this.input = input;
        }

        public bool TryReadNextField(out WireType wireType, out byte[] bytes)
        {
            bool ok = true;
            try
            {
                wireType = (WireType)input.ReadByte();

                switch (wireType)
                {
                    case WireType.Bit8:
                        bytes = input.ReadBytes(1);
                        break;

                    case WireType.Bit16:
                        bytes = input.ReadBytes(2);
                        break;

                    case WireType.Bit32:
                        bytes = input.ReadBytes(4);
                        break;

                    case WireType.Bit64:
                        bytes = input.ReadBytes(8);
                        break;

                    case WireType.Bit128:
                        bytes = input.ReadBytes(16);
                        break;

                    case WireType.BitVar:
                        {
                            ushort size = this.ReadVarTag();
                            bytes = input.ReadBytes(size);
                            break;
                        }

                    case WireType.Nothing:
                        bytes = Array.Empty<byte>();
                        break;

                    default:
                        bytes = Array.Empty<byte>();
                        ok = false;
                        break;
                }
            }
            catch (EndOfStreamException)
            {
                throw new UnreadableException("The stream ended while reading a field.");
            }
            return ok;
        }

        private ushort ReadVarTag()
        {
            ushort tag = 0;
            byte current;
            do
            {
                current = this.input.ReadByte();
                tag = unchecked((ushort)((tag << 7) | (current & 0x7f)));
            }
            while ((current & 0x80) != 0 && tag < 0x4000);
            return tag;
        }

        private byte[] ReadFixedSizeField(WireType expected)
        {
            if (this.TryReadNextField(out WireType wireType, out byte[] bytes))
            {
                if (wireType == expected)
                {
                    return bytes;
                }
            }
            throw new UnreadableException("The type of field does not match.");
        }

        private bool TryReadOptionalFixedSizeField(WireType expected, out byte[] bytes)
        {
            if (this.TryReadNextField(out WireType wireType, out bytes))
            {
                if (wireType == expected)
                {
                    return true;
                }
                else if (wireType == WireType.Nothing)
                {
                    return false;
                }
            }
            throw new UnreadableException("The type of field does not match.");
        }

        public byte[] Read128BitsField() => this.ReadFixedSizeField(WireType.Bit128);

        public bool TryReadOptional128BitsField(out byte[] bytes) => this.TryReadOptionalFixedSizeField(WireType.Bit128, out bytes);

        public byte[] Read16BitsField() => this.ReadFixedSizeField(WireType.Bit16);

        public bool TryReadOptional16BitsField(out byte[] bytes) => this.TryReadOptionalFixedSizeField(WireType.Bit16, out bytes);

        public byte[] Read32BitsField() => this.ReadFixedSizeField(WireType.Bit32);

        public bool TryReadOptional32BitsField(out byte[] bytes) => this.TryReadOptionalFixedSizeField(WireType.Bit32, out bytes);

        public byte[] Read64BitsField() => this.ReadFixedSizeField(WireType.Bit64);

        public bool TryReadOptional64BitsField(out byte[] bytes) => this.TryReadOptionalFixedSizeField(WireType.Bit64, out bytes);

        public byte Read8BitsField() => this.ReadFixedSizeField(WireType.Bit8)[0];

        public bool TryReadOptional8BitsField(out byte value)
        {
            bool result = this.TryReadOptionalFixedSizeField(WireType.Bit8, out byte[] bytes);
            value = bytes[0];
            return result;
        }
    }
}
