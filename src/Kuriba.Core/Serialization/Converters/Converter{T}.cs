using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
