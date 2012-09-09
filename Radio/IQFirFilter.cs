using System;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe sealed class IQFirFilter
    {
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        public IQFirFilter() : this(new float[0])
        {
        }

        public IQFirFilter(float[] coefficients)
        {
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
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

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    _rFilter.ProcessInterleaved(ptr, length);
                    _event.Set();
                });

            _iFilter.ProcessInterleaved(ptr + 1, length);
            _event.WaitOne();
        }

        public void SetCoefficients(float[] coefficients)
        {
            _rFilter.SetCoefficients(coefficients);
            _iFilter.SetCoefficients(coefficients);
        }
    }
}
