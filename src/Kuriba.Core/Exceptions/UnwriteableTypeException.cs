using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the serializer encounters a type that cannot be written in a message.
    /// </summary>
    public class UnwriteableTypeException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="UnwriteableTypeException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public UnwriteableTypeException(string message) : base(message) { }
    }
}
