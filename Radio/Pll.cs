using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
#if !__MonoCS__
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
#endif
    public struct Pll
    {
        private const float DefaultZeta = 0.707f;
        private const float DefaultLockTime = 0.5f; // sec
        private const float DefaultPhaseAdjM = 0.0f;
        private const float DefaultPhaseAdjB = -2.25f;
        private const float DefaultRange = 20f;
        private const float DefaultBandwidth = 10.0f;
        private const float DefaultLockThreshold = 3.2f;

        private float _sampleRate;
        private float _phase;
        private float _frequencyRadian;
        private float _minFrequencyRadian;
        private float _maxFrequencyRadian;
        private float _defaultFrequency;
        private float _range;
        private float _bandwidth;
        private float _alpha;
        private float _beta;
        private float _zeta;
        private float _phaseAdj;
        private float _phaseAdjM;
        private float _phaseAdjB;
        private float _lockAlpha;
        private float _lockOneMinusAlpha;
        private float _lockTime;
        private float _phaseErrorAvg;
        private float _adjustedPhase;
        private float _lockThreshold;

        public Pll(float frequency)
        {
            _sampleRate = 0;
            _phase = 0;
            _frequencyRadian = 0;
            _minFrequencyRadian = 0;
            _maxFrequencyRadian = 0;
            _defaultFrequency = frequency;
            _range = DefaultRange;
            _bandwidth = DefaultBandwidth;
            _alpha = 0;
            _beta = 0;
            _zeta = DefaultZeta;
            _phaseAdj = 0;
            _phaseAdjM = DefaultPhaseAdjM;
            _phaseAdjB = DefaultPhaseAdjB;
            _lockAlpha = 0;
            _lockOneMinusAlpha = 1;
            _lockTime = DefaultLockTime;
            _phaseErrorAvg = 0;
            _adjustedPhase = 0;
            _lockThreshold = DefaultLockThreshold;
            Configure();
        }

        public float AdjustedPhase
        {
            get { return _adjustedPhase; }
        }

        public float Phase
        {
            get { return _phase; }
        }

        public float SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    Configure();
                }
            }
        }

        public float DefaultFrequency
        {
            get { return _defaultFrequency; }
            set
            {
                if (_defaultFrequency != value)
                {
                    _defaultFrequency = value;
                    Configure();
                }
            }
        }

        public float Range
        {
            get { return _range; }
            set
            {
                if (_range != value)
                {
                    _range = value;
                    Configure();
                }
            }
        }

        public float Bandwidth
        {
            get { return _bandwidth; }
            set
            {
                if (_bandwidth != value)
                {
                    _bandwidth = value;
                    Configure();
                }
            }
        }

        public float LockTime
        {
            get { return _lockTime; }
            set
            {
                if (_lockTime != value)
                {
                    _lockTime = value;
                    Configure();
                }
            }
        }

        public float LockThreshold
        {
            get { return _lockThreshold; }
            set
            {
                if (_lockThreshold != value)
                {
                    _lockThreshold = value;
                    Configure();
                }
            }
        }

        public float Zeta
        {
            get { return _zeta; }
            set
            {
                if (_zeta != value)
                {
                    _zeta = value;
                    Configure();
                }
            }
        }

        public float PhaseAdjM
        {
            get { return _phaseAdjM; }
            set
            {
                if (_phaseAdjM != value)
                {
                    _phaseAdjM = value;
                    Configure();
                }
            }
        }

        public float PhaseAdjB
        {
            get { return _phaseAdjB; }
            set
            {
                if (_phaseAdjB != value)
                {
                    _phaseAdjB = value;
                    Configure();
                }
            }
        }

        public bool IsLocked
        {
            get { return _phaseErrorAvg < _lockThreshold; }
        }

        private void Configure()
        {
            _phase = 0.0f;
            var norm = (float) (2.0 * Math.PI / _sampleRate);
            _frequencyRadian = _defaultFrequency * norm;
            _minFrequencyRadian = (_defaultFrequency - _range) * norm;
            _maxFrequencyRadian = (_defaultFrequency + _range) * norm;
            _alpha = 2.0f * _zeta * _bandwidth * norm;
            _beta = (_alpha * _alpha) / (4.0f * _zeta * _zeta);
            _phaseAdj = _phaseAdjM * _sampleRate + _phaseAdjB;
            _lockAlpha = (float) (1.0 - Math.Exp(-1.0 / (_sampleRate * _lockTime)));
            _lockOneMinusAlpha = 1.0f - _lockAlpha;
        }

        public Complex Process(float sample)
        {
            var osc = Trig.SinCos(_phase);

            osc *= sample;
            var phaseError = -osc.FastArgument();

            ProcessPhaseError(phaseError);

            return osc;
        }

        public Complex Process(Complex sample)
        {
            var osc = Trig.SinCos(_phase);

            osc *= sample;
            var phaseError = -osc.FastArgument();

            ProcessPhaseError(phaseError);

            return osc;
        }

        private void ProcessPhaseError(float phaseError)
        {
            _frequencyRadian += _beta * phaseError;

            if (_frequencyRadian > _maxFrequencyRadian)
            {
                _frequencyRadian = _maxFrequencyRadian;
            }
            else if (_frequencyRadian < _minFrequencyRadian)
            {
                _frequencyRadian = _minFrequencyRadian;
            }

            _phaseErrorAvg = _lockOneMinusAlpha * _phaseErrorAvg + _lockAlpha * phaseError * phaseError;
            _phase += _frequencyRadian + _alpha * phaseError;
            _phase %= (float) (2.0 * Math.PI);
            _adjustedPhase = _phase + _phaseAdj;
        }
    }
}
