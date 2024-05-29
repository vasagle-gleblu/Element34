using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Element34.Utilities
{
    /// <summary>
    /// Represents a read-only collection of elements that can be safely disposed.
    /// </summary>
    /// <typeparam name="T">The type of elements contained in the collection.</typeparam>
    public class DisposableReadOnlyCollection<T> : IDisposable, IEnumerable<T>
    {
        private readonly ReadOnlyCollection<T> readOnlyCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableReadOnlyCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The enumerable collection whose elements are copied to the new read-only collection.</param>
        public DisposableReadOnlyCollection(IEnumerable<T> collection)
        {
            readOnlyCollection = new ReadOnlyCollection<T>(new List<T>(collection));
        }

        /// <summary>
        /// Gets the underlying <see cref="ReadOnlyCollection{T}"/>.
        /// </summary>
        public ReadOnlyCollection<T> ReadOnlyCollection => readOnlyCollection;

        /// <summary>
        /// Gets the number of elements contained in the collection.
        /// </summary>
        public int Count => readOnlyCollection.Count;

        /// <summary>
        /// Gets the element at the specified index in the read-only collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>The element at the specified index in the collection.</returns>
        /// <exception cref="IndexOutOfRangeException">Thrown when the index is outside the bounds of the collection.</exception>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= readOnlyCollection.Count)
                    throw new IndexOutOfRangeException("Index is out of range.");

                return readOnlyCollection[index];
            }
        }

        /// <summary>
        /// Returns the zero-based index of the first occurrence of a specific item within the collection.
        /// </summary>
        /// <param name="item">The item to locate in the collection.</param>
        /// <returns>The index of the item if found in the collection; otherwise, -1.</returns>
        public int IndexOf(T item)
        {
            return readOnlyCollection.IndexOf(item);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator for the collection of type <typeparamref name="T"/>.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return readOnlyCollection.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Releases all resources used by the <see cref="DisposableReadOnlyCollection{T}"/>.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    // Handle managed resource disposal here if necessary
                }
                disposed = true;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool disposed = false;
    }

}
