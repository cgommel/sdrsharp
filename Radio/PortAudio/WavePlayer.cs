using System;
using System.Runtime.InteropServices;
using PortAudioSharp;

namespace SDRSharp.Radio.PortAudio
{
    public unsafe delegate void AudioBufferNeededDelegate(float* buffer, int length);

    public unsafe class WavePlayer : IDisposable
    {
        private IntPtr _streamHandle;
        private GCHandle _gcHandle;
        private readonly AudioBufferNeededDelegate _bufferNeeded;
        private readonly PaStreamCallbackDelegate _paCallback = PaStreamCallback;

        public WavePlayer(int deviceIndex, double sampleRate, int framesPerBuffer, AudioBufferNeededDelegate bufferNeededDelegate)
        {
            _bufferNeeded = bufferNeededDelegate;

            var ouputParams = new PaStreamParameters();
            ouputParams.device = deviceIndex;
            ouputParams.channelCount = 2;
            ouputParams.suggestedLatency = 0;
            ouputParams.sampleFormat = PaSampleFormat.PaFloat32;

            var pe = PortAudioAPI.Pa_IsFormatSupported(IntPtr.Zero, ref ouputParams, sampleRate);
            if (pe != PaError.paNoError)
            {
                throw new ApplicationException(pe.ToString());
            }

            _gcHandle = GCHandle.Alloc(this);

            pe = PortAudioAPI.Pa_OpenStream(
                out _streamHandle,
                IntPtr.Zero,
                ref ouputParams,
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
             float* input,
             float* output,
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
            var instance = (WavePlayer) gcHandle.Target;

            #endregion

            try
            {
                if (instance._bufferNeeded != null)
                {
                    instance._bufferNeeded(output, (int) frameCount);
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
