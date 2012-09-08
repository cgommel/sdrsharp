using System;

namespace SDRSharp.Radio
{
    public static unsafe class Fourier
    {
        public const float MaxPower = 0.0f;
        public const float MinPower = -130.0f;

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
                var strength = (float)(10.5 * Math.Log10(m)) + offset;
                if (float.IsNaN(strength))
                {
                    strength = MinPower;
                }
                else if (strength > MaxPower)
                {
                    strength = MaxPower;
                }
                else if (strength < MinPower)
                {
                    strength = MinPower;
                }
                power[i] = strength;
            }
        }
        
        public static void SpectrumPower(Complex* buffer, float* power, int length, float offset)
        {
            for (var i = 0; i < length; i++)
            {
                var m = buffer[i].Real * buffer[i].Real + buffer[i].Imag * buffer[i].Imag;
                var strength = (float) (10 * Math.Log10(m)) + offset;
                if (float.IsNaN(strength))
                {
                    strength = MinPower;
                }
                else if (strength > MaxPower)
                {
                    strength = MaxPower;
                }
                else if (strength < MinPower)
                {
                    strength = MinPower;
                }
                power[i] = strength;
            }
        }

        public static void ScaleFFT(float* src, byte* dest, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var magnitude = src[i];
                if (magnitude < MinPower)
                {
                    magnitude = MinPower;
                }
                else if (magnitude > MaxPower)
                {
                    magnitude = MaxPower;
                }
                dest[i] = (byte)((magnitude - MinPower) * byte.MaxValue / (MaxPower - MinPower));
            }
        }

        public static void ScaleFFT(float[] src, byte[] dest, int length)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }
            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }
            if (src.Length < length)
            {
                throw new ArgumentException("src.Length should be greater or equal to length");
            }
            if (dest.Length < length)
            {
                throw new ArgumentException("dest.Length should be greater or equal to length");
            }
            for (var i = 0; i < length; i++)
            {
                var magnitude = src[i];
                if (magnitude < MinPower)
                {
                    magnitude = MinPower;
                }
                else if (magnitude > MaxPower)
                {
                    magnitude = MaxPower;
                }
                dest[i] = (byte)((magnitude - MinPower) * byte.MaxValue / (MaxPower - MinPower));
            }
        }

        public static void SmoothCopy(byte[] source, byte[] destination, int sourceLength, float scale, int offset)
        {
            fixed (byte* srcPtr = source)
            fixed (byte* dstPtr = destination)
            {
                var r = sourceLength / scale / destination.Length;
                if (r > 1.0f)
                {
                    var n = (int)Math.Ceiling(r);
                    for (var i = 0; i < destination.Length; i++)
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
                    for (var i = 0; i < destination.Length; i++)
                    {
                        var index = (int)(r * i + offset);
                        if (index >= 0 && index < sourceLength)
                        {
                            dstPtr[i] = srcPtr[index];
                        }
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
            int i, j, jm1, k, l, m, le, le2, ip, nd4;
            float ur, ui, sr, si, tr, ti;

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
                    tr = buffer[j].Real;
                    ti = buffer[j].Imag;
                    buffer[j].Real = buffer[i].Real;
                    buffer[j].Imag = buffer[i].Imag;
                    buffer[i].Real = tr;
                    buffer[i].Imag = ti;
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
                ur = 1;
                ui = 0;

                sr = (float)Math.Cos(Math.PI / le2);
                si = (float)-Math.Sin(Math.PI / le2);

                for (j = 1; j <= le2; ++j)
                {
                    jm1 = j - 1;

                    for (i = jm1; i <= nm1; i += le)
                    {
                        ip = i + le2;
                        tr = buffer[ip].Real * ur - buffer[ip].Imag * ui;
                        ti = buffer[ip].Real * ui + buffer[ip].Imag * ur;
                        buffer[ip].Real = buffer[i].Real - tr;
                        buffer[ip].Imag = buffer[i].Imag - ti;
                        buffer[i].Real = buffer[i].Real + tr;
                        buffer[i].Imag = buffer[i].Imag + ti;
                    }

                    tr = ur;
                    ur = tr * sr - ui * si;
                    ui = tr * si + ui * sr;
                }
            }

            nd4 = nd2 / 2;
            for (i = 0; i < nd4; i++)
            {
                Complex tmp = buffer[i];
                buffer[i] = buffer[nd2 - i - 1];
                buffer[nd2 - i - 1] = tmp;

                tmp = buffer[nd2 + i];
                buffer[nd2 + i] = buffer[nd2 + nd2 - i - 1];
                buffer[nd2 + nd2 - i - 1] = tmp;
            }
        }

        public static void ForwardTransform(Complex* buffer, int length)
        {
            int nm1 = length - 1;
            int nd2 = length / 2;
            int i, j, jm1, k, l, m, le, le2, ip, nd4;
            float ur, ui, sr, si, tr, ti;

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
                    tr = buffer[j].Real;
                    ti = buffer[j].Imag;
                    buffer[j].Real = buffer[i].Real;
                    buffer[j].Imag = buffer[i].Imag;
                    buffer[i].Real = tr;
                    buffer[i].Imag = ti;
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
                ur = 1;
                ui = 0;

                sr = (float) Math.Cos(Math.PI / le2);
                si = (float) -Math.Sin(Math.PI / le2);

                for (j = 1; j <= le2; ++j)
                {
                    jm1 = j - 1;

                    for (i = jm1; i <= nm1; i += le)
                    {
                        ip = i + le2;
                        tr = buffer[ip].Real * ur - buffer[ip].Imag * ui;
                        ti = buffer[ip].Real * ui + buffer[ip].Imag * ur;
                        buffer[ip].Real = buffer[i].Real - tr;
                        buffer[ip].Imag = buffer[i].Imag - ti;
                        buffer[i].Real = buffer[i].Real + tr;
                        buffer[i].Imag = buffer[i].Imag + ti;
                    }

                    tr = ur;
                    ur = tr * sr - ui * si;
                    ui = tr * si + ui * sr;
                }
            }

            nd4 = nd2 / 2;
            for (i = 0; i < nd4; i++)
            {
                Complex tmp = buffer[i];
                buffer[i] = buffer[nd2 - i - 1];
                buffer[nd2 - i - 1] = tmp;

                tmp = buffer[nd2 + i];
                buffer[nd2 + i] = buffer[nd2 + nd2 - i - 1];
                buffer[nd2 + nd2 - i - 1] = tmp;
            }
        }
    }
}
