namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when the serializer is asked to serialize a type that is not a proper message.
    /// </summary>
    public class InvalidMessageTypeException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="InvalidMessageTypeException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public InvalidMessageTypeException(string message) : base(message) { }
    }
}
