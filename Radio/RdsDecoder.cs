using System;

namespace SDRSharp.Radio
{
    public unsafe class RdsDecoder
    {
        private const int PllDefaultFrequency = 57000;
        private const int PllRange = 12;
        private const int PllBandwith = 1;
        private const float PllZeta = 0.707f;
        private const float PllLockTime = 0.5f;
        private const float PllLockThreshold = 3.2f;
        private const float RdsBitRate = 1187.5f;

        private readonly IQFirFilter _baseBandFilter = new IQFirFilter(null, false);
        private readonly FirFilter _matchedFilter = new FirFilter();
        private readonly RdsDetectorBank _bitDecoder = new RdsDetectorBank();
        private readonly Pll* _pll;
        private readonly UnsafeBuffer _pllBuffer;
        private readonly Oscillator* _osc;
        private readonly UnsafeBuffer _oscBuffer;
        private readonly IirFilter* _syncFilter;
        private readonly UnsafeBuffer _syncFilterBuffer;

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

        public RdsDecoder()
        {
            _pllBuffer = UnsafeBuffer.Create(sizeof(Pll));
            _pll = (Pll*) _pllBuffer;

            _oscBuffer = UnsafeBuffer.Create(sizeof(Oscillator));
            _osc = (Oscillator*) _oscBuffer;

            _syncFilterBuffer = UnsafeBuffer.Create(sizeof(IirFilter));
            _syncFilter = (IirFilter*) _syncFilterBuffer;
        }

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

        public ushort PICode
        {
            get { return _bitDecoder.PICode; }
        }

        private void Configure()
        {
            _osc->SampleRate = _sampleRate;
            _osc->Frequency = PllDefaultFrequency;

            var decimationStageCount = 0;
            while (_sampleRate >= 20000 * Math.Pow(2.0, decimationStageCount))
            {
                decimationStageCount++;
            }

            _decimator = new IQDecimator(decimationStageCount, _sampleRate, true, false);
            _decimationFactor = (int) Math.Pow(2.0, decimationStageCount);
            _demodulationSampleRate = _sampleRate / _decimationFactor;

            var coefficients = FilterBuilder.MakeLowPassKernel(_demodulationSampleRate, 200, 2500, WindowType.BlackmanHarris);
            _baseBandFilter.SetCoefficients(coefficients);
            
            _pll->SampleRate = (float) _demodulationSampleRate;
            _pll->DefaultFrequency = 0;
            _pll->Range = PllRange;
            _pll->Bandwidth = PllBandwith;
            _pll->Zeta = PllZeta;
            _pll->LockTime = PllLockTime;
            _pll->LockThreshold = PllLockThreshold;

            var matchedFilterLength = (int) (_demodulationSampleRate / RdsBitRate) | 1;
            coefficients = FilterBuilder.MakeSin(_demodulationSampleRate, RdsBitRate, matchedFilterLength);
            _matchedFilter.SetCoefficients(coefficients);

            _syncFilter->Init(IirFilterType.BandPass, RdsBitRate, _demodulationSampleRate, 500);
        }

        public void Reset()
        {
            _bitDecoder.Reset();
            _syncFilter->Reset();
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
                _osc->Tick();
                _rawPtr[i] = _osc->Out * baseBand[i];
            }

            // Decimate
            _decimator.Process(_rawPtr, length);
            length /= _decimationFactor;

            // Filter
            _baseBandFilter.Process(_rawPtr, length);

            // PLL
            for (var i = 0; i < length; i++)
            {
                _dataPtr[i] = _pll->Process(_rawPtr[i]).Imag;
            }

            //if (!_pll->IsLocked)
            //{
            //    _bitDecoder.Reset();
            //    return;
            //}

            // Matched filter
            _matchedFilter.Process(_dataPtr, length);

            // Recover signal energy to sustain the oscillation in the IIR
            for (var i = 0; i < length; i++)
            {
                _magPtr[i] = Math.Abs(_dataPtr[i]);
            }

            // Synchronize to RDS bitrate
            _syncFilter->Process(_magPtr, length);

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
