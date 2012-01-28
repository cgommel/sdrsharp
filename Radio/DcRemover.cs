namespace SDRSharp.Radio
{
    public unsafe class DcRemover
    {
        private readonly float _ratio;
        private float _mean;

        public DcRemover(float ratio)
        {
            _ratio = ratio;
        }

        public float Offset
        {
            get { return _mean; }
        }

        public void Process(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var m = _mean * (1 - _ratio) + buffer[i] * _ratio;
                _mean = float.IsNaN(m) || float.IsInfinity(m) ? _mean : m;
                buffer[i] = buffer[i] - _mean;
            }
        }
    }
}
