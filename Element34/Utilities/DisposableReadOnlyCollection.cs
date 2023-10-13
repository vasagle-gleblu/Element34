using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Element34.Utilities
{
    public class DisposableReadOnlyCollection<T> : IDisposable, IEnumerable<T>
    {
        private ReadOnlyCollection<T> readOnlyCollection;
        protected bool disposed;

        public DisposableReadOnlyCollection(IEnumerable<T> collection)
        {
            readOnlyCollection = new ReadOnlyCollection<T>(new List<T>(collection));
        }

        public ReadOnlyCollection<T> GetReadOnlyCollection()
        {
            return readOnlyCollection;
        }

        public int Count => readOnlyCollection.Count;

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= GetReadOnlyCollection().Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return GetReadOnlyCollection()[index];
            }
            set
            {
                throw new NotSupportedException("The collection is read-only.");
            }
        }

        public int IndexOf(T item)
        {
            return GetReadOnlyCollection().IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("The collection is read-only.");
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException("The collection is read-only.");
        }

        public IEnumerator<T> GetEnumerator()
        {
            return readOnlyCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (readOnlyCollection != null)
                    readOnlyCollection = null;
            }

            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
