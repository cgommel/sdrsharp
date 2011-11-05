namespace SDRSharp.Radio
{
    /// <summary>
    /// The theory behind this code is in section 4.2.1
    /// of http://www.digitalsignallabs.com/Digradio.pdf
    /// </summary>
    public class FmDetector
    {
        private const double AFGain = 0.001;
        private const double TimeConst = 0.000001;
        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);

        public void Demodulate(Complex[] iq, double[] audio)
        {
            for (var i = 1; i < iq.Length; i++)
            {
                // Polar discriminator
                var f = iq[i] * iq[i - 1].Conjugate();
                // Limiting
                var m = f.Modulus();
                if (m > 0.0)
                {
                    f /= m;
                }
                // Angle estimate
                var a = f.Argument();
                audio[i] = a * AFGain;
            }
            audio[0] = audio[1]; // Too cheap?
            _dcRemover.Process(audio);
        }

        public double Offset
        {
            get { return _dcRemover.Offset; }
        }
    }
}
