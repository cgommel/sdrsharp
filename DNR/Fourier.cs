using System;
using SDRSharp.Radio;

namespace SDRSharp.DNR
{
    public unsafe static class FourierDNR
    {
        public static void ForwardTransform(Complex* samples, int length)
        {
            int nm1 = length - 1;
            int nd2 = length / 2;
            int i, j, jm1, k, l, m, le, le2, ip;
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
        }

        public static void InverseTransform(Complex* samples, int length)
        {
            for (int i = 0; i < length; i++)
            {
                samples[i].Imag = -samples[i].Imag;
            }

            ForwardTransform(samples, length);

            var factor = 1.0f / length;

            for (int i = 0; i < length; i++)
            {
                samples[i].Real = samples[i].Real * factor;
                samples[i].Imag = -samples[i].Imag * factor;
            }
        }
    }
}
