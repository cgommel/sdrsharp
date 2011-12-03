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
        private int _squelchThreshold;
        private double _noiseThreshold;

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
            if (_squelchThreshold > 0)
            {
                var hiss = _hissFilter.Process(a);
                var n = (1 - _noiseAveragingRatio) * _noiseLevel + _noiseAveragingRatio * Math.Abs(hiss);
                if (!double.IsNaN(n))
                {
                    _noiseLevel = n;
                }
                if (_noiseLevel > _noiseThreshold)
                {
                    return 0;
                }
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
                _noiseAveragingRatio = 150.0 / _sampleRate;
                var hpf = FilterBuilder.MakeHighPassKernel(_sampleRate, 5, 3000, WindowType.Blackman);
                _hissFilter = new FirFilter(hpf);
            }
        }

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                _squelchThreshold = value;
                _noiseThreshold = Math.Log10(2 - _squelchThreshold / 100.0);
            }
        }
    }
}
