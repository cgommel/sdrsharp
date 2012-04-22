using System;

namespace SDRSharp.Radio
{
    public unsafe class Vfo
    {
        public const int DefaultCwSideTone = 600;
        public const int DefaultBandwidth = 2400;
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
        private Decimator _decimator;
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
        private bool _needNewDecimator;

        public Vfo()
        {
            _bandwidth = DefaultBandwidth;
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
                _needNewFilters = true;
                _needNewDecimator = true;
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
                _needNewDecimator = true;
                _needNewFilters = true;
                Configure();
            }
        }

        private void Configure()
        {
            _localOscillator.SampleRate = _sampleRate;
            _localOscillator.Frequency = _frequency;
            _agc.SampleRate = _sampleRate / _decimationFactor;
            if (_needNewDecimator)
            {
                _needNewDecimator = false;
                _decimator = new Decimator(_decimationFactor);
            }
            if (_needNewFilters)
            {
                _needNewFilters = false;
                InitFilters();
            }
            switch (_detectorType)
            {
                case DetectorType.USB:
                    _usbDetector.SampleRate = _sampleRate / _decimationFactor;
                    _usbDetector.BfoFrequency = -_bandwidth / 2;
                    _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                    break;

                case DetectorType.LSB:
                    _lsbDetector.SampleRate = _sampleRate / _decimationFactor;
                    _lsbDetector.BfoFrequency = -_bandwidth / 2;
                    _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                    break;

                case DetectorType.CWU:
                    _usbDetector.SampleRate = _sampleRate / _decimationFactor;
                    _usbDetector.BfoFrequency = -DefaultCwSideTone;
                    _localOscillator.Frequency -= _usbDetector.BfoFrequency;
                    break;

                case DetectorType.CWL:
                    _lsbDetector.SampleRate = _sampleRate / _decimationFactor;
                    _lsbDetector.BfoFrequency = -DefaultCwSideTone;
                    _localOscillator.Frequency += _lsbDetector.BfoFrequency;
                    break;

                case DetectorType.NFM:
                    _fmDetector.Mode = FmMode.Narrow;
                    _fmDetector.SampleRate = _sampleRate / _decimationFactor;
                    break;

                case DetectorType.WFM:
                    _fmDetector.Mode = FmMode.Wide;
                    _fmDetector.SampleRate = _sampleRate / _decimationFactor;
                    break;
            }
        }

        private void InitFilters()
        {
            int iqBW;
            int cutoff1;
            int cutoff2;
            var iqOrder = Math.Min(_filterOrder, MaxQuadratureFilterOrder);

            if (_detectorType == DetectorType.NFM)
            {
                iqBW = Math.Max(_bandwidth, MaxNFMBandwidth) / 2;
            }
            else
            {
                iqBW = _bandwidth / 2;
            }
            var coeffs = FilterBuilder.MakeLowPassKernel(_sampleRate / _decimationFactor, iqOrder, iqBW, _windowType);
            _iqFilter = new IQFirFilter(coeffs);

            if (_detectorType == DetectorType.CWL || _detectorType == DetectorType.CWU)
            {
                cutoff1 = DefaultCwSideTone - _bandwidth / 2;
                cutoff2 = DefaultCwSideTone + _bandwidth / 2;
            }
            else if (_detectorType == DetectorType.LSB || _detectorType == DetectorType.USB)
            {
                cutoff1 = MinSSBAudioFrequency;
                cutoff2 = _bandwidth;
            }
            else if (_detectorType == DetectorType.DSB)
            {
                cutoff1 = MinSSBAudioFrequency;
                cutoff2 = _bandwidth / 2;
            }
            else if (_detectorType == DetectorType.NFM)
            {
                cutoff1 = MinNFMAudioFrequency;
                cutoff2 = _bandwidth / 2;
            }
            else
            {
                cutoff1 = MinBCAudioFrequency;
                cutoff2 = Math.Min(_bandwidth / 2, MaxBCAudioFrequency);
            }
            coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate / _decimationFactor, _filterOrder, cutoff1, cutoff2, _windowType);
            _audioFilter = new FirFilter(coeffs);
        }

        public void ProcessBuffer(Complex* iq, float* audio, int length)
        {
            DownConvert(iq, length);
            if (_decimationFactor >= 2)
            {
                _decimator.Process(iq, length);
                length /= _decimationFactor;
            }
            _iqFilter.ProcessSymmetricKernel(iq, length);
            Demodulate(iq, audio, length);
            _audioFilter.ProcessSymmetricKernel(audio, length);
            if (_useAgc)
            {
                _agc.Process(audio, length);
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