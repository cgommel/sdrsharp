namespace SDRSharp.Radio
{
    /// <summary>
    /// The theory behind this code is in section 4.2.1
    /// of http://www.digitalsignallabs.com/Digradio.pdf
    /// </summary>
    public class FmDetector
    {
        private const double AFGain = 0.001;

        public void Demodulate(Complex[] iq, double[] audio)
        {
            UnityLimit(iq);
            for (var i = 1; i < iq.Length; i++)
            {
                var f = iq[i] * iq[i - 1].Conjugate();
                audio[i] = f.Phase() * AFGain;
            }
            audio[0] = audio[1]; // Too cheap?
        }

        private static void UnityLimit(Complex[] iq)
        {
            for (var i = 0; i < iq.Length; i++)
            {
                var m = iq[i].Modulus();
                if (m > 0.0)
                {
                    iq[i] /= m;
                }
            }
        }
    }
}
