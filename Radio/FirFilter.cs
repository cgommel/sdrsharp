#if __MonoCS__
#define MANAGED_ONLY
#endif

using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class FirFilter : IDisposable
    {
#if !MANAGED_ONLY

        #region Native API

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessBuffer(
            [In, Out] double[] buffer,
            int bufferSize,
            [In, Out] double[] queue,
            double[] coeffs,
            int queueSize,
            ref int index);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern double FirProcessSample(
            double sample,
            [In, Out] double[] queue,
            double[] coeffs,
            int queueSize,
            ref int index);

        #endregion

        private int _index;
#else
        private readonly double* _coeffPtr;
        private readonly double* _queuePtr;
        private readonly GCHandle _coeffHandle;
        private readonly GCHandle _queueHandle;
#endif

        private readonly double[] _coefficients;
        private readonly double[] _queue;


        public FirFilter(double[] coefficients)
        {
            _coefficients = coefficients;
            _queue = new double[_coefficients.Length];

#if MANAGED_ONLY
            _coeffHandle = GCHandle.Alloc(_coefficients, GCHandleType.Pinned);
            _queueHandle = GCHandle.Alloc(_queue, GCHandleType.Pinned);

            _coeffPtr = (double*) _coeffHandle.AddrOfPinnedObject();
            _queuePtr = (double*) _queueHandle.AddrOfPinnedObject();
#endif
        }

        public void Dispose()
        {
#if MANAGED_ONLY
            _coeffHandle.Free();
            _queueHandle.Free();
#endif
        }

#if MANAGED_ONLY

        public double Process(double sample)
        {
            _queuePtr[0] = sample;

            /* calc FIR */
            var accum = 0.0;
            for (var i = 0; i < _queue.Length; i++)
            {
                accum += _coeffPtr[i] * _queuePtr[i];
            }

            /* shift delay line */
            Array.Copy(_queue, 0, _queue, 1, _queue.Length - 1);

            return accum;
        }

        public void Process(double[] buffer)
        {
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
        }
#else

        public double Process(double sample)
        {
            return FirProcessSample(sample, _queue, _coefficients, _queue.Length, ref _index);
        }

        public void Process(double[] buffer)
        {
            FirProcessBuffer(buffer, buffer.Length, _queue, _coefficients, _queue.Length, ref _index);
        }

#endif
    }
}
