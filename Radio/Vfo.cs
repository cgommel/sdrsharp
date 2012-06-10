using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class Vfo
    {
        private const float TimeConst = 0.01f;
        public const int DefaultCwSideTone = 600;
        public const int DefaultSSBBandwidth = 2400;
        public const int DefaultWFMBandwidth = 180000;
        public const int MinSSBAudioFrequency = 100;
        public const int MinBCAudioFrequency = 20;
        public const int MaxBCAudioFrequency = 16000;
        public const int MaxQuadratureFilterOrder = 300;
        public const int MaxNFMBandwidth = 15000;
        public const int MinNFMAudioFrequency = 300;

        private readonly AutomaticGainControl _agc = new AutomaticGainControl();
        private readonly Oscillator _localOscillator = new Oscillator();
        private readonly AmDetector _amDetector = new AmDetector();
        private readonly FmDetector _fmDetector = new FmDetector();
        private readonly LsbDetector _lsbDetector = new LsbDetector();
        private readonly UsbDetector _usbDetector = new UsbDetector();
        private readonly DsbDetector _dsbDetector = new DsbDetector();
        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);
        private readonly StereoDecoder _stereoDecoder = new StereoDecoder();
        private IQDecimator _baseBandDecimator;
        private FirFilter _audioFilter;
        private IQFirFilter _iqFilter;
        private DetectorType _detectorType;
        private WindowType _windowType;
        private double _sampleRate;
        private int _bandwidth;
        private int _frequency;
        private int _filterOrder;
        private bool _needNewFilters;
        private int _decimationStageCount;
        private int _baseBandDecimationStageCount;
        private int _audioDecimationStageCount;
        private bool _needNewDecimators;
        private int _cwToneShift;
        private bool _needConfigure;
        private bool _useAgc;
        private float _agcThreshold;
        private float _agcDecay;
        private float _agcSlope;
        private bool _agcUseHang;
        private int _fmSquelchThreshold;
        private bool _fmStereo = true;
        private UnsafeBuffer _rawAudioBuffer;
        private float* _rawAudioPtr;

        public Vfo()
        {
            _bandwidth = DefaultSSBBandwidth;
            _filterOrder = FilterBuilder.DefaultFilterOrder;
            _needConfigure = true;
        }

        public DetectorType DetectorType
        {
            get
            {
                return _detectorType;
            }
            set
            {
                if (value != _detectorType)
                {
                    _needNewDecimators = (_detectorType == DetectorType.WFM && value != DetectorType.WFM) ||
                                         (_detectorType != DetectorType.WFM && value == DetectorType.WFM);
                    _detectorType = value;
                    _needConfigure = true;
                }
            }
        }

        public int Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    _needConfigure = true;
                }
            }
        }

        public int FilterOrder
        {
            get
            {
                return _filterOrder;
            }
            set
            {
                if (_filterOrder != value)
                {
                    _filterOrder = value;
                    _needNewFilters = true;
                    _needConfigure = true;
                }
            }
        }

        public double SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    _needNewDecimators = true;
                    _needConfigure = true;
                }
            }
        }

        public WindowType WindowType
        {
            get
            {
                return _windowType;
            }
            set
            {
                if (_windowType != value)
                {
                    _windowType = value;
                    _needNewFilters = true;
                    _needConfigure = true;
                }
            }
        }

        public int Bandwidth
        {
            get
            {
                return _bandwidth;
            }
            set
            {
                if (_bandwidth != value)
                {
                    _bandwidth = value;
                    _needNewFilters = true;
                    _needConfigure = true;
                }
            }
        }

        public bool UseAGC
        {
            get { return _useAgc; }
            set { _useAgc = value; }
        }

        public float AgcThreshold
        {
            get { return _agcThreshold; }
            set
            {
                if (_agcThreshold != value)
                {
                    _agcThreshold = value;
                    _needConfigure = true;
                }
            }
        }

        public float AgcDecay
        {
            get { return _agcDecay; }
            set
            {
                if (_agcDecay != value)
                {
                    _agcDecay = value;
                    _needConfigure = true;
                }
            }
        }

        public float AgcSlope
        {
            get { return _agcSlope; }
            set
            {
                if (_agcSlope != value)
                {
                    _agcSlope = value;
                    _needConfigure = true;
                }
            }
        }

        public bool AgcHang
        {
            get { return _agcUseHang; }
            set
            {
                if (_agcUseHang != value)
                {
                    _agcUseHang = value;
                    _needConfigure = true;
                }
            }
        }

        public int FmSquelch
        {
            get { return _fmSquelchThreshold; }
            set
            {
                if (_fmSquelchThreshold != value)
                {
                    _fmSquelchThreshold = value;
                    _needConfigure = true;
                }
            }
        }

        public int DecimationStageCount
        {
            get { return _decimationStageCount; }
            set
            {
                if (_decimationStageCount != value)
                {
                    _decimationStageCount = value;
                    _needNewDecimators = true;
                    _needConfigure = true;
                }
            }
        }

        public int CWToneShift
        {
            get { return _cwToneShift; }
            set
            {
                if (_cwToneShift != value)
                {
                    _cwToneShift = value;
                    _needNewFilters = true;
                    _needConfigure = true;
                }
            }
        }

        public bool FmStereo
        {
            get { return _fmStereo; }
            set
            {
                if (_fmStereo != value)
                {
                    _fmStereo = value;
                    _needConfigure = true;
                }
            }
        }

        public bool SignalIsStereo
        {
            get { return _detectorType == DetectorType.WFM && _fmStereo && _stereoDecoder.IsPllLocked; }
        }

        private void Configure()
        {
            _localOscillator.SampleRate = _sampleRate;
            _localOscillator.Frequency = _frequency;
            if (_needNewDecimators)
            {
                _needNewDecimators = false;
                if (_detectorType == DetectorType.WFM)
                {
                    var afSamplerate = _sampleRate / Math.Pow(2.0, _decimationStageCount);
                    _audioDecimationStageCount = 0;
                    while (afSamplerate * Math.Pow(2.0, _audioDecimationStageCount) < DefaultWFMBandwidth && _audioDecimationStageCount < _decimationStageCount)
                    {
                        _audioDecimationStageCount++;
                    }
                    _baseBandDecimationStageCount = _decimationStageCount - _audioDecimationStageCount;
                }
                else
                {
                    _baseBandDecimationStageCount = _decimationStageCount;
                    _audioDecimationStageCount = 0;
                }
                if (_baseBandDecimator != null)
                {
                    _baseBandDecimator.Dispose();
                }
                _baseBandDecimator = new IQDecimator(_baseBandDecimationStageCount, _sampleRate);
                _needNewFilters = true;
            }
            if (_needNewFilters)
            {
                _needNewFilters = false;
                InitFilters();
            }
            var baseBandSampleRate = _sampleRate / Math.Pow(2.0, _baseBandDecimationStageCount);
            _usbDetector.SampleRate = baseBandSampleRate;
            _lsbDetector.SampleRate = baseBandSampleRate;
            _fmDetector.SampleRate = baseBandSampleRate;
            _fmDetector.SquelchThreshold = _fmSquelchThreshold;
            _stereoDecoder.Configure(_fmDetector.SampleRate, _audioDecimationStageCount);
            _stereoDecoder.ForceMono = !_fmStereo;
            switch (_detectorType)
            {
                case DetectorType.USB:
                    _usbDetector.BfoFrequency = -_bandwidth / 2;
                    _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                    break;

                case DetectorType.LSB:
                    _lsbDetector.BfoFrequency = -_bandwidth / 2;
                    _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                    break;

                case DetectorType.CWU:
                    _usbDetector.BfoFrequency = -_cwToneShift;
                    _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                    break;

                case DetectorType.CWL:
                    _lsbDetector.BfoFrequency = -_cwToneShift;
                    _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                    break;

                case DetectorType.NFM:
                    _fmDetector.Mode = FmMode.Narrow;
                    break;

                case DetectorType.WFM:
                    _fmDetector.Mode = FmMode.Wide;
                    break;
            }

            _agc.SampleRate = _sampleRate / Math.Pow(2.0, _decimationStageCount);
            _agc.Decay = _agcDecay;
            _agc.Slope = _agcSlope;
            _agc.Threshold = _agcThreshold;
            _agc.UseHang = _agcUseHang;
        }

        private void InitFilters()
        {
            int cutoff1 = 0;
            int cutoff2 = 10000;
            var iqBW = _bandwidth / 2;
            int iqOrder = _detectorType == DetectorType.WFM ? 30 : Math.Min(_filterOrder, MaxQuadratureFilterOrder);
            
            var coeffs = FilterBuilder.MakeLowPassKernel(_sampleRate / Math.Pow(2.0, _baseBandDecimationStageCount), iqOrder, iqBW, _windowType);
            if (_iqFilter == null)
            {
                _iqFilter = new IQFirFilter(coeffs);
            }
            else
            {
                _iqFilter.SetCoefficients(coeffs);
            }

            switch (_detectorType)
            {
                case DetectorType.AM:
                    cutoff1 = MinBCAudioFrequency;
                    cutoff2 = Math.Min(_bandwidth / 2, MaxBCAudioFrequency);
                    break;

                case DetectorType.CWU:
                case DetectorType.CWL:
                    cutoff1 = _cwToneShift - _bandwidth / 2;
                    cutoff2 = _cwToneShift + _bandwidth / 2;
                    break;

                case DetectorType.USB:
                case DetectorType.LSB:
                    cutoff1 = MinSSBAudioFrequency;
                    cutoff2 = _bandwidth;
                    break;

                case DetectorType.DSB:
                    cutoff1 = MinSSBAudioFrequency;
                    cutoff2 = _bandwidth / 2;
                    break;

                case DetectorType.NFM:
                    cutoff1 = MinNFMAudioFrequency;
                    cutoff2 = _bandwidth / 2;
                    break;
            }

            coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate / Math.Pow(2.0, _baseBandDecimationStageCount + _audioDecimationStageCount), _filterOrder, cutoff1, cutoff2, _windowType);
            if (_audioFilter == null)
            {
                _audioFilter = new FirFilter(coeffs);
            }
            else
            {
                _audioFilter.SetCoefficients(coeffs);
            }
        }

        public void ProcessBuffer(Complex* iqBuffer, float* audioBuffer, int length)
        {
            if (_needConfigure)
            {
                Configure();
                _needConfigure = false;
            }

            DownConvert(iqBuffer, length);

            if (_baseBandDecimator.StageCount > 0)
            {
                _baseBandDecimator.Process(iqBuffer, length);
                length /= (int) Math.Pow(2.0, _baseBandDecimator.StageCount);
            }

            _iqFilter.Process(iqBuffer, length);

            if (_rawAudioBuffer == null || _rawAudioBuffer.Length != length)
            {
                _rawAudioBuffer = UnsafeBuffer.Create(length, sizeof (float));
                _rawAudioPtr = (float*) _rawAudioBuffer;
            }

            Demodulate(iqBuffer, _rawAudioPtr, length);

            if (_detectorType != DetectorType.WFM)
            {
                _audioFilter.Process(_rawAudioPtr, length);
            }

            if (_useAgc && _detectorType != DetectorType.WFM && _detectorType != DetectorType.NFM)
            {
                _agc.Process(_rawAudioPtr, length);
            }

            if (_detectorType == DetectorType.AM)
            {
                _dcRemover.Process(_rawAudioPtr, length);
            }

            if (_detectorType == DetectorType.WFM)
            {
                _stereoDecoder.Process(_rawAudioPtr, audioBuffer, length);
            }
            else
            {
                MonoToStereo(_rawAudioPtr, audioBuffer, length);
            }
        }

        private static void MonoToStereo(float* input, float* output, int inputLength)
        {
            for (var i = 0; i < inputLength; i++)
            {
                output[i * 2] = input[i];
                output[i * 2 + 1] = input[i];
            }
        }

        private void DownConvert(Complex* iq, int length)
        {
            for (var i = 0; i < length; i++)
            {
                iq[i] *= _localOscillator.Tick();
            }
        }

        private void Demodulate(Complex* iq, float* audio, int length)
        {
            switch (_detectorType)
            {
                case DetectorType.WFM:
                case DetectorType.NFM:
                    _fmDetector.Demodulate(iq, audio, length);
                    break;

                case DetectorType.AM:
                    _amDetector.Demodulate(iq, audio, length);
                    break;

                case DetectorType.DSB:
                    _dsbDetector.Demodulate(iq, audio, length);
                    break;

                case DetectorType.CWL:
                case DetectorType.LSB:
                    _lsbDetector.Demodulate(iq, audio, length);
                    break;

                case DetectorType.CWU:
                case DetectorType.USB:
                    _usbDetector.Demodulate(iq, audio, length);
                    break;
            }
        }
    }
}