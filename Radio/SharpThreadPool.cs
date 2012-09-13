using System;
using System.Collections.Generic;
using System.Threading;

namespace SDRSharp.Radio
{
    public class SharpThreadPool
    {
        private struct WorkItem
        {
            private readonly WaitCallback _callback;
            private readonly object _parameter;

            public WorkItem(WaitCallback callback, object parameter)
            {
                _callback = callback;
                _parameter = parameter;
            }

            public void Invoke()
            {
                _callback(_parameter);
            }
        }

        private readonly Queue<WorkItem> _jobQueue = new Queue<WorkItem>();
        private readonly Thread[] _workerThreads;

        private int _threadsWaiting;
        private bool _terminated;

        public SharpThreadPool() :  this(Environment.ProcessorCount)
        {
        }

        public SharpThreadPool(int threadCount)
        {
            _workerThreads = new Thread[threadCount];

            for (var i = 0; i < _workerThreads.Length; i++)
            {
                _workerThreads[i] = new Thread(DispatchLoop);
                _workerThreads[i].Priority = ThreadPriority.Highest;
                _workerThreads[i].Start();
            }
        }

        public void QueueUserWorkItem(WaitCallback callback)
        {
            QueueUserWorkItem(callback, null);
        }

        public void QueueUserWorkItem(WaitCallback callback, object parameter)
        {
            var workItem = new WorkItem(callback, parameter);

            lock (_jobQueue)
            {
                _jobQueue.Enqueue(workItem);
                if (_threadsWaiting > 0)
                {
                    Monitor.Pulse(_jobQueue);
                }
            }
        }

        private void DispatchLoop()
        {
            while (true)
            {

                WorkItem workItem;

                lock (_jobQueue)
                {
                    if (_terminated)
                    {
                        return;
                    }

                    while (_jobQueue.Count == 0)
                    {
                        _threadsWaiting++;

                        try
                        {
                            Monitor.Wait(_jobQueue);
                        }
                        finally
                        {
                            _threadsWaiting--;
                        }

                        if (_terminated)
                        {
                            return;
                        }
                    }

                    workItem = _jobQueue.Dequeue();
                }

                workItem.Invoke();
            }
        }
        
        public void Dispose()
        {
            _terminated = true;

            lock (_jobQueue)
            {
                Monitor.PulseAll(_jobQueue);
            }

            for (var i = 0; i < _workerThreads.Length; i++)
            {
                _workerThreads[i].Join();
            }
        }
    }
}
