using System;

namespace Kuriba.Core.Serialization.Adapters
{
    internal class ArrayAdapter : IArray
    {
        private class Enumerator : IArrayEnumerator
        {
            private readonly ArrayAdapter arrayAdapter;
            private readonly int[] indices;
            private readonly int depth;
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
                if (++this.indices[this.depth] > this.upperBound) return false;
                
                this.current = this.Dimensions == 1 ? this.Array.GetValue(this.indices) : new Enumerator(this);
                return true;
            }
        }

        readonly Array array;

        public ArrayAdapter(Array array)
        {
            this.array = array;
        }

        public IArrayEnumerator GetEnumerator() => new Enumerator(this);
    }

    internal class ArrayAdapterPrototype : IArrayAdapterPrototype
    {
        public IArray Wrap(object arrayLike) => new ArrayAdapter((Array)arrayLike);
        public IArrayBuilder New(Type arrayType) => new ArrayBuilder(arrayType);
    }

    internal class ArrayBuilder : IArrayBuilder
    {
        private readonly Type arrayType;
        private Array? array;
        private readonly int[] dimensions;
        private readonly int[] indices;
        private int depth;

        public ArrayBuilder(Type arrayType)
        {
            this.arrayType = arrayType;
            this.array = null;
            this.depth = -1;

            int rank = this.arrayType.GetArrayRank();
            this.dimensions = new int[rank];
            this.indices = new int[rank];
        }

        public object Finish()
        {
            return this.array ?? throw new InvalidOperationException("The array hasn't been built yet.");
        }
        
        public int Push(int size)
        {
            this.depth++;

            if (this.array == null)
            {
                this.dimensions[this.depth] = size;
                if (this.depth == this.dimensions.Length - 1)
                {
                    this.array = MakeArray();
                }
            }

            this.indices[this.depth] = 0;

            return this.dimensions.Length - this.depth;
        }

        private Array MakeArray()
        {
            var constructorSignature = new Type[this.dimensions.Length];
            Array.Fill(constructorSignature, typeof(int));

            return (Array)this.arrayType
                .GetConstructor(constructorSignature)!
                .Invoke(Array.ConvertAll(this.dimensions, d => (object)d));
        }

        public void AddValue(object? value)
        {
            this.array!.SetValue(value, this.indices);
            this.indices[this.depth]++;
        }

        public void Pop()
        {
            if (--this.depth >= 0)
            {
                this.indices[this.depth]++;
            }
        }
    }
}