namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a circular reference is found in an object that is being serialized.
    /// </summary>
    public class CircularReferenceException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="CircularReferenceException"/> with a default message.
        /// </summary>
        /// <param name="encounteredTwice">The object that was encountered multiple times while traversing the message object.</param>
        public CircularReferenceException(object encounteredTwice)
        : base($"Circular reference detected : the object {encounteredTwice} was encountered twice during serialization.") { }
    }
}
