using System;

namespace SDRSharp.Radio
{
    public class Oscillator
    {
        private const double Epsilon = 1e-10;
        private const double Rad2Pi = 2.0 * Math.PI;

        private int _sampleRate;
        private int _frequency;
        private double _phase;
        private double _anglePerSample;
        private double _outI;
        private double _outQ;

        public int SampleRate
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

        public double OutI
        {
            get { return _outI; }
        }

        public double OutQ
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
            _outI = Math.Cos(_phase);
            _outQ = Math.Sin(_phase);
            _phase = (_phase + _anglePerSample) % Rad2Pi;
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Out;
        }
    }
}
