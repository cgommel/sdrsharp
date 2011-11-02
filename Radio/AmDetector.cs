namespace SDRSharp.Radio
{
    public class AmDetector
    {
        private const double TimeConst = 0.000001;
        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);

        public void Demodulate(Complex[] iq, double[] audio)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                var sample = iq[i].Modulus();
                audio[i] = sample;
            }
            _dcRemover.Process(audio);
        }
    }
}