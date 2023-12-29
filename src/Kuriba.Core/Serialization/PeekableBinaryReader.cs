using System;
using System.Collections.Generic;
using System.IO;

namespace Kuriba.Core.Serialization
{
    internal class PeekableBinaryReader
    {
        private BinaryReader underlying;
        private Queue<byte> buffer;

        public PeekableBinaryReader(BinaryReader underlyingReader)
        {
            this.underlying = underlyingReader;
            this.buffer = new Queue<byte>();
        }

        public byte ReadByte()
        {
            return buffer.Count > 0 ? buffer.Dequeue() : underlying.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            byte[] result = new byte[count];

            int i;
            for (i = 0; i < count && this.buffer.Count > 0; i++)
            {
                result[i] = this.buffer.Dequeue();
            }

            int remainingCount = count - i;
            byte[] remaining = this.underlying.ReadBytes(remainingCount);

            if (remaining.Length < remainingCount)
            {
                // put back what we managed to read in the buffer
                for (int j = 0; j < i; j++) this.buffer.Enqueue(result[j]);
                foreach (byte t in remaining) this.buffer.Enqueue(t);

                throw new EndOfStreamException("Could not read enough bytes.");
            }

            Array.Copy(remaining, 0, result, i, remainingCount);

            return result;
        }

        public byte PeekByte()
        {
            if (this.buffer.Count == 0)
            {
                this.buffer.Enqueue(this.underlying.ReadByte());
            }

            return this.buffer.Peek();
        }

        public void SkipByte()
        {
            // read and throw the value away
            this.ReadByte();
        }
    }
}