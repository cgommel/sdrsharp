using System;

namespace SDRSharp.Radio
{
    public enum WindowType
    {
        None,
        Hamming,
        Blackman,
        BlackmanHarris,
        HannPoisson,
        Youssef
    }

    public static class FilterBuilder
    {
        public const int DefaultFilterOrder = 500;

        public static double[] MakeWindow(WindowType windowType, int length)
        {
            var w = new double[length];
            for (var i = 0; i < length; i++)
            {
                double n;
                double a0;
                double a1;
                double a2;
                double a3;
                double alpha;

                w[i] = 1.0;
                
                switch (windowType)
                {
                    case WindowType.Hamming:
                        a0 = 0.54;
                        a1 = 0.46;
                        a2 = 0.0;
                        a3 = 0.0;
                        w[i] *= a0
                              - a1 * Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.Blackman:
                        a0 = 0.42;
                        a1 = 0.5;
                        a2 = 0.08;
                        a3 = 0.0;
                        w[i] *= a0
                              - a1 * Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.BlackmanHarris:
                        a0 = 0.35875;
                        a1 = 0.48829;
                        a2 = 0.14128;
                        a3 = 0.01168;
                        w[i] *= a0
                              - a1 * Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.HannPoisson:
                        n = i - length / 2.0;
                        alpha = 0.005;
                        w[i] *= 0.5 * (1.0 + Math.Cos(2.0 * Math.PI * n / length)) * Math.Exp(-2.0 * alpha * Math.Abs(n) / length);
                        break;

                    case WindowType.Youssef:
                        a0 = 0.35875;
                        a1 = 0.48829;
                        a2 = 0.14128;
                        a3 = 0.01168;
                        n = i - length / 2.0;
                        alpha = 0.005;
                        w[i] *= a0
                              - a1 * Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * Math.Cos(6.0 * Math.PI * i / length);
                        w[i] *= Math.Exp(-2.0 * alpha * Math.Abs(n) / length);
                        break;
                }
            }
            return w;
        }

        public static double[] MakeLowPassKernel(int sampleRate, int filterOrder, int cutoffFrequency, WindowType windowType)
        {
            var cutoffRadians = 2 * Math.PI * cutoffFrequency / sampleRate;
            
            var h = new double[filterOrder + 1];
            var w = MakeWindow(windowType, filterOrder + 1);

            for (var i = 0; i <= filterOrder; i++)
            {
                var n = i - filterOrder / 2.0;
                if (n == 0)
                {
                    h[i] = cutoffRadians;
                }
                else
                {
                    h[i] = Math.Sin(cutoffRadians * n) / n;
                    h[i] *= w[i];
                }
            }
            Normalize(h);
            return h;
        }

        public static double[] MakeHighPassKernel(int sampleRate, int filterOrder, int cutoffFrequency, WindowType windowType)
        {
            return InvertSpectrum(MakeLowPassKernel(sampleRate, filterOrder, cutoffFrequency, windowType));
        }

        public static double[] MakeBandPassKernel(int sampleRate, int filterOrder, int cutoff1, int cutoff2, WindowType windowType)
        {
            var bw = (cutoff2 - cutoff1) / 2;
            var fshift = cutoff2 - bw;
            var shiftRadians = 2 * Math.PI * fshift / sampleRate;

            var h = MakeLowPassKernel(sampleRate, filterOrder, bw, windowType);

            for (var i = 0; i < h.Length; i++)
            {
                var n = i - filterOrder / 2;
                h[i] *= 2 * Math.Cos(shiftRadians * n);
            }
            return h;
        }

        #region Utility functions

        private static void Normalize(double[] h)
        {
            // Normalize the filter kernel for unity gain at DC
            var sum = 0.0;
            for (var i = 0; i < h.Length; i++)
            {
                sum += h[i];
            }
            for (var i = 0; i < h.Length; i++)
            {
                h[i] /= sum;
            }
        }

        // See the bottom of
        // http://www.dspguide.com/ch14/4.htm
        // for an explanation of spectral inversion
        private static double[] InvertSpectrum(double[] h)
        {
            for (var i = 0; i < h.Length; i++)
            {
                h[i] = -h[i];
            }
            h[(h.Length - 1) / 2] += 1.0;
            return h;
        }

        #endregion
    }
}
