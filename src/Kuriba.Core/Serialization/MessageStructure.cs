using Kuriba.Core.Exceptions;
using Kuriba.Core.Messages;
using Kuriba.Core.Serialization.Converters;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Kuriba.Core.Serialization
{
    internal partial class MessageStructure
    {
        private readonly Type messageType;
        private readonly byte messageId;
        private readonly SortedList<byte, MessageField> fields;

        public Type MessageType => this.messageType;

        public byte MessageId => this.messageId;

        public IEnumerable<KeyValuePair<byte, MessageField>> Fields => this.fields;

        public byte? LastFieldId
        {
            get
            {
                if (this.fields.Count > 0)
                {
                    return this.fields.Last().Key;
                }
                else
                {
                    return null;
                }
            }
        }

        public MessageStructure(Type messageType)
        {
            this.messageType = messageType;

            MessageAttribute messageMetadata = messageType.GetCustomAttribute<MessageAttribute>()
                ?? throw new InvalidMessageTypeException($"Type {messageType.FullName} has no Message attribute.");

            this.messageId = messageMetadata.Id;
            this.fields = new SortedList<byte, MessageField>();

            foreach (var property in messageType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.GetCustomAttribute<MessageFieldAttribute>() is MessageFieldAttribute propertyMetadata)
                {
                    this.fields.Add(propertyMetadata.Id, new MessageField(property));
                }
            }
        }
    }
}
