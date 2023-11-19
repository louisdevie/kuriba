using System.Collections.Generic;

namespace Kuriba.Core.Serialization.Adapters
{
    internal interface IArray
    {
        /// <summary>
        /// Gets an enumerator to traverse this array.
        /// </summary>
        /// <returns>An <see cref="IArrayEnumerator"/> for this array.</returns>
        IArrayEnumerator GetEnumerator();
    }

    internal interface IArrayEnumerator
    {
        /// <summary>
        /// The dimensions of the slice this enumerator needs to traverse.
        /// </summary>
        int Dimensions { get; }

        /// <summary>
        /// The current lower-rank slice of the array the enumerator is currently over.
        /// </summary>
        object? Current { get; }

        /// <summary>
        /// Starts iterating a slice of the array.
        /// </summary>
        /// <returns>The size of the slice in its first dimesion.</returns>
        int Begin();

        /// <summary>
        /// Try to advance to the next lower-rank slice in this slice.
        /// </summary>
        /// <returns><see langword="true"/> if the enumerator moved, <see langword="false"/> if the end of the slice was reached.</returns>
        bool Next();
    }

    internal interface IArrayAdapterPrototype
    {
        /// <summary>
        /// Creates an <see cref="IArray"/> adapting an array-like structure.
        /// </summary>
        /// <param name="arrayLike">The data structure to wrap in an <see cref="IArray"/>.</param>
        /// <returns>A new instance of an <see cref="IArray"/>.</returns>
        IArray Wrap(object arrayLike);
    }
}
