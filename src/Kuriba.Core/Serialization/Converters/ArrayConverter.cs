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
            bool result;
            
            if (type.IsConstructedGenericType
                && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                containedType = type.GenericTypeArguments[0];
                result = true;
            }
            else
            {
                containedType = null;
                result = false;
            }

            return result;
        }

        public static bool IsArrayType(Type type, [NotNullWhen(true)] out Type? containedType)
        {
            bool result;
            
            if (type.IsArray)
            {
                containedType = type.GetElementType()!;
                result = true;
            }
            else
            {
                containedType = null;
                result = false;
            }

            return result;
        }

        public override bool CanConvert(Type type, ConverterFactory factory)
        {
            bool result;
            
            if (IsArrayType(type, out var containedType)
                || IsConstructedListType(type, out containedType))
            {
                result = factory.HasConverterFor(containedType);
            }
            else
            {
                result = false;
            }

            return result;
        }

        public override Converter SpecializeFor(Type type, ConverterFactory factory)
        {
            return new SpecializedArrayConverter(type, factory);
        }
    }

    internal class SpecializedArrayConverter : Converter
    {
        private readonly Converter containedTypeConverter;
        private readonly Type specializedArrayType;
        private readonly IArrayAdapterPrototype arrayAdapterPrototype;
        private readonly Type containedType;

        public SpecializedArrayConverter(Type type, ConverterFactory factory)
        {
            // create the appropriate prototype
            if (ArrayConverter.IsArrayType(type, out var containedTypeFound))
            {
                this.arrayAdapterPrototype = arrayAdapterPrototype = new ArrayAdapterPrototype();
            }
            else if (ArrayConverter.IsConstructedListType(type, out containedTypeFound))
            {
                this.arrayAdapterPrototype = (IArrayAdapterPrototype)typeof(ListAdapterPrototype<>)
                                           .MakeGenericType(containedTypeFound)
                                           .GetConstructor(Array.Empty<Type>())!.Invoke(null);
            }
            else
            {
                throw new ArgumentException($"A specialized array converter cannot handle {type.FullName}.");
            }

            this.specializedArrayType = type;
            this.containedType = containedTypeFound;
            this.containedTypeConverter = factory.GetConverterFor(this.containedType);
        }

        public override bool CanConvert(Type type, ConverterFactory factory) => type == this.specializedArrayType;

        public override object? Read(Type typeToRead, IMessageReader input)
        {
            IArrayBuilder arrayBuilder = this.arrayAdapterPrototype.New(typeToRead);
            this.ReadRecursive(arrayBuilder, input);

            return arrayBuilder.Finish();
        }
        
        private void ReadRecursive(IArrayBuilder builder, IMessageReader input)
        {
            // read header
            ushort size = input.ReadArrayHeader();
            int dimensions = builder.Push(size);

            // read values
            for (int i = 0; i < size; i++)
            {
                if (dimensions == 1)
                {
                    // down to a 1-dimensional array
                    builder.AddValue(this.containedTypeConverter.Read(this.containedType, input));
                }
                else
                {
                    // still dealing with a multi-dimensional array
                    this.ReadRecursive(builder, input);
                }
            }

            builder.Pop();
        }

        public override void Write(object? value, IMessageWriter output)
        {
            if (value == null)
            {
                throw new NullReferenceException("The array to serialize was null.");
            }

            IArray array = this.arrayAdapterPrototype.Wrap(value);
            this.WriteRecursive(array.GetEnumerator(), output);
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
                    this.containedTypeConverter.Write(enumerator.Current, output);
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