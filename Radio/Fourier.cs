#if __MonoCS__
#define MANAGED_ONLY
#endif

using System;
#if !MANAGED_ONLY
using System.Runtime.InteropServices;
#endif

namespace SDRSharp.Radio
{
    public static class Fourier
    {
        public const double MaxPower = 0.0;
        public const double MinPower = -120.0;

#if !MANAGED_ONLY
        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FFT([In, Out] Complex[] coeffs, int len);
#endif

        public static void ForwardTransform(Complex[] buffer)
        {
            ForwardTransform(buffer, buffer.Length);
        }

        public static void ForwardTransform(Complex[] buffer, int length)
        {
#if MANAGED_ONLY
            ManagedForwardTransform(buffer, length);
#else
            NativeForwardTransform(buffer, length);
#endif
        }

        public static void SpectrumPower(Complex[] buffer, double[] power)
        {
            SpectrumPower(buffer, power, buffer.Length);
        }

        public static void SpectrumPower(Complex[] buffer, double[] power, int length)
        {
            if (buffer.Length < length || power.Length < length)
            {
                throw new ArgumentException("The lenghts of arguments do not match");
            }

            for (var i = 0; i < length; i++)
            {
                var strenght = 20.0 * Math.Log10(buffer[i].Modulus());
                if (double.IsNaN(strenght))
                {
                    strenght = MinPower;
                }
                else if (strenght > MaxPower)
                {
                    strenght = MaxPower;
                }
                else if (strenght < MinPower)
                {
                    strenght = MinPower;
                }
                power[i] =  strenght;
            }
        }

        public static void ApplyFFTWindow(Complex[] buffer, double[] window)
        {
            ApplyFFTWindow(buffer, window, buffer.Length);
        }

        public static void ApplyFFTWindow(Complex[] buffer, double[] window, int length)
        {
            if (length > buffer.Length || length > window.Length)
            {
                throw new ArgumentException("Lengths of arguments do not match");
            }

            var m1 = 0.0;
            var m2 = 0.0;
            for (var i = 0; i < length; i++)
            {
                m1 += buffer[i].Modulus();
                buffer[i] *= window[i];
                m2 += buffer[i].Modulus();
            }
            var r = m1 / m2;
            if (!double.IsNaN(r))
            {
                for (var i = 0; i < length; i++)
                {
                    buffer[i] *= r;
                }
            }
        }

#if MANAGED_ONLY

        private unsafe static void ManagedForwardTransform(Complex[] iq, int length)
        {
            fixed (Complex* samples = iq)
            {
                int nm1 = length - 1;
                int nd2 = length / 2;
                int i, j, jm1, k, l, m, le, le2, ip, nd4;
                double ur, ui, sr, si, tr, ti;

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

                    sr = Math.Cos(Math.PI / le2);
                    si = -Math.Sin(Math.PI / le2);

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

#else

        private static void NativeForwardTransform(Complex[] samples, int length)
        {
            FFT(samples, length);
        }

#endif

    }
}
