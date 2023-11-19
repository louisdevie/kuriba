namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a duplicate identifier is found.
    /// </summary>
    public class DuplicateIdentifierException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="DuplicateIdentifierException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public DuplicateIdentifierException(string message) : base(message) { }
    }
}
