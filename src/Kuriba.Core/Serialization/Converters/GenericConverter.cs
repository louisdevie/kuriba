using System;

namespace Kuriba.Core.Serialization.Converters
{
    /// <summary>
    /// A generic converter implementation which creates different specializations of it self as needed. 
    /// </summary>
    public abstract class GenericConverter : IConverter
    {
        /// <inheritdoc cref="IConverter.CanConvert(Type, ConverterFactory)"/>
        public abstract bool CanConvert(Type type, ConverterFactory factory);

        /// <summary>
        /// Gets the appropriate instance of the converter to handle a specific type.
        /// </summary>
        /// <param name="type">The type of values to handle.</param>
        /// <param name="factory">The factory that manages this converter. Is other converters are needed, they should come from this factory.</param>
        /// <returns>Itself, or another converter instance fit for the given type.</returns>
        public abstract Converter SpecializeFor(Type type, ConverterFactory factory);

        Converter IConverter.SetupConverter(Type type, ConverterFactory factory)
        {
            Converter specialized = SpecializeFor(type, factory);
            factory.AddConverter(specialized); // adds the new converter so that it can be reused later.
            return specialized;
        }
    }
}
