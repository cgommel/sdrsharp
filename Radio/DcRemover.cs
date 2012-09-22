using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    public unsafe struct DcRemover
    {
        private float _average;
        private float _ratio;
        private float _oneMinusRatio;

        public DcRemover(float ratio)
        {
            _ratio = ratio;
            _oneMinusRatio = 1.0f - ratio;
            _average = 0.0f;
        }

        public void Init(float ratio)
        {
            _ratio = ratio;
            _oneMinusRatio = 1.0f - ratio;
            _average = 0.0f;
        }

        public float Offset
        {
            get { return _average; }
        }

        public void Process(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                _average = _average * _oneMinusRatio + buffer[i] * _ratio;
                buffer[i] -= _average;
            }
        }

        public void ProcessInterleaved(float* buffer, int length)
        {
            length *= 2;

            for (var i = 0; i < length; i += 2)
            {
                _average = _average * _oneMinusRatio + buffer[i] * _ratio;
                buffer[i] -= _average;
            }
        }

        public void Reset()
        {
            _average = 0.0f;
        }
    }
}
