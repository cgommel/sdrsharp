using System;

namespace SDRSharp.Radio
{
    public unsafe class FirFilter : IDisposable
    {
        private const double Epsilon = 1e-6;

        private float* _coeffPtr;
        private UnsafeBuffer _coeffBuffer;

        private float* _queuePtr;
        private UnsafeBuffer _queueBuffer;

        private int _queueSize;
        private bool _isSymmetric;
        private bool _isSparse;

        public FirFilter(float[] coefficients)
        {
            SetCoefficients(coefficients);
        }

        ~FirFilter()
        {
            Dispose();
        }

        public void Dispose()
        {
            _coeffBuffer.Dispose();
            _queueBuffer.Dispose();
            GC.SuppressFinalize(this);
        }

        public int Length
        {
            get { return _queueSize; }
        }

        public void SetCoefficients(float[] coefficients)
        {
            if (_coeffBuffer == null || coefficients.Length != _queueSize)
            {
                _queueSize = coefficients.Length;

                if (_coeffBuffer != null)
                {
                    _coeffBuffer.Dispose();
                }
                _coeffBuffer = UnsafeBuffer.Create(_queueSize, sizeof(float));
                _coeffPtr = (float*) _coeffBuffer;


                if (_queueBuffer != null)
                {
                    _queueBuffer.Dispose();
                }
                _queueBuffer = UnsafeBuffer.Create(_queueSize, sizeof(float));
                _queuePtr = (float*) _queueBuffer;
            }

            for (var i = 0; i < _queueSize; i++)
            {
                _coeffPtr[i] = coefficients[i];
            }

            _isSymmetric = true;
            _isSparse = true;

            var halfLen = _queueSize / 2;

            for (var i = 0; i < halfLen; i++)
            {
                var j = _queueSize - 1 - i;
                if (Math.Abs(_coeffPtr[i] - _coeffPtr[j]) > Epsilon)
                {
                    _isSymmetric = false;
                    _isSparse = false;
                    break;
                }
                if (i % 2 == 0)
                {
                    _isSparse = _coeffPtr[i] == 0f && _coeffPtr[j] == 0f;
                }
            }
        }
        
        private float ProcessSparseSymmetricKernel(float sample)
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

        private float ProcessSymmetricKernel(float sample)
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

        private float ProcessStandard(float sample)
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

        public float Process(float sample)
        {
            if (_isSparse)
            {
                return ProcessSparseSymmetricKernel(sample);
            }
            if (_isSymmetric)
            {
                return ProcessSymmetricKernel(sample);
            }
            return ProcessStandard(sample);
        }

        private void ProcessSymmetricKernel(float* buffer, int length)
        {
            for (var n = 0; n < length; n++)
            {
                buffer[n] = ProcessSymmetricKernel(buffer[n]);
            }
        }

        private void ProcessSparseSymmetricKernel(float* buffer, int length)
        {
            for (var n = 0; n < length; n++)
            {
                buffer[n] = ProcessSparseSymmetricKernel(buffer[n]);
            }
        }

        private void ProcessStandard(float* buffer, int length)
        {
            for (var n = 0; n < length; n++)
            {
                buffer[n] = ProcessStandard(buffer[n]);
            }
        }

        public void Process(float* buffer, int length)
        {
            if (_isSparse)
            {
                ProcessSparseSymmetricKernel(buffer, length);
            }
            else if (_isSymmetric)
            {
                ProcessSymmetricKernel(buffer, length);
            }
            else
            {
                ProcessStandard(buffer, length);
            }
        }
    }
}
