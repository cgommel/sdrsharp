#if __MonoCS__
#define MANAGED_ONLY
#endif

using System;
#if !MANAGED_ONLY
using System.Runtime.InteropServices;
#endif

namespace SDRSharp.Radio
{
    public class Vfo
    {
        #region FIR Filter PInvokes

#if !MANAGED_ONLY

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void InitAudio(double[] coeffs, int len);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessAudio([In, Out] double[] sample, int len);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void InitIQ(double[] coeffs, int len);

        [DllImport("SDRSharp.Filters.dll")]
        private static extern void FirProcessIQ([In, Out] Complex[] buffer, int len);

#endif

        #endregion

        public const int DefaultCwSideTone = 600;
        public const int DefaultBandwidth = 2400;
        public const int MinSSBAudioFrequency = 200;
        public const int MinBCAudioFrequency = 20;
        public const int MaxBCAudioFrequency = 20000;
        public const int MaxQuadratureFilterOrder = 300;

        private readonly AutomaticGainControl _agc = new AutomaticGainControl();
        private readonly Oscillator _localOscillator = new Oscillator();

        private readonly AmDetector _amDetector = new AmDetector();
        private readonly FmDetector _fmDetector = new FmDetector();
        private readonly LsbDetector _lsbDetector = new LsbDetector();
        private readonly UsbDetector _usbDetector = new UsbDetector();

#if MANAGED_ONLY
        private FirFilter _audioFilter;
        private IQFirFilter _iqFilter;
#endif

        private DetectorType _detectorType;
        private WindowType _windowType;
        private int _sampleRate;
        private int _bandwidth;
        private int _frequency;
        private int _filterOrder;
        private bool _needNewFilters;
        private bool _useAgc;

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

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
                _needNewFilters = true;
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

        public int AgcAttack
        {
            get { return _agc.Attack; }
            set { _agc.Attack = value; }
        }

        public int AgcDecay
        {
            get { return _agc.Decay; }
            set { _agc.Decay = value; }
        }

        private void Configure()
        {
            _agc.SampleRate = _sampleRate;
            _localOscillator.SampleRate = _sampleRate;
            _localOscillator.Frequency = _frequency;
            if (_needNewFilters)
            {
                _needNewFilters = false;
                InitFilters();
            }
            if (_detectorType == DetectorType.USB)
            {
                int bfo;
                if (_frequency > _sampleRate / 2 - _bandwidth)
                {
                    bfo = _sampleRate / 2 - _frequency;
                }
                else
                {
                    bfo = _bandwidth;
                }
                bfo = bfo / 2 + MinSSBAudioFrequency;
                _usbDetector.SampleRate = _sampleRate;
                _usbDetector.BfoFrequency = -bfo;
                _localOscillator.Frequency -= _usbDetector.BfoFrequency;
            }
            else if (_detectorType == DetectorType.LSB)
            {
                int bfo;
                if (_frequency < -_sampleRate / 2 + _bandwidth)
                {
                    bfo = _sampleRate / 2 + _frequency;
                }
                else
                {
                    bfo = _bandwidth;
                }
                bfo = bfo / 2 + MinSSBAudioFrequency;
                _lsbDetector.SampleRate = _sampleRate;
                _lsbDetector.BfoFrequency = -bfo;
                _localOscillator.Frequency += _lsbDetector.BfoFrequency;
            }
            else if (_detectorType == DetectorType.FM)
            {
                _fmDetector.SampleRate = _sampleRate;
            }
        }

        private void InitFilters()
        {
            var cwMode = _bandwidth <= DefaultCwSideTone;
            var iqOrder = Math.Min(_filterOrder, MaxQuadratureFilterOrder);

            double[] coeffs;
            if (cwMode)
            {
                const int iqBW = DefaultCwSideTone / 2;
                coeffs = FilterBuilder.MakeLowPassKernel(_sampleRate, iqOrder, iqBW, _windowType);
            }
            else
            {
                coeffs = FilterBuilder.MakeLowPassKernel(_sampleRate, iqOrder, _bandwidth / 2, _windowType);
            }
            
#if MANAGED_ONLY
            _iqFilter = new IQFirFilter(coeffs);
#else
            InitIQ(coeffs, coeffs.Length);
#endif
            if (cwMode)
            {
                var cutoff1 = DefaultCwSideTone - _bandwidth / 2;
                var cutoff2 = DefaultCwSideTone + _bandwidth / 2;
                coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate, _filterOrder, cutoff1, cutoff2, _windowType);
            }
            else
            {
                if (_detectorType == DetectorType.LSB || _detectorType == DetectorType.USB)
                {
                    const int cutoff1 = MinSSBAudioFrequency;
                    var cutoff2 = _bandwidth - cutoff1;
                    coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate, _filterOrder, cutoff1, cutoff2, _windowType);
                }
                else
                {
                    const int cutoff1 = MinBCAudioFrequency;
                    int cutoff2 = Math.Min(_bandwidth / 2, MaxBCAudioFrequency);
                    coeffs = FilterBuilder.MakeBandPassKernel(_sampleRate, _filterOrder, cutoff1, cutoff2, _windowType);
                }
            }

#if MANAGED_ONLY
            _audioFilter = new FirFilter(coeffs);
#else                
            InitAudio(coeffs, coeffs.Length);
#endif
        }

        public void ProcessBuffer(Complex[] iq, double[] audio)
        {
            DownConvert(iq);
            FilterIQ(iq);
            Demodulate(iq, audio);
            FilterAudio(audio);
            if (_useAgc)
            {
                _agc.Process(audio);
            }
        }

        private void DownConvert(Complex[] iq)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                _localOscillator.Tick();
                iq[i] *= _localOscillator;
            }
        }

        private void FilterAudio(double[] audio)
        {
#if MANAGED_ONLY
            _audioFilter.Process(audio);
#else
            FirProcessAudio(audio, audio.Length);
#endif
        }

        private void FilterIQ(Complex[] iq)
        {
#if MANAGED_ONLY
            _iqFilter.Process(iq);
#else
            FirProcessIQ(iq, iq.Length);
#endif
        }

        private void Demodulate(Complex[] iq, double[] audio)
        {
            switch (_detectorType)
            {
                case DetectorType.FM:
                    _fmDetector.Demodulate(iq, audio);
                    break;

                case DetectorType.AM:
                    _amDetector.Demodulate(iq, audio);
                    break;

                case DetectorType.LSB:
                    _lsbDetector.Demodulate(iq, audio);
                    break;

                case DetectorType.USB:
                    _usbDetector.Demodulate(iq, audio);
                    break;
            }
        }
    }
}