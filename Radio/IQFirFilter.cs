using System;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe sealed class IQFirFilter
    {
        private readonly bool _isMultiThteaded;
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
        private readonly AutoResetEvent _event;

        public IQFirFilter(float[] coefficients, bool isMultiThteaded)
        {
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
            _isMultiThteaded = isMultiThteaded;
            if (_isMultiThteaded)
            {
                _event = new AutoResetEvent(false);
            }
        }

        ~IQFirFilter()
        {
            Dispose();
        }

        public void Dispose()
        {
            _rFilter.Dispose();
            _iFilter.Dispose();
            GC.SuppressFinalize(this);
        }

        public void Process(Complex* iq, int length)
        {
            var ptr = (float*) iq;

            if (_isMultiThteaded)
            {
                ThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        _rFilter.ProcessInterleaved(ptr, length);
                        _event.Set();
                    });
            }
            else
            {
                _rFilter.ProcessInterleaved(ptr, length);
            }

            _iFilter.ProcessInterleaved(ptr + 1, length);

            if (_isMultiThteaded)
            {
                _event.WaitOne();
            }
        }

        public void SetCoefficients(float[] coefficients)
        {
            _rFilter.SetCoefficients(coefficients);
            _iFilter.SetCoefficients(coefficients);
        }
    }
}
