using System;
using System.Runtime.InteropServices;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.Radio
{
    public unsafe delegate void BufferNeededDelegate(Complex* iqBuffer, float* audioBuffer, int length);

    public unsafe class AudioControl : IDisposable
    {
        private const int MaxOutputSampleRate = 32000;
        private const float InputGain = 0.01f;

        private float[] _audioBuffer;
        private float* _audioPtr;
        private GCHandle _audioHandle;
        private Complex[] _iqBuffer;
        private Complex* _iqPtr;
        private GCHandle _iqHandle;
        private Complex[] _recorderIQBuffer;
        private Complex* _recorderIQPtr;
        private GCHandle _recorderIQHandle;

        private WavePlayer _wavePlayer;
        private WaveRecorder _waveRecorder;
        private WaveDuplex _waveDuplex;
        private WaveFile _waveFile;
        private FifoStream _audioStream;

        private float _audioGain;
        private float _outputGain;
        private int _inputDevice;
        private double _inputSampleRate;
        private int _inputBufferSize;
        private int _bufferSizeInMs;
        private int _outputDevice;
        private double _outputSampleRate;
        private int _outputBufferSize;
        private int _decimationFactor;
        private bool _swapIQ;

        public event BufferNeededDelegate BufferNeeded;

        public AudioControl()
        {
            AudioGain = 10.0f;
        }

        public void Dispose()
        {
            if (_iqHandle.IsAllocated)
            {
                _iqHandle.Free();
            }
            if (_audioHandle.IsAllocated)
            {
                _audioHandle.Free();
            }
            if (_recorderIQHandle.IsAllocated)
            {
                _recorderIQHandle.Free();
            }
        }

        public float AudioGain
        {
            get
            {
                return _audioGain;
            }
            set
            {
                _audioGain = value;
                _outputGain = (float) Math.Pow(value / 10.0, 10);
            }
        }

        public bool SwapIQ
        {
            get
            {
                return _swapIQ;
            }
            set
            {
                _swapIQ = value;
            }
        }

        public double SampleRate
        {
            get
            {
                return _inputSampleRate;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return _inputSampleRate != 0;
            }
        }

        public int BufferSize
        {
            get
            {
                return _inputBufferSize;
            }
        }

        public int BufferSizeInMs
        {
            get
            {
                return _bufferSizeInMs;
            }
        }

        public int DecimationFactor
        {
            get
            {
                return _decimationFactor;
            }
        }

        private void DuplexFiller(float* buffer, int length)
        {
            if (BufferNeeded == null)
            {
                return;
            }

            #region Fill IQ buffer

            if (_iqBuffer == null || _iqBuffer.Length < length)
            {
                if (_iqHandle.IsAllocated)
                {
                    _iqHandle.Free();
                }
                _iqBuffer = new Complex[length];
                _iqHandle = GCHandle.Alloc(_iqBuffer, GCHandleType.Pinned);
                _iqPtr = (Complex*) _iqHandle.AddrOfPinnedObject();
            }

            FillIQ(buffer, _iqPtr, length);

            #endregion

            #region Prepare audio buffer

            if (_audioBuffer == null || _audioBuffer.Length < length)
            {
                if (_audioHandle.IsAllocated)
                {
                    _audioHandle.Free();
                }
                _audioBuffer = new float[length];
                _audioHandle = GCHandle.Alloc(_audioBuffer, GCHandleType.Pinned);
                _audioPtr = (float*) _audioHandle.AddrOfPinnedObject();
            }

            #endregion

            if (_swapIQ)
            {
                SwapIQBuffer();
            }

            BufferNeeded(_iqPtr, _audioPtr, length);

            #region Fill audio buffer

            FillAudio(buffer, length);

            #endregion
        }

        private void PlayerFiller(float* buffer, int length)
        {
            if (BufferNeeded == null)
            {
                return;
            }

            if (_audioBuffer == null || _audioBuffer.Length < length)
            {
                if (_audioHandle.IsAllocated)
                {
                    _audioHandle.Free();
                }
                _audioBuffer = new float[length];
                _audioHandle = GCHandle.Alloc(_audioBuffer, GCHandleType.Pinned);
                _audioPtr = (float*) _audioHandle.AddrOfPinnedObject();
            }

            if (_iqBuffer == null || _iqBuffer.Length < length * _decimationFactor)
            {
                if (_iqHandle.IsAllocated)
                {
                    _iqHandle.Free();
                }
                _iqBuffer = new Complex[length * _decimationFactor];
                _iqHandle = GCHandle.Alloc(_iqBuffer, GCHandleType.Pinned);
                _iqPtr = (Complex*) _iqHandle.AddrOfPinnedObject();
            }

            if (_waveFile != null)
            {
                _waveFile.Read(_iqBuffer, length * _decimationFactor);
            }
            else
            {
                _audioStream.Read(_iqBuffer, 0, length * _decimationFactor);
            }

            if (_swapIQ)
            {
                SwapIQBuffer();
            }

            BufferNeeded(_iqPtr, _audioPtr, length * _decimationFactor);

            FillAudio(buffer, length);
        }

        private void RecorderFiller(float* buffer, int length)
        {
            if (_audioStream.Length > length * 4)
            {
                return;
            }

            #region Fill IQ buffer

            if (_recorderIQBuffer == null || _recorderIQBuffer.Length < length)
            {
                if (_recorderIQHandle.IsAllocated)
                {
                    _recorderIQHandle.Free();
                }
                _recorderIQBuffer = new Complex[length];
                _recorderIQHandle = GCHandle.Alloc(_recorderIQBuffer, GCHandleType.Pinned);
                _recorderIQPtr = (Complex*) _recorderIQHandle.AddrOfPinnedObject();
            }

            FillIQ(buffer, _recorderIQPtr, length);

            #endregion

            #region Fill the FiFo

            _audioStream.Write(_recorderIQBuffer, 0, _recorderIQBuffer.Length);

            #endregion
        }

        private static void FillIQ(float* buffer, Complex* iqBuffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                iqBuffer[i].Real = buffer[i * 2] * InputGain;
                iqBuffer[i].Imag = buffer[i * 2 + 1] * InputGain;
            }
        }

        private void FillAudio(float* buffer, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var audio = _audioPtr[i] * _outputGain;
                buffer[i * 2] = audio;
                buffer[i * 2 + 1] = audio;
            }
        }

        private void SwapIQBuffer()
        {
            for (var i = 0; i < _iqBuffer.Length; i++)
            {
                var temp = _iqPtr[i].Real;
                _iqPtr[i].Real = _iqPtr[i].Imag;
                _iqPtr[i].Imag = temp;
            }
        }

        public void Stop()
        {
            if (_wavePlayer != null)
            {
                try
                {
                    _wavePlayer.Dispose();
                }
                finally
                {
                    _wavePlayer = null;
                }
            }
            if (_waveRecorder != null)
            {
                try
                {
                    _waveRecorder.Dispose();
                }
                finally
                {
                    _waveRecorder = null;
                }
            }
            if (_audioStream != null)
            {
                try
                {
                    _audioStream.Close();
                }
                finally
                {
                    _audioStream = null;
                }
            }
            if (_waveDuplex != null)
            {
                try
                {
                    _waveDuplex.Dispose();
                }
                finally
                {
                    _waveDuplex = null;
                }
            }
            if (_waveFile != null)
            {
                try
                {
                    _waveFile.Dispose();
                }
                finally
                {
                    _waveFile = null;
                }
            }
            _inputSampleRate = 0;
        }

        public void Play()
        {
            if (_wavePlayer == null && _waveDuplex == null)
            {
                if (_waveFile == null)
                {
                    if (_inputDevice == _outputDevice)
                    {
                        _waveDuplex = new WaveDuplex(_inputDevice, _inputSampleRate, _inputBufferSize, DuplexFiller);
                    }
                    else
                    {
                        _audioStream = new FifoStream();
                        _waveRecorder = new WaveRecorder(_inputDevice, _inputSampleRate, _inputBufferSize, RecorderFiller);
                        _wavePlayer = new WavePlayer(_outputDevice, _outputSampleRate, _outputBufferSize, PlayerFiller);
                    }
                }
                else
                {
                    _wavePlayer = new WavePlayer(_outputDevice, _outputSampleRate, _outputBufferSize, PlayerFiller);
                }
            }
        }

        public void OpenDevice(int inputDevice, int outputDevice, int inputSampleRate, int bufferSizeInMs)
        {
            Stop();

            _inputDevice = inputDevice;
            _outputDevice = outputDevice;
            _inputSampleRate = inputSampleRate;
            _bufferSizeInMs = bufferSizeInMs;
            _inputBufferSize = (int)(_bufferSizeInMs * _inputSampleRate / 1000);

            _decimationFactor = GetDecimationFactor();

            if (_decimationFactor < 2 || _inputDevice == _outputDevice)
            {
                _outputSampleRate = _inputSampleRate;
                _outputBufferSize = _inputBufferSize;
            }
            else
            {
                _inputBufferSize = _inputBufferSize / _decimationFactor * _decimationFactor;
                _outputSampleRate = _inputSampleRate / _decimationFactor;
                _outputBufferSize = _inputBufferSize / _decimationFactor;
            }
        }

        public void OpenFile(string filename, int outputDevice, int bufferSizeInMs)
        {
            Stop();

            try
            {
                _waveFile = new WaveFile(filename);
                _outputDevice = outputDevice;
                _bufferSizeInMs = bufferSizeInMs;
                _inputSampleRate = _waveFile.SampleRate;
                _inputBufferSize = (int)(_bufferSizeInMs * _inputSampleRate / 1000);

                _decimationFactor = GetDecimationFactor();

                if (_decimationFactor < 2)
                {
                    _outputSampleRate = _inputSampleRate;
                    _outputBufferSize = _inputBufferSize;
                }
                else
                {
                    _inputBufferSize = _inputBufferSize / _decimationFactor * _decimationFactor;
                    _outputSampleRate = _inputSampleRate / _decimationFactor;
                    _outputBufferSize = _inputBufferSize / _decimationFactor;
                }
            }
            catch
            {
                Stop();
            }
        }

        private int GetDecimationFactor()
        {
            if (_inputSampleRate <= MaxOutputSampleRate)
            {
                return 1;
            }

            int result = 1;
            int suggestedRate;
            do
            {
                result *= 2;
                suggestedRate = MaxOutputSampleRate * result;
            } while (_inputSampleRate > suggestedRate);

            return result;
        }
    }
}