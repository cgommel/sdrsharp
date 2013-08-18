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
    public unsafe sealed class FmDetector
    {
        private const float NarrowAFGain = 0.5f;
        private const float FMGain = 0.00001f;
        private const float TimeConst = 0.000001f;
        
        private const int MinHissFrequency = 4000;
        private const int MaxHissFrequency = 6000;
        private const int HissFilterOrder = 20;
        private const float HissFactor = 0.00002f;

        private readonly DcRemover* _dcRemoverPtr;
        private readonly UnsafeBuffer _dcRemoverBuffer = UnsafeBuffer.Create(sizeof(DcRemover));
        private float* _hissPtr;
        private UnsafeBuffer _hissBuffer;
        private FirFilter _hissFilter;
        private Complex _iqState;
        private float _noiseLevel;
        private double _sampleRate;
        private float _noiseAveragingRatio;
        private int _squelchThreshold;
        private bool _isSquelchOpen;
        private float _noiseThreshold;
        private FmMode _mode;

        public FmDetector()
        {
            _dcRemoverPtr = (DcRemover*) _dcRemoverBuffer;
            _dcRemoverPtr->Init(TimeConst);
        }

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                // Polar discriminator
                var f = iq[i] * _iqState.Conjugate();

                // Limiting
                var m = f.Modulus();
                if (m > 0.0f)
                {
                    f /= m;
                }

                // Angle estimate
                var a = f.Argument();

                // Scale
                audio[i] = a * FMGain;

                _iqState = iq[i];
            }
            //_dcRemoverPtr->Process(audio, length);
            if (_mode == FmMode.Narrow)
            {
                ProcessSquelch(audio, length);
                for (var i = 0; i < length; i++)
                {
                    audio[i] *= NarrowAFGain;
                }
            }
        }

        private void ProcessSquelch(float* audio, int length)
        {
            if (_squelchThreshold > 0)
            {
                if (_hissBuffer == null || _hissBuffer.Length != length)
                {
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

                _isSquelchOpen = _noiseLevel < _noiseThreshold;
            }
            else
            {
                _isSquelchOpen = true;
            }
        }

        public float Offset
        {
            get { return _dcRemoverPtr->Offset; }
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
                    _noiseAveragingRatio = (float) (30.0 / _sampleRate);
                    var bpk = FilterBuilder.MakeBandPassKernel(_sampleRate, HissFilterOrder, MinHissFrequency, MaxHissFrequency, WindowType.BlackmanHarris);
                    if (_hissFilter != null)
                    {
                        _hissFilter.Dispose();
                    }
                    _hissFilter = new FirFilter(bpk, 1);
                    _dcRemoverPtr->Reset();
                }
            }
        }

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                if (_squelchThreshold != value)
                {
                    _squelchThreshold = value;
                    _noiseThreshold = (float) Math.Log10(2 - _squelchThreshold/100.0) * HissFactor;
                }
            }
        }

        public bool IsSquelchOpen
        {
            get { return _isSquelchOpen; }
        }

        public FmMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }
    }
}
