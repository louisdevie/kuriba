namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when a field exceed the limitations of the kuriba binary format.
    /// </summary>
    public class OversizedException : SerializationException
    {
        /// <summary>
        /// Creates a new <see cref="OversizedException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public OversizedException(string message) : base(message) { }
    }
}
