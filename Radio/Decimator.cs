using System;

namespace SDRSharp.Radio
{
    public static class DecimationKernels
    {
        #region Constants

        private const float Cic3Max = 0.5f - 0.4985f;
        private const float Hb11TapMax = 0.5f - 0.475f;
        private const float Hb15TapMax = 0.5f - 0.451f;
        private const float Hb19TapMax = 0.5f - 0.428f;
        private const float Hb23TapMax = 0.5f - 0.409f;
        private const float Hb27TapMax = 0.5f - 0.392f;
        private const float Hb31TapMax = 0.5f - 0.378f;
        private const float Hb35TapMax = 0.5f - 0.366f;
        private const float Hb39TapMax = 0.5f - 0.356f;
        private const float Hb43TapMax = 0.5f - 0.347f;
        private const float Hb47TapMax = 0.5f - 0.340f;
        private const float Hb51TapMax = 0.5f - 0.333f;

        private static readonly float[] _kernel11 =
            {
                0.0060431029837374152f,
                0.0f,
                -0.049372515458761493f,
                0.0f,
                0.29332944952052842f,
                0.5f,
                0.29332944952052842f,
                0.0f,
                -0.049372515458761493f,
                0.0f,
                0.0060431029837374152f
            };

        private static readonly float[] _kernel15 =
            {
                -0.001442203300285281f,
                0.0f,
                0.013017512802724852f,
                0.0f,
                -0.061653278604903369f,
                0.0f,
                0.30007792316024057f,
                0.5f,
                0.30007792316024057f,
                0.0f,
                -0.061653278604903369f,
                0.0f,
                0.013017512802724852f,
                0.0f,
                -0.001442203300285281f
            };

        private static readonly float[] _kernel19 =
            {
                0.00042366527106480427f,
                0.0f,
                -0.0040717333369021894f,
                0.0f,
                0.019895653881950692f,
                0.0f,
                -0.070740034412329067f,
                0.0f,
                0.30449249772844139f,
                0.5f,
                0.30449249772844139f,
                0.0f,
                -0.070740034412329067f,
                0.0f,
                0.019895653881950692f,
                0.0f,
                -0.0040717333369021894f,
                0.0f,
                0.00042366527106480427f
            };

        private static readonly float[] _kernel23 =
            {
                -0.00014987651418332164f,
                0.0f,
                0.0014748633283609852f,
                0.0f,
                -0.0074416944990005314f,
                0.0f,
                0.026163522731980929f,
                0.0f,
                -0.077593699116544707f,
                0.0f,
                0.30754683719791986f,
                0.5f,
                0.30754683719791986f,
                0.0f,
                -0.077593699116544707f,
                0.0f,
                0.026163522731980929f,
                0.0f,
                -0.0074416944990005314f,
                0.0f,
                0.0014748633283609852f,
                0.0f,
                -0.00014987651418332164f
            };

        private static readonly float[] _kernel27 =
            {
                0.000063730426952664685f,
                0.0f,
                -0.00061985193978569082f,
                0.0f,
                0.0031512504783365756f,
                0.0f,
                -0.011173151342856621f,
                0.0f,
                0.03171888754393197f,
                0.0f,
                -0.082917863582770729f,
                0.0f,
                0.3097770473566307f,
                0.5f,
                0.3097770473566307f,
                0.0f,
                -0.082917863582770729f,
                0.0f,
                0.03171888754393197f,
                0.0f,
                -0.011173151342856621f,
                0.0f,
                0.0031512504783365756f,
                0.0f,
                -0.00061985193978569082f,
                0.0f,
                0.000063730426952664685f
            };

        private static readonly float[] _kernel31 =
            {
                -0.000030957335326552226f,
                0.0f,
                0.00029271992847303054f,
                0.0f,
                -0.0014770381124258423f,
                0.0f,
                0.0052539088990950535f,
                0.0f,
                -0.014856378748476874f,
                0.0f,
                0.036406651919555999f,
                0.0f,
                -0.08699862567952929f,
                0.0f,
                0.31140967076042625f,
                0.5f,
                0.31140967076042625f,
                0.0f,
                -0.08699862567952929f,
                0.0f,
                0.036406651919555999f,
                0.0f,
                -0.014856378748476874f,
                0.0f,
                0.0052539088990950535f,
                0.0f,
                -0.0014770381124258423f,
                0.0f,
                0.00029271992847303054f,
                0.0f,
                -0.000030957335326552226f
            };

