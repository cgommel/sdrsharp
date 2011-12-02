using System;

namespace SDRSharp.Radio
{
    /// <summary>
    /// The theory behind this code is in section 4.2.1
    /// of http://www.digitalsignallabs.com/Digradio.pdf
    /// </summary>
    public class FmDetector
    {
        private const double AFGain = 0.001;
        private const double TimeConst = 0.000001;
        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);
        private FirFilter _hissFilter;
        private Complex _iqState;
        private double _noiseLevel;
        private int _sampleRate;
        private double _noiseAveragingRatio;
        private double _squelchThreshold = 0.07;

        public void Demodulate(Complex[] iq, double[] audio)
        {
            audio[0] = GetAudio(_iqState, iq[0]);
            for (var i = 1; i < iq.Length; i++)
            {
                audio[i] = GetAudio(iq[i - 1], iq[i]);
            }
            _iqState = iq[iq.Length - 1];
            _dcRemover.Process(audio);
        }

        public double GetAudio(Complex z0, Complex z1)
        {
            // Polar discriminator
            var f = z1 * z0.Conjugate();
            // Limiting
            var m = f.Modulus();
            if (m > 0.0)
            {
                f /= m;
            }
            // Angle estimate
            var a = f.Argument();

            // Squelch
            var hiss = _hissFilter.Process(a);
            var n = (1 - _noiseAveragingRatio) * _noiseLevel + _noiseAveragingRatio * Math.Abs(hiss);
            if (!double.IsNaN(n))
            {
                _noiseLevel = n;
            }
            if (_noiseLevel > SquelchThreshold)
            {
                return 0;
            }

            return a * AFGain;
        }

        public double Offset
        {
            get { return _dcRemover.Offset; }
        }

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
            set
            {
                _sampleRate = value;
                _noiseAveragingRatio = 100.0 / _sampleRate;
                _hissFilter = new FirFilter(FilterBuilder.MakeHighPassKernel(_sampleRate, 5, 10000, WindowType.Blackman));
            }
        }

        public double SquelchThreshold
        {
            get { return _squelchThreshold; }
            set { _squelchThreshold = value; }
        }
    }
}
