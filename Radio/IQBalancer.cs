using System;
using System.Configuration;

namespace SDRSharp.Radio
{
    public class IQBalancer
    {
        private const int FFTBins = 128;
        private const double DcTimeConst = 0.001;
        private const double Increment = 0.001;

        private int _maxAutomaticPasses = GetDefaultAutomaticBalancePasses();
        private bool _autoBalanceIQ = true;
        private double _meanI;
        private double _meanQ;
        private double _gain = 1.0;
        private double _phase;
        private readonly Random _rng = new Random();
        private readonly Complex[] _fft = new Complex[FFTBins];
        private readonly double[] _spectrum = new double[FFTBins];
        private readonly double[] _fftWindow = FilterBuilder.MakeWindow(WindowType.Hamming, FFTBins);

        public double Phase
        {
            get { return _phase; }
        }

        public double Gain
        {
            get { return _gain; }
        }

        public int MaxAutomaticPasses
        {
            get { return _maxAutomaticPasses; }
            set { _maxAutomaticPasses = value; }
        }

        public bool AutoBalanceIQ
        {
            get { return _autoBalanceIQ; }
            set { _autoBalanceIQ = value; }
        }

        private static int GetDefaultAutomaticBalancePasses()
        {
            var passesString = ConfigurationManager.AppSettings["automaticBalancePasses"];
            int result;
            if (int.TryParse(passesString, out result))
            {
                return result;
            }
            return 50;
        }

        public void Process(Complex[] iq)
        {
            if (_autoBalanceIQ)
            {
                RemoveDC(iq);
                EstimateImbalance(iq, MaxAutomaticPasses);
                Adjust(iq, _phase, _gain);
            }
        }

        private void RemoveDC(Complex[] iq)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                // I branch
                var temp = _meanI * (1 - DcTimeConst) + iq[i].Real * DcTimeConst;
                if (!double.IsNaN(temp))
                {
                    _meanI = temp;
                }
                iq[i].Real = iq[i].Real - _meanI;

                // Q branch
                temp = _meanQ * (1 - DcTimeConst) + iq[i].Imag * DcTimeConst;
                if (!double.IsNaN(temp))
                {
                    _meanQ = temp;
                }
                iq[i].Imag = iq[i].Imag - _meanQ;
            }
        }

        private void EstimateImbalance(Complex[] iq, int passes)
        {
            Array.Copy(iq, _fft, _fft.Length);
            Adjust(_fft, _phase, _gain);
            Fourier.ApplyFFTWindow(_fft, _fftWindow);
            Fourier.ForwardTransform(_fft);
            Fourier.SpectrumPower(_fft, _spectrum);

            var current = Utility(_spectrum);
            var utility = current;

            var i = 0;
            for (var count = 0; count < passes; count++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var gainIncrement = Increment * GetRandomDirection();
                    var phaseIncrement = Increment * GetRandomDirection();

                    Array.Copy(iq, i, _fft, 0, _fft.Length);
                    Adjust(_fft, _phase + phaseIncrement, _gain + gainIncrement);
                    Fourier.ApplyFFTWindow(_fft, _fftWindow);
                    Fourier.ForwardTransform(_fft);
                    Fourier.SpectrumPower(_fft, _spectrum);

                    var u = Utility(_spectrum);
                    if (u > utility)
                    {
                        utility = u;
                        _gain += gainIncrement;
                        _phase += phaseIncrement;
                    }
                }

                i += _fft.Length;
                if (i >= iq.Length - _fft.Length)
                {
                    i = 0;
                }
            }
        }

        private double GetRandomDirection()
        {
            return (_rng.NextDouble() - 0.5) * 2.0;
        }

        private static double Utility(double[] spectrum)
        {
            var result = 0.0;
            var halfLength = spectrum.Length / 2;
            for (var i = 0; i < halfLength; i++)
            {
                var distanceFromCenter = Math.Abs(i - halfLength);

                if (distanceFromCenter / (double) halfLength > 0.05)
                {
                    result += Math.Abs(spectrum[i] - spectrum[spectrum.Length - 2 - i]);
                }
            }

            return result;
        }

        private static void Adjust(Complex[] buffer, double phase, double gain)
        {
            var m1 = 0.0;
            var m2 = 0.0;
            for (var i = 0; i < buffer.Length; i++)
            {
                m1 += buffer[i].Modulus();
                buffer[i].Real += Math.Sin(phase) * buffer[i].Imag;
                buffer[i].Imag *= gain;
                m2 += buffer[i].Modulus();
            }
            var r = m1 / m2;
            if (!double.IsNaN(r))
            {
                for (var i = 0; i < buffer.Length; i++)
                {
                    buffer[i] *= r;
                }
            }
        }
    }
}
