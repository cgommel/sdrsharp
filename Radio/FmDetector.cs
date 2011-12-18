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

        private const int MinHissFrequency = 4000;
        private const int MaxHissFrequency = 6000;
        private const int HissFilterOrder = 21;
        private const double HissFactor = 0.001;

        private readonly DcRemover _dcRemover = new DcRemover(TimeConst);
        private double[] _hissBuffer;
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
            ProcessSquelch(audio);
        }

        public double GetAudio(Complex previous, Complex current)
        {
            // Polar discriminator
            var f = current * previous.Conjugate();

            // Limiting
            var m = f.Modulus();
            if (m > 0.0)
            {
                f /= m;
            }

            // Angle estimate
            var a = f.Argument();

            return a * AFGain;
        }

        private void ProcessSquelch(double[] audio)
        {
            if (_squelchThreshold > 0)
            {
                if (_hissBuffer == null || _hissBuffer.Length != audio.Length)
                {
                    _hissBuffer = (double[]) audio.Clone();
                }
                else
                {
                    Array.Copy(audio, _hissBuffer, audio.Length);
                }

                _hissFilter.Process(_hissBuffer);

                for (var i = 0; i < _hissBuffer.Length; i++)
                {
                    var n = (1 - _noiseAveragingRatio) * _noiseLevel + _noiseAveragingRatio * Math.Abs(_hissBuffer[i]);
                    if (!double.IsNaN(n))
                    {
                        _noiseLevel = n;
                    }
                    if (_noiseLevel > _noiseThreshold)
                    {
                        audio[i] = 0.0;
                    }
                }
            }
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
                _noiseAveragingRatio = 30.0 / _sampleRate;
                var bpk = FilterBuilder.MakeBandPassKernel(_sampleRate, HissFilterOrder, MinHissFrequency, MaxHissFrequency, WindowType.BlackmanHarris);
                _hissFilter = new FirFilter(bpk);
            }
        }

        public int SquelchThreshold
        {
            get { return _squelchThreshold; }
            set
            {
                _squelchThreshold = value;
                _noiseThreshold = Math.Log10(2 - _squelchThreshold / 100.0) * HissFactor;
            }
        }
    }
}
