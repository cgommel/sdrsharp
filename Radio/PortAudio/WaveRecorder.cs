using System;
using System.Runtime.InteropServices;
using PortAudioSharp;

namespace SDRSharp.Radio.PortAudio
{
    public delegate void AudioBufferAvailableDelegate(float[] buffer);

    public class WaveRecorder : IDisposable
    {
        private IntPtr _streamHandle;
        private GCHandle _gcHandle;
        private float[] _callbackBuffer;
        private readonly AudioBufferAvailableDelegate _bufferAvailable;
        private readonly PaStreamCallbackDelegate _paCallback = PaStreamCallback;

        public WaveRecorder(int deviceIndex, int sampleRate, int framesPerBuffer, AudioBufferAvailableDelegate bufferAvailable)
        {
            _callbackBuffer = new float[framesPerBuffer * 2];
            _bufferAvailable = bufferAvailable;

            var inputParams = new PaStreamParameters();
            inputParams.device = deviceIndex;
            inputParams.channelCount = 2;
            inputParams.suggestedLatency = 0;
            inputParams.sampleFormat = PaSampleFormat.PaFloat32;

            var pe = PortAudioAPI.Pa_IsFormatSupported(ref inputParams, IntPtr.Zero, sampleRate);
            if (pe != PaError.paNoError)
            {
                throw new ApplicationException(pe.ToString());
            }

            _gcHandle = GCHandle.Alloc(this);

            pe = PortAudioAPI.Pa_OpenStream(
                out _streamHandle,
                ref inputParams,
                IntPtr.Zero,
                sampleRate,
                (uint) framesPerBuffer,
                PaStreamFlags.PaNoFlag,
                _paCallback,
                (IntPtr) _gcHandle);

            if (pe != PaError.paNoError)
            {
                _gcHandle.Free();
                throw new ApplicationException(pe.ToString());
            }

            pe = PortAudioAPI.Pa_StartStream(_streamHandle);
            if (pe != PaError.paNoError)
            {
                PortAudioAPI.Pa_CloseStream(_streamHandle);
                _gcHandle.Free();
                throw new ApplicationException(pe.ToString());
            }
        }

        private static PaStreamCallbackResult PaStreamCallback(
             IntPtr input,
             IntPtr output,
             uint frameCount,
            ref PaStreamCallbackTimeInfo timeInfo,
            PaStreamCallbackFlags statusFlags,
             IntPtr userData)
        {
            #region GC boilerplate

            var gcHandle = GCHandle.FromIntPtr(userData);
            if (!gcHandle.IsAllocated)
            {
                return PaStreamCallbackResult.PaAbort;
            }
            var instance = (WaveRecorder) gcHandle.Target;

            #endregion

            if (instance._callbackBuffer.Length != frameCount * 2)
            {
                instance._callbackBuffer = new float[frameCount * 2];
            }

            try
            {
                if (instance._bufferAvailable != null)
                {
                    Marshal.Copy(input, instance._callbackBuffer, 0, (int) frameCount * 2);
                    instance._bufferAvailable(instance._callbackBuffer);
                }
            }
            catch
            {
                return PaStreamCallbackResult.PaAbort;
            }

            return PaStreamCallbackResult.PaContinue;
         }

        public void Dispose()
        {
            if (_streamHandle != IntPtr.Zero)
            {
                PortAudioAPI.Pa_StopStream(_streamHandle);
                PortAudioAPI.Pa_CloseStream(_streamHandle);
                _streamHandle = IntPtr.Zero;
            }
            _gcHandle.Free();
        }
    }
}
