using Kuriba.Core.Exceptions;
using Kuriba.Core.Serialization.Adapters;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Kuriba.Core.Serialization.Converters
{
    internal class ArrayConverter : GenericConverter
    {
        public static bool IsConstructedListType(Type type, [NotNullWhen(true)] out Type? containedType)
        {
            if (type.IsConstructedGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                containedType = type.GenericTypeArguments[0];
                return true;
            }
            else
            {
                containedType = null;
                return false;
            }
        }

        public static bool IsArrayType(Type type, [NotNullWhen(true)] out Type? containedType)
        {
            if (type.IsArray)
            {
                containedType = type.GetElementType()!;
                return true;
            }
            else
            {
                containedType = null;
                return false;
            }
        }

        public override bool CanConvert(Type type, ConverterFactory factory)
        {
            Type? containedType;
            if (IsArrayType(type, out containedType)
                || IsConstructedListType(type, out containedType))
            {
                return factory.HasConverterFor(containedType);
            }
            else
            {
                return false;
            }
        }

        public override Converter SpecializeFor(Type type, ConverterFactory factory)
        {
            return new SpecializedArrayConverter(type, factory);
        }
    }

    internal class SpecializedArrayConverter : Converter
    {
        private Converter containedTypeConverter;
        private Type specializedArrayType;
        private IArrayAdapterPrototype arrayAdapterPrototype;

        public SpecializedArrayConverter(Type type, ConverterFactory factory)
        {
            Type? containedType;

            // create the appropriate prototype
            if (ArrayConverter.IsArrayType(type, out containedType))
            {
                this.arrayAdapterPrototype = arrayAdapterPrototype = new ArrayAdapterPrototype();
            }
            else if (ArrayConverter.IsConstructedListType(type, out containedType))
            {
                this.arrayAdapterPrototype = (IArrayAdapterPrototype)typeof(ListAdapterPrototype<>)
                                           .MakeGenericType(containedType)
                                           .GetConstructor(Array.Empty<Type>())!.Invoke(null);
            }
            else
            {
                throw new ArgumentException($"A specialized array converter cannot handle {type.FullName}.");
            }

            this.specializedArrayType = type;
            this.containedTypeConverter = factory.GetConverterFor(containedType);
        }

        public override bool CanConvert(Type type, ConverterFactory factory) => type == this.specializedArrayType;

        public override void Write(object? value, IMessageWriter output)
        {
            if (value == null)
            {
                output.WriteEmptyField();
            }
            else
            {
                IArray array = this.arrayAdapterPrototype.Wrap(value);
                this.WriteRecursive(array.GetEnumerator(), output);
            }
        }

        private void WriteRecursive(IArrayEnumerator enumerator, IMessageWriter output)
        {
            // write header
            int size = enumerator.Begin();
            if (size > ushort.MaxValue) throw new OversizedException($"Arrays cannot contain more than {ushort.MaxValue} elements.");
            output.WriteArrayHeader((ushort)size);

            // write values
            while (enumerator.Next())
            {
                if (enumerator.Dimensions == 1)
                {
                    // down to a 1-dimensional array
                    if (enumerator.Current == null)
                    {
                        output.WriteEmptyField();
                    }
                    else
                    {
                        this.containedTypeConverter.Write(enumerator.Current, output);
                    }
                }
                else
                {
                    // still dealing with a multi-dimensional array
                    this.WriteRecursive((IArrayEnumerator)enumerator.Current!, output);
                }
            }
        }
    }
}