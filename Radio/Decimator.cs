using System;

namespace SDRSharp.Radio
{
    public static class DecimationKernels
    {
        public static readonly float[] Kernel3 =
            {
                 0.294364576460385720f,
                 0.411270847079228510f,
                 0.294364576460385720f
            };

        public static readonly float[] Kernel13 =
            {
                 0.036530403450071476f,
                 0.000000000000000000f,
                -0.082652565132325037f,
                 0.000000000000000000f,
                 0.302621929129989590f,
                 0.487000465104527710f,
                 0.302621929129989590f,
                 0.000000000000000000f,
                -0.082652565132325037f,
                 0.000000000000000000f,
                 0.036530403450071476f
            };

        /// <summary>
        /// Contributed by N4IP
        /// </summary>
        public static readonly float[] Kernel27 = 
        {
           0.0001091015490441f,
           0f,
          -0.0008931218291173f,
           0f,
           0.00398874281589f,
           0f,
          -0.01283769838303f,
           0f,
           0.03400417599697f,
           0f,
          -0.08499629276978f,
           0f,
           0.3106255550843f,
           0.5f,
           0.3106255550843f,
           0f,
          -0.08499629276978f,
           0f,
           0.03400417599697f,
           0f,
          -0.01283769838303f,
           0f,
           0.00398874281589f,
           0f,
          -0.0008931218291173f,
           0f,
           0.0001091015490441f
        };

        /// <summary>
        /// remez(50, [0 0.4 0.5 1], [1 1 0 0])
        /// </summary>
        public static readonly float[] Kernel51 =
            {
              -1.52185189431147e-003f,
               2.39407323127730e-003f,
               3.02672488679517e-003f,
              -8.07123272361622e-004f,
              -4.09494525762120e-003f,
               1.50646390367273e-006f,
               6.06133319406013e-003f,
               2.30351198622223e-003f,
              -7.59073688406466e-003f,
              -5.93550115264379e-003f,
               8.22282559806108e-003f,
               1.10377901327734e-002f,
              -7.10008916508878e-003f,
              -1.74212516425997e-002f,
               3.24603510245672e-003f,
               2.46786856756526e-002f,
               4.58660676811564e-003f,
              -3.21842032135820e-002f,
              -1.83336086778423e-002f,
               3.91768843476922e-002f,
               4.23624382230808e-002f,
              -4.48800058356878e-002f,
              -9.24932524100208e-002f,
               4.86102511300211e-002f,
               3.13628520516380e-001f,
               4.50091223780026e-001f,
               3.13628520516380e-001f,
               4.86102511300211e-002f,
              -9.24932524100208e-002f,
              -4.48800058356878e-002f,
               4.23624382230808e-002f,
               3.91768843476922e-002f,
              -1.83336086778423e-002f,
              -3.21842032135820e-002f,
               4.58660676811564e-003f,
               2.46786856756526e-002f,
               3.24603510245672e-003f,
              -1.74212516425997e-002f,
              -7.10008916508878e-003f,
               1.10377901327734e-002f,
               8.22282559806108e-003f,
              -5.93550115264379e-003f,
              -7.59073688406466e-003f,
               2.30351198622223e-003f,
               6.06133319406013e-003f,
               1.50646390367273e-006f,
              -4.09494525762120e-003f,
              -8.07123272361622e-004f,
               3.02672488679517e-003f,
               2.39407323127730e-003f,
              -1.52185189431147e-003f
            };

        public static readonly float[][] Kernels =
            {
                //Kernel51,
                Kernel27,
                Kernel27,
                Kernel13,
                Kernel3
            };

        public static float[] GetKenrel(int stage, int totalStages)
        {
            stage = totalStages - 1 - stage;
            if (stage >= Kernels.Length)
            {
                return Kernels[Kernels.Length - 1];
            }
            return Kernels[stage];
        }
    }

    public unsafe class IQDecimator : IDisposable
    {
        private readonly IQFirFilter[] _filters;
        private readonly int _decimationFactor;

        public IQDecimator(int decimationFactor)
        {
            _decimationFactor = decimationFactor;
            var stages = (int) (Math.Log(decimationFactor) / Math.Log(2));
            _filters = new IQFirFilter[stages];
            for (var i = 0; i < stages; i++)
            {
                var kernel = DecimationKernels.GetKenrel(i, stages);
                _filters[i] = new IQFirFilter(kernel);
            }
        }

        ~IQDecimator()
        {
            Dispose();
        }

        public void Dispose()
        {
            for (var i = 0; i < _filters.Length; i++)
            {
                _filters[i].Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void Process(Complex* buffer, int length)
        {
            for (var n = 0; n < _filters.Length; n++)
            {
                _filters[n].Process(buffer, length);
                length >>= 1;
                for (var i = 0; i < length; i++)
                {
                    buffer[i] = buffer[i << 1];
                }
            }
        }

        public int Factor
        {
            get { return _decimationFactor; }
        }
    }


    public unsafe class FloatDecimator : IDisposable
    {
        private readonly FirFilter[] _filters;
        private readonly int _decimationFactor;

        public FloatDecimator(int decimationFactor)
        {
            _decimationFactor = decimationFactor;
            var stages = (int)(Math.Log(decimationFactor) / Math.Log(2));
            _filters = new FirFilter[stages];
            for (var i = 0; i < stages; i++)
            {
                var kernel = DecimationKernels.GetKenrel(i, stages);
                _filters[i] = new FirFilter(kernel);
            }
        }

        ~FloatDecimator()
        {
            Dispose();
        }

        public void Dispose()
        {
            for (var n = 0; n < _filters.Length; n++)
            {
                _filters[n].Dispose();
            }
            GC.SuppressFinalize(this);
        }

        public void Process(float* buffer, int length)
        {
            for (var n = 0; n < _filters.Length; n++)
            {
                _filters[n].Process(buffer, length);
                length >>= 1;
                for (var i = 0; i < length; i++)
                {
                    buffer[i] = buffer[i << 1];
                }
            }
        }

        public int Factor
        {
            get { return _decimationFactor; }
        }
    }
}
