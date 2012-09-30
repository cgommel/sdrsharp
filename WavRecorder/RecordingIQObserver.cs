using SDRSharp.Radio;

namespace SDRSharp.WavRecorder
{
    public unsafe class RecordingIQObserver: IIQObserver
    {
        public delegate void IQReadyDelegate(Complex *buffer, int length);
        public event IQReadyDelegate IQReady;        

        private volatile bool _enabled;
        private double _sampleRate;
        
        public bool Enabled
        {
            get { return _enabled; }
            set { _enabled = value; }
        }

        public double SampleRate
        {
            set { _sampleRate = value; }
            get { return _sampleRate; }
        }

        public void IQSamplesAvailable(Complex* buffer, int length)
        {
            var handler = IQReady;                        
            if (handler != null)
            {
                IQReady(buffer, length);
            }
        }        
    }
}
