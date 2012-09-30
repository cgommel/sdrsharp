using System;

namespace SDRSharp.Radio
{
    public unsafe sealed class AmDetector
    {
        private float _avg;
        private float _powerThreshold;
        private int _squelchThreshold;
        private bool _isSquelchOpen;

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                if (_squelchThreshold != value)
                {
                    _squelchThreshold = value;
                    _powerThreshold = (_squelchThreshold / 100.0f - 1.0f) * 100f - 50.0f;
                }
            }
        }

        public bool IsSquelchOpen
        {
            get { return _isSquelchOpen; }
        }

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = iq[i].Modulus();
                if (_squelchThreshold == 0)
                {
                    audio[i] = sample;
                }
                else
                {
                    var power = (float) (20.0f * Math.Log10(1e-60 + sample));
                    _avg = 0.99f * _avg + 0.01f * power;
                    _isSquelchOpen = _avg > _powerThreshold;
                    if (_isSquelchOpen)
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
}