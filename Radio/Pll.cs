using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public struct Pll
    {
        private const double DefaultZeta = 0.707;
        private const double DefaultLockTime = 0.5; // sec
        private const double DefaultPhaseAdjM = -7.267e-6f;
        private const double DefaultPhaseAdjB = 3.677f;
        private const double DefaultRange = 20;
        private const double DefaultBandwidth = 10;
        private const double DefaultLockThreshold = 3.2;

        private double _sampleRate;
        private double _phase;
        private double _frequencyRadian;
        private double _minFrequencyRadian;
        private double _maxFrequencyRadian;
        private double _defaultFrequency;
        private double _range;
        private double _bandwidth;
        private double _alpha;
        private double _beta;
        private double _zeta;
        private double _phaseAdj;
        private double _phaseAdjM;
        private double _phaseAdjB;
        private double _lockAlpha;
        private double _lockTime;
        private double _phaseErrorAvg;
        private double _adjustedPhase;
        private double _lockThreshold;

        public Pll(double frequency)
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
            _lockTime = DefaultLockTime;
            _phaseErrorAvg = 0;
            _adjustedPhase = 0;
            _lockThreshold = DefaultLockThreshold;
            Configure();
        }

        public double AdjustedPhase
        {
            get { return _adjustedPhase; }
        }

        public double Phase
        {
            get { return _phase; }
        }

        public double SampleRate
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

        public double DefaultFrequency
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

        public double Range
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

        public double Bandwidth
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

        public double LockTime
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

        public double LockThreshold
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

        public double Zeta
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

        public double PhaseAdjM
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

        public double PhaseAdjB
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
            _phase = 0.0;
            var norm = 2 * Math.PI / _sampleRate;
            _frequencyRadian = _defaultFrequency * norm;
            _minFrequencyRadian = (_defaultFrequency - _range) * norm;
            _maxFrequencyRadian = (_defaultFrequency + _range) * norm;
            _alpha = 2.0 * _zeta * _bandwidth * norm;
            _beta = (_alpha * _alpha) / (4.0 * _zeta * _zeta);
            _phaseAdj = _phaseAdjM * _sampleRate + _phaseAdjB;
            _lockAlpha = 1.0 - Math.Exp(-1.0 / (_sampleRate * _lockTime));
        }

        public Complex Process(float sample)
        {
            Complex osc;
            osc.Real = (float) Math.Cos(_phase);
            osc.Imag = (float) Math.Sin(_phase);

            osc *= sample;
            var phaseError = -osc.FastArgument();

            ProcessPhaseError(phaseError);

            return osc;
        }

        public Complex Process(Complex sample)
        {
            Complex osc;
            osc.Real = (float) Math.Cos(_phase);
            osc.Imag = (float) Math.Sin(_phase);

            osc *= sample;
            var phaseError = -osc.FastArgument();

            ProcessPhaseError(phaseError);

            return osc;
        }

        private void ProcessPhaseError(double phaseError)
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

            _phaseErrorAvg = (1.0 - _lockAlpha) * _phaseErrorAvg + _lockAlpha * phaseError * phaseError;
            _phase += _frequencyRadian + _alpha * phaseError;
            _phase %= 2.0 * Math.PI;
            _adjustedPhase = _phase + _phaseAdj;
        }
    }
}
