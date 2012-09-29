using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public enum DecimationFilterType
    {
        Fast,
        Baseband,
        Audio
    }

    public static class DecimationKernels
    {
        #region Constants

        public const float Cic3Max = 0.5f - 0.4985f;
        public const float Hb11TapMax = 0.5f - 0.475f;
        public const float Hb15TapMax = 0.5f - 0.451f;
        public const float Hb19TapMax = 0.5f - 0.428f;
        public const float Hb23TapMax = 0.5f - 0.409f;
        public const float Hb27TapMax = 0.5f - 0.392f;
        public const float Hb31TapMax = 0.5f - 0.378f;
        public const float Hb35TapMax = 0.5f - 0.366f;
        public const float Hb39TapMax = 0.5f - 0.356f;
        public const float Hb43TapMax = 0.5f - 0.347f;
        public const float Hb47TapMax = 0.5f - 0.340f;
        public const float Hb51TapMax = 0.5f - 0.333f;

        public static readonly float[] Kernel11 =
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

        public static readonly float[] Kernel15 =
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

        public static readonly float[] Kernel19 =
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

        public static readonly float[] Kernel23 =
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

        public static readonly float[] Kernel27 =
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

        public static readonly float[] Kernel31 =
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

        public static readonly float[] Kernel35 =
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

        public static readonly float[] Kernel39 =
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

        public static readonly float[] Kernel43 =
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

        public static readonly float[] Kernel47 =
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

        public static readonly float[] Kernel51 =
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

        #endregion
    }

    public unsafe sealed class IQDecimator
    {
        private readonly bool _isMultithreaded;
        private readonly FloatDecimator _rDecimator;
        private readonly FloatDecimator _iDecimator;
        private readonly SharpEvent _event = new SharpEvent(false);

        public IQDecimator(int stageCount, double samplerate, bool useFastFilters, bool isMultithreaded)
        {
            _isMultithreaded = isMultithreaded;
            int childThreads;
            if (_isMultithreaded)
            {
                childThreads = Environment.ProcessorCount / 2;
            }
            else
            {
                childThreads = 1;
            }
            var filterType = useFastFilters ? DecimationFilterType.Fast : DecimationFilterType.Baseband;
            _rDecimator = new FloatDecimator(stageCount, samplerate, filterType, childThreads);
            _iDecimator = new FloatDecimator(stageCount, samplerate, filterType, childThreads);
        }

        public IQDecimator(int stageCount, double samplerate, bool useFastFilters) : this(stageCount, samplerate, useFastFilters, false)
        {
        }

        public IQDecimator(int stageCount, double samplerate) : this(stageCount, samplerate, false)
        {
        }

        public void Process(Complex* buffer, int length)
        {
            var rPtr = (float*) buffer;
            var iPtr = rPtr + 1;

            if (_isMultithreaded)
            {
                DSPThreadPool.QueueUserWorkItem(
                    delegate
                        {
                            _rDecimator.ProcessInterleaved(rPtr, length);
                            _event.Set();
                        });
            }
            else
            {
                _rDecimator.ProcessInterleaved(rPtr, length);
            }

            _iDecimator.ProcessInterleaved(iPtr, length);

            if (_isMultithreaded)
            {
                _event.WaitOne();
            }
        }

        public int StageCount
        {
            get { return _rDecimator.StageCount; }
        }
    }

    public unsafe sealed class FloatDecimator
    {
        private readonly int _stageCount;
        private readonly int _threadCount;
        private readonly int _cicCount;
        private readonly CicDecimator[,] _cicDecimators;
        private readonly FirFilter[] _firFilters;

        public FloatDecimator(int stageCount) : this(stageCount, 0, DecimationFilterType.Audio, 1)
        {
        }

        public FloatDecimator(int stageCount, double samplerate, DecimationFilterType filterType) :
            this(stageCount, samplerate, filterType, 1)
        {
        }

        public FloatDecimator(int stageCount, double samplerate, DecimationFilterType filterType, int threadCount)
        {
            _stageCount = stageCount;
            _threadCount = threadCount;

            _cicCount = 0;
            var firCount = 0;

            switch (filterType)
            {
                case DecimationFilterType.Fast:
                    _cicCount = stageCount;
                    break;

                case DecimationFilterType.Audio:
                    firCount = stageCount;
                    break;

                case DecimationFilterType.Baseband:
                    while (_cicCount < stageCount && samplerate >= 500000)
                    {
                        _cicCount++;
                        samplerate /= 2;
                    }
                    firCount = stageCount - _cicCount;
                    break;
            }

            _cicDecimators = new CicDecimator[threadCount, _cicCount];
            for (var i = 0; i < threadCount; i++)
            {
                for (var j = 0; j < _cicCount; j++)
                {
                    _cicDecimators[i, j] = new CicDecimator();
                }
            }

            var kernel = filterType == DecimationFilterType.Audio ? DecimationKernels.Kernel47 : DecimationKernels.Kernel23;
            _firFilters = new FirFilter[firCount];
            for (var i = 0; i < firCount; i++)
            {
                _firFilters[i] = new FirFilter(kernel);
            }
        }

        public int StageCount
        {
            get { return _stageCount; }
        }

        public void Process(float* buffer, int length)
        {
            DecimateStage1(buffer, length);
            length >>= _cicCount;
            DecimateStage2(buffer, length);
        }

        public void ProcessInterleaved(float* buffer, int length)
        {            
            DecimateStage1Interleaved(buffer, length);
            length >>= _cicCount;
            DecimateStage2Interleaved(buffer, length);
        }

        private void DecimateStage1(float* buffer, int sampleCount)
        {
            for (var n = 0; n < _cicCount; n++)
            {
                var contextId = 0;
                var chunk = buffer;
                var chunkLength = sampleCount;

                #region Filter and down-sample

                _cicDecimators[contextId, n].Process(chunk, chunkLength);

                #endregion

                sampleCount /= 2;
            }
        }

        private void DecimateStage2(float* buffer, int length)
        {
            for (var n = 0; n < _firFilters.Length; n++)
            {
                _firFilters[n].Process(buffer, length);
                length /= 2;
                for (int i = 0, j = 0; i < length; i++, j += 2)
                {
                    buffer[i] = buffer[j];
                }
            }
        }

        private void DecimateStage1Interleaved(float* buffer, int sampleCount)
        {
            for (var n = 0; n < _cicCount; n++)
            {
                var contextId = 0;
                var chunk = buffer;
                var chunkLength = sampleCount;

                #region Filter and down-sample

                _cicDecimators[contextId, n].ProcessInterleaved(chunk, chunkLength);

                #endregion

                sampleCount /= 2;
            }
        }

        private void DecimateStage2Interleaved(float* buffer, int length)
        {
            for (var n = 0; n < _firFilters.Length; n++)
            {
                _firFilters[n].ProcessInterleaved(buffer, length);
                for (int i = 0, j = 0; i < length; i += 2, j += 4)
                {
                    buffer[i] = buffer[j];
                }
                length /= 2;
            }
        }
    }

#if !__MonoCS__
    [StructLayout(LayoutKind.Sequential, Pack = 16)]
#endif
    public unsafe struct CicDecimator
    {
        private float _xOdd;
        private float _xEven;

        public float XOdd
        {
            get { return _xOdd; }
            set { _xOdd = value; }
        }

        public float XEven
        {
            get { return _xEven; }
            set { _xEven = value; }
        }

        public void Process(float* buffer, int length)
        {
            for (int i = 0, j = 0; i < length; i += 2, j++)
            {
                var even = buffer[i];
                var odd = buffer[i + 1];
                buffer[j] = (float) (0.125 * (odd + _xEven + 3.0 * (_xOdd + even)));
                _xOdd = odd;
                _xEven = even;
            }
        }

        public void ProcessInterleaved(float* buffer, int length)
        {
            length *= 2;
            for (int i = 0, j = 0; i < length; i += 4, j += 2)
            {
                var even = buffer[i];
                var odd = buffer[i + 2];
                buffer[j] = (float) (0.125 * (odd + _xEven + 3.0 * (_xOdd + even)));
                _xOdd = odd;
                _xEven = even;
            }
        }
    }
}
