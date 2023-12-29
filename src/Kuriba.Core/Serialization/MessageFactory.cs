using Kuriba.Core.Exceptions;
using Kuriba.Core.Messages;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Kuriba.Core.Serialization
{
    /// <summary>
    /// This class assemble incoming messages and remembers how to destructure
    /// outgoing messages.
    /// </summary>
    public class MessageFactory
    {
        private static MessageFactory? defaultInstance;

        /// <summary>
        /// The default factory used for (de)serialization.
        /// </summary>
        public static MessageFactory Default
        {
            get
            {
                defaultInstance ??= new MessageFactory();
                return defaultInstance;
            }

            set => defaultInstance = value;
        }

        private readonly ConcurrentDictionary<Type, MessageStructure> structuresByType;
        private readonly ConcurrentDictionary<byte, MessageStructure> structuresById;

        /// <summary>
        /// Creates a new factory without any message types registered.
        /// </summary>
        public MessageFactory()
        {
            this.structuresByType = new ConcurrentDictionary<Type, MessageStructure>();
            this.structuresById = new ConcurrentDictionary<byte, MessageStructure>();
        }

        internal MessageStructure StructureForType(Type messageType)
        {
            this.RegisterMessageType(messageType);
            return this.structuresByType[messageType];
        }

        internal MessageStructure? StructureForId(byte messageId)
        {
            structuresById.TryGetValue(messageId, out MessageStructure? structure);
            return structure;
        }

        /// <summary>
        /// Adds a message type to the factory.
        /// </summary>
        /// <param name="messageType">The type of the message to register.</param>
        /// <exception cref="DuplicateIdentifierException"></exception>
        public void RegisterMessageType(Type messageType)
        {
            if (!this.structuresByType.ContainsKey(messageType))
            {
                MessageStructure structure = new MessageStructure(messageType);

                if (this.structuresById.TryGetValue(structure.MessageId, out MessageStructure? existing))
                {
                    if (existing.MessageType != messageType)
                    {
                        throw new DuplicateIdentifierException($"Message types {existing.MessageType.FullName} and {messageType.FullName} share the same message id.");
                    }
                }

                this.structuresByType[messageType] = structure;
                this.structuresById[structure.MessageId] = structure;
            }
        }

        /// <summary>
        /// Adds a message type to the factory.
        /// </summary>
        /// <typeparam name="T">The type of the message to register.</typeparam>
        /// <exception cref="DuplicateIdentifierException"></exception>
        public void RegisterMessageType<T>()
        {
            this.RegisterMessageType(typeof(T));
        }

        /// <summary>
        /// Search for all the types of messages in a given assembly and adds them.
        /// </summary>
        /// <param name="assembly">The assembly to search.</param>
        /// <exception cref="DuplicateIdentifierException"></exception>
        public void RegisterMessageTypesFromAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.GetTypes())
            {
                if (type.GetCustomAttribute<MessageAttribute>() != null)
                {
                    this.RegisterMessageType(type);
                }
            }
        }
    }
}
