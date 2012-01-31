using System;

namespace SDRSharp.Radio
{
    public class Oscillator
    {
        private const double Epsilon = 1e-10;

        private int _sampleRate;
        private int _frequency;
        private int _tick;
        private double _anglePerSample;
        private float _outI;
        private float _outQ;

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
            var wt = (_anglePerSample * _tick) % rad2Pi;
            if (Math.Abs(wt) < Epsilon)
            {
                _tick = 0;
            }
            _outI = (float) Math.Cos(wt);
            _outQ = (float) Math.Sin(wt);
            _tick++;
        }

        public static implicit operator Complex(Oscillator osc)
        {
            return osc.Out;
        }
    }
}
