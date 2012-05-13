using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class IQFirFilter : IFilter, IDisposable
    {
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;

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
            _rFilter.ProcessInterleaved(ptr, length);
            _iFilter.ProcessInterleaved(ptr + 1, length);
        }

        public void SetCoefficients(float[] coefficients)
        {
            _rFilter.SetCoefficients(coefficients);
            _iFilter.SetCoefficients(coefficients);
        }
    }
}
