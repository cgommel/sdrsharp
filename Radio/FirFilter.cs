#if __MonoCS__
#define MANAGED_ONLY
#endif

using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
#if MANAGED_ONLY
    public unsafe class FirFilter : IDisposable
    {
        private readonly double* _coeffPtr;
        private readonly double* _queuePtr;
        private readonly GCHandle _coeffHandle;
        private readonly GCHandle _queueHandle;

        private readonly double[] _coefficients;
        private readonly double[] _queue;
#else
    public class FirFilter : IDisposable
    {
        #region Native API

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessBuffer(
            [In, Out] double[] buffer,
            int bufferSize,
            IntPtr filterHandle);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern IntPtr MakeSimpleFilter(
            double[] coeffs,
            int bufferSize);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FreeSimpleFilter(IntPtr filterHandle);

        #endregion

        private readonly IntPtr _filterHandle;
#endif

        public FirFilter(double[] coefficients)
        {
#if MANAGED_ONLY
            _coefficients = coefficients;
            _queue = new double[_coefficients.Length];

            _coeffHandle = GCHandle.Alloc(_coefficients, GCHandleType.Pinned);
            _queueHandle = GCHandle.Alloc(_queue, GCHandleType.Pinned);

            _coeffPtr = (double*) _coeffHandle.AddrOfPinnedObject();
            _queuePtr = (double*) _queueHandle.AddrOfPinnedObject();
#else
            _filterHandle = MakeSimpleFilter(coefficients, coefficients.Length);
#endif
        }

        public void Dispose()
        {
#if MANAGED_ONLY
            _coeffHandle.Free();
            _queueHandle.Free();
#else
            FreeSimpleFilter(_filterHandle);
#endif
        }

        public void Process(double[] buffer)
        {
#if MANAGED_ONLY
            for (var n = 0; n < buffer.Length; n++)
            {
                _queuePtr[0] = buffer[n];

                /* calc FIR */
                var accum = 0.0;
                for (var i = 0; i < _queue.Length; i++)
                {
                    accum += _coeffPtr[i] * _queuePtr[i];
                }

                /* shift delay line */
                Array.Copy(_queue, 0, _queue, 1, _queue.Length - 1);

                buffer[n] = accum;
            }
#else
            FirProcessBuffer(buffer, buffer.Length, _filterHandle);
#endif
        }
    }
}
