using System;

namespace Services.Common.ObjUtils
{
    public abstract class DisposableModel : IDisposable
    {
        private bool IsDisposed { get; set; }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                DisposeUnmanagedResources();
            }

            IsDisposed = true;
        }

        protected virtual void DisposeUnmanagedResources()
        {
        }

        ~DisposableModel()
        {
            Dispose(false);
        }
    }
}