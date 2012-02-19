using System;

namespace SDRSharp.Radio
{
    public unsafe class Decimator
    {
        private const int DefaultFilterOrder = 10;
        private readonly IQFirFilter[] _filters;
        private readonly int _decimationFactor;

        public Decimator(double sampleRate, int decimationFactor)
        {
            _decimationFactor = decimationFactor;
            var stages = (int) (Math.Log(decimationFactor) / Math.Log(2));
            _filters = new IQFirFilter[stages];
            for (var i = 0; i < stages; i++)
            {
                var kernel = FilterBuilder.MakeLowPassKernel(sampleRate, DefaultFilterOrder, (int)(sampleRate / _decimationFactor), WindowType.BlackmanHarris);
                _filters[i] = new IQFirFilter(kernel);
                sampleRate /= 2;
            }
        }

        public void Process(Complex* buffer, int length)
        {
            for (var n = 0; n < _filters.Length; n++)
            {
                _filters[n].Process(buffer, length);
                length /= 2;
                for (var i = 0; i < length; i++)
                {
                    buffer[i] = buffer[i * 2];
                }
            }
        }
    }
}
