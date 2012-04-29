using System;

namespace SDRSharp.Radio
{
    public unsafe class Vfo
    {
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
        private ComplexDecimator _baseBandDecimator;
        private FloatDecimator _audioDecimator;
        private FirFilter _audioFilter;
        private IQFirFilter _iqFilter;
        private DetectorType _detectorType;
        private WindowType _windowType;
        private double _sampleRate;
        private int _bandwidth;
        private int _frequency;
        private int _filterOrder;
        private bool _needNewFilters;
        private bool _useAgc;
        private int _decimationFactor = 1;
        private bool _needNewDecimators;
        private int _cwToneShift;

        public Vfo()
        {
            _bandwidth = DefaultSSBBandwidth;
            _filterOrder = FilterBuilder.DefaultFilterOrder;
            Configure();
        }

        public DetectorType DetectorType
        {
            get
            {
                return _detectorType;
            }
            set
            {
                _needNewDecimators = (_detectorType == DetectorType.WFM && value != DetectorType.WFM) ||
                                    (_detectorType != DetectorType.WFM && value == DetectorType.WFM);
                _detectorType = value;
                Configure();
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
                _frequency = value;
                Configure();
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
                _filterOrder = value;
                _needNewFilters = true;
                Configure();
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
                _sampleRate = value;
                _needNewDecimators = true;
                Configure();
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
                _windowType = value;
                _needNewFilters = true;
                Configure();
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
                _bandwidth = value;
                _needNewFilters = true;
                Configure();
            }
        }

        public bool UseAGC
        {
            get { return _useAgc; }
            set { _useAgc = value; }
        }

        public float AgcThreshold
        {
            get { return _agc.Threshold; }
            set { _agc.Threshold = value; }
        }

        public float AgcDecay
        {
            get { return _agc.Decay; }
            set { _agc.Decay = value; }
        }

        public float AgcSlope
        {
            get { return _agc.Slope; }
            set { _agc.Slope = value; }
        }

        public bool AgcHang
        {
            get { return _agc.UseHang; }
            set { _agc.UseHang = value; }
        }

        public int FmSquelch
        {
            get { return _fmDetector.SquelchThreshold; }
            set { _fmDetector.SquelchThreshold = value; }
        }

        public int DecimationFactor
        {
            get { return _decimationFactor; }
            set
            {
                _decimationFactor = value;
                _needNewDecimators = true;
                Configure();
            }
        }

        public int CWToneShift
        {
            get { return _cwToneShift; }
            set
            {
                _cwToneShift = value;
                _needNewFilters = true;
                Configure();
            }
        }

        private void Configure()
        {
            lock (this)
            {
                _localOscillator.SampleRate = _sampleRate;
                _localOscillator.Frequency = _frequency;
                int baseBandDecimationFactor;
                int audioDecimationFactor;
                if (_detectorType == DetectorType.WFM)
                {
                    var afSamplerate = _sampleRate / _decimationFactor;
                    audioDecimationFactor = 1;
                    while (afSamplerate * audioDecimationFactor < DefaultWFMBandwidth && audioDecimationFactor < _decimationFactor)
                    {
                        audioDecimationFactor *= 2;
                    }
                    baseBandDecimationFactor = _decimationFactor / audioDecimationFactor;
                }
                else
                {
                    baseBandDecimationFactor = _decimationFactor;
                    audioDecimationFactor = 1;
                }
                if (_needNewDecimators)
                {
                    _needNewDecimators = false;
                    _baseBandDecimator = new ComplexDecimator(baseBandDecimationFactor);
                    _audioDecimator = new FloatDecimator(audioDecimationFactor);
                    _needNewFilters = true;
                }
                if (_needNewFilters)
                {
                    _needNewFilters = false;
                    InitFilters(baseBandDecimationFactor, audioDecimationFactor);
                }
                switch (_detectorType)
                {
                    case DetectorType.USB:
                        _usbDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        _usbDetector.BfoFrequency = -_bandwidth / 2;
                        _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                        break;

                    case DetectorType.LSB:
                        _lsbDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        _lsbDetector.BfoFrequency = -_bandwidth / 2;
                        _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                        break;

                    case DetectorType.CWU:
                        _usbDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        _usbDetector.BfoFrequency = -_cwToneShift;
                        _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                        break;

                    case DetectorType.CWL:
                        _lsbDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        _lsbDetector.BfoFrequency = -_cwToneShift;
                        _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                        break;

                    case DetectorType.NFM:
                        _fmDetector.Mode = FmMode.Narrow;
                        _fmDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        break;

                    case DetectorType.WFM:
                        _fmDetector.Mode = FmMode.Wide;
                        _fmDetector.SampleRate = _sampleRate / baseBandDecimationFactor;
                        break;
                }
                _agc.SampleRate = _sampleRate / _decimationFactor;
            }
        }

        private void InitFilters(int baseBandDecimationFactor, int audioDecimationFactor)
        {
            int iqBW;
            int cutoff1 = 0;
            int cutoff2 = 10000;
            var iqOrder = Math.Min(_filterOrder, MaxQuadratureFilterOrder);

            switch (_detectorType)
            {
                case DetectorType.NFM:
                    iqBW = Math.Max(_bandwidth, MaxNFMBandwidth) / 2;
                    break;

                case DetectorType.WFM:
                    iqBW = 90000;
                    iqOrder = 20;
                    break;

                default:
                    iqBW = _bandwidth / 2;
                    break;
            }

            var coeffs = FilterBuilder.MakeLowPassKernel(_sampleRate / baseBandDecimationFactor, iqOrder, iqBW, _windowType);
            _iqFilter = new IQFirFilter(coeffs);

            var afOrder = _filterOrder;

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

                case DetectorType.WFM:
                    cutoff1 = MinBCAudioFrequency;
                    cutoff2 = MaxBCAudioFrequency;
                    afOrder = 50;
                    break;
            }

            coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate / baseBandDecimationFactor / audioDecimationFactor, afOrder, cutoff1, cutoff2, _windowType);
            _audioFilter = new FirFilter(coeffs);
        }

        public void ProcessBuffer(Complex* iqBuffer, float* audioBuffer, int length)
        {
            lock (this)
            {
                DownConvert(iqBuffer, length);

                if (_baseBandDecimator.Factor >= 2)
                {
                    _baseBandDecimator.Process(iqBuffer, length);
                    length /= _baseBandDecimator.Factor;
                }
                
                _iqFilter.ProcessSymmetricKernel(iqBuffer, length);

                var audio = stackalloc float[length];

                Demodulate(iqBuffer, audio, length);
                
                if (_audioDecimator.Factor >= 2)
                {
                    _audioDecimator.Process(audio, length);
                    length /= _audioDecimator.Factor;
                }
                
                _audioFilter.ProcessSymmetricKernel(audio, length);
                
                if (_useAgc && _detectorType != DetectorType.WFM)
                {
                    _agc.Process(audio, length);
                }

                Utils.Memcpy(audioBuffer, audio, length * sizeof(float));
            }
        }

        private void DownConvert(Complex* iq, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _localOscillator.Tick();
                iq[i] *= _localOscillator;
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