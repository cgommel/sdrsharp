namespace SDRSharp.Radio
{
    public unsafe sealed class AmDetector
    {
        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = iq[i].Modulus();
                audio[i] = sample;
            }
        }
    }
}