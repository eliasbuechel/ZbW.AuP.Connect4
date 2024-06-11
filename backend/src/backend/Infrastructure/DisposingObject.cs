using System.Diagnostics;

namespace backend.Infrastructure
{
    internal abstract class DisposingObject : IDisposable
    {
        ~DisposingObject()
        {
            if (!_disposed)
                Debug.Assert(false);
        }

        public void Dispose()
        {
            if (_disposed)
            {
                Debug.Assert(false);
                return;
            }

            _disposed = true;

        }

        protected abstract void OnDispose();

        private bool _disposed;
    }
}