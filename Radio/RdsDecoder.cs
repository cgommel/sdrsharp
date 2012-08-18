using System;
using System.IO;
using System.Text;

namespace SDRSharp.Radio
{
    public unsafe class RdsDecoder
    {
        private const int PllDefaultFrequency = 57000;
        private const int PllRange = 12;
        private const int PllBandwith = 1;
        private const double RdsBitRate = 1187.5;

        private readonly Pll _pll = new Pll();
        private readonly Oscillator _osc = new Oscillator();
        private readonly IQFirFilter _baseBandFilter = new IQFirFilter();
        private readonly FirFilter _matchedFilter = new FirFilter();
        private readonly RdsDemod _demodulator = new RdsDemod();

        private UnsafeBuffer _rawBuffer;
        private Complex* _rawPtr;
        private UnsafeBuffer _magBuffer;
        private float* _magPtr;
        private IQDecimator _decimator;
        private double _sampleRate;
        private double _demodulationSampleRate;
        private int _decimationFactor;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (value != _sampleRate)
                {
                    _sampleRate = value;
                    Configure();
                }
            }
        }

        private void Configure()
        {
            _osc.SampleRate = _sampleRate;
            _osc.Frequency = PllDefaultFrequency;

            var decimationStageCount = 0;
            while (_sampleRate > 8000 * Math.Pow(2.0, decimationStageCount))
            {
                decimationStageCount++;
            }

            _decimator = new IQDecimator(decimationStageCount, _sampleRate);
            _decimationFactor = (int) Math.Pow(2.0, decimationStageCount);
            _demodulationSampleRate = _sampleRate / _decimationFactor;

            var coefficients = FilterBuilder.MakeLowPassKernel(_demodulationSampleRate, 400, 2400, WindowType.BlackmanHarris);
            _baseBandFilter.SetCoefficients(coefficients);

            _demodulator.SampleRate = _demodulationSampleRate;

            _pll.SampleRate = _demodulationSampleRate;
            _pll.DefaultFrequency = 0;
            _pll.Range = PllRange;
            _pll.Bandwidth = PllBandwith;

            var matchedFilterLength = (int) (_demodulationSampleRate / RdsBitRate);
            coefficients = new float[matchedFilterLength * 2 + 1];
            for (int i = 0; i <= matchedFilterLength; i++)
            {
                var t = i / _demodulationSampleRate;
                var x = t * RdsBitRate;
                var x64 = 64.0 * x;
                coefficients[i + matchedFilterLength] = (float) (.75 * Math.Cos(2.0 * 2.0 * Math.PI * x) * ((1.0 / (1.0 / x - x64)) - (1.0 / (9.0 / x - x64))));
                coefficients[matchedFilterLength - i] = (float) (-.75 * Math.Cos(2.0 * 2.0 * Math.PI * x) * ((1.0 / (1.0 / x - x64)) - (1.0 / (9.0 / x - x64))));
            }
            _matchedFilter.SetCoefficients(coefficients);
        }

        public void Process(float* baseBand, int length)
        {
            if (_rawBuffer == null || _rawBuffer.Length != length)
            {
                _rawBuffer = UnsafeBuffer.Create(length, sizeof(Complex));
                _rawPtr = (Complex*) _rawBuffer;
            }

            if (_magBuffer == null || _magBuffer.Length != length)
            {
                _magBuffer = UnsafeBuffer.Create(length, sizeof(float));
                _magPtr = (float*) _magBuffer;
            }

            // Downconvert
            for (var i = 0; i < length; i++)
            {
                _osc.Tick();
                _rawPtr[i] = _osc.Out * baseBand[i];
            }

            // Decimate
            _decimator.Process(_rawPtr, length);
            length /= _decimationFactor;

            // Filter
            _baseBandFilter.Process(_rawPtr, length);

            // PLL
            for (var i = 0; i < length; i++)
            {
                _magPtr[i] = _pll.Process(_rawPtr[i]).Imag;
            }
            //Console.WriteLine(_pll.IsLocked);

            // Matched filter
            _matchedFilter.Process(_magPtr, length);

            // Square
            for (var i = 0; i < length; i++)
            {
                _magPtr[i] = _magPtr[i] * _magPtr[i];
            }

            //var sb = new StringBuilder();
            //for (var i = 0; i < length; i++)
            //{
            //    sb.Append(_magPtr[i]);
            //    sb.AppendLine();
            //}
            //File.WriteAllText(@"c:\test.csv", sb.ToString());

            // To be continued
        }
    }
}
