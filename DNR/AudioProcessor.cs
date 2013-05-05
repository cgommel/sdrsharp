using SDRSharp.Radio;

namespace SDRSharp.DNR
{
    public unsafe class AudioProcessor : IAudioProcessor
    {
        private const int FftSize = 1024 * 4;
              
        private double _sampleRate;             
        private bool _bypass = true;
                
        private NoiseFilter _filter1;
        private NoiseFilter _filter2;

        private bool _needNewFilters;
        
        public double SampleRate
        {
            get { return _sampleRate; }
            set { _sampleRate = value; _needNewFilters = true; }
        }

        public bool Bypass
        {
            get { return _bypass; }
            set { _bypass = value; }
        }

        public int NoiseThreshold { get; set; }

        public void Process(float* buffer, int length)
        {
            if (_needNewFilters)
            {
                _filter1 = new NoiseFilter(FftSize);
                _filter2 = new NoiseFilter(FftSize);

                _needNewFilters = false;
            }

            _filter1.NoiseThreshold = NoiseThreshold;
            _filter2.NoiseThreshold = NoiseThreshold;

            _filter1.ProcessInterleaved(buffer, length);
            _filter2.ProcessInterleaved(buffer + 1, length);
        }
    }
}