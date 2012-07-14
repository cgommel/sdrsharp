using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class StereoDecoder
    {
        private const int PilotDefaultFrequency = 19000;
        private const int PilotBandwidth = 10;
        private const int PilotRange = 20;
        private const double PllThreshold = 3.2;
        private const double PllLockTime = 0.5; // sec
        private const double PilotPllZeta = 0.707;
        private const double PhaseAdjM = -7.267e-6f;
        private const double PhaseAdjB = 3.677f;

        private const float DeemphasisTime = 50e-6f; //75e-6f

        private double _pilotPhase;
        private double _pilotFrequency;
        private double _pilotFrequencyH;
        private double _pilotFrequencyL;
        private double _pilotAlpha;
        private double _pilotBeta;
        private double _pilotPhaseAdj;
        private double _pilotLockAlpha;
        private double _pilotEnergyAvg;
        private bool _pllLocked;

        private UnsafeBuffer _channelABuffer;
        private UnsafeBuffer _channelBBuffer;
        private float* _channelAPtr;
        private float* _channelBPtr;
        private FirFilter _channelAFilter;
        private FirFilter _channelBFilter;
        private FloatDecimator _channelADecimator;
        private FloatDecimator _channelBDecimator;
        private double _sampleRate;
        private int _audioDecimationFactor;
        private float _deemphasisAlpha;
        private float _deemphasisAvgL;
        private float _deemphasisAvgR;
        private bool _forceMono;

        public bool ForceMono
        {
            get { return _forceMono; }
            set { _forceMono = value; }
        }

        public bool IsPllLocked
        {
            get { return _pllLocked; }
        }

        public void Process(float* baseBand, float* interleavedStereo, int length)
        {
            if (_forceMono)
            {
                ProcessMono(baseBand, interleavedStereo, length);
            }
            else
            {
                ProcessStereo(baseBand, interleavedStereo, length);
            }
        }

        private void ProcessMono(float* baseBand, float* interleavedStereo, int length)
        {
            #region Prepare buffer

            if (_channelABuffer == null || _channelABuffer.Length != length)
            {
                _channelABuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelAPtr = (float*)_channelABuffer;
            }

            #endregion

            #region Decimate L+R

            Utils.Memcpy(_channelAPtr, baseBand, length * sizeof(float));
            _channelADecimator.Process(_channelAPtr, length);

            #endregion

            #region Filter L+R

            length /= _audioDecimationFactor;
            _channelAFilter.Process(_channelAPtr, length);

            #endregion

            #region Process deemphasis

            for (var i = 0; i < length; i++)
            {
                _deemphasisAvgL = (1f - _deemphasisAlpha) * _deemphasisAvgL + _deemphasisAlpha * _channelAPtr[i];
                _channelAPtr[i] = _deemphasisAvgL;
            }

            #endregion

            #region Fill output buffer

            for (var i = 0; i < length; i++)
            {
                interleavedStereo[i * 2] = _channelAPtr[i];
                interleavedStereo[i * 2 + 1] = _channelAPtr[i];
            }

            #endregion
        }

        private void ProcessStereo(float* baseBand, float* interleavedStereo, int length)
        {
            #region Prepare L-R buffer

            if (_channelBBuffer == null || _channelBBuffer.Length != length)
            {
                _channelBBuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelBPtr = (float*) _channelBBuffer;
            }

            #endregion

            #region Demodulate L-R

            Utils.Memcpy(_channelBPtr, baseBand, length * sizeof(float));

            for (var i = 0; i < length; i++)
            {
                Complex osc;
                osc.Real = (float) Math.Cos(_pilotPhase);
                osc.Imag = (float) Math.Sin(_pilotPhase);

                var sample = _channelBPtr[i] * osc;
                var phaseError = -sample.Argument();

                _pilotFrequency += _pilotBeta * phaseError;

                if (_pilotFrequency > _pilotFrequencyH)
                {
                    _pilotFrequency = _pilotFrequencyH;
                }
                if (_pilotFrequency < _pilotFrequencyL)
                {
                    _pilotFrequency = _pilotFrequencyL;
                }

                _pilotEnergyAvg = (1.0 - _pilotLockAlpha) * _pilotEnergyAvg + _pilotLockAlpha * phaseError * phaseError;

                _pilotPhase += _pilotFrequency + _pilotAlpha * phaseError;

                var dsbCarrier = (float) Math.Sin((_pilotPhase + _pilotPhaseAdj) * 2.0f);
                _channelBPtr[i] *= dsbCarrier;
            }

            _pilotPhase %= 2.0 * Math.PI;

            _pllLocked = _pilotEnergyAvg < PllThreshold;

            if (!_pllLocked)
            {
                ProcessMono(baseBand, interleavedStereo, length);
                return;
            }

            #endregion

            #region Decimate L-R

            _channelBDecimator.Process(_channelBPtr, length);

            #endregion

            #region Prepare L+R buffer

            if (_channelABuffer == null || _channelABuffer.Length != length)
            {
                _channelABuffer = UnsafeBuffer.Create(length, sizeof(float));
                _channelAPtr = (float*)_channelABuffer;
            }

            #endregion

            #region Decimate L+R

            Utils.Memcpy(_channelAPtr, baseBand, length * sizeof(float));
            _channelADecimator.Process(_channelAPtr, length);

            #endregion

            #region Recover and filter L and R audio channels

            length /= _audioDecimationFactor;

            _channelAFilter.Process(_channelAPtr, length);
            _channelBFilter.Process(_channelBPtr, length);

            for (var i = 0; i < length; i++)
            {
                var a = _channelAPtr[i];
                var b = 2f * _channelBPtr[i];
                interleavedStereo[i * 2]     = a + b;
                interleavedStereo[i * 2 + 1] = a - b;
            }

            #endregion

            #region Process deemphasis

            for (var i = 0; i < length; i++)
            {
                _deemphasisAvgL = (1f - _deemphasisAlpha) * _deemphasisAvgL + _deemphasisAlpha * interleavedStereo[i * 2];
                interleavedStereo[i * 2] = _deemphasisAvgL;

                _deemphasisAvgR = (1f - _deemphasisAlpha) * _deemphasisAvgR + _deemphasisAlpha * interleavedStereo[i * 2 + 1];
                interleavedStereo[i * 2 + 1] = _deemphasisAvgR;
            }

            #endregion
        }

        public void Configure(double sampleRate, int decimationStageCount)
        {
            _audioDecimationFactor = (int) Math.Pow(2.0, decimationStageCount);

            if (_sampleRate != sampleRate)
            {
                _sampleRate = sampleRate;

                _pilotPhase = 0.0;
                _pilotFrequency = -PilotDefaultFrequency * 2 * Math.PI / _sampleRate;
                _pilotFrequencyL = (-PilotDefaultFrequency - PilotRange) * 2 * Math.PI / _sampleRate;
                _pilotFrequencyH = (-PilotDefaultFrequency + PilotRange) * 2 * Math.PI / _sampleRate;
                _pilotAlpha = 2.0 * PilotPllZeta * PilotBandwidth * 2 * Math.PI / _sampleRate;
                _pilotBeta = (_pilotAlpha * _pilotAlpha) / (4.0 * PilotPllZeta * PilotPllZeta);
                _pilotPhaseAdj = PhaseAdjM * _sampleRate + PhaseAdjB;
                _pilotLockAlpha = 1.0 - Math.Exp(-1.0 / (_sampleRate * PllLockTime));
                
                var outputSampleRate = sampleRate / _audioDecimationFactor;
                var coefficients = FilterBuilder.MakeBandPassKernel(outputSampleRate, 250, Vfo.MinBCAudioFrequency, Vfo.MaxBCAudioFrequency, WindowType.BlackmanHarris);
                _channelAFilter = new FirFilter(coefficients);
                _channelBFilter = new FirFilter(coefficients);

                _deemphasisAlpha = (float)(1.0 - Math.Exp(-1.0 / (outputSampleRate * DeemphasisTime)));
                _deemphasisAvgL = 0;
                _deemphasisAvgR = 0;
            }

            if (_channelADecimator == null || _channelBDecimator == null || decimationStageCount != _channelADecimator.StageCount)
            {
                _channelADecimator = new FloatDecimator(decimationStageCount);
                _channelBDecimator = new FloatDecimator(decimationStageCount);
            }
        }
    }
}
