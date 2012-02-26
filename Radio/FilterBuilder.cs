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

        public static float[] MakeWindow(WindowType windowType, int length)
        {
            var w = new float[length];
            length--;
            for (var i = 0; i <= length; i++)
            {
                float n;
                float a0;
                float a1;
                float a2;
                float a3;
                float alpha;

                w[i] = 1.0f;
                
                switch (windowType)
                {
                    case WindowType.Hamming:
                        a0 = 0.54f;
                        a1 = 0.46f;
                        a2 = 0.0f;
                        a3 = 0.0f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.Blackman:
                        a0 = 0.42f;
                        a1 = 0.5f;
                        a2 = 0.08f;
                        a3 = 0.0f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.BlackmanHarris:
                        a0 = 0.35875f;
                        a1 = 0.48829f;
                        a2 = 0.14128f;
                        a3 = 0.01168f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.HannPoisson:
                        n = i - length / 2.0f;
                        alpha = 0.005f;
                        w[i] *= 0.5f * (float) ((1.0 + Math.Cos(2.0 * Math.PI * n / length)) * Math.Exp(-2.0 * alpha * Math.Abs(n) / length));
                        break;

                    case WindowType.Youssef:
                        a0 = 0.35875f;
                        a1 = 0.48829f;
                        a2 = 0.14128f;
                        a3 = 0.01168f;
                        n = i - length / 2.0f;
                        alpha = 0.005f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length);
                        w[i] *= (float) Math.Exp(-2.0 * alpha * Math.Abs(n) / length);
                        break;
                }
            }
            return w;
        }

        public static float[] MakeLowPassKernel(double sampleRate, int filterOrder, int cutoffFrequency, WindowType windowType)
        {
            var cutoffRadians = 2 * Math.PI * cutoffFrequency / sampleRate;
            
            var h = new float[filterOrder + 1];
            var w = MakeWindow(windowType, filterOrder + 1);

            for (var i = 0; i <= filterOrder; i++)
            {
                var n = i - filterOrder / 2;
                if (n == 0)
                {
                    h[i] = (float) cutoffRadians;
                }
                else
                {
                    h[i] = (float) (Math.Sin(cutoffRadians * n) / n) * w[i];
                }
            }
            Normalize(h);
            return h;
        }

        public static float[] MakeHighPassKernel(double sampleRate, int filterOrder, int cutoffFrequency, WindowType windowType)
        {
            return InvertSpectrum(MakeLowPassKernel(sampleRate, filterOrder, cutoffFrequency, windowType));
        }

        public static float[] MakeBandPassKernel(double sampleRate, int filterOrder, int cutoff1, int cutoff2, WindowType windowType)
        {
            var bw = (cutoff2 - cutoff1) / 2;
            var fshift = cutoff2 - bw;
            var shiftRadians = 2 * Math.PI * fshift / sampleRate;

            var h = MakeLowPassKernel(sampleRate, filterOrder, bw, windowType);

            for (var i = 0; i < h.Length; i++)
            {
                var n = i - filterOrder / 2;
                h[i] *= (float) (2 * Math.Cos(shiftRadians * n));
            }
            return h;
        }

        #region Utility functions

        public static void Normalize(float[] h)
        {
            // Normalize the filter kernel for unity gain at DC
            var sum = 0.0f;
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
        private static float[] InvertSpectrum(float[] h)
        {
            for (var i = 0; i < h.Length; i++)
            {
                h[i] = -h[i];
            }
            h[(h.Length - 1) / 2] += 1.0f;
            return h;
        }

        #endregion
    }
}
