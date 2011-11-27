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
        private Complex _iqState;

        public void Demodulate(Complex[] iq, double[] audio)
        {
            audio[0] = GetAudio(_iqState, iq[0]);
            for (var i = 1; i < iq.Length; i++)
            {
                audio[i] = GetAudio(iq[i - 1], iq[i]);
            }
            _iqState = iq[iq.Length - 1];
            _dcRemover.Process(audio);
        }

        public double GetAudio(Complex z0, Complex z1)
        {
            var m = z0.Modulus();
            if (m > 0.0)
            {
                z0 /= m;
            }
            m = z1.Modulus();
            if (m > 0.0)
            {
                z1 /= m;
            }

            // Polar discriminator
            var f = z1 * z0.Conjugate();
            // Limiting
            m = f.Modulus();
            if (m > 0.0)
            {
                f /= m;
            }
            // Angle estimate
            var a = f.Argument();
            return a * AFGain;
        }

        public double Offset
        {
            get { return _dcRemover.Offset; }
        }
    }
}
