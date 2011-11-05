namespace SDRSharp.Radio
{
    public class DcRemover
    {
        private readonly double _ratio;
        private double _mean;

        public DcRemover(double ratio)
        {
            _ratio = ratio;
        }

        public double Offset
        {
            get { return _mean; }
        }

        public void Process(double[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                _mean = _mean * (1 - _ratio) + buffer[i] * _ratio;
                buffer[i] = buffer[i] - _mean;
            }
        }
    }
}
