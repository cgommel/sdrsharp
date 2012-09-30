using SDRSharp.Radio;

namespace SDRSharp.WavRecorder
{    
    public unsafe class RecordingAudioProcessor: IAudioProcessor
    {
        public delegate void AudioReadyDelegate(float* audio, int length);

        private double _sampleRate;
        private bool _bypass;

        public event AudioReadyDelegate AudioReady;

        public bool Bypass
        {
            get { return _bypass; }
            set { _bypass = value; }
        }

        public double SampleRate
        {
            get { return _sampleRate; }
            set { _sampleRate = value; }
        }

        public void Process(float* audio, int length)
        {           
        
            var handler = AudioReady;
            if (handler != null)
            {
                handler(audio, length);
            }
        }
    }
}
