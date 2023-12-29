using System;
using System.Collections.Generic;

namespace Kuriba.Core.Serialization
{
    internal class MessageStructureIterator : IDisposable
    {
        private readonly IEnumerator<KeyValuePair<byte, MessageField>> fields;
        private MessageField? currentField;
        private int padding;
        private int lastFieldId;

        public byte Padding => this.padding >= 0 ? (byte)this.padding
            : throw new InvalidOperationException("End of message reached");

        public MessageField CurrentField => this.currentField
            ?? throw new InvalidOperationException("End of message reached");

        public MessageStructureIterator(MessageStructure structure)
        {
            this.fields = structure.Fields.GetEnumerator();
            this.fields.Reset();
            this.currentField = null;
            this.padding = -1;
            this.lastFieldId = -1;
        }

        public bool NextField()
        {
            if (!this.fields.MoveNext())
            {
                this.currentField = null;
                this.padding = -1;
                return false;
            }

            var kvp = this.fields.Current;

            this.padding = kvp.Key - this.lastFieldId - 1;
            this.lastFieldId = kvp.Key;
            this.currentField = kvp.Value;

            return true;
        }

        public void Dispose()
        {
            fields.Dispose();
        }
    }
}
