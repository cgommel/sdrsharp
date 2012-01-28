#if __MonoCS__
#define MANAGED_ONLY
#endif

using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class FirFilter : IDisposable
    {
        private readonly float* _coeffPtr;
        private readonly GCHandle _coeffHandle;
        private readonly float[] _coefficients;

#if MANAGED_ONLY

        private readonly float* _queuePtr;
        private readonly GCHandle _queueHandle;
        private readonly float[] _queue;

#else

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessBuffer(
            float* buffer,
            int bufferSize,
            IntPtr filterHandle);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern IntPtr MakeSimpleFilter(
            float* coeffs,
            int bufferSize);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FreeSimpleFilter(IntPtr filterHandle);

        private readonly IntPtr _filterHandle;
#endif

        public FirFilter(float[] coefficients)
        {
            _coefficients = coefficients;
            _coeffHandle = GCHandle.Alloc(_coefficients, GCHandleType.Pinned);
            _coeffPtr = (float*) _coeffHandle.AddrOfPinnedObject();

#if MANAGED_ONLY
            _queue = new float[_coefficients.Length];
            _queueHandle = GCHandle.Alloc(_queue, GCHandleType.Pinned);
            _queuePtr = (float*) _queueHandle.AddrOfPinnedObject();
#else
            _filterHandle = MakeSimpleFilter(_coeffPtr, coefficients.Length);
#endif
        }

        public void Dispose()
        {
            _coeffHandle.Free();

#if MANAGED_ONLY
            _queueHandle.Free();
#else
            FreeSimpleFilter(_filterHandle);
#endif
        }

#if MANAGED_ONLY
        public float Process(float sample)
        {
            _queuePtr[0] = sample;

            /* calc FIR */
            var accum = 0.0f;
            for (var i = 0; i < _queue.Length; i++)
            {
                accum += _coeffPtr[i] * _queuePtr[i];
            }

            /* shift delay line */
            Array.Copy(_queue, 0, _queue, 1, _queue.Length - 1);

            return accum;
        }
#endif

        public void Process(float* buffer, int length)
        {
#if MANAGED_ONLY
            for (var n = 0; n < length; n++)
            {
                _queuePtr[0] = buffer[n];

                /* calc FIR */
                var accum = 0.0f;
                for (var i = 0; i < _queue.Length; i++)
                {
                    accum += _coeffPtr[i] * _queuePtr[i];
                }

                /* shift delay line */
                Array.Copy(_queue, 0, _queue, 1, _queue.Length - 1);

                buffer[n] = accum;
            }
#else
            FirProcessBuffer(buffer, length, _filterHandle);
#endif
        }
    }
}