        private static readonly float[] _kernel35 =
            {
                0.000017017718072971716f,
                0.0f,
                -0.00015425042851962818f,
                0.0f,
                0.00076219685751140838f,
                0.0f,
                -0.002691614694785393f,
                0.0f,
                0.0075927497927344764f,
                0.0f,
                -0.018325727896057686f,
                0.0f,
                0.040351004914363969f,
                0.0f,
                -0.090198224668969554f,
                0.0f,
                0.31264689763504327f,
                0.5f,
                0.31264689763504327f,
                0.0f,
                -0.090198224668969554f,
                0.0f,
                0.040351004914363969f,
                0.0f,
                -0.018325727896057686f,
                0.0f,
                0.0075927497927344764f,
                0.0f,
                -0.002691614694785393f,
                0.0f,
                0.00076219685751140838f,
                0.0f,
                -0.00015425042851962818f,
                0.0f,
                0.000017017718072971716f
            };

        private static readonly float[] _kernel39 =
            {
                -0.000010175082832074367f,
                0.0f,
                0.000088036416015024345f,
                0.0f,
                -0.00042370835558387595f,
                0.0f,
                0.0014772557414459019f,
                0.0f,
                -0.0041468438954260153f,
                0.0f,
                0.0099579126901608011f,
                0.0f,
                -0.021433527104289002f,
                0.0f,
                0.043598963493432855f,
                0.0f,
                -0.092695953625928404f,
                0.0f,
                0.31358799113382152f,
                0.5f,
                0.31358799113382152f,
                0f,
                -0.092695953625928404f,
                0.0f,
                0.043598963493432855f,
                0.0f,
                -0.021433527104289002f,
                0.0f,
                0.0099579126901608011f,
                0.0f,
                -0.0041468438954260153f,
                0.0f,
                0.0014772557414459019f,
                0.0f,
                -0.00042370835558387595f,
                0.0f,
                0.000088036416015024345f,
                0.0f,
                -0.000010175082832074367f
            };

        private static readonly float[] _kernel43 =
            {
                0.0000067666739082756387f,
                0.0f,
                -0.000055275221547958285f,
                0.0f,
                0.00025654074579418561f,
                0.0f,
                -0.0008748125689163153f,
                0.0f,
                0.0024249876017061502f,
                0.0f,
                -0.0057775190656021748f,
                0.0f,
                0.012299834239523121f,
                0.0f,
                -0.024244050662087069f,
                0.0f,
                0.046354303503099069f,
                0.0f,
                -0.094729903598633314f,
                0.0f,
                0.31433918020123208f,
                0.5f,
                0.31433918020123208f,
                0.0f,
                -0.094729903598633314f,
                0.0f,
                0.046354303503099069f,
                0.0f,
                -0.024244050662087069f,
                0.0f,
                0.012299834239523121f,
                0.0f,
                -0.0057775190656021748f,
                0.0f,
                0.0024249876017061502f,
                0.0f,
                -0.0008748125689163153f,
                0.0f,
                0.00025654074579418561f,
                0.0f,
                -0.000055275221547958285f,
                0.0f,
                0.0000067666739082756387f
            };

        private static readonly float[] _kernel47 =
            {
                -0.0000045298314172004251f,
                0.0f,
                0.000035333704512843228f,
                0.0f,
                -0.00015934776420643447f,
                0.0f,
                0.0005340788063118928f,
                0.0f,
                -0.0014667949695500761f,
                0.0f,
                0.0034792089350833247f,
                0.0f,
                -0.0073794356720317733f,
                0.0f,
                0.014393786384683398f,
                0.0f,
                -0.026586603160193314f,
                0.0f,
                0.048538673667907428f,
                0.0f,
                -0.09629115286535718f,
                0.0f,
                0.31490673428547367f,
                0.5f,
                0.31490673428547367f,
                0.0f,
                -0.09629115286535718f,
                0.0f,
                0.048538673667907428f,
                0.0f,
                -0.026586603160193314f,
                0.0f,
                0.014393786384683398f,
                0.0f,
                -0.0073794356720317733f,
                0.0f,
                0.0034792089350833247f,
                0.0f,
                -0.0014667949695500761f,
                0.0f,
                0.0005340788063118928f,
                0.0f,
                -0.00015934776420643447f,
                0.0f,
                0.000035333704512843228f,
                0.0f,
                -0.0000045298314172004251f
            };

