using System.Runtime.InteropServices;
using WaveLib;

namespace SDRSharp.Radio
{
    public class WaveDevices
    {
        public static WAVEOUTCAPS[] GetDevCapsPlayback()
        {
            int waveOutDevicesCount = WaveNative.waveOutGetNumDevs();
            if (waveOutDevicesCount > 0)
            {
                WAVEOUTCAPS[] list = new WAVEOUTCAPS[waveOutDevicesCount + 1];
                for (int uDeviceID = -1; uDeviceID < waveOutDevicesCount; uDeviceID++)
                {
                    WAVEOUTCAPS waveOutCaps = new WAVEOUTCAPS();
                    WaveNative.waveOutGetDevCaps(uDeviceID, ref waveOutCaps, Marshal.SizeOf(typeof(WAVEOUTCAPS)));
                    list[uDeviceID + 1] = waveOutCaps;
                }
                return list;
            }
            return null;
       }

        public static WAVEINCAPS[] GetDevCapsRecording()
        {
            int waveInDevicesCount = WaveNative.waveInGetNumDevs();
            if (waveInDevicesCount > 0)
            {
                WAVEINCAPS[] list = new WAVEINCAPS[waveInDevicesCount + 1];
                for (int uDeviceID = -1; uDeviceID < waveInDevicesCount; uDeviceID++)
                {
                    WAVEINCAPS waveInCaps = new WAVEINCAPS();
                    WaveNative.waveInGetDevCaps(uDeviceID, ref waveInCaps, Marshal.SizeOf(typeof(WAVEINCAPS)));
                    list[uDeviceID + 1] = waveInCaps;
                }
                return list;
            }
            return null;
        }
    }
}
