using System;
using System.Data;

namespace Element34.Utilities
{
    /// <summary>
    /// Represents a wrapper for <see cref="DataRow"/> that provides disposal capabilities.
    /// </summary>
    public class DisposableDataRow : IDisposable
    {
        private DataRow _dataRow;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableDataRow"/> class.
        /// </summary>
        /// <param name="dataRow">The <see cref="DataRow"/> to wrap.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dataRow"/> is null.</exception>
        public DisposableDataRow(DataRow dataRow)
        {
            _dataRow = dataRow ?? throw new ArgumentNullException(nameof(dataRow));
        }

        /// <summary>
        /// Gets or sets the value of the specified column in the <see cref="DataRow"/>.
        /// </summary>
        /// <param name="column">The name of the column.</param>
        /// <returns>The value of the specified column.</returns>
        public object this[string column]
        {
            get => _dataRow[column];
            set => _dataRow[column] = value;
        }

        /// <summary>
        /// Gets or sets the value at the specified index in the <see cref="DataRow"/>.
        /// </summary>
        /// <param name="column">The zero-based index of the column.</param>
        /// <returns>The value at the specified index.</returns>
        public object this[int column]
        {
            get => _dataRow[column];
            set => _dataRow[column] = value;
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="DisposableDataRow"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if(_dataRow != null)
                    _dataRow = null;
            }

            _disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DisposableDataRow"/> class.
        /// </summary>
        ~DisposableDataRow()
        {
            Dispose(false);
        }
    }

}
