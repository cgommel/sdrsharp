namespace SDRSharp.Radio
{
    public unsafe sealed class DcRemover
    {
        private readonly float _ratio;
        private float _average;

        public DcRemover(float ratio)
        {
            _ratio = ratio;
        }

        public float Offset
        {
            get { return _average; }
        }

        public void Process(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var m = _average * (1 - _ratio) + buffer[i] * _ratio;
                _average = float.IsNaN(m) || float.IsInfinity(m) ? _average : m;
                buffer[i] = buffer[i] - _average;
            }
        }
    }
}
