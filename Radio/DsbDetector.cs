namespace SDRSharp.Radio
{
    public unsafe class DsbDetector
    {
        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                audio[i] = iq[i].Real;
            }
        }
    }
}