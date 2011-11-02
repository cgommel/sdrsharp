namespace SDRSharp.Radio
{
    public class FirFilter
    {
        private readonly double[] _coefficients;
        private readonly double[] _queue;
        private int _index;

        public FirFilter(double[] coefficients)
        {
            _index = 0;
            _coefficients = coefficients;
            _queue = new double[_coefficients.Length];
        }

        public double Process(double sample)
        {
            double result = 0.0;
            if (--_index < 0)
                _index = _coefficients.Length - 1;
            _queue[_index] = sample;
            for (int j = 0; j < _coefficients.Length; j++)
            {
                result += _queue[_index] * _coefficients[j];
                if (++_index >= _coefficients.Length)
                    _index = 0;
            }
            return result;
        }

        public void Process(double[] buffer)
        {
            for (var i = 0; i < buffer.Length; i++)
            {
                double result = 0.0;
                if (--_index < 0)
                    _index = _coefficients.Length - 1;
                _queue[_index] = buffer[i];
                for (int j = 0; j < _coefficients.Length; j++)
                {
                    result += _queue[_index] * _coefficients[j];
                    if (++_index >= _coefficients.Length)
                        _index = 0;
                }
                buffer[i] = result;
            }
        }
    }
}
