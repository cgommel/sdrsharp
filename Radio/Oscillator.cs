using System;

namespace SDRSharp.Radio
{
    public unsafe struct Oscillator
    {
        private double _sinOfAnglePerSample;
        private double _cosOfAnglePerSample;
        private double _vectR;
        private double _vectI;
        private double _outR;
        private double _outI;
        private double _sampleRate;
        private double _frequency;
        private double _anglePerSample;
        private bool _configureNeeded;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    _configureNeeded = true;
                }
            }
        }

        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    _configureNeeded = true;
                }
            }
        }

        public double StateReal
        {
            get { return _vectR; }
            set { _vectR = value; }
        }

        public double StateImag
        {
            get { return _vectI; }
            set { _vectI = value; }
        }

        public double StateSin
        {
            get { return _sinOfAnglePerSample; }
            set { _sinOfAnglePerSample = value; }
        }

        public double StateCos
        {
            get { return _cosOfAnglePerSample; }
            set { _cosOfAnglePerSample = value; }
        }

        private void Configure()
        {
            if (_vectI == default(double) && _vectR == default(double))
            {
                _vectR = 1.0;
            }
            _anglePerSample = 2.0 * Math.PI * _frequency / _sampleRate;
            _sinOfAnglePerSample = Math.Sin(_anglePerSample);
            _cosOfAnglePerSample = Math.Cos(_anglePerSample);
        }

        public Complex Out
        {
            get
            {
                return new Complex((float) _outR, (float) _outI);
            }
        }

        public float OutI
        {
            get { return (float) _outR; }
        }

        public float OutQ
        {
            get { return (float) _outI; }
        }

        public void Tick()
        {
            if (_configureNeeded)
            {
                Configure();
                _configureNeeded = false;
            }

            _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
            _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
            var oscGn = 1.95 - (_vectR * _vectR + _vectI * _vectI);
            _vectR = oscGn * _outR;
            _vectI = oscGn * _outI;
        }

        public void Mix(float* buffer, int length)
        {
            if (_configureNeeded)
            {
                Configure();
                _configureNeeded = false;
            }

            for (var i = 0; i < length; i++)
            {
                _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
                _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
                var oscGn = 1.95 - (_vectR * _vectR + _vectI * _vectI);
                _vectR = oscGn * _outR;
                _vectI = oscGn * _outI;

                buffer[i] *= (float) _outR;
            }
        }

        public void Mix(Complex* buffer, int length)
        {
            Mix(buffer, length, 0, 1);
        }

        public void Mix(Complex* buffer, int length, int startIndex, int stepSize)
        {
            if (_configureNeeded)
            {
                Configure();
                _configureNeeded = false;
            }

            for (var i = startIndex; i < length; i += stepSize)
            {
                _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
                _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
                var oscGn = 1.95 - (_vectR * _vectR + _vectI * _vectI);
                _vectR = oscGn * _outR;
                _vectI = oscGn * _outI;

                Complex osc;
                osc.Real = (float) _outR;
                osc.Imag = (float) _outI;

                buffer[i] *= osc;
            }
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Out;
        }
    }
}
