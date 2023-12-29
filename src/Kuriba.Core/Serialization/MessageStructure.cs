using Kuriba.Core.Exceptions;
using Kuriba.Core.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Kuriba.Core.Serialization
{
    internal partial class MessageStructure
    {
        private readonly Type messageType;
        private readonly byte messageId;
        private readonly SortedList<byte, MessageField> fields;
        private readonly ConstructorInfo constructor;

        public Type MessageType => this.messageType;

        public byte MessageId => this.messageId;

        public IEnumerable<KeyValuePair<byte, MessageField>> Fields => this.fields;

        public byte? LastFieldId
        {
            get
            {
                byte? id = null;

                if (this.fields.Count > 0)
                {
                    id = this.fields.Last().Key;
                }

                return id;
            }
        }

        public MessageStructure(Type messageType)
        {
            this.messageType = messageType;

            MessageAttribute messageMetadata =
                this.messageType.GetCustomAttribute<MessageAttribute>()
                ?? throw new InvalidMessageTypeException(
                    $"Type {this.messageType.FullName} has no Message attribute.");

            this.constructor =
                this.messageType.GetConstructor(Array.Empty<Type>())
                ?? throw new InvalidMessageTypeException(
                    $"Type {this.messageType.FullName} has no default constructor.");

            this.messageId = messageMetadata.Id;
            this.fields = new SortedList<byte, MessageField>();

            foreach (var property in this.messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetCustomAttribute<MessageFieldAttribute>() is MessageFieldAttribute propertyMetadata)
                {
                    this.fields.Add(propertyMetadata.Id, new MessageField(property));
                }
            }
        }

        public MessageField GetField(byte fieldId)
        {
            return this.fields[fieldId];
        }

        public object CreateInstance()
        {
            return this.constructor.Invoke(null);
        }
    }
}