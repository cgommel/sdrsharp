using System;

namespace SDRSharp.Radio
{
    public enum FmMode
    {
        Narrow,
        Wide
    }

    /// <summary>
    /// The theory behind this code is in section 4.2.1
    /// of http://www.digitalsignallabs.com/Digradio.pdf
    /// </summary>
    public unsafe class FmDetector
    {
        private const float NarrowAFGain = 0.00001f;
        private const float WideAFGain = 0.000001f;
        private const float TimeConst = 0.000001f;

        private const int MinHissFrequency = 4000;
        private const int MaxHissFrequency = 6000;
        private const int HissFilterOrder = 20;
        private const float HissFactor = 0.00002f;

        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);
        private float* _hissPtr;
        private UnsafeBuffer _hissBuffer;
        private FirFilter _hissFilter;
        private Complex _iqState;
        private float _noiseLevel;
        private double _sampleRate;
        private float _noiseAveragingRatio;
        private int _squelchThreshold;
        private float _noiseThreshold;
        private FmMode _mode;
        private float _afGain = NarrowAFGain;

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                audio[i] = GetAudio(_iqState, iq[i]);
                _iqState = iq[i];
            }
            _dcRemover.Process(audio, length);
            ProcessSquelch(audio, length);
        }

        public float GetAudio(Complex previous, Complex current)
        {
            // Polar discriminator
            var f = current * previous.Conjugate();

            // Limiting
            var m = f.Modulus();
            if (m > 0.0)
            {
                f /= m;
            }

            // Angle estimate
            var a = f.Argument();

            return a * _afGain;
        }

        private void ProcessSquelch(float* audio, int length)
        {
            if (_squelchThreshold > 0)
            {
                if (_hissBuffer == null || _hissBuffer.Length != length)
                {
                    _hissBuffer = UnsafeBuffer.Create<float>(length);
                    _hissPtr = (float*) _hissBuffer;
                }

                Utils.Memcpy(_hissPtr, audio, length * sizeof(float));

                _hissFilter.Process(_hissPtr, length);

                for (var i = 0; i < _hissBuffer.Length; i++)
                {
                    var n = (1 - _noiseAveragingRatio) * _noiseLevel + _noiseAveragingRatio * Math.Abs(_hissPtr[i]);
                    if (!float.IsNaN(n))
                    {
                        _noiseLevel = n;
                    }
                    if (_noiseLevel > _noiseThreshold)
                    {
                        audio[i] = 0.0f;
                    }
                }
            }
        }

        public float Offset
        {
            get { return _dcRemover.Offset; }
        }

        public double SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
                _noiseAveragingRatio = (float) (30.0 / _sampleRate);
                var bpk = FilterBuilder.MakeBandPassKernel(_sampleRate, HissFilterOrder, MinHissFrequency, MaxHissFrequency, WindowType.BlackmanHarris);
                _hissFilter = new FirFilter(bpk);
            }
        }

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                _squelchThreshold = value;
                _noiseThreshold = (float) Math.Log10(2 - _squelchThreshold / 100.0) * HissFactor;
            }
        }

        public FmMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                _afGain = value == FmMode.Narrow ? NarrowAFGain : WideAFGain;
            }
        }
    }
}
