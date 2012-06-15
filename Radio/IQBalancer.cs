using System;

namespace SDRSharp.Radio
{
    public unsafe class IQBalancer
    {
        private const int FFTBins = 512;
        private const float DcTimeConst = 0.001f;
        private const float BaseIncrement = 0.001f;

        private int _maxAutomaticPasses = Utils.GetIntSetting("automaticIQBalancePasses", 10);
        private bool _autoBalanceIQ = true;
        private float _averageI;
        private float _averageQ;
        private float _gain = 1.0f;
        private float _phase;
        private Complex* _iqPtr;
        private readonly float* _windowPtr;
        private readonly UnsafeBuffer _windowBuffer;
        private readonly Random _rng = new Random();

        public IQBalancer()
        {
            var window = FilterBuilder.MakeWindow(WindowType.Hamming, FFTBins);
            _windowBuffer = UnsafeBuffer.Create(window);
            _windowPtr = (float*)_windowBuffer;
        }

        public float Phase
        {
            get { return (float)Math.Asin(_phase); }
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

        public void Process(Complex* iq, int length)
        {
            if (_autoBalanceIQ && length >= FFTBins)
            {
                _iqPtr = iq;
                RemoveDC(iq, length);
                EstimateImbalance();
                Adjust(iq, length, _phase, _gain);
            }
        }

        private void RemoveDC(Complex* iq, int length)
        {
            for (var i = 0; i < length; i++)
            {
                // I branch
                _averageI = _averageI * (1 - DcTimeConst) + iq[i].Real * DcTimeConst;
                iq[i].Real -= _averageI;

                // Q branch
                _averageQ = _averageQ * (1 - DcTimeConst) + iq[i].Imag * DcTimeConst;
                iq[i].Imag -= _averageQ;
            }
        }

        private void EstimateImbalance()
        {
            var currentUtility = Utility(_phase, _gain);

            for (var count = 0; count < _maxAutomaticPasses; count++)
            {
                var phaseIncrement = BaseIncrement * GetRandomDirection();
                var gainIncrement = BaseIncrement * GetRandomDirection();

                var candidatePhase = _phase + phaseIncrement;
                var candidateGain = _gain + gainIncrement;
                var candidateUtility = Utility(candidatePhase, candidateGain);

                if (candidateUtility > currentUtility)
                {
                    currentUtility = candidateUtility;
                    _gain = candidateGain;
                    _phase = candidatePhase;
                }
            }
        }

        private float GetRandomDirection()
        {
            return (float)(_rng.NextDouble() - 0.5) * 2.0f;
        }

        private float Utility(float phase, float gain)
        {
            var fftPtr = stackalloc Complex[FFTBins];
            var spectrumPtr = stackalloc float[FFTBins];
            Utils.Memcpy(fftPtr, _iqPtr, FFTBins * sizeof(Complex));
            Adjust(fftPtr, FFTBins, phase, gain);
            Fourier.ApplyFFTWindow(fftPtr, _windowPtr, FFTBins);
            Fourier.ForwardTransform(fftPtr, FFTBins);
            Fourier.SpectrumPower(fftPtr, spectrumPtr, FFTBins);

            var result = 0.0f;
            for (var i = 0; i < FFTBins / 2; i++)
            {
                var distanceFromCenter = FFTBins / 2 - i;

                if (distanceFromCenter > 0.05f * FFTBins / 2)
                {
                    result += Math.Abs(spectrumPtr[i] - spectrumPtr[FFTBins - 2 - i]);
                }
            }

            return result;
        }

#if ACCURACY_PRIVILEGED

        private static void Adjust(Complex* buffer, int length, float phase, float gain)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[i].Imag = (float)(buffer[i].Imag * (1.0 + gain) * Math.Cos(phase));
                buffer[i].Real = (float)(buffer[i].Real - buffer[i].Imag * Math.Tan(phase));
            }
        }

#else

        private static void Adjust(Complex* buffer, int length, float phase, float gain)
        {
            for (var i = 0; i < length; i++)
            {
                buffer[i].Real += phase * buffer[i].Imag;
                buffer[i].Imag *= gain;
            }
        }

#endif
    }
}
