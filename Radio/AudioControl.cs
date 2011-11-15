using System;
using SDRSharp.Radio.PortAudio;

namespace SDRSharp.Radio
{
    public delegate void BufferNeededDelegate(Complex[] iqBuffer, double[] audioBuffer);

    public class AudioControl
    {
        private const double InputGain = 0.01;

        private double[] _audioBuffer;
        private Complex[] _iqBuffer;
        private Complex[] _recorderIQBuffer;

        private WavePlayer _wavePlayer;
        private WaveRecorder _waveRecorder;
        private WaveDuplex _waveDuplex;
        private WaveFile _waveFile;
        private FifoStream<Complex> _audioStream;

        private double _audioGain;
        private double _outputGain;
        private int _sampleRate;
        private int _inputDevice;
        private int _bufferSizeInMs;
        private int _outputDevice;
        private bool _swapIQ;

        public event BufferNeededDelegate BufferNeeded;

        public AudioControl()
        {
            AudioGain = 10.0;
        }

        public double AudioGain
        {
            get
            {
                return _audioGain;
            }
            set
            {
                _audioGain = value;
                _outputGain = Math.Pow(value / 10.0, 10);
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

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
        }

        private void DuplexFiller(float[] buffer)
        {
            if (BufferNeeded == null)
            {
                return;
            }

            #region Fill IQ buffer

            if (_iqBuffer == null || _iqBuffer.Length != buffer.Length / 2)
            {
                _iqBuffer = new Complex[buffer.Length / 2];
            }

            FillIQ(buffer, _iqBuffer);

            #endregion

            #region Prepare audio buffer

            if (_audioBuffer == null || _audioBuffer.Length != buffer.Length / 2)
            {
                _audioBuffer = new double[buffer.Length / 2];
            }

            #endregion

            if (_swapIQ)
            {
                SwapIQBuffer();
            }

            BufferNeeded(_iqBuffer, _audioBuffer);

            #region Fill audio buffer

            FillAudio(buffer);

            #endregion
        }

        private void PlayerFiller(float[] buffer)
        {
            if (BufferNeeded == null)
            {
                return;
            }

            if (_audioBuffer == null || _audioBuffer.Length != buffer.Length / 2)
            {
                _audioBuffer = new double[buffer.Length / 2];
            }

            if (_iqBuffer == null || _iqBuffer.Length != buffer.Length / 2)
            {
                _iqBuffer = new Complex[buffer.Length / 2];
            }

            if (_waveFile != null)
            {
                _waveFile.Read(_iqBuffer);
            }
            else
            {
                _audioStream.Read(_iqBuffer, 0, _iqBuffer.Length);
            }

            if (_swapIQ)
            {
                SwapIQBuffer();
            }

            BufferNeeded(_iqBuffer, _audioBuffer);

            FillAudio(buffer);
        }

        private void RecorderFiller(float[] buffer)
        {
            if (_audioStream.Length > buffer.Length * 2)
            {
                return;
            }

            #region Fill IQ buffer

            if (_recorderIQBuffer == null || _recorderIQBuffer.Length != buffer.Length / 2)
            {
                _recorderIQBuffer = new Complex[buffer.Length / 2];
            }

            FillIQ(buffer, _recorderIQBuffer);

            #endregion

            #region Fill the FiFo

            _audioStream.Write(_recorderIQBuffer, 0, _recorderIQBuffer.Length);

            #endregion
        }

        private static void FillIQ(float[] buffer, Complex[] iqBuffer)
        {
            for (var i = 0; i < iqBuffer.Length; i++)
            {
                iqBuffer[i].Real = buffer[i * 2] * InputGain;
                iqBuffer[i].Imag = buffer[i * 2 + 1] * InputGain;
            }
        }

        private void FillAudio(float[] buffer)
        {
            for (var i = 0; i < _audioBuffer.Length; i++)
            {
                var audio = (float)(_audioBuffer[i] * _outputGain);
                buffer[i * 2] = audio;
                buffer[i * 2 + 1] = audio;
            }
        }

        private void SwapIQBuffer()
        {
            for (var i = 0; i < _iqBuffer.Length; i++)
            {
                var temp = _iqBuffer[i].Real;
                _iqBuffer[i].Real = _iqBuffer[i].Imag;
                _iqBuffer[i].Imag = temp;
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
            _sampleRate = 0;
        }

        public void Play()
        {
            if (_wavePlayer == null && _waveDuplex == null)
            {
                var bufferSize = _bufferSizeInMs * _sampleRate / 1000;

                if (_waveFile == null)
                {
                    if (_inputDevice == _outputDevice)
                    {
                        _waveDuplex = new WaveDuplex(_inputDevice, _sampleRate, bufferSize, DuplexFiller);
                    }
                    else
                    {
                        _audioStream = new FifoStream<Complex>();
                        _waveRecorder = new WaveRecorder(_inputDevice, _sampleRate, bufferSize, RecorderFiller);
                        _wavePlayer = new WavePlayer(_outputDevice, _sampleRate, bufferSize, PlayerFiller);
                    }
                }
                else
                {
                    _wavePlayer = new WavePlayer(_outputDevice, _sampleRate, bufferSize, PlayerFiller);
                }
            }
        }

        public void OpenDevice(int inputDevice, int outputDevice, int sampleRate, int bufferSizeInMs)
        {
            Stop();

            _bufferSizeInMs = bufferSizeInMs;
            _inputDevice = inputDevice;
            _outputDevice = outputDevice;
            _sampleRate = sampleRate;
        }

        public void OpenFile(string filename, int outputDevice, int bufferSizeInMs)
        {
            Stop();

            try
            {
                _outputDevice = outputDevice;
                _bufferSizeInMs = bufferSizeInMs;
                _waveFile = new WaveFile(filename);
                _sampleRate = _waveFile.SampleRate;

            }
            catch (Exception)
            {
                Stop();
            }
        }
    }
}