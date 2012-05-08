using System;

namespace SDRSharp.Radio
{
    public static unsafe class Fourier
    {
        public const float MaxPower = 0.0f;
        public const float MinPower = -130.0f;

        public static void ForwardTransform(Complex[] buffer, int length)
        {
            fixed (Complex* bufferPtr = buffer)
            {
                ForwardTransform(bufferPtr, length);
            }
        }

        public static void SpectrumPower(Complex* buffer, float* power, int length)
        {
            SpectrumPower(buffer, power, length, 0.0f);
        }
        
        public static void SpectrumPower(Complex[] buffer, float[] power, int length, float offset)
        {
            fixed (Complex* bufferPtr = buffer)
            fixed (float* powerPtr = power)
            {
                SpectrumPower(bufferPtr, powerPtr, length, offset);
            }
        }
        
        public static void SpectrumPower(Complex* buffer, float* power, int length, float offset)
        {
            for (var i = 0; i < length; i++)
            {
                var m = buffer[i].Real * buffer[i].Real + buffer[i].Imag * buffer[i].Imag;
                var strength = (float) (20.0 * 0.5 * Math.Log10(m)) + offset;
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
                dest[i] = (byte) (magnitude * byte.MaxValue / (MaxPower - MinPower));
            }
        }

        public static void ScaleFFT(float[] src, byte[] dest, int length)
        {
            fixed (float* srcPtr = src)
            fixed (byte* destPtr = dest)
            {
                ScaleFFT(srcPtr, destPtr, length);
            }
        }

        public static void ApplyFFTWindow(Complex[] buffer, float[] window, int length)
        {
            fixed (Complex* bufferPtr = buffer)
            fixed (float* windowPtr = window)
            {
                ApplyFFTWindow(bufferPtr, windowPtr, length);
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

        public static void ForwardTransform(Complex* samples, int length)
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
                    tr = samples[j].Real;
                    ti = samples[j].Imag;
                    samples[j].Real = samples[i].Real;
                    samples[j].Imag = samples[i].Imag;
                    samples[i].Real = tr;
                    samples[i].Imag = ti;
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
                        tr = samples[ip].Real * ur - samples[ip].Imag * ui;
                        ti = samples[ip].Real * ui + samples[ip].Imag * ur;
                        samples[ip].Real = samples[i].Real - tr;
                        samples[ip].Imag = samples[i].Imag - ti;
                        samples[i].Real = samples[i].Real + tr;
                        samples[i].Imag = samples[i].Imag + ti;
                    }

                    tr = ur;
                    ur = tr * sr - ui * si;
                    ui = tr * si + ui * sr;
                }
            }

            nd4 = nd2 / 2;
            for (i = 0; i < nd4; i++)
            {
                Complex tmp = samples[i];
                samples[i] = samples[nd2 - i - 1];
                samples[nd2 - i - 1] = tmp;

                tmp = samples[nd2 + i];
                samples[nd2 + i] = samples[nd2 + nd2 - i - 1];
                samples[nd2 + nd2 - i - 1] = tmp;
            }
        }
    }
}
