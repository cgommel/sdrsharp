using System;
using System.Configuration;

namespace SDRSharp.Radio
{
    public class IQBalancer
    {
        private const int FFTBins = 128;
        private const double DcTimeConst = 0.00001;
        private const double Increment = 0.0001;

        private int _maxPasses = 5000;
        private int _maxAutomaticPasses = GetDefaultAutomaticBalancePasses();
        private bool _balanceIQ;
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

        public int MaxPasses
        {
            get { return _maxPasses; }
            set { _maxPasses = value; }
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

        public event EventHandler ImbalanceEstimationSucceeded;

        public event EventHandler ImbalanceEstimationFailed;

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
            //Adjust(iq, 0.1, 0.25); // Uncomment for testing
            RemoveDC(iq);

            if (_balanceIQ)
            {
                EstimateImbalanceWithEvents(iq, _maxPasses);
                _balanceIQ = false;
            }
            else if (_autoBalanceIQ)
            {
                EstimateImbalance(iq, MaxAutomaticPasses);
            }
            Adjust(iq, _phase, _gain);
        }

        public void BalanceIQ()
        {
            _balanceIQ = true;
        }

        private void RemoveDC(Complex[] iq)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                // I branch
                var temp = _meanI * (1 - DcTimeConst) + iq[i].Real * DcTimeConst;
                if (!double.IsNaN(temp) && !double.IsInfinity(temp))
                {
                    _meanI = temp;
                }
                iq[i].Real = iq[i].Real - _meanI;

                // Q branch
                temp = _meanQ * (1 - DcTimeConst) + iq[i].Imag * DcTimeConst;
                if (!double.IsNaN(temp) && !double.IsInfinity(temp))
                {
                    _meanQ = temp;
                }
                iq[i].Imag = iq[i].Imag - _meanQ;
            }
        }

        private bool EstimateImbalance(Complex[] iq, int passes)
        {
            Array.Copy(iq, _fft, _fft.Length);
            Adjust(_fft, _phase, _gain);
            Fourier.ApplyFFTWindow(_fft, _fftWindow);
            Fourier.ForwardTransform(_fft);
            Fourier.SpectrumPower(_fft, _spectrum);

            var current = Utility(_spectrum);
            var utility = current;

            for (var count = 0; count < passes; count++)
            {
                var gainIncrement = Increment * GetRandomDirection();
                var phaseIncrement = Increment * GetRandomDirection();

                Array.Copy(iq, _fft, _fft.Length);
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

            return utility > current;
        }

        private void EstimateImbalanceWithEvents(Complex[] iq, int passes)
        {
            if (EstimateImbalance(iq, passes))
            {
                if (ImbalanceEstimationSucceeded != null)
                {
                    ImbalanceEstimationSucceeded(this, EventArgs.Empty);
                }
            }
            else
            {
                if (ImbalanceEstimationFailed != null)
                {
                    ImbalanceEstimationFailed(this, EventArgs.Empty);
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
