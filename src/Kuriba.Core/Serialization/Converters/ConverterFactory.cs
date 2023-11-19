using Kuriba.Core.Exceptions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Converters
{
    /// <summary>
    /// A class which decides the converter to use for (de)serializing certain types of fields.
    /// </summary>
    public class ConverterFactory
    {
        private static ConverterFactory? defaultInstance;

        /// <summary>
        /// Gets the default factory used by <see cref="MessageSerializer"/>.
        /// </summary>
        public static ConverterFactory Default
        {
            get
            {
                if (defaultInstance == null)
                {
                    defaultInstance = new ConverterFactory();
                    IntegerConverters.AddAll(defaultInstance);
                    NumberConverters.AddAll(defaultInstance);
                    TextConverters.AddAll(defaultInstance);
                }
                return defaultInstance;
            }
        }

        private readonly List<IConverter> converters;

        /// <summary>
        /// Creates a new empty factory.
        /// </summary>
        public ConverterFactory()
        {
            this.converters = new List<IConverter>();
        }

        /// <summary>
        /// Registers a converter into the factory.
        /// </summary>
        /// <param name="converter">The converter to add.</param>
        public void AddConverter(IConverter converter)
        {
            lock (this.converters)
            {
                this.converters.Add(converter);
            }
        }

        /// <summary>
        /// Unregisters a converter from the factory.
        /// </summary>
        /// <param name="converter">The converter instance to remove.</param>
        public void RemoveConverter(IConverter converter)
        {
            lock (this.converters)
            {
                this.converters.Remove(converter);
            }
        }

        /// <summary>
        /// Gets a converter instance that can (de)serialize values of type <paramref name="fieldType"/>. 
        /// </summary>
        /// <param name="fieldType">The type of values to (de)serialize.</param>
        /// <returns>A converter fit for the given type. If multiple converters can convert that type, the one added the latest will be used.</returns>
        /// <exception cref="UnwriteableTypeException"></exception>
        public Converter GetConverterFor(Type fieldType)
        {
            lock (this.converters)
            {
                if (this.converters.FindLast(c => c.CanConvert(fieldType, this)) is IConverter converter)
                {
                    return converter.SetupConverter(fieldType, this);
                }
                else
                {
                    throw new UnwriteableTypeException($"No converter found to serialize {fieldType.FullName}");
                }
            }
        }

        /// <summary>
        /// Check if there is an available converter instance that can (de)serialize values of type <paramref name="fieldType"/>.
        /// </summary>
        /// <param name="fieldType">The type of values to (de)serialize.</param>
        /// <returns><see langword="true"/> if there is a converter fit for the given type.</returns>
        public bool HasConverterFor(Type fieldType)
        {
            lock (this.converters)
            {
                return this.converters.Exists(c => c.CanConvert(fieldType, this));
            }
        }
    }
}
