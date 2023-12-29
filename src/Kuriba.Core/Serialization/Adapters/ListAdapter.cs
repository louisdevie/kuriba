using System;
using System.Collections.Generic;

namespace Kuriba.Core.Serialization.Adapters
{
    internal class ListAdapter<T> : IArray
    {
        private class Enumerator : IArrayEnumerator
        {
            private readonly ListAdapter<T> adapter;
            private readonly IEnumerator<T> underlying;

            public Enumerator(ListAdapter<T> adapter)
            {
                this.adapter = adapter;
                this.underlying = this.adapter.list.GetEnumerator();
            }

            public object? Current => this.underlying.Current;

            public int Dimensions => 1; // a list always has 1 dimension

            public int Begin()
            {
                this.underlying.Reset();
                return this.adapter.list.Count;
            }

            public bool Next() => this.underlying.MoveNext();
        }

        private readonly List<T> list;

        public ListAdapter(List<T> list)
        {
            this.list = list;
        }

        public IArrayEnumerator GetEnumerator() => new Enumerator(this);
    }

    internal class ListAdapterPrototype<T> : IArrayAdapterPrototype
    {
        public IArray Wrap(object arrayLike) => new ListAdapter<T>((List<T>)arrayLike);

        public IArrayBuilder New(Type arrayType) => new ListBuilder<T>();
    }

    internal class ListBuilder<T> : IArrayBuilder
    {
        private List<T>? list;

        public object Finish() => this.list ?? throw new InvalidOperationException("The list has not been created yet.");

        public int Push(int size)
        {
            if (this.list != null) throw new InvalidOperationException("A list can only have one dimension.");
            this.list = new List<T>(size);
            return 1;
        }

        public void AddValue(object? value)
        {
            if (this.list == null) throw new InvalidOperationException("The list has not been created yet.");
            this.list.Add((T)value!);
        }

        public void Pop() { }
    }
}
