using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class FirFilter : IDisposable
    {
        private readonly double[] _coefficients;
        private readonly double[] _queue;
        private readonly double* _coeffPtr;
        private readonly double* _queuePtr;
        private readonly GCHandle _coeffHandle;
        private readonly GCHandle _queueHandle;

        public FirFilter(double[] coefficients)
        {
            //_index = 0;
            _coefficients = coefficients;
            _queue = new double[_coefficients.Length];

            _coeffHandle = GCHandle.Alloc(_coefficients, GCHandleType.Pinned);
            _queueHandle = GCHandle.Alloc(_queue, GCHandleType.Pinned);

            _coeffPtr = (double*) _coeffHandle.AddrOfPinnedObject();
            _queuePtr = (double*) _queueHandle.AddrOfPinnedObject();
        }

        public void Dispose()
        {
            _coeffHandle.Free();
            _queueHandle.Free();
        }

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
    }
}
