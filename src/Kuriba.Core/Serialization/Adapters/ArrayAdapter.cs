using Kuriba.Core.Serialization.Converters;
using System;
using System.Collections;

namespace Kuriba.Core.Serialization.Adapters
{
    internal class ArrayAdapter : IArray
    {
        public class Enumerator : IArrayEnumerator
        {
            private ArrayAdapter arrayAdapter;
            private int[] indices;
            private int depth;
            private int upperBound;
            private object? current;

            private Array Array => this.arrayAdapter.array;

            public Enumerator(ArrayAdapter arrayAdapter)
            {
                this.arrayAdapter = arrayAdapter;
                this.indices = new int[this.Array.Rank];
                this.depth = 0;
            }

            private Enumerator(Enumerator parent)
            {
                this.arrayAdapter = parent.arrayAdapter;
                this.indices = parent.indices;
                this.depth = parent.depth + 1;
            }

            public object? Current => this.current;

            public int Dimensions => this.indices.Length - this.depth;

            public int Begin()
            {
                this.indices[this.depth] = this.Array.GetLowerBound(this.depth) - 1;
                this.upperBound = this.Array.GetUpperBound(this.depth);
                return this.Array.GetLength(this.depth);
            }

            public bool Next()
            {
                if (++this.indices[this.depth] <= this.upperBound)
                {
                    if (this.Dimensions == 1)
                    {
                        this.current = this.Array.GetValue(this.indices);
                    }
                    else
                    {
                        this.current = new Enumerator(this);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        Array array;

        public ArrayAdapter(Array array)
        {
            this.array = array;
        }

        public IArrayEnumerator GetEnumerator() => new Enumerator(this);
    }

    internal class ArrayAdapterPrototype : IArrayAdapterPrototype
    {
        public IArray Wrap(object arrayLike) => new ArrayAdapter((Array)arrayLike);
    }
}