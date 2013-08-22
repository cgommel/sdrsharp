using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
#if !__MonoCS__
    [StructLayout(LayoutKind.Sequential, Pack = 16, Size = 80)]
#endif
    public unsafe struct Oscillator
    {
        private Complex _rotation;
        private Complex _vector;
        private double _sampleRate;
        private double _frequency;

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

        private void Configure()
        {
            if (_vector.Real == default(float) && _vector.Imag == default(float))
            {
                _vector.Real = 1.0f;
            }
            if (_sampleRate != default(double))
            {
                var anglePerSample = 2.0 * Math.PI * _frequency / _sampleRate;
                _rotation = Complex.FromAngle(anglePerSample);
            }
        }

        public Complex Phase
        {
            get { return _vector; }
            set { _vector = value; }
        }

        public float Real
        {
            get { return _vector.Real; }
            set { _vector.Real = value; }
        }

        public float Imag
        {
            get { return _vector.Imag; }
            set { _vector.Imag = value; }
        }

        public void Tick()
        {
            _vector *= _rotation;
            _vector = _vector.NormalizeFast();
        }

        public void Mix(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                Tick();
                buffer[i] *= _vector.Real;
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
                Tick();
                buffer[i] *= _vector;
            }
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Phase;
        }
    }
}
