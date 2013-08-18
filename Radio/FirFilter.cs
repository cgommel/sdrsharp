using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe interface IFilter
    {
        void Process(float* buffer, int length);
        void ProcessInterleaved(float* buffer, int length);
    }

#if !__MonoCS__
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
#endif
    public unsafe sealed class FirFilter : IDisposable, IFilter
    {
        private const double Epsilon = 1e-6;
        private const int CircularBufferSize = 2;

        private float* _coeffPtr;
        private UnsafeBuffer _coeffBuffer;

        private float* _queuePtr;
        private UnsafeBuffer _queueBuffer;

        private int _queueSize;
        private int _offset;
        private bool _isSymmetric;
        private bool _isHalfBand;
        private int _decimationFactor;

        public FirFilter()
            : this(new float[0])
        {
        }

        public FirFilter(float[] coefficients)
            : this(coefficients, 1)
        {
        }

        public FirFilter(float[] coefficients, int decimationFactor)
        {
            SetCoefficients(coefficients);
            if (_decimationFactor != 0 && _decimationFactor != decimationFactor)
            {
                throw new ArgumentException("This decimation factor cannot be used with a half band filter", "decimationFactor");
            }
            if (decimationFactor <= 0)
            {
                throw new ArgumentException("The decimation factor must be greater than zero", "decimationFactor");
            }
            _decimationFactor = decimationFactor;
        }

        ~FirFilter()
        {
            Dispose();
        }

        public void Dispose()
        {
            _coeffBuffer = null;
            _queueBuffer = null;
            _coeffPtr = null;
            _queuePtr = null;
            GC.SuppressFinalize(this);
        }

        public int Length
        {
            get { return _queueSize; }
        }

        public void SetCoefficients(float[] coefficients)
        {
            if (coefficients == null)
            {
                return;
            }

            if (_coeffBuffer == null || coefficients.Length != _queueSize)
            {
                _queueSize = coefficients.Length;
                _offset = _queueSize * (CircularBufferSize - 1);

                _coeffBuffer = UnsafeBuffer.Create(_queueSize, sizeof(float));
                _coeffPtr = (float*) _coeffBuffer;

                _queueBuffer = UnsafeBuffer.Create(_queueSize * CircularBufferSize, sizeof(float));
                _queuePtr = (float*) _queueBuffer;
            }

            for (var i = 0; i < _queueSize; i++)
            {
                _coeffPtr[i] = coefficients[i];
            }

            if (_queueSize % 2 != 0)
            {
                _isSymmetric = true;
                _isHalfBand = true;

                var halfLen = _queueSize / 2;

                for (var i = 0; i < halfLen; i++)
                {
                    var j = _queueSize - 1 - i;
                    if (Math.Abs(_coeffPtr[i] - _coeffPtr[j]) > Epsilon)
                    {
                        _isSymmetric = false;
                        _isHalfBand = false;
                        break;
                    }
                    if (i % 2 != 0)
                    {
                        _isHalfBand = _coeffPtr[i] == 0f && _coeffPtr[j] == 0f;
                    }
                }

                if (_isHalfBand)
                {
                    _decimationFactor = 2;
                }
            }
        }

        private void ProcessSymmetricKernel(float* buffer, int length)
        {
            for (int n = 0, m = 0; n < length; n += _decimationFactor, m++)
            {
                var queue = _queuePtr + _offset;

                for (int k = 0, l = n + _decimationFactor - 1; k < _decimationFactor; k++, l--)
                {
                    queue[k] = buffer[l];
                }

                var acc = 0.0f;

                var halfLen = _queueSize / 2;
                var len = halfLen;

                var ptr1 = _coeffPtr;
                var ptr2 = queue;
                var ptr3 = queue + _queueSize - 1;

                if (len >= 4)
                {
                    do
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
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
                    acc += *ptr1++ * (*ptr2++ + *ptr3--);
                }
                acc += queue[halfLen] * _coeffPtr[halfLen];

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        private void ProcessSymmetricKernelInterleaved(float* buffer, int length)
        {
            length <<= 1;
            for (int n = 0, m = 0; n < length; n += _decimationFactor * 2, m += 2)
            {
                var queue = _queuePtr + _offset;

                for (int k = 0, l = n + 2 * (_decimationFactor - 1); k < _decimationFactor; k++, l -= 2)
                {
                    queue[k] = buffer[l];
                }

                var acc = 0.0f;

                var halfLen = _queueSize / 2;
                var len = halfLen;

                var ptr1 = _coeffPtr;
                var ptr2 = queue;
                var ptr3 = queue + _queueSize - 1;

                if (len >= 4)
                {
                    do
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
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
                    acc += *ptr1++ * (*ptr2++ + *ptr3--);
                }
                acc += queue[halfLen] * _coeffPtr[halfLen];

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        private void ProcessHalfBandKernel(float* buffer, int length)
        {
            for (int n = 0, m = 0; n < length; n += 2, m++)
            {
                var queue = _queuePtr + _offset;

                queue[0] = buffer[n + 1];
                queue[1] = buffer[n];

                var acc = 0.0f;

                var halfLen = _queueSize / 2;
                var len = halfLen;

                var ptr1 = _coeffPtr;
                var ptr2 = queue;
                var ptr3 = queue + _queueSize - 1;

                if (len >= 8)
                {
                    do
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
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
                    acc += ptr1[0] * (ptr2[0] + ptr3[0])
                            + ptr1[2] * (ptr2[2] + ptr3[-2]);
                    ptr1 += 4;
                    ptr2 += 4;
                    ptr3 -= 4;
                    len -= 4;
                }
                while (len-- > 0)
                {
                    acc += *ptr1++ * (*ptr2++ + *ptr3--);
                }
                acc += queue[halfLen] * _coeffPtr[halfLen];

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        private void ProcessHalfBandInterleaved(float* buffer, int length)
        {
            length <<= 1;
            for (int n = 0, m = 0; n < length; n += 4, m += 2)
            {
                var queue = _queuePtr + _offset;

                queue[0] = buffer[n + 2];
                queue[1] = buffer[n];

                var acc = 0.0f;

                var halfLen = _queueSize / 2;
                var len = halfLen;

                var ptr1 = _coeffPtr;
                var ptr2 = queue;
                var ptr3 = queue + _queueSize - 1;

                if (len >= 8)
                {
                    do
                    {
                        acc += ptr1[0] * (ptr2[0] + ptr3[0])
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
                    acc += ptr1[0] * (ptr2[0] + ptr3[0])
                         + ptr1[2] * (ptr2[2] + ptr3[-2]);
                    ptr1 += 4;
                    ptr2 += 4;
                    ptr3 -= 4;
                    len -= 4;
                }
                while (len-- > 0)
                {
                    acc += *ptr1++ * (*ptr2++ + *ptr3--);
                }
                acc += queue[halfLen] * _coeffPtr[halfLen];

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        private void ProcessStandard(float* buffer, int length)
        {
            for (int n = 0, m = 0; n < length; n+= _decimationFactor, m++)
            {
                var queue = _queuePtr + _offset;

                for (int k = 0, l = n + _decimationFactor - 1; k < _decimationFactor; k++, l--)
                {
                    queue[k] = buffer[l];
                }

                var acc = 0.0f;

                var len = _queueSize;
                var ptr1 = queue;
                var ptr2 = _coeffPtr;
                if (len >= 4)
                {
                    do
                    {
                        acc += ptr1[0] * ptr2[0]
                             + ptr1[1] * ptr2[1]
                             + ptr1[2] * ptr2[2]
                             + ptr1[3] * ptr2[3];
                        ptr1 += 4;
                        ptr2 += 4;
                    } while ((len -= 4) >= 4);
                }
                while (len-- > 0)
                {
                    acc += *ptr1++ * *ptr2++;
                }

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        private void ProcessStandardInterleaved(float* buffer, int length)
        {
            length <<= 1;
            for (int n = 0, m = 0; n < length; n += _decimationFactor * 2, m += 2)
            {
                var queue = _queuePtr + _offset;

                for (int k = 0, l = n + 2 * (_decimationFactor - 1); k < _decimationFactor; k++, l -= 2)
                {
                    queue[k] = buffer[l];
                }

                var acc = 0.0f;

                var len = _queueSize;
                var ptr1 = queue;
                var ptr2 = _coeffPtr;
                if (len >= 4)
                {
                    do
                    {
                        acc += ptr1[0] * ptr2[0]
                             + ptr1[1] * ptr2[1]
                             + ptr1[2] * ptr2[2]
                             + ptr1[3] * ptr2[3];
                        ptr1 += 4;
                        ptr2 += 4;
                    } while ((len -= 4) >= 4);
                }
                while (len-- > 0)
                {
                    acc += *ptr1++ * *ptr2++;
                }

                if ((_offset -= _decimationFactor) < 0)
                {
                    var oldOffset = _offset + _decimationFactor;
                    _offset += _queueSize * (CircularBufferSize - 1);
                    Utils.Memcpy(_queuePtr + _offset + _decimationFactor, _queuePtr + oldOffset, (_queueSize - _decimationFactor) * sizeof(float));
                }

                buffer[m] = acc;
            }
        }

        public void Process(float* buffer, int length)
        {
            if (_isHalfBand)
            {
                ProcessHalfBandKernel(buffer, length);
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

        public void ProcessInterleaved(float* buffer, int length)
        {
            if (_isHalfBand)
            {
                ProcessHalfBandInterleaved(buffer, length);
            }
            else if (_isSymmetric)
            {
                ProcessSymmetricKernelInterleaved(buffer, length);
            }
            else
            {
                ProcessStandardInterleaved(buffer, length);
            }
        }
    }
}
