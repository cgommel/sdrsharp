#if __MonoCS__
#define MANAGED_ONLY
#endif

#if !MANAGED_ONLY
using System;
using System.Runtime.InteropServices;
#endif

namespace SDRSharp.Radio
{
    public unsafe class IQFirFilter : IDisposable
    {
#if MANAGED_ONLY
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
#else
        #region Native API

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessComplexBuffer(
            Complex* buffer,
            int bufferSize,
            IntPtr filterHandle);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern IntPtr MakeComplexFilter(
            double* coeffs,
            int bufferSize);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FreeComplexFilter(IntPtr filterHandle);

        #endregion

        private readonly IntPtr _filterHandle;
#endif

        public IQFirFilter(double[] coefficients)
        {
#if MANAGED_ONLY
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
#else
            fixed (double* ptr = coefficients)
            {
                _filterHandle = MakeComplexFilter(ptr, coefficients.Length);
            }
#endif
        }

        public void Dispose()
        {
#if !MANAGED_ONLY
            FreeComplexFilter(_filterHandle);
#endif
        }

        public void Process(Complex[] iq)
        {
#if MANAGED_ONLY
            for (var i = 0; i < iq.Length; i++)
            {
                iq[i].Real = _rFilter.Process(iq[i].Real);
                iq[i].Imag = _iFilter.Process(iq[i].Imag);
            }
#else
            fixed (Complex* ptr = iq)
            {
                FirProcessComplexBuffer(ptr, iq.Length, _filterHandle);
            }
#endif
        }
    }
}
