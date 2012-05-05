using System;

namespace SDRSharp.Radio
{
    public unsafe class IQFirFilter : IDisposable
    {
        private readonly int _filterLength;
        private FirFilter _rFilter;
        private FirFilter _iFilter;

        public IQFirFilter(float[] coefficients)
        {
            _filterLength = coefficients.Length;
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
            for (var i = 0; i < length; i++)
            {
                iq[i].Real = _rFilter.Process(iq[i].Real);
                iq[i].Imag = _iFilter.Process(iq[i].Imag);
            }
        }

        public void SetCoefficients(float[] coefficients)
        {
            _rFilter.SetCoefficients(coefficients);
            _iFilter.SetCoefficients(coefficients);
        }
    }
}
