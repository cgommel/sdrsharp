using System;
using System.Configuration;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class IQBalancer : IDisposable
    {
        private const int FFTBins = 128;
        private const float DcTimeConst = 0.001f;
        private const float Increment = 0.001f;

        private int _maxAutomaticPasses = GetDefaultAutomaticBalancePasses();
        private bool _autoBalanceIQ = true;
        private float _meanI;
        private float _meanQ;
        private float _gain = 1.0f;
        private float _phase;
        private readonly Random _rng = new Random();
        private readonly Complex[] _fft = new Complex[FFTBins];
        private readonly Complex* _fftPtr;
        private readonly GCHandle _fftHandle;
        private readonly float[] _spectrum = new float[FFTBins];
        private readonly float* _spectrumPtr;
        private readonly GCHandle _spectrumHandle;
        private readonly float[] _window = FilterBuilder.MakeWindow(WindowType.Hamming, FFTBins);
        private readonly float* _windowPtr;
        private readonly GCHandle _windowHandle;

        public IQBalancer()
        {
            _fftHandle = GCHandle.Alloc(_fft, GCHandleType.Pinned);
            _fftPtr = (Complex*) _fftHandle.AddrOfPinnedObject();

            _spectrumHandle = GCHandle.Alloc(_spectrum, GCHandleType.Pinned);
            _spectrumPtr = (float*) _spectrumHandle.AddrOfPinnedObject();

            _windowHandle = GCHandle.Alloc(_window, GCHandleType.Pinned);
            _windowPtr = (float*) _windowHandle.AddrOfPinnedObject();
        }

        public void Dispose()
        {
            _fftHandle.Free();
            _spectrumHandle.Free();
            _windowHandle.Free();
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
            var passesString = ConfigurationManager.AppSettings["automaticBalancePasses"];
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
                EstimateImbalance(iq, length, MaxAutomaticPasses);
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

        private void EstimateImbalance(Complex* iq, int length, int passes)
        {
            Utils.Memcpy(_fftPtr, iq, FFTBins * sizeof(Complex));
            Adjust(_fftPtr, FFTBins, _phase, _gain);
            Fourier.ApplyFFTWindow(_fftPtr, _windowPtr, FFTBins);
            Fourier.ForwardTransform(_fftPtr, FFTBins);
            Fourier.SpectrumPower(_fftPtr, _spectrumPtr, FFTBins);

            var utility = Utility(_spectrumPtr, FFTBins);

            var i = 0;
            for (var count = 0; count < passes; count++)
            {
                for (var j = 0; j < 4; j++)
                {
                    var gainIncrement = Increment * GetRandomDirection();
                    var phaseIncrement = Increment * GetRandomDirection();

                    Utils.Memcpy(_fftPtr, iq + i, FFTBins * sizeof(Complex));
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

                i += FFTBins;
                if (i >= length - FFTBins)
                {
                    i = 0;
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
                var distanceFromCenter = Math.Abs(i - halfLength);

                if (distanceFromCenter > 0.05f * halfLength)
                {
                    result += Math.Abs(spectrum[i] - spectrum[length - 2 - i]);
                }
            }

            return result;
        }

        private static void Adjust(Complex* buffer, int length, float phase, float gain)
        {
            var m1 = 0.0f;
            var m2 = 0.0f;
            for (var i = 0; i < length; i++)
            {
                m1 += buffer[i].Modulus();
                buffer[i].Real += phase * buffer[i].Imag;
                buffer[i].Imag *= gain;
                m2 += buffer[i].Modulus();
            }
            var r = m1 / m2;
            if (!float.IsNaN(r))
            {
                for (var i = 0; i < length; i++)
                {
                    buffer[i] *= r;
                }
            }
        }
    }
}
