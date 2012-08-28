using System;

namespace SDRSharp.Radio
{
    public unsafe class RdsDecoder
    {
        private const int PllDefaultFrequency = 57000;
        private const int PllRange = 12;
        private const int PllBandwith = 1;
        private const double PllLockTime = 0.5;
        private const double PllLockThreshold = 3.2;
        private const double RdsBitRate = 1187.5;

        private readonly Pll _pll = new Pll();
        private readonly Oscillator _osc = new Oscillator();
        private readonly IQFirFilter _baseBandFilter = new IQFirFilter();
        private readonly FirFilter _matchedFilter = new FirFilter();
        private readonly RdsDetectorBank _bitDecoder = new RdsDetectorBank();
        private IirFilter _syncFilter;

        private UnsafeBuffer _rawBuffer;
        private Complex* _rawPtr;
        private UnsafeBuffer _magBuffer;
        private float* _magPtr;
        private UnsafeBuffer _dataBuffer;
        private float* _dataPtr;
        private IQDecimator _decimator;
        private double _sampleRate;
        private double _demodulationSampleRate;
        private int _decimationFactor;

        private float _lastSync;
        private float _lastData;
        private float _lastSyncSlope;
        private bool _lastBit;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (value != _sampleRate)
                {
                    _sampleRate = value;
                    Configure();
                }
            }
        }

        public string RadioText
        {
            get { return _bitDecoder.RadioText; }
        }

        public string ProgramService
        {
            get { return _bitDecoder.ProgramService; }
        }

        private void Configure()
        {
            _osc.SampleRate = _sampleRate;
            _osc.Frequency = PllDefaultFrequency;

            var decimationStageCount = 0;
            while (_sampleRate > 8000 * Math.Pow(2.0, decimationStageCount))
            {
                decimationStageCount++;
            }

            _decimator = new IQDecimator(decimationStageCount, _sampleRate, true);
            _decimationFactor = (int) Math.Pow(2.0, decimationStageCount);
            _demodulationSampleRate = _sampleRate / _decimationFactor;

            var coefficients = FilterBuilder.MakeLowPassKernel(_demodulationSampleRate, 200, 2400, WindowType.BlackmanHarris);
            _baseBandFilter.SetCoefficients(coefficients);

            _pll.SampleRate = _demodulationSampleRate;
            _pll.DefaultFrequency = 0;
            _pll.Range = PllRange;
            _pll.Bandwidth = PllBandwith;
            _pll.LockTime = PllLockTime;
            _pll.LockThreshold = PllLockThreshold;

            var matchedFilterLength = (int) (_demodulationSampleRate / RdsBitRate);
            coefficients = new float[matchedFilterLength * 2 + 1];
            for (int i = 0; i <= matchedFilterLength; i++)
            {
                var t = i / _demodulationSampleRate;
                var x = t * RdsBitRate;
                var x64 = 64.0 * x;
                coefficients[i + matchedFilterLength] = (float) (.75 * Math.Cos(2.0 * 2.0 * Math.PI * x) * ((1.0 / (1.0 / x - x64)) - (1.0 / (9.0 / x - x64))));
                coefficients[matchedFilterLength - i] = (float) (-.75 * Math.Cos(2.0 * 2.0 * Math.PI * x) * ((1.0 / (1.0 / x - x64)) - (1.0 / (9.0 / x - x64))));
            }
            _matchedFilter.SetCoefficients(coefficients);

            _syncFilter = new IirFilter(IirFilterType.BandPass, RdsBitRate, _demodulationSampleRate, 500);
        }

        public void Reset()
        {
            _bitDecoder.Reset();
        }

        public void Process(float* baseBand, int length)
        {
            #region Initialize buffers

            if (_rawBuffer == null || _rawBuffer.Length != length)
            {
                _rawBuffer = UnsafeBuffer.Create(length, sizeof(Complex));
                _rawPtr = (Complex*) _rawBuffer;
            }

            if (_magBuffer == null || _magBuffer.Length != length)
            {
                _magBuffer = UnsafeBuffer.Create(length, sizeof(float));
                _magPtr = (float*) _magBuffer;
            }

            if (_dataBuffer == null || _dataBuffer.Length != length)
            {
                _dataBuffer = UnsafeBuffer.Create(length, sizeof(float));
                _dataPtr = (float*) _dataBuffer;
            }

            #endregion

            // Downconvert
            for (var i = 0; i < length; i++)
            {
                _osc.Tick();
                _rawPtr[i] = _osc.Out * baseBand[i];
            }

            // Decimate
            _decimator.Process(_rawPtr, length);
            length /= _decimationFactor;

            // Filter
            _baseBandFilter.Process(_rawPtr, length);

            // PLL
            for (var i = 0; i < length; i++)
            {
                _dataPtr[i] = _pll.Process(_rawPtr[i]).Imag;
            }

            if (!_pll.IsLocked)
            {
                _bitDecoder.Reset();
                return;
            }

            // Matched filter
            _matchedFilter.Process(_dataPtr, length);

            // Recover signal energy
            for (var i = 0; i < length; i++)
            {
                _magPtr[i] = _dataPtr[i] * _dataPtr[i];
            }

            // Synchronize to RDS bitrate
            _syncFilter.Process(_magPtr, length);

            // Detect RDS bits
            for (int i = 0; i < length; i++)
            {
                var data = _dataPtr[i];
                var syncVal = _magPtr[i];
                var slope = syncVal - _lastSync;
                _lastSync = syncVal;
                if (slope < 0.0f && _lastSyncSlope * slope < 0.0f)
                {
                    bool bit = _lastData > 0;
                    _bitDecoder.Process(bit ^ _lastBit);
                    _lastBit = bit;
                }
                _lastData = data;
                _lastSyncSlope = slope;
            }
        }
    }
}
