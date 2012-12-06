using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class IQFirFilter
    {
        private readonly bool _isMultiThteaded;
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
        private readonly SharpEvent _event;

        public IQFirFilter(float[] coefficients) : this (coefficients, false)
        {
        }

        public IQFirFilter(float[] coefficients, bool isMultiThteaded)
        {
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
            _isMultiThteaded = isMultiThteaded;
            if (_isMultiThteaded)
            {
                _event = new SharpEvent(false);
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
                DSPThreadPool.QueueUserWorkItem(
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