        private static readonly float[] _kernel51 =
            {
                0.0000033359253688981639f,
                0.0f,
                -0.000024584155158361803f,
                0.0f,
                0.00010677777483317733f,
                0.0f,
                -0.00034890723143173914f,
                0.0f,
                0.00094239127078189603f,
                0.0f,
                -0.0022118302078923137f,
                0.0f,
                0.0046575030752162277f,
                0.0f,
                -0.0090130973415220566f,
                0.0f,
                0.016383673864361164f,
                0.0f,
                -0.028697281101743237f,
                0.0f,
                0.05043292242400841f,
                0.0f,
                -0.097611898315791965f,
                0.0f,
                0.31538104435015801f,
                0.5f,
                0.31538104435015801f,
                0.0f,
                -0.097611898315791965f,
                0.0f,
                0.05043292242400841f,
                0.0f,
                -0.028697281101743237f,
                0.0f,
                0.016383673864361164f,
                0.0f,
                -0.0090130973415220566f,
                0.0f,
                0.0046575030752162277f,
                0.0f,
                -0.0022118302078923137f,
                0.0f,
                0.00094239127078189603f,
                0.0f,
                -0.00034890723143173914f,
                0.0f,
                0.00010677777483317733f,
                0.0f,
                -0.000024584155158361803f,
                0.0f,
                0.0000033359253688981639f
            };

        private static readonly float[][] _kernels =
            {
                _kernel11,
                _kernel15,
                _kernel19,
                _kernel23,
                _kernel27,
                _kernel31,
                _kernel35,
                _kernel39,
                _kernel43,
                _kernel47,
                _kernel51
            };

        private static readonly float[] _bands = 
            {
                Hb11TapMax,
                Hb15TapMax,
                Hb19TapMax,
                Hb23TapMax,
                Hb27TapMax,
                Hb31TapMax,
                Hb35TapMax,
                Hb39TapMax,
                Hb43TapMax,
                Hb47TapMax,
                Hb51TapMax
            };

        #endregion

        public static IFilter[] GetIQFilters(int stageCount, double samplerate, bool useFastFilters)
        {
            var filters = new IFilter[stageCount];
            int i = 0;
            while (i < stageCount && samplerate >= 500000)
            {
                filters[i++] = new CicFilter();
                samplerate /= 2;
            }

            var kernel = useFastFilters ? _kernel11 : _kernel23;

            while (i < stageCount)
            {
                filters[i++] = new IQFirFilter(kernel);
            }

            return filters;
        }

        public static FirFilter[] GetFloatFilters(int stageCount)
        {
            var filters = new FirFilter[stageCount];

            for (var i = 0; i < stageCount; i++)
            {
                filters[i] = new FirFilter(_kernel47);
            }

            return filters;
        }
    }

    public unsafe interface IFilter : IDisposable
    {
        void Process(Complex* buffer, int length);
    }

    public unsafe sealed class IQDecimator : IFilter
    {
        private readonly IFilter[] _filters;

        public IQDecimator(int stageCount, double samplerate, bool useFastFilters)
        {
            _filters = DecimationKernels.GetIQFilters(stageCount, samplerate, useFastFilters);
        }

        public IQDecimator(int stageCount, double samplerate) : this(stageCount, samplerate, false)
        {
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
                length /= 2;
                for (int i = 0, j = 0; i < length; i++, j += 2)
                {
                    buffer[i] = buffer[j];
                }
            }
        }

        public int StageCount
        {
            get { return _filters.Length; }
        }
    }

    public unsafe sealed class FloatDecimator : IDisposable
    {
        private readonly FirFilter[] _filters;

        public FloatDecimator(int stageCount)
        {
            _filters = DecimationKernels.GetFloatFilters(stageCount);
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
                length /= 2;
                for (int i = 0, j = 0; i < length; i++, j += 2)
                {
                    buffer[i] = buffer[j];
                }
            }
        }

        public int StageCount
        {
            get { return _filters.Length; }
        }
    }

    public unsafe sealed class CicFilter : IFilter
    {
        private Complex _xOdd;
        private Complex _xEven;

        public void Process(Complex* buffer, int length)
        {
            int i;
            Complex even, odd;
            for (i = 0; i < length; i += 2)
            {
                even = buffer[i];
                odd = buffer[i + 1];
                buffer[i].Real = (float) (0.125 * (odd.Real + _xEven.Real + 3.0 * (_xOdd.Real + even.Real)));
                buffer[i].Imag = (float) (0.125 * (odd.Imag + _xEven.Imag + 3.0 * (_xOdd.Imag + even.Imag)));
                _xOdd = odd;
                _xEven = even;
            }
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}
