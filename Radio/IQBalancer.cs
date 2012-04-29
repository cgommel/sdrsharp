using System;
using System.Configuration;

namespace SDRSharp.Radio
{
    public unsafe class IQBalancer : IDisposable
    {
        private const int FFTBins = 1024;
        private const float DcTimeConst = 0.001f;
        private const float Increment = 0.001f;

        private int _maxAutomaticPasses = GetDefaultAutomaticBalancePasses();
        private bool _autoBalanceIQ = true;
        private float _meanI;
        private float _meanQ;
        private float _gain = 1.0f;
        private float _phase;
        private readonly Random _rng = new Random();
        private readonly Complex* _fftPtr;
        private readonly UnsafeBuffer _fftBuffer;
        private readonly float* _spectrumPtr;
        private readonly UnsafeBuffer _spectrumBuffer;
        private readonly float* _windowPtr;
        private readonly UnsafeBuffer _windowBuffer;

        public IQBalancer()
        {
            _fftBuffer = UnsafeBuffer.Create<Complex>(FFTBins);
            _fftPtr = (Complex*) _fftBuffer;

            _spectrumBuffer = UnsafeBuffer.Create<float>(FFTBins);
            _spectrumPtr = (float*) _spectrumBuffer;

            var window = FilterBuilder.MakeWindow(WindowType.Hamming, FFTBins);
            _windowBuffer = UnsafeBuffer.Create(window);
            _windowPtr = (float*) _windowBuffer;
        }

        public void Dispose()
        {
            _fftBuffer.Dispose();
            _spectrumBuffer.Dispose();
            _windowBuffer.Dispose();
        }

        public float Phase
        {
            get { return (float) Math.Asin(_phase); }
        }

        public float Gain
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
            var passesString = ConfigurationManager.AppSettings["automaticIQBalancePasses"];
            int result;
            if (int.TryParse(passesString, out result))
            {
                return result;
            }
            return 50;
        }

        public void Process(Complex* iq, int length)
        {
            if (_autoBalanceIQ)
            {
                RemoveDC(iq, length);
                EstimateImbalance(iq, length);
                Adjust(iq, length, _phase, _gain);
            }
        }

        private void RemoveDC(Complex* iq, int length)
        {
            for (var i = 0; i < length; i++)
            {
                // I branch
                var temp = _meanI * (1 - DcTimeConst) + iq[i].Real * DcTimeConst;
                if (!float.IsNaN(temp))
                {
                    _meanI = temp;
                }
                iq[i].Real = iq[i].Real - _meanI;

                // Q branch
                temp = _meanQ * (1 - DcTimeConst) + iq[i].Imag * DcTimeConst;
                if (!float.IsNaN(temp))
                {
                    _meanQ = temp;
                }
                iq[i].Imag = iq[i].Imag - _meanQ;
            }
        }

        private void EstimateImbalance(Complex* iq, int length)
        {
            if (length < FFTBins)
            {
                return;
            }

            Utils.Memcpy(_fftPtr, iq, FFTBins * sizeof(Complex));
            Adjust(_fftPtr, FFTBins, _phase, _gain);
            Fourier.ApplyFFTWindow(_fftPtr, _windowPtr, FFTBins);
            Fourier.ForwardTransform(_fftPtr, FFTBins);
            Fourier.SpectrumPower(_fftPtr, _spectrumPtr, FFTBins);

            var utility = Utility(_spectrumPtr, FFTBins);

            for (var count = 0; count < _maxAutomaticPasses; count++)
            {
                var gainIncrement = Increment * GetRandomDirection();
                var phaseIncrement = Increment * GetRandomDirection();

                Utils.Memcpy(_fftPtr, iq, FFTBins * sizeof(Complex));
                Adjust(_fftPtr, FFTBins, _phase + phaseIncrement, _gain + gainIncrement);
                Fourier.ApplyFFTWindow(_fftPtr, _windowPtr, FFTBins);
                Fourier.ForwardTransform(_fftPtr, FFTBins);
                Fourier.SpectrumPower(_fftPtr, _spectrumPtr, FFTBins);

                var u = Utility(_spectrumPtr, FFTBins);
                if (u > utility)
                {
                    utility = u;
                    _gain += gainIncrement;
                    _phase += phaseIncrement;
                }
            }
        }

        private float GetRandomDirection()
        {
            return (float) (_rng.NextDouble() - 0.5) * 2.0f;
        }

        private static float Utility(float* spectrum, int length)
        {
            var result = 0.0f;
            var halfLength = length / 2;
            for (var i = 0; i < halfLength; i++)
            {
                var distanceFromCenter = halfLength - i;

                if (distanceFromCenter > 0.05f * halfLength)
                {
                    result += Math.Abs(spectrum[i] - spectrum[length - 2 - i]);
                }
            }

            return result;
        }

        private static void Adjust(Complex* buffer, int length, float phase, float gain)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[i].Real += phase * buffer[i].Imag;
                buffer[i].Imag *= gain;
            }
        }
    }
}
