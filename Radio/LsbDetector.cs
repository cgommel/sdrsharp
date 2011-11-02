namespace SDRSharp.Radio
{
    public class LsbDetector
    {
        private readonly Oscillator _bfo = new Oscillator();

        public void Demodulate(Complex[] iq, double[] audio)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                _bfo.Tick();
                audio[i] = iq[i].Real * _bfo.OutQ - iq[i].Imag * _bfo.OutI;
            }
        }

        public int SampleRate
        {
            get { return _bfo.SampleRate; }
            set { _bfo.SampleRate = value; }
        }

        public int BfoFrequency
        {
            get { return _bfo.Frequency; }
            set { _bfo.Frequency = value; }
        }
    }
}