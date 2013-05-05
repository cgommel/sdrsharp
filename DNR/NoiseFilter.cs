using System;
using SDRSharp.Radio;

namespace SDRSharp.DNR
{
    public unsafe class NoiseFilter : FftProcessor
    {
        private const int WindowSize = 32;

        private float _noiseThreshold;

        private readonly UnsafeBuffer _gainBuffer;
        private readonly float* _gainPtr;

        private readonly UnsafeBuffer _smoothedGainBuffer;
        private readonly float* _smoothedGainPtr;

        private readonly UnsafeBuffer _powerBuffer;
        private readonly float* _powerPtr;

        public NoiseFilter(int fftSize)
            : base(fftSize)
        {
            _gainBuffer = UnsafeBuffer.Create(fftSize, sizeof(float));
            _gainPtr = (float*) _gainBuffer;

            _smoothedGainBuffer = UnsafeBuffer.Create(fftSize, sizeof(float));
            _smoothedGainPtr = (float*) _smoothedGainBuffer;

            _powerBuffer = UnsafeBuffer.Create(fftSize, sizeof(float));
            _powerPtr = (float*) _powerBuffer;
        }

        public float NoiseThreshold
        {
            get { return _noiseThreshold; }
            set
            {
                _noiseThreshold = value;
            }
        }

        protected override void ProcessFft(Complex* buffer, int length)
        {
            Fourier.SpectrumPower(buffer, _powerPtr, length);

            for (var i = 0; i < length; i++)
            {
                _gainPtr[i] = _powerPtr[i] > _noiseThreshold ? 1.0f : 0.0f;
            }

            for (var i = 0; i < length; i++)
            {
                var sum = 0.0f;

                for (var j = -WindowSize / 2; j < WindowSize / 2; j++)
                {
                    var index = i + j;
                    if (index >= length)
                    {
                        index -= length;
                    }
                    if (index < 0)
                    {
                        index += length;
                    }
                    sum += _gainPtr[index];
                }

                var gain = sum / WindowSize;

                _smoothedGainPtr[i] = gain;
            }

            for (var i = 0; i < length; i++)
            {
                buffer[i] *= _smoothedGainPtr[i];
            }
        }
    }
}