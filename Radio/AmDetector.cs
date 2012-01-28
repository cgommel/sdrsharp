namespace SDRSharp.Radio
{
    public unsafe class AmDetector
    {
        private const float TimeConst = 0.01f;
        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);

        public void Demodulate(Complex* iq, float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var sample = iq[i].Modulus();
                audio[i] = sample;
            }
            _dcRemover.Process(audio, length);
        }

        public float Amplitude
        {
            get { return _dcRemover.Offset; }
        }
    }
}