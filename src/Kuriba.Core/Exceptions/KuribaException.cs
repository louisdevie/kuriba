using System;

namespace Kuriba.Core.Exceptions
{
    /// <summary>
    /// The base exception for all Kuriba-specific errors.
    /// </summary>
    public class KuribaException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="KuribaException"/> with the specified error message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public KuribaException(string message) : base(message) { }
    }
}
