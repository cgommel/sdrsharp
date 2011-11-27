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
                var m = _mean * (1 - _ratio) + buffer[i] * _ratio;
                _mean = double.IsNaN(m) || double.IsInfinity(m) ? _mean : m;
                buffer[i] = buffer[i] - _mean;
            }
        }
    }
}
