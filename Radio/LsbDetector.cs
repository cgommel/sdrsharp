namespace SDRSharp.Radio
{
    public unsafe sealed class LsbDetector
    {
        private readonly Oscillator _bfo = new Oscillator();

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _bfo.Tick();
                audio[i] = iq[i].Real * _bfo.OutQ - iq[i].Imag * _bfo.OutI;
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