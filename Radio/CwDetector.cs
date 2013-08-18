namespace SDRSharp.Radio
{
    public unsafe sealed class CwDetector
    {
        private Oscillator _bfo = new Oscillator();

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _bfo.Tick();
                audio[i] = (iq[i] * _bfo).Real;
            }
        }

        public double SampleRate
        {
            get { return _bfo.SampleRate; }
            set { _bfo.SampleRate = value; }
        }

        public int BfoFrequency
        {
            get { return (int) _bfo.Frequency; }
            set { _bfo.Frequency = value; }
        }
    }
}