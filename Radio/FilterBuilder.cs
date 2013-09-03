using System;

namespace SDRSharp.Radio
{
    public enum WindowType
    {
        None,
        Hamming,
        Blackman,
        BlackmanHarris4,
        BlackmanHarris7,
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
                float a4;
                float a5;
                float a6;
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

                    case WindowType.BlackmanHarris4:
                        a0 = 0.35875f;
                        a1 = 0.48829f;
                        a2 = 0.14128f;
                        a3 = 0.01168f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length);
                        break;

                    case WindowType.BlackmanHarris7:
                        a0 = 0.27105140069342f;
                        a1 = 0.43329793923448f;
                        a2 = 0.21812299954311f;
                        a3 = 0.06592544638803f;
                        a4 = 0.01081174209837f;
                        a5 = 0.00077658482522f;
                        a6 = 0.00001388721735f;
                        w[i] *= a0
                              - a1 * (float) Math.Cos(2.0 * Math.PI * i / length)
                              + a2 * (float) Math.Cos(4.0 * Math.PI * i / length)
                              - a3 * (float) Math.Cos(6.0 * Math.PI * i / length)
                              + a4 * (float) Math.Cos(8.0 * Math.PI * i / length)
                              - a5 * (float) Math.Cos(10.0 * Math.PI * i / length)
                              + a6 * (float) Math.Cos(12.0 * Math.PI * i / length);
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

        public static float[] MakeSinc(double sampleRate, double frequency, int length)
        {
            if (length % 2 == 0)
            {
                throw new ArgumentException("Length should be odd", "length");
            }

            var freqInRad = 2.0 * Math.PI * frequency / sampleRate;
            var h = new float[length];

            for (var i = 0; i < length; i++)
            {
                var n = i - length / 2;
                if (n == 0)
                {
                    h[i] = (float) freqInRad;
                }
                else
                {
                    h[i] = (float) (Math.Sin(freqInRad * n) / n);
                }
            }

            return h;
        }

        public static float[] MakeSin(double sampleRate, double frequency, int length)
        {
            if (length % 2 == 0)
            {
                throw new ArgumentException("Length should be odd", "length");
            }

            var freqInRad = 2.0 * Math.PI * frequency / sampleRate;
            var h = new float[length];

            var halfLength = length / 2;
            for (var i = 0; i <= halfLength; i++)
            {
                var y = (float) Math.Sin(freqInRad * i);
                h[halfLength + i] = y;
                h[halfLength - i] = -y;
            }

            return h;
        }

        public static float[] MakeLowPassKernel(double sampleRate, int filterOrder, int cutoffFrequency, WindowType windowType)
        {
            filterOrder |= 1;

            var h = MakeSinc(sampleRate, cutoffFrequency, filterOrder);
            var w = MakeWindow(windowType, filterOrder);

            ApplyWindow(h, w);

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

        public static void ApplyWindow(float[] coefficients, float[] window)
        {
            for (var i = 0; i < coefficients.Length; i++)
            {
                coefficients[i] *= window[i];
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
