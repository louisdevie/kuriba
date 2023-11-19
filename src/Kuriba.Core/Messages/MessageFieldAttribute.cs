using Kuriba.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Messages
{
    /// <summary>
    /// Indicates that this field or property should be included in the transmitted message.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MessageFieldAttribute : Attribute
    {
        private readonly byte id;

        /// <summary>
        /// The unique identifier of the field.
        /// </summary>
        public byte Id => id;

        /// <summary>
        /// Indicates that this field or property should be included in the transmitted message.
        /// </summary>
        /// <param name="id">
        /// An identifier for the field, that must be unique inside the message.<br/>
        /// The identifiers should start at 0 and be consecutive to optimize the size of the transferred messages.
        /// </param>
        public MessageFieldAttribute(byte id)
        {
            if (id == 255) throw new InvalidMessageTypeException("The field identifier 255 is reserved.");
            this.id = id;
        }
    }
}
