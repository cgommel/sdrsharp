using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class AmDetector
    {
        private float _avg;
        private float _powerThreshold;
        private int _squelchThreshold;

        public AmDetector()
        {
            _powerThreshold = -200.0f;
        }

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                if (_squelchThreshold != value)
                {
                    _squelchThreshold = value;
                    _powerThreshold = (_squelchThreshold / 100f - 1.0f) * 70f - 70.0f;
                }
            }
        }

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = iq[i].Modulus();
                var power = (float) (20.0f * Math.Log10(1e-60 + sample));
                _avg = 0.99f * _avg + 0.01f * Math.Max(power, -130.0f);
                if (_avg > _powerThreshold)
                {
                    audio[i] = sample;
                }
                else
                {
                    audio[i] = 0f;
                }
            }
        }
    }
}