#if __MonoCS__
#define MANAGED_ONLY
#endif

#if !MANAGED_ONLY
using System.Runtime.InteropServices;
#endif

namespace SDRSharp.Radio
{
    public class IQFirFilter
    {
#if MANAGED_ONLY
        private readonly FirFilter _rFilter;
        private readonly FirFilter _iFilter;
#else
        private int _indexR;
        private int _indexI;
        private readonly double[] _coefficients;
        private readonly double[] _rQueue;
        private readonly double[] _iQueue;

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessComplexBuffer(
            [In, Out] Complex[] buffer,
            int bufferSize,
            [In, Out] double[] queueR,
            [In, Out] double[] queueI,
            double[] coeffs,
            int queueSize,
            ref int indexR,
            ref int indexI);
#endif

        public IQFirFilter(double[] coefficients)
        {
#if MANAGED_ONLY
            _rFilter = new FirFilter(coefficients);
            _iFilter = new FirFilter(coefficients);
#else
            _coefficients = coefficients;
            _rQueue = new double[_coefficients.Length];
            _iQueue = new double[_coefficients.Length];
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
            FirProcessComplexBuffer(iq, iq.Length, _rQueue, _iQueue, _coefficients, _iQueue.Length, ref _indexR, ref _indexI);
#endif
        }
    }
}
