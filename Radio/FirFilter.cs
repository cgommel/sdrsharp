using System;

namespace SDRSharp.Radio
{
    public unsafe class FirFilter : IDisposable
    {
        private readonly float* _coeffPtr;
        private readonly UnsafeBuffer _coeffBuffer;

        private readonly float* _queuePtr;
        private readonly UnsafeBuffer _queueBuffer;

        private readonly int _queueSize;

        public FirFilter(float[] coefficients)
        {
            _coeffBuffer = UnsafeBuffer.Create(coefficients);
            _coeffPtr = (float*) _coeffBuffer;

            _queueBuffer = UnsafeBuffer.Create<float>(coefficients.Length);
            _queuePtr = (float*) _queueBuffer;

            _queueSize = coefficients.Length;
        }

        public void Dispose()
        {
            _coeffBuffer.Dispose();
            _queueBuffer.Dispose();
        }

        public float* Coefficients
        {
            get { return _coeffPtr; }
        }
        
        public float ProcessSparseSymmetricKernel(float sample)
        {
            _queuePtr[0] = sample;

            var result = 0.0f;

            var halfLen = _queueSize / 2;
            var len = halfLen;

            var ptr1 = _coeffPtr;
            var ptr2 = _queuePtr;
            var ptr3 = _queuePtr + _queueSize - 1;

            if (len >= 8)
            {
                do
                {
                    result += ptr1[0] * (ptr2[0] + ptr3[0])
                            + ptr1[2] * (ptr2[2] + ptr3[-2])
                            + ptr1[4] * (ptr2[4] + ptr3[-4])
                            + ptr1[6] * (ptr2[6] + ptr3[-6]);

                    ptr1 += 8;
                    ptr2 += 8;
                    ptr3 -= 8;
                } while ((len -= 8) >= 8);
            }
            if (len >= 4)
            {
                result += ptr1[0] * (ptr2[0] + ptr3[0])
                        + ptr1[2] * (ptr2[2] + ptr3[-2]);
                ptr1 += 4;
                ptr2 += 4;
                ptr3 -= 4;
                len -= 4;
            }
            while (len-- > 0)
            {
                result += *ptr1++ * (*ptr2++ + *ptr3--);
            }
            result += _queuePtr[halfLen] * _coeffPtr[halfLen];

            Utils.Memcpy(_queuePtr + 1, _queuePtr, (_queueSize - 1) * sizeof(float));

            return result;
        }

        public float ProcessSymmetricKernel(float sample)
        {
            _queuePtr[0] = sample;

            var result = 0.0f;

            var halfLen = _queueSize / 2;
            var len = halfLen;

            var ptr1 = _coeffPtr;
            var ptr2 = _queuePtr;
            var ptr3 = _queuePtr + _queueSize - 1;

            if (len >= 4)
            {
                do
                {
                    result += ptr1[0] * (ptr2[0] + ptr3[0])
                            + ptr1[1] * (ptr2[1] + ptr3[-1])
                            + ptr1[2] * (ptr2[2] + ptr3[-2])
                            + ptr1[3] * (ptr2[3] + ptr3[-3]);

                    ptr1 += 4;
                    ptr2 += 4;
                    ptr3 -= 4;
                } while ((len -= 4) >= 4);
            }
            while (len-- > 0)
            {
                result += *ptr1++ * (*ptr2++ + *ptr3--);
            }
            result += _queuePtr[halfLen] * _coeffPtr[halfLen];

            Utils.Memcpy(_queuePtr + 1, _queuePtr, (_queueSize - 1) * sizeof(float));

            return result;
        }

        public void ProcessSymmetricKernel(float* buffer, int length)
        {
            for (var n = 0; n < length; n++)
            {
                buffer[n] = ProcessSymmetricKernel(buffer[n]);
            }
        }

        public float Process(float sample)
        {
            _queuePtr[0] = sample;

            var result = 0.0f;

            var len = _queueSize;
            var ptr1 = _queuePtr;
            var ptr2 = _coeffPtr;
            if (len >= 4)
            {
                do
                {
                    result += ptr1[0] * ptr2[0]
                            + ptr1[1] * ptr2[1]
                            + ptr1[2] * ptr2[2]
                            + ptr1[3] * ptr2[3];
                    ptr1 += 4;
                    ptr2 += 4;
                } while ((len -= 4) >= 4);
            }
            while (len-- > 0)
            {
                result += *ptr1++ * *ptr2++;
            }

            Utils.Memcpy(_queuePtr + 1, _queuePtr, (_queueSize - 1) * sizeof(float));

            return result;
        }

        public void Process(float* buffer, int length)
        {
            for (var n = 0; n < length; n++)
            {
                buffer[n] = Process(buffer[n]);
            }
        }
    }
}
