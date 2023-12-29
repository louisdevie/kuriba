namespace Kuriba.Core.Serialization
{
    internal class MessageBuilder
    {
        private readonly MessageStructure structure;
        private readonly byte? lastField;
        private byte? currentField;
        private object message;

        public MessageBuilder(MessageStructure structure, byte? lastField)
        {
            this.structure = structure;
            this.lastField = lastField;
            this.currentField = null;
            this.message = structure.CreateInstance();
        }

        public bool ReadNewField()
        {
            bool result;

            if (this.lastField == null)
            {
                result = false;
            }
            else if (this.currentField == null)
            {
                this.currentField = 0;
                result = true;
            }
            else if (this.currentField < this.lastField)
            {
                this.currentField++;
                result = true;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public void SkipFields(byte count)
        {
            this.currentField += count;
        }

        public void ReadFieldFromRawData(IMessageReader reader)
        {
            this.structure.GetField(this.currentField!.Value).ReadValueIntoMessage(this.message, reader);
        }

        public object Finish()
        {
            return this.message;
        }
    }
}