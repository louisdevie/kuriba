namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// The base exception for all serialization-related errors.
    /// </summary>
    public class SerializationException : KuribaException
    {
        /// <summary>
        /// Creates a new <see cref="SerializationException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public SerializationException(string message) : base(message) { }
    }
}
