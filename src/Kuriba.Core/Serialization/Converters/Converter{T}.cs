﻿using System;

namespace Kuriba.Core.Serialization.Converters
{
    /// <summary>
    /// A <see cref="Converter"/> for the specific type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type handled by the converter.</typeparam>
    public abstract class Converter<T> : Converter
    {
        /// <inheritdoc cref="Converter.CanConvert(Type, ConverterFactory)"/>
        public override bool CanConvert(Type type, ConverterFactory factory) => type == typeof(T);

        /// <inheritdoc cref="Converter.Write(object?, IMessageWriter)"/>
        public override void Write(object? value, IMessageWriter output)
        {
            if (value == null)
            {
                output.WriteVarField(Array.Empty<byte>());
            }
            else
            {
                WriteValue((T)value, output);
            }
        }

        /// <summary>
        /// Writes the value using the given <see cref="IMessageWriter"/>.
        /// </summary>
        /// <param name="value">The value to write.</param>
        /// <param name="output">The writer to use.</param>
        protected abstract void WriteValue(T value, IMessageWriter output);

        /// <inheritdoc cref="Converter.Read(Type, IMessageReader)"/>
        public override object? Read(Type typeToRead, IMessageReader input)
        {
            return this.ReadValue(input);
        }

        /// <summary>
        /// Read a value using the given <see cref="IMessageReader"/>.
        /// </summary>
        /// <param name="input">The reader to use.</param>
        /// <returns>The value read.</returns>
        protected abstract T ReadValue(IMessageReader input);
    }
}
