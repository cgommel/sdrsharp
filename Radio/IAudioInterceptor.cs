using SDRSharp.Radio;

namespace SDRSharp.Radio
{
    public interface IAudioInterceptor
    {

        FloatFifoStream Input { get; set; } 
        FloatFifoStream Output { get; set; }

        double SampleRate { get; set; }

        int OutputBufferSize { set; }

        void Start(); 
        void Stop();        
        
    }
}
