using System.Threading;

namespace SDRSharp.Radio
{
    public static class DSPThreadPool
    {
        private static SharpThreadPool _threadPool;

        public static void Initialize()
        {
            if (_threadPool == null)
            {
                _threadPool = new SharpThreadPool();
            }
        }

        public static void Initialize(int threadCount)
        {
            if (_threadPool == null)
            {
                _threadPool = new SharpThreadPool(threadCount);
            }
        }

        public static void QueueUserWorkItem(WaitCallback callback)
        {
            if (_threadPool == null)
            {
                _threadPool = new SharpThreadPool();
            }
            _threadPool.QueueUserWorkItem(callback);
        }

        public static void QueueUserWorkItem(WaitCallback callback, object parameter)
        {
            if (_threadPool == null)
            {
                _threadPool = new SharpThreadPool();
            }
            _threadPool.QueueUserWorkItem(callback, parameter);
        }

        public static void Terminate()
        {
            if (_threadPool != null)
            {
                _threadPool.Dispose();
                _threadPool = null;
            }
        }
    }
}
