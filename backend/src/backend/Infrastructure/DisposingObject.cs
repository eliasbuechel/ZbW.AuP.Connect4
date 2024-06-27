using System.Diagnostics;

namespace backend.infrastructure
{
    internal abstract class DisposingObject : IDisposable
    {
        ~DisposingObject()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            if (disposing)
                OnDispose();

            _disposed = true;
        }

        protected abstract void OnDispose();

        private bool _disposed;
    }
}