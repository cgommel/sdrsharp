using System;

namespace SDRSharp.Radio
{
    public static unsafe class Fourier
    {
        public const int MaxSizeBits = 22; // 4M
        public const int MaxSize = 1 << MaxSizeBits;
        private const int HalfSize = MaxSize / 2;

        private static UnsafeBuffer _lutBuffer = UnsafeBuffer.Create(HalfSize, sizeof(Complex));
        private static Complex* _lut;

        static Fourier()
        {
            _lut = (Complex*)_lutBuffer;

            const double twoPi = 2.0 * Math.PI;
            var angle = twoPi / MaxSize;

            for (var i = 0; i < HalfSize; i++)
            {
                _lut[i] = Complex.FromAngle((angle * i) % twoPi).Conjugate();
            }
        }

        public static void SpectrumPower(Complex* buffer, float* power, int length)
        {
            SpectrumPower(buffer, power, length, 0.0f);
        }
        
        public static void SpectrumPower(Complex[] buffer, float[] power, int length, float offset)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (power == null)
            {
                throw new ArgumentNullException("power");
            }
            if (buffer.Length < length)
            {
                throw new ArgumentException("buffer.Length should be greater or equal to length");
            }
            if (power.Length < length)
            {
                throw new ArgumentException("power.Length should be greater or equal to length");
            }
            for (var i = 0; i < length; i++)
            {
                var m = buffer[i].Real * buffer[i].Real + buffer[i].Imag * buffer[i].Imag;
                var strength = (float)(10.0 * Math.Log10(1e-60 + m)) + offset;
                power[i] = strength;
            }
        }
        
        public static void SpectrumPower(Complex* buffer, float* power, int length, float offset)
        {
            for (var i = 0; i < length; i++)
            {
                var m = buffer[i].Real * buffer[i].Real + buffer[i].Imag * buffer[i].Imag;
                var strength = (float) (10.0 * Math.Log10(1e-60 + m)) + offset;
                power[i] = strength;
            }
        }

        public static void ScaleFFT(float* src, byte* dest, int length, float minPower, float maxPower)
        {
            var scale = byte.MaxValue / (maxPower - minPower);
            for (var i = 0; i < length; i++)
            {
                var magnitude = src[i];
                if (magnitude < minPower)
                {
                    magnitude = minPower;
                }
                else if (magnitude > maxPower)
                {
                    magnitude = maxPower;
                }
                dest[i] = (byte) ((magnitude - minPower) *  scale);
            }
        }

        public static void ScaleFFT(float[] src, byte[] dest, int length, float minPower, float maxPower)
        {
            var scale = byte.MaxValue / (maxPower - minPower);
            for (var i = 0; i < length; i++)
            {
                var magnitude = src[i];
                if (magnitude < minPower)
                {
                    magnitude = minPower;
                }
                else if (magnitude > maxPower)
                {
                    magnitude = maxPower;
                }
                dest[i] = (byte) ((magnitude - minPower) * scale);
            }
        }

        public static void SmoothCopy(byte[] source, byte[] destination, int sourceLength, float scale, int offset)
        {
            fixed (byte* srcPtr = source)
            fixed (byte* dstPtr = destination)
            {
                SmoothCopy(srcPtr, dstPtr, sourceLength, destination.Length, scale, offset);
            }
        }

        public static void SmoothCopy(byte* srcPtr, byte* dstPtr, int sourceLength, int destinationLength, float scale, int offset)
        {
            var r = sourceLength / scale / destinationLength;
            if (r > 1.0f)
            {
                var n = (int) Math.Ceiling(r);
                for (var i = 0; i < destinationLength; i++)
                {
                    var k = (int)(i * r - n * 0.5f);
                    var max = (byte) 0;
                    for (var j = 0; j < n; j++)
                    {
                        var index = k + j + offset;
                        if (index >= 0 && index < sourceLength)
                        {
                            if (max < srcPtr[index])
                            {
                                max = srcPtr[index];
                            }
                        }
                    }
                    dstPtr[i] = max;
                }
            }
            else
            {
                for (var i = 0; i < destinationLength; i++)
                {
                    var index = (int)(r * i + offset);
                    if (index >= 0 && index < sourceLength)
                    {
                        dstPtr[i] = srcPtr[index];
                    }
                }
            }
        }

        public static void ApplyFFTWindow(Complex[] buffer, float[] window, int length)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }
            if (window == null)
            {
                throw new ArgumentNullException("window");
            }
            if (buffer.Length < length)
            {
                throw new ArgumentException("buffer.Length should be greater or equal to length");
            }
            if (window.Length < length)
            {
                throw new ArgumentException("window.Length should be greater or equal to length");
            }
            for (var i = 0; i < length; i++)
            {
                buffer[i].Real *= window[i];
                buffer[i].Imag *= window[i];
            }
        }

        public static void ApplyFFTWindow(Complex* buffer, float* window, int length)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[i].Real *= window[i];
                buffer[i].Imag *= window[i];
            }
        }

        public static void ForwardTransform(Complex[] buffer, int length)
        {
            int nm1 = length - 1;
            int nd2 = length / 2;
            int i, j, jm1, k, l, m, n, le, le2, ip, nd4;
            Complex u, t;

            m = 0;
            i = length;
            while (i > 1)
            {
                ++m;
                i = (i >> 1);
            }

            j = nd2;

            for (i = 1; i < nm1; ++i)
            {
                if (i < j)
                {
                    t = buffer[j];
                    buffer[j] = buffer[i];
                    buffer[i] = t;
                }

                k = nd2;

                while (k <= j)
                {
                    j = j - k;
                    k = k / 2;
                }

                j += k;
            }

            for (l = 1; l <= m; ++l)
            {
                le = 1 << l;
                le2 = le / 2;

                n = MaxSizeBits - l;

                for (j = 1; j <= le2; ++j)
                {
                    jm1 = j - 1;

                    u = _lut[jm1 << n];

                    for (i = jm1; i <= nm1; i += le)
                    {
                        ip = i + le2;

                        t = u * buffer[ip];
                        buffer[ip] = buffer[i] - t;
                        buffer[i] += t;
                    }
                }
            }

            nd4 = nd2 / 2;
            for (i = 0; i < nd4; i++)
            {
                t = buffer[i];
                buffer[i] = buffer[nd2 - i - 1];
                buffer[nd2 - i - 1] = t;

                t = buffer[nd2 + i];
                buffer[nd2 + i] = buffer[nd2 + nd2 - i - 1];
                buffer[nd2 + nd2 - i - 1] = t;
            }
        }

        public static void ForwardTransform(Complex* buffer, int length)
        {
            int nm1 = length - 1;
            int nd2 = length / 2;
            int i, j, jm1, k, l, m, n, le, le2, ip, nd4;
            Complex u, t;

            m = 0;
            i = length;
            while (i > 1)
            {
                ++m;
                i = (i >> 1);
            }

            j = nd2;

            for (i = 1; i < nm1; ++i)
            {
                if (i < j)
                {
                    t = buffer[j];
                    buffer[j] = buffer[i];
                    buffer[i] = t;
                }

                k = nd2;

                while (k <= j)
                {
                    j = j - k;
                    k = k / 2;
                }

                j += k;
            }

            for (l = 1; l <= m; ++l)
            {
                le = 1 << l;
                le2 = le / 2;

                n = MaxSizeBits - l;

                for (j = 1; j <= le2; ++j)
                {
                    jm1 = j - 1;

                    u = _lut[jm1 << n];

                    for (i = jm1; i <= nm1; i += le)
                    {
                        ip = i + le2;

                        t = u * buffer[ip];
                        buffer[ip] = buffer[i] - t;
                        buffer[i] += t;
                    }
                }
            }

            nd4 = nd2 / 2;
            for (i = 0; i < nd4; i++)
            {
                t = buffer[i];
                buffer[i] = buffer[nd2 - i - 1];
                buffer[nd2 - i - 1] = t;

                t = buffer[nd2 + i];
                buffer[nd2 + i] = buffer[nd2 + nd2 - i - 1];
                buffer[nd2 + nd2 - i - 1] = t;
            }
        }
    }
}
