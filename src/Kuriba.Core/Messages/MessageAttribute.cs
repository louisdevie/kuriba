using System;

namespace Kuriba.Core.Messages
{
    /// <summary>
    /// Indicates that this class is a message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MessageAttribute : Attribute
    {
        private readonly byte id;

        /// <summary>
        /// The unique identifier of the message.
        /// </summary>
        public byte Id => id;

        /// <summary>
        /// Indicates that this class is a message.
        /// </summary>
        /// <param name="id">A unique identifier for the message.</param>
        public MessageAttribute(byte id)
        {
            this.id = id;
        }
    }
}
