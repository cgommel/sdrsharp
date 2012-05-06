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
        private const float WideAFGain = 0.00001f;
        private const float TimeConst = 0.000001f;

        private const float DeemphasisTime = 75e-6f; //50e-6f

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
        private float _deemphasisAlpha;
        private float _deemphasisAvg;

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                audio[i] = GetAudio(_iqState, iq[i]);
                _iqState = iq[i];
            }
            _dcRemover.Process(audio, length);
            switch (_mode)
            {
                case FmMode.Narrow:
                    ProcessSquelch(audio, length);
                    break;

                case FmMode.Wide:
                    ProcessDeemphasis(audio, length);
                    break;
            }
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
                    if (_hissBuffer != null)
                    {
                        _hissBuffer.Dispose();
                    }
                    _hissBuffer = UnsafeBuffer.Create(length, sizeof(float));
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

        private void ProcessDeemphasis(float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _deemphasisAvg = (1f - _deemphasisAlpha) * _deemphasisAvg + _deemphasisAlpha * audio[i];
                audio[i] = _deemphasisAvg;
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
                if (value != _sampleRate)
                {
                    _sampleRate = value;
                    _noiseAveragingRatio = (float)(30.0 / _sampleRate);
                    var bpk = FilterBuilder.MakeBandPassKernel(_sampleRate, HissFilterOrder, MinHissFrequency, MaxHissFrequency, WindowType.BlackmanHarris);
                    _hissFilter = new FirFilter(bpk);

                    _deemphasisAlpha = (float)(1.0 - Math.Exp(-1.0 / (_sampleRate * DeemphasisTime)));
                }
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
                if (_mode != value)
                {
                    _mode = value;
                    _afGain = value == FmMode.Narrow ? NarrowAFGain : WideAFGain;
                    _deemphasisAvg = 0;
                }
            }
        }
    }
}
