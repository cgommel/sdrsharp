using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
#if !__MonoCS__
    [StructLayout(LayoutKind.Sequential, Pack = 16, Size = 80)]
#endif
    public unsafe struct Oscillator
    {
        private float _sinOfAnglePerSample;
        private float _cosOfAnglePerSample;
        private float _vectR;
        private float _vectI;
        private float _outR;
        private float _outI;
        private double _sampleRate;
        private double _frequency;
        private double _anglePerSample;

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

        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    Configure();
                }
            }
        }

        public float StateReal
        {
            get { return _vectR; }
            set { _vectR = value; }
        }

        public float StateImag
        {
            get { return _vectI; }
            set { _vectI = value; }
        }

        public float StateSin
        {
            get { return _sinOfAnglePerSample; }
            set { _sinOfAnglePerSample = value; }
        }

        public float StateCos
        {
            get { return _cosOfAnglePerSample; }
            set { _cosOfAnglePerSample = value; }
        }

        private void Configure()
        {
            if (_vectI == default(float) && _vectR == default(float))
            {
                _vectR = 1.0f;
            }
            if (_sampleRate != default(double))
            {
                _anglePerSample = 2.0 * Math.PI * _frequency / _sampleRate;
                _sinOfAnglePerSample = (float) Math.Sin(_anglePerSample);
                _cosOfAnglePerSample = (float) Math.Cos(_anglePerSample);
            }
        }

        public Complex Out
        {
            get
            {
                return new Complex(_outR, _outI);
            }
        }

        public float OutI
        {
            get { return _outR; }
        }

        public float OutQ
        {
            get { return _outI; }
        }

        public void Tick()
        {
            _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
            _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
            var oscGn = 1.95f - (_vectR * _vectR + _vectI * _vectI);
            _vectR = oscGn * _outR;
            _vectI = oscGn * _outI;
        }

        public void Mix(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
                _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
                var oscGn = 1.95f - (_outR * _outR + _outI * _outI);
                _vectR = oscGn * _outR;
                _vectI = oscGn * _outI;

                buffer[i] *= _outR;
            }
        }

        public void Mix(Complex* buffer, int length)
        {
            Mix(buffer, length, 0, 1);
        }

        public void Mix(Complex* buffer, int length, int startIndex, int stepSize)
        {
            for (var i = startIndex; i < length; i += stepSize)
            {
                _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
                _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
                var oscGn = 1.95f - (_outR * _outR + _outI * _outI);
                _vectR = oscGn * _outR;
                _vectI = oscGn * _outI;

                Complex osc;
                osc.Real = _outR;
                osc.Imag = _outI;

                buffer[i] *= osc;
            }
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Out;
        }
    }
}
