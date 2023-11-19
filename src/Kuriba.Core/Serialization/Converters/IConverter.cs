using System;

namespace Kuriba.Core.Serialization.Converters
{
    /// <summary>
    /// An abstraction for all kinds of converters.
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Check if the converter can handle a specific type.
        /// </summary>
        /// <param name="type">The type of values to handle.</param>
        /// <param name="factory">The factory that manages this converter. If others converters are needed, they should come from this factory.</param>
        /// <returns><see langword="true"/> if it can be used to (de)serialize this type.</returns>
        bool CanConvert(Type type, ConverterFactory factory);

        internal Converter SetupConverter(Type type, ConverterFactory factory);
    }
}
