using Kuriba.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Kuriba.Core.Serialization
{
    internal class MessageWriter : IMessageWriter, IReferenceTracker
    {
        private readonly BinaryWriter output;
        private readonly HashSet<object> visitedObjects;

        public MessageWriter(BinaryWriter output)
        {
            this.output = output;
            this.visitedObjects = new HashSet<object>();
        }

        public void TrackObject(object? obj)
        {
            if (obj != null && !visitedObjects.Add(obj))
            {
                throw new CircularReferenceException(obj);
            }
        }

        public void WriteMessage(Type type, object? message)
        {
            if (message == null) return;

            this.TrackObject(message);

            MessageStructure structure = MessageFactory.Default.StructureForType(type);

            this.WriteHeader(structure.MessageId, structure.LastFieldId);
            
            using var iter = new MessageStructureIterator(structure);
            while (iter.NextField())
            {
                if (iter.Padding > 0)
                {
                    this.WritePadding(iter.Padding);
                }

                iter.CurrentField.WriteValueFromMessage(message, this, this);
            }
        }

        private void WriteHeader(byte messageId, byte? lastFieldId)
        {
            this.output.Write(messageId);

            if (lastFieldId == null)
            {
                this.output.Write((byte)0xff);
            }
            else
            {
                this.output.Write(lastFieldId.Value);
            }
        }

        private void WritePadding(byte padding)
        {
            this.output.Write((byte)WireType.Padding);
            this.output.Write(padding);
        }

        public void Write8BitsField(byte payload)
        {
            this.output.Write((byte)WireType.Bit8);
            this.output.Write(payload);
        }

        public void Write16BitsField(byte[] payload)
        {
            if (payload.Length != 2) throw new ArgumentException($"Write16BitsField expected 2 bytes, got {payload.Length}");
            this.output.Write((byte)WireType.Bit16);
            this.output.Write(payload);
        }

        public void Write32BitsField(byte[] payload)
        {
            if (payload.Length != 4) throw new ArgumentException($"Write32BitsField expected 4 bytes, got {payload.Length}");
            this.output.Write((byte)WireType.Bit32);
            this.output.Write(payload);
        }

        public void Write64BitsField(byte[] payload)
        {
            if (payload.Length != 8) throw new ArgumentException($"Write64BitsField expected 8 bytes, got {payload.Length}");
            this.output.Write((byte)WireType.Bit64);
            this.output.Write(payload);
        }

        public void Write128BitsField(byte[] payload)
        {
            if (payload.Length != 16) throw new ArgumentException($"Write128BitsField expected 16 bytes, got {payload.Length}");
            this.output.Write((byte)WireType.Bit128);
            this.output.Write(payload);
        }

        private void WriteVarTag(ushort tag)
        {
            if (tag >= 0x4000)
            {
                this.output.Write((byte)(0x80 | (byte)(0x7f & (tag >> 14))));
            }
            if (tag >= 0x80)
            {
                this.output.Write((byte)(0x80 | (byte)(0x7f & (tag >> 7))));
            }
            this.output.Write((byte)(tag & 0x7f));
        }

        public void WriteVarField(byte[] payload)
        {
            this.output.Write((byte)WireType.BitVar);
            this.WriteVarTag((ushort)payload.Length);
            this.output.Write(payload);
        }

        public void WriteArrayHeader(ushort count)
        {
            this.output.Write((byte)WireType.Array);
            this.WriteVarTag(count);
        }

        public void WriteEmptyField()
        {
            this.output.Write((byte)WireType.Nothing);
        }
    }
}
