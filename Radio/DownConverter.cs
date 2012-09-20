using System;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe sealed class DownConverter
    {
        private readonly Oscillator[] _oscillators;
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        private double _sampleRate;
        private double _frequency;
        private int _completedCount;
        private bool _configureNeeded;

        public DownConverter(int phaseCount)
        {
            _oscillators = new Oscillator[phaseCount];
            for (var i = 0; i < phaseCount; i++)
            {
                _oscillators[i] = new Oscillator();
            }
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
                    _configureNeeded = true;
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
                    _configureNeeded = true;
                }
            }
        }

        private void Configure()
        {
            var targetAngularFrequency = 2.0 * Math.PI * _frequency / _sampleRate;

            var targetSin = Math.Sin(targetAngularFrequency);
            var targetCos = Math.Cos(targetAngularFrequency);

            var threadFrequency = _frequency * _oscillators.Length;

            var vectR = _oscillators[0].StateReal;
            var vectI = _oscillators[0].StateImag;

            for (var i = 0; i < _oscillators.Length; i++)
            {
                _oscillators[i].SampleRate = _sampleRate;
                _oscillators[i].Frequency = threadFrequency;
                _oscillators[i].StateReal = vectR;
                _oscillators[i].StateImag = vectI;

                var outR = vectR * targetCos - vectI * targetSin;
                var outI = vectI * targetCos + vectR * targetSin;
                var oscGn = 1.95 - (vectR * vectR + vectI * vectI);
                vectR = oscGn * outR;
                vectI = oscGn * outI;
            }
        }

        public void Process(Complex* buffer, int length)
        {
            if (_configureNeeded)
            {
                Configure();
                _configureNeeded = false;
            }

            _completedCount = 0;

            for (var i = 1; i < _oscillators.Length; i++)
            {
                DSPThreadPool.QueueUserWorkItem(
                    delegate (object parameter)
                        {
                            var index = (int) parameter;
                            _oscillators[index].Mix(buffer, length, index, _oscillators.Length);
                            Interlocked.Increment(ref _completedCount);
                            _event.Set();
                        }, i);
            }

            _oscillators[0].Mix(buffer, length, 0, _oscillators.Length);

            if (_oscillators.Length > 1)
            {
                Interlocked.Increment(ref _completedCount);

                while (_completedCount < _oscillators.Length)
                {
                    _event.WaitOne();
                }
            }
        }
    }
}
