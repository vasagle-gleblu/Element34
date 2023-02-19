using System;
using System.Data;

namespace Element34.Utilities.DataManager
{
    internal class DisposableDataRow : IDisposable
    {
        private DataRow _dr;
        protected bool disposed;

        public object this[string column]
        {
            get
            {
                return _dr[column];
            }

            set
            {
                _dr[column] = value;
            }
        }

        public object this[int column]
        {
            get
            {
                return _dr[column];
            }

            set
            {
                _dr[column] = value;
            }
        }

        public DisposableDataRow(DataRow dr)
        {
            _dr = dr;
            disposed = false;
        }

        public DisposableDataRow(DisposableDataRow ddr)
        {
            _dr = ddr._dr;
            disposed = ddr.disposed;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            if (disposing)
            {
                if (_dr != null)
                    _dr = null;
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
