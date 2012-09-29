using System;

namespace SDRSharp.Radio
{
    public unsafe class IQBalancer
    {
        private const int FFTBins = 1024;
        private const float DcTimeConst = 0.001f;
        private const float BaseIncrement = 0.001f;
        private const float PowerThreshold = 20.0f; // in dB

        private DcRemover _dcRemoverI = new DcRemover(DcTimeConst);
        private DcRemover _dcRemoverQ = new DcRemover(DcTimeConst);
        private float _gain = 1.0f;
        private float _phase;
        private float _averagePower;
        private float _powerRange;
        private Complex* _iqPtr;
        private int _maxAutomaticPasses = Utils.GetIntSetting("automaticIQBalancePasses", 10);
        private bool _autoBalanceIQ;
        private readonly bool _isMultithreaded;
        private readonly float* _windowPtr;
        private readonly UnsafeBuffer _windowBuffer;
        private readonly Random _rng = new Random();
        private readonly SharpEvent _event = new SharpEvent(false);

        public IQBalancer()
        {
            var window = FilterBuilder.MakeWindow(WindowType.Hamming, FFTBins);
            _windowBuffer = UnsafeBuffer.Create(window);
            _windowPtr = (float*) _windowBuffer;
            _isMultithreaded = Environment.ProcessorCount > 1;
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

        public void Reset()
        {
            _phase = 0.0f;
            _gain = 1.0f;
            _dcRemoverI.Reset();
            _dcRemoverQ.Reset();
        }

        public void Process(Complex* iq, int length)
        {
            if (_autoBalanceIQ && length >= FFTBins)
            {
                RemoveDC(iq, length);
                _iqPtr = iq;
                EstimateImbalance();
                Adjust(iq, length, _phase, _gain);
            }
        }

        private void RemoveDC(Complex* iq, int length)
        {
            var iPtr = (float*) iq;
            var qPtr = iPtr + 1;

            if (_isMultithreaded)
            {
                DSPThreadPool.QueueUserWorkItem(
                    delegate
                    {
                        // I branch
                        _dcRemoverI.ProcessInterleaved(iPtr, length);
                        _event.Set();
                    });
            }
            else
            {
                // I branch
                _dcRemoverI.ProcessInterleaved(iPtr, length);
            }

            // Q branch
            _dcRemoverQ.ProcessInterleaved(qPtr, length);

            if (_isMultithreaded)
            {
                _event.WaitOne();
            }
        }

        private void EstimateImbalance()
        {
            EstimatePower();
            if (_powerRange < PowerThreshold)
            {
                return;
            }

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
            return _rng.NextDouble() > 0.5 ? 1f : -1f;
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
            const int halfBins = FFTBins / 2;
            for (var i = 0; i < halfBins; i++)
            {
                var distanceFromCenter = halfBins - i;

                if (distanceFromCenter > 0.05f * halfBins && distanceFromCenter < 0.95f * halfBins)
                {
                    var j = FFTBins - 2 - i;
                    if (spectrumPtr[i] - _averagePower > PowerThreshold || spectrumPtr[j] - _averagePower > PowerThreshold)
                    {
                        var distance = spectrumPtr[i] - spectrumPtr[j];
                        result += distance * distance;
                    }
                }
            }

            return result;
        }

        private void EstimatePower()
        {
            var fftPtr = stackalloc Complex[FFTBins];
            var spectrumPtr = stackalloc float[FFTBins];
            Utils.Memcpy(fftPtr, _iqPtr, FFTBins * sizeof(Complex));
            Fourier.ApplyFFTWindow(fftPtr, _windowPtr, FFTBins);
            Fourier.ForwardTransform(fftPtr, FFTBins);
            Fourier.SpectrumPower(fftPtr, spectrumPtr, FFTBins);

            const int halfBins = FFTBins / 2;
            var max = float.NegativeInfinity;
            var avg = 0f;
            var count = 0;
            for (var i = 0; i < halfBins; i++)
            {
                var distanceFromCenter = halfBins - i;

                if (distanceFromCenter > 0.05f * halfBins && distanceFromCenter < 0.95f * halfBins)
                {
                    var j = FFTBins - 2 - i;

                    if (spectrumPtr[i] > max)
                    {
                        max = spectrumPtr[i];
                    }

                    if (spectrumPtr[j] > max)
                    {
                        max = spectrumPtr[j];
                    }

                    avg += spectrumPtr[i] + spectrumPtr[j];
                    count += 2;
                }
            }
            avg /= count;

            _powerRange = max - avg;
            _averagePower = avg;
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
                buffer[i].Real = buffer[i].Real * gain + phase * buffer[i].Imag;
            }
        }

#endif
    }
}
