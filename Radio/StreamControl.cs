using System;
using System.Threading;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.Radio
{
    public unsafe delegate void BufferNeededDelegate(Complex* iqBuffer, float* audioBuffer, int length);

    public unsafe class StreamControl : IDisposable
    {
        private const int WaveBufferSize = 16 * 1024;
        private const int MinOutputSampleRate = 24000;
        
        private static readonly float _inputGain = (float) (0.01f * Math.Pow(10, Utils.GetDoubleSetting("inputGain", 0)));

        private float* _dspOutPtr;
        private UnsafeBuffer _dspOutBuffer;

        private float* _audioOutPtr;
        private UnsafeBuffer _audioOutBuffer;

        private Complex* _iqInPtr;
        private UnsafeBuffer _iqInBuffer;
        
        private Complex* _dspInPtr;
        private UnsafeBuffer _dspInBuffer;

        private WavePlayer _wavePlayer;
        private WaveRecorder _waveRecorder;
        private WaveDuplex _waveDuplex;
        private WaveFile _waveFile;
        private ComplexFifoStream _iqStream;
        private FloatFifoStream _audioStream;
        private Thread _waveReadThread;
        private Thread _dspThread;

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

        public StreamControl()
        {
            AudioGain = 10.0f;
        }

        ~StreamControl()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
            GC.SuppressFinalize(this);
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

        private void DuplexFiller(float* buffer, int frameCount)
        {
            #region Prepare buffers

            if (_dspInBuffer == null || _dspInBuffer.Length != frameCount)
            {
                _dspInBuffer = UnsafeBuffer.Create(frameCount, sizeof(Complex));
                _dspInPtr = (Complex*) _dspInBuffer;
            }

            if (_dspOutBuffer == null || _dspOutBuffer.Length != _dspInBuffer.Length)
            {
                _dspOutBuffer = UnsafeBuffer.Create(_dspInBuffer.Length, sizeof(float));
                _dspOutPtr = (float*) _dspOutBuffer;
            }

            #endregion

            FillIQ(buffer, _dspInPtr, frameCount);

            ProcessIQ();

            FillAudio(_dspOutPtr, buffer, _dspOutBuffer.Length);
        }

        private void PlayerFiller(float* buffer, int frameCount)
        {
            #region Prepare buffer

            if (_audioOutBuffer == null || _audioOutBuffer.Length != frameCount)
            {
                _audioOutBuffer = UnsafeBuffer.Create(frameCount, sizeof(float));
                _audioOutPtr = (float*) _audioOutBuffer;
            }

            #endregion

            _audioStream.Read(_audioOutPtr, 0, _audioOutBuffer.Length);

            FillAudio(_audioOutPtr, buffer, _audioOutBuffer.Length);
        }

        private void RecorderFiller(float* buffer, int frameCount)
        {
            if (_iqStream.Length > _inputBufferSize * 2)
            {
                return;
            }

            #region Prepare buffer

            if (_iqInBuffer == null || _iqInBuffer.Length != frameCount)
            {
                _iqInBuffer = UnsafeBuffer.Create(frameCount, sizeof(Complex));
                _iqInPtr = (Complex*) _iqInBuffer;
            }

            #endregion

            FillIQ(buffer, _iqInPtr, frameCount);

            _iqStream.Write(_iqInPtr, frameCount);
        }

        private void WaveFileProc()
        {
            var waveInBuffer = new Complex[WaveBufferSize];
            fixed (Complex* waveInPtr = waveInBuffer)
            {
                while (IsPlaying)
                {
                    if (_iqStream.Length < _inputBufferSize * 4)
                    {
                        _waveFile.Read(waveInBuffer, waveInBuffer.Length);
                        _iqStream.Write(waveInPtr, waveInBuffer.Length);
                    }
                    else
                    {
                        Thread.Sleep(1);
                    }
                }
            }
        }

        private void DSPProc()
        {
            #region Prepare buffers

            if (_dspInBuffer == null || _dspInBuffer.Length != _inputBufferSize)
            {
                _dspInBuffer = UnsafeBuffer.Create(_inputBufferSize, sizeof(Complex));
                _dspInPtr = (Complex*) _dspInBuffer;
            }

            if (_dspOutBuffer == null || _dspOutBuffer.Length != _outputBufferSize)
            {
                _dspOutBuffer = UnsafeBuffer.Create(_outputBufferSize, sizeof(float));
                _dspOutPtr = (float*) _dspOutBuffer;
            }

            #endregion

            while (IsPlaying)
            {
                _iqStream.Read(_dspInPtr, _dspInBuffer.Length);
                ProcessIQ();
                _audioStream.Write(_dspOutPtr, _dspOutBuffer.Length);
            }
        }

        private void ProcessIQ()
        {
            if (BufferNeeded != null)
            {
                if (_swapIQ)
                {
                    SwapIQBuffer();
                }
                BufferNeeded(_dspInPtr, _dspOutPtr, _dspInBuffer.Length);
            }
        }

        private static void FillIQ(float* source, Complex* destination, int length)
        {
            for (var i = 0; i < length; i++)
            {
                destination[i].Real = source[i * 2] * _inputGain;
                destination[i].Imag = source[i * 2 + 1] * _inputGain;
            }
        }

        private void FillAudio(float* monoSource, float* stereoDestination, int length)
        {
            for (var i = 0; i < length; i++)
            {
                var audio = monoSource[i] * _outputGain;
                stereoDestination[i * 2] = audio;
                stereoDestination[i * 2 + 1] = audio;
            }
        }

        private void SwapIQBuffer()
        {
            for (var i = 0; i < _dspInBuffer.Length; i++)
            {
                var temp = _dspInPtr[i].Real;
                _dspInPtr[i].Real = _dspInPtr[i].Imag;
                _dspInPtr[i].Imag = temp;
            }
        }

        public void Stop()
        {
            if (_wavePlayer != null)
            {
                _wavePlayer.Dispose();
                _wavePlayer = null;
            }
            if (_waveRecorder != null)
            {
                _waveRecorder.Dispose();
                _waveRecorder = null;
            }
            if (_waveDuplex != null)
            {
                _waveDuplex.Dispose();
                _waveDuplex = null;
            }
            _inputSampleRate = 0;
            if (_waveReadThread != null)
            {
                _waveReadThread.Join();
                _waveReadThread = null;
            }
            if (_iqStream != null)
            {
                _iqStream.Close();
            }
            if (_audioStream != null)
            {
                _audioStream.Close();
            }
            if (_dspThread != null)
            {
                _dspThread.Join();
                _dspThread = null;
            }
            if (_waveFile != null)
            {
                _waveFile.Dispose();
                _waveFile = null;
            }
            if (_iqStream != null)
            {
                _iqStream.Dispose();
                _iqStream = null;
            }
            if (_audioStream != null)
            {
                _audioStream.Dispose();
                _audioStream = null;
            }
            _dspOutBuffer = null;
            _audioOutBuffer = null;
            _iqInBuffer = null;
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
                        _iqStream = new ComplexFifoStream(true);
                        _audioStream = new FloatFifoStream(_outputBufferSize);
                        _waveRecorder = new WaveRecorder(_inputDevice, _inputSampleRate, _inputBufferSize, RecorderFiller);
                        _wavePlayer = new WavePlayer(_outputDevice, _outputSampleRate, _outputBufferSize, PlayerFiller);
                        _dspThread = new Thread(DSPProc);
                        _dspThread.Start();
                    }
                }
                else
                {
                    _iqStream = new ComplexFifoStream(true);
                    _audioStream = new FloatFifoStream(_outputBufferSize);
                    _wavePlayer = new WavePlayer(_outputDevice, _outputSampleRate, _outputBufferSize, PlayerFiller);
                    _waveReadThread = new Thread(WaveFileProc);
                    _waveReadThread.Start();
                    _dspThread = new Thread(DSPProc);
                    _dspThread.Start();
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

            if (_inputDevice == _outputDevice)
            {
                _decimationFactor = 1;
                _outputSampleRate = _inputSampleRate;
                _outputBufferSize = _inputBufferSize;
            }
            else
            {
                _decimationFactor = GetDecimationFactor();
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
            if (_inputSampleRate <= MinOutputSampleRate)
            {
                return 1;
            }

            int result = 1024;
            while (_inputSampleRate < MinOutputSampleRate * result && result > 0)
            {
                result /= 2;
            }

            return result;
        }
    }
}