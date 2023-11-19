using Kuriba.Core.Serialization.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Kuriba.Core.Serialization.Adapters
{
    internal class ListAdapter<T> : IArray
    {
        public class Enumerator : IArrayEnumerator
        {
            private ListAdapter<T> adapter;
            private IEnumerator<T> underlying;

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

        private List<T> list;

        public ListAdapter(List<T> list)
        {
            this.list = list;
        }

        public IArrayEnumerator GetEnumerator() => new Enumerator(this);
    }

    internal class ListAdapterPrototype<T> : IArrayAdapterPrototype
    {
        public IArray Wrap(object arrayLike) => new ListAdapter<T>((List<T>)arrayLike);
    }
}
