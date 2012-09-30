#define USE_MONITOR

using System.Threading;

namespace SDRSharp.Radio
{
#if USE_MONITOR

    public sealed class SharpEvent
    {
        private bool _state;
        private bool _waiting;

        public SharpEvent(bool initialState)
        {
            _state = initialState;
        }

        public SharpEvent() : this(false)
        {
        }

        ~SharpEvent()
        {
            Dispose();
        }

        public void Dispose()
        {
            Set();
            System.GC.SuppressFinalize(this);
        }

        public void Set()
        {
            lock (this)
            {
                _state = true;
                if (_waiting)
                {
                    Monitor.Pulse(this);
                }
            }
        }

        public void WaitOne()
        {
            lock (this)
            {
                if (!_state)
                {
                    _waiting = true;
                    try
                    {
                        Monitor.Wait(this);
                    }
                    finally
                    {
                        _waiting = false;
                    }
                }
                _state = false;
            }
        }

        public void Reset()
        {
            lock (this)
            {
                _state = false;
            }
        }
    }

#else

    public sealed class SharpEvent
    {
        private readonly AutoResetEvent _event;

        public SharpEvent(bool initialState)
        {
            _event = new AutoResetEvent(initialState);
        }

        public void Set()
        {
            _event.Set();
        }

        public void WaitOne()
        {
            _event.WaitOne();
        }
    }

#endif
}
