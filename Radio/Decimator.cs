using System;

namespace SDRSharp.Radio
{
    public unsafe class Decimator
    {
        private static readonly float[] _filterKernel = {
          0.0001091015490441f,                 0,-0.0008931218291173f,                 0,
            0.00398874281589f,                 0, -0.01283769838303f,                 0,
            0.03400417599697f,                 0, -0.08499629276978f,                 0,
             0.3106255550843f,               0.5f,   0.3106255550843f,                 0,
           -0.08499629276978f,                 0,  0.03400417599697f,                 0,
           -0.01283769838303f,                 0,  0.00398874281589f,                 0,
          -0.0008931218291173f,                 0,0.0001091015490441f
        };

        private readonly IQFirFilter[] _filters;

        public Decimator(int decimationFactor)
        {
            var stages = (int) (Math.Log(decimationFactor) / Math.Log(2));
            _filters = new IQFirFilter[stages];
            for (var i = 0; i < stages; i++)
            {
                _filters[i] = new IQFirFilter(_filterKernel);
            }
        }

        public void Process(Complex* buffer, int length)
        {
            for (var n = 0; n < _filters.Length; n++)
            {
                _filters[n].ProcessSparseSymmetricKernel(buffer, length);
                length /= 2;
                for (var i = 0; i < length; i++)
                {
                    buffer[i] = buffer[i * 2];
                }
            }
        }
    }
}
