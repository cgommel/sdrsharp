namespace SDRSharp.Radio
{
    public unsafe sealed class AmDetector
    {
        private float _avg;
        private float _threshold;
        private int _squelchThreshold;

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                if (_squelchThreshold != value)
                {
                    _squelchThreshold = value;
                    _threshold = 0.0001f * _squelchThreshold / 100.0f;
                }
            }
        }

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = iq[i].Modulus();
                _avg = 0.99f * _avg + 0.01f * sample;
                if (_avg > _threshold)
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