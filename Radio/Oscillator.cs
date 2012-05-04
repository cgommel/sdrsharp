using System;

namespace SDRSharp.Radio
{
    public class Oscillator
    {
        private const double Epsilon = 1e-10;

        private double _sampleRate;
        private int _frequency;
        private double _phase;
        private double _anglePerSample;
        private float _outI;
        private float _outQ;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                _sampleRate = value;
                Configure();
            }
        }

        public int Frequency
        {
            get { return _frequency; }
            set
            {
                _frequency = value;
                Configure();
            }
        }

        private void Configure()
        {
            _anglePerSample = 2.0 * Math.PI * _frequency / _sampleRate;
        }

        public float OutI
        {
            get { return _outI; }
        }

        public float OutQ
        {
            get { return _outQ; }
        }

        public Complex Out
        {
            get
            {
                return new Complex(_outI, _outQ);
            }
        }

        public void Tick()
        {
            const double rad2Pi = 2.0 * Math.PI;
            _outI = (float) Math.Cos(_phase);
            _outQ = (float) Math.Sin(_phase);
            _phase += _anglePerSample;
            _phase = _phase % rad2Pi;
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Out;
        }
    }
}
