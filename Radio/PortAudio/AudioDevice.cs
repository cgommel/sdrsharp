using System.Collections.Generic;
using PortAudioSharp;

namespace SDRSharp.Radio.PortAudio
{
    public class AudioDevice
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Host { get; set; }
        public DeviceDirection Direction { get; set; }
        public bool IsDefault { get; set; }

        public static List<AudioDevice> GetDevices(DeviceDirection direction)
        {
            var result = new List<AudioDevice>();

            var defaultIn = PortAudioAPI.Pa_GetDefaultInputDevice();
            var defaultOut = PortAudioAPI.Pa_GetDefaultOutputDevice();
            var count = PortAudioAPI.Pa_GetDeviceCount();
            for (var i = 0; i < count; i++)
            {
                var di = PortAudioAPI.Pa_GetDeviceInfo(i);
                var deviceDirection = di.maxInputChannels > 0 ? (di.maxOutputChannels > 0 ? DeviceDirection.InputOutput : DeviceDirection.Input) : DeviceDirection.Output;
                if (deviceDirection == direction || deviceDirection == DeviceDirection.InputOutput)
                {
                    var hi = PortAudioAPI.Pa_GetHostApiInfo(di.hostApi);
                    var ad = new AudioDevice();
                    ad.Name = di.name;
                    ad.Host = hi.name;
                    ad.Index = i;
                    ad.Direction = deviceDirection;
                    ad.IsDefault = i == defaultIn || i == defaultOut;
                    result.Add(ad);
                }
            }

            return result;
        }

        public override string ToString()
        {
            return "[" + Host + "] " + Name;
        }
    }
}
