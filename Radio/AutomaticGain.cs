using System;

namespace SDRSharp.Radio
{
    public class AutomaticGainControl
    {
        private const double MeanLevel = 0.00003;
        private const double MaxFactor = 1000.0;
        private const double MinFactor = 0.001;
        private const double TimeConst = 0.1;

        private int _attack;
        private int _decay;
        private int _sampleRate;
        private double _mean;

        public int Attack
        {
            get
            {
                return _attack;
            }
            set
            {
                _attack = value;
            }
        }

        public int Decay
        {
            get
            {
                return _decay;
            }
            set
            {
                _decay = value;
            }
        }

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
            }
        }

        public void Process(double[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                if (double.IsNaN(buffer[i]))
                {
                    buffer[i] = 0.0;
                }
                var peak = Math.Log10(1.0 + 10.0 * Math.Abs(buffer[i]));

                var ratio = (peak > _mean ? _attack : _decay) * TimeConst / _sampleRate;
                _mean = _mean * (1 - ratio) + peak * ratio;

                var factor = MeanLevel/_mean;
                if (factor > MaxFactor)
                {
                    factor = MaxFactor;
                }
                if (factor < MinFactor)
                {
                    factor = MinFactor;
                }
                buffer[i] = buffer[i] * factor;
            }
        }
    }
}
