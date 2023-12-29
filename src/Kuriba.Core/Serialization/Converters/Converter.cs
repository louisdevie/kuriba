using System;

namespace Kuriba.Core.Serialization.Converters
{
    /// <summary>
    /// A converter is used to (de)serialize the values of the message fields. 
    /// </summary>
    public abstract class Converter : IConverter
    {
        /// <inheritdoc cref="IConverter.CanConvert(Type, ConverterFactory)"/>
        public abstract bool CanConvert(Type type, ConverterFactory factory);

        /// <summary>
        /// Writes the value using the given <see cref="IMessageWriter"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="output">The writer to use.</param>
        public abstract void Write(object? value, IMessageWriter output);

        /// <summary>
        /// Reads a value using the given <see cref="IMessageReader"/>.
        /// </summary>
        /// <param name="typeToRead">The type of the value to read.s</param>
        /// <param name="input">The reader to use.</param>
        public abstract object? Read(Type typeToRead, IMessageReader input);

        Converter IConverter.SetupConverter(Type type, ConverterFactory factory) => this;
    }
}
