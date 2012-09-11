using System;

namespace SDRSharp.Radio
{
    public sealed unsafe class Oscillator
    {
        private double _sampleRate;
        private double _frequency;
        private double _anglePerSample;
        private double _sinOfAnglePerSample;
        private double _cosOfAnglePerSample;
        private double _vectR;
        private double _vectI;
        private double _outR;
        private double _outI;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    _vectR = 1;
                    _vectI = 0;
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

        private void Configure()
        {
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
            _outR = _vectR * _cosOfAnglePerSample - _vectI * _sinOfAnglePerSample;
            _outI = _vectI * _cosOfAnglePerSample + _vectR * _sinOfAnglePerSample;
            double oscGn = 1.95 - (_vectR * _vectR + _vectI * _vectI);
            _vectR = oscGn * _outR;
            _vectI = oscGn * _outI;
        }

        public void Mix(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                Tick();
                buffer[i] *= (float) _outR;
            }
        }

        public void Mix(Complex* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                Tick();

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
