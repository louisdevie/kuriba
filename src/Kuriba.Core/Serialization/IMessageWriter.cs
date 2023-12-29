namespace Kuriba.Core.Serialization
{
    /// <summary>
    /// A class that writes to a message buffer.
    /// </summary>
    public interface IMessageWriter
    {
        /// <summary>
        /// Writes a field with a fixed size of 8 bits.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void Write8BitsField(byte payload);

        /// <summary>
        /// Writes a field with a fixed size of 16 bits.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void Write16BitsField(byte[] payload);

        /// <summary>
        /// Writes a field with a fixed size of 32 bits.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void Write32BitsField(byte[] payload);

        /// <summary>
        /// Writes a field with a fixed size of 64 bits.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void Write64BitsField(byte[] payload);

        /// <summary>
        /// Writes a field with a fixed size of 128 bits.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void Write128BitsField(byte[] payload);

        /// <summary>
        /// Writes a field with a variable size.
        /// </summary>
        /// <param name="payload">The data to write.</param>
        void WriteVarField(byte[] payload);

        /// <summary>
        /// Writes a header indicating that the field contains multiple values of variable size.
        /// </summary>
        /// <param name="count">The number of values the field will have.</param>
        void WriteArrayHeader(ushort count);

        /// <summary>
        /// Writes a field with no value. This is different from a variable/array field of length 0.
        /// </summary>
        void WriteEmptyField();
    }
}
