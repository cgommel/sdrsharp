#if __MonoCS__
#define MANAGED_ONLY
#endif

#if !MANAGED_ONLY
using System;
using System.Runtime.InteropServices;
#endif

namespace SDRSharp.Radio
{
    public unsafe class IQFirFilter 
#if !MANAGED_ONLY
        : IDisposable
#endif
    {
#if MANAGED_ONLY
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
#else

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessComplexBuffer(
            Complex* buffer,
            int bufferSize,
            IntPtr filterHandle);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern IntPtr MakeComplexFilter(
            float[] coeffs,
            int bufferSize);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FreeComplexFilter(IntPtr filterHandle);

        private readonly IntPtr _filterHandle;
#endif

        public IQFirFilter(float[] coefficients)
        {
#if MANAGED_ONLY
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
#else
            _filterHandle = MakeComplexFilter(coefficients, coefficients.Length);
#endif
        }

#if !MANAGED_ONLY
        public void Dispose()
        {
            FreeComplexFilter(_filterHandle);
        }
#endif

        public void Process(Complex* iq, int length)
        {
#if MANAGED_ONLY
            for (var i = 0; i < length; i++)
            {
                iq[i].Real = _rFilter.Process(iq[i].Real);
                iq[i].Imag = _iFilter.Process(iq[i].Imag);
            }
#else
            FirProcessComplexBuffer(iq, length, _filterHandle);
#endif
        }
    }
}
