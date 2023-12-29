using Kuriba.Core.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Kuriba.Core.Serialization
{
    internal class MessageReader : IMessageReader
    {
        private readonly PeekableBinaryReader input;

        public MessageReader(BinaryReader input)
        {
            this.input = new PeekableBinaryReader(input);
        }

        public object? ReadMessage(Type? expectedMessageType = null)
        {
            this.ReadHeader(out byte messageId, out byte? lastFieldId);

            MessageStructure? structure = MessageFactory.Default.StructureForId(messageId);
            if (structure is null)
            {
                throw new UnreadableException($"Unknown message with ID {messageId}.");
            }

            if (expectedMessageType != null && !expectedMessageType.IsAssignableFrom(structure.MessageType))
            {
                throw new UnreadableException(
                    $"Message was expected to be of type {expectedMessageType.FullName}, but was actually {structure.MessageType}.");
            }

            var builder = new MessageBuilder(structure, lastFieldId);
            while (builder.ReadNewField())
            {
                if (this.TryReadPadding(out byte? padding))
                {
                    builder.SkipFields(padding.Value);
                }

                builder.ReadFieldFromRawData(this);
            }

            return builder.Finish();
        }

        private void ReadHeader(out byte messageId, out byte? lastFieldId)
        {
            try
            {
                messageId = this.input.ReadByte();
                lastFieldId = this.input.ReadByte();

                if (lastFieldId == 0xff)
                {
                    lastFieldId = null;
                }
            }
            catch (EndOfStreamException)
            {
                throw new UnreadableException("The stream ended while reading a message header.");
            }
        }

        private bool TryReadPadding([NotNullWhen(true)] out byte? padding)
        {
            bool ok;
            try
            {
                var wireType = (WireType)input.PeekByte();

                if (wireType == WireType.Padding)
                {
                    input.SkipByte(); // consume the wire type byte
                    padding = input.ReadByte();
                    ok = true;
                }
                else
                {
                    ok = false;
                    padding = null;
                }
            }
            catch (EndOfStreamException)
            {
                throw new UnreadableException("The stream ended while reading a field.");
            }

            return ok;
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
            } while ((current & 0x80) != 0 && tag < 0x4000);

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

        public byte[] Read128BitsField() => this.ReadFixedSizeField(WireType.Bit128);

        public byte[] Read16BitsField() => this.ReadFixedSizeField(WireType.Bit16);

        public byte[] Read32BitsField() => this.ReadFixedSizeField(WireType.Bit32);

        public byte[] Read64BitsField() => this.ReadFixedSizeField(WireType.Bit64);

        public byte Read8BitsField() => this.ReadFixedSizeField(WireType.Bit8)[0];

        public byte[] ReadVarField()
        {
            if (this.TryReadNextField(out WireType wireType, out byte[] bytes))
            {
                if (wireType == WireType.BitVar)
                {
                    return bytes;
                }
            }

            throw new UnreadableException("The type of field does not match.");
        }

        public ushort ReadArrayHeader()
        {
            var wireType = (WireType)input.ReadByte();
            if (wireType != WireType.Array) throw new UnreadableException("The type of field does not match.");
            return this.ReadVarTag();
        }
    }
}