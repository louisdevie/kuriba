namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a message cannot be read because the data does not match its structure.
    /// </summary>
    public class UnreadableException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="UnreadableException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public UnreadableException(string message) : base(message) { }
    }
}
