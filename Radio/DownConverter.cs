using System;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe sealed class DownConverter
    {
        private readonly int _phaseCount;
        private readonly UnsafeBuffer _oscillatorsBuffer;
        private readonly Oscillator* _oscillators;
        private readonly SharpEvent _event = new SharpEvent(false);

        private double _sampleRate;
        private double _frequency;
        private int _completedCount;

        public DownConverter(int phaseCount)
        {
            _phaseCount = phaseCount;
            _oscillatorsBuffer = UnsafeBuffer.Create(sizeof(Oscillator) * phaseCount);
            _oscillators = (Oscillator*) _oscillatorsBuffer;
        }

        public DownConverter() : this(Environment.ProcessorCount)
        {
        }

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    Configure();
                }
            }
        }

        public double Frequency
        {
            get { return _frequency; }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    Configure();
                }
            }
        }

        private void Configure()
        {
            if (_sampleRate == default(double))
            {
                return;
            }

            var threadFrequency = _frequency * _phaseCount;

            for (var i = 0; i < _phaseCount; i++)
            {
                _oscillators[i].SampleRate = _sampleRate;
                _oscillators[i].Frequency = threadFrequency;
            }

            var targetAngularFrequency = 2.0 * Math.PI * _frequency / _sampleRate;
            var targetSin = (float) Math.Sin(targetAngularFrequency);
            var targetCos = (float) Math.Cos(targetAngularFrequency);

            var vectR = _oscillators[0].StateReal;
            var vectI = _oscillators[0].StateImag;

            for (var i = 1; i < _phaseCount; i++)
            {
                var outR = vectR * targetCos - vectI * targetSin;
                var outI = vectI * targetCos + vectR * targetSin;
                var oscGn = 1.95f - (outR * outR + outI * outI);
                vectR = oscGn * outR;
                vectI = oscGn * outI;

                _oscillators[i].StateReal = vectR;
                _oscillators[i].StateImag = vectI;
            }
        }

        public void Process(Complex* buffer, int length)
        {
            _completedCount = 0;

            for (var i = 1; i < _phaseCount; i++)
            {
                DSPThreadPool.QueueUserWorkItem(
                    delegate (object parameter)
                        {
                            var index = (int) parameter;
                            _oscillators[index].Mix(buffer, length, index, _phaseCount);
                            Interlocked.Increment(ref _completedCount);
                            _event.Set();
                        }, i);
            }

            _oscillators[0].Mix(buffer, length, 0, _phaseCount);

            if (_phaseCount > 1)
            {
                Interlocked.Increment(ref _completedCount);

                while (_completedCount < _phaseCount)
                {
                    _event.WaitOne();
                }
            }
        }
    }
}
