using System;
using System.IO;
using WaveLib;

namespace SDRSharp.Radio
{
    public delegate void BufferNeededDelegate(Complex[] iqBuffer, double[] audioBuffer);

    public class AudioControl
    {
        private const double InputGain = 0.01;
        private const int BufferSize = 1024 * 16;
        private const int MaxQueue = 2 * BufferSize;
        private const int BufferCount = 4;

        private byte[] _outBuffer = new byte[BufferSize];
        private byte[] _inBuffer;
        private double[] _audioBuffer;
        private Complex[] _iqBuffer;
        private WaveOutPlayer _player;
        private WaveInRecorder _recorder;
        private Stream _audioStream;
        private WaveFormat _format;
        private string _source;
        private bool _isPCM;
        private int _sampleRate;
        private int _inputDevice;
        private int _outputDevice;

        public event BufferNeededDelegate BufferNeeded;

        public AudioControl()
        {
            IGain = 1.0;
            QGain = 1.0;
            AudioGain = 10.0;
        }

        public double IGain { get; set; }

        public double QGain { get; set; }

        public double AudioGain { get; set; }

        public bool SwapIQ { get; set; }

        public int SampleRate
        {
            get
            {
                return _sampleRate;
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
        }

        private void PlayerFiller(IntPtr data, int size)
        {
            int pos = 0;
            while (pos < size)
            {
                int toget = size - pos;
                int got = _audioStream.Read(_outBuffer, pos, toget);
                if (got < toget && _audioStream is WaveStream)
                    _audioStream.Position = 0; // loop if the file ends
                if (got <= 0)
                    break;
                pos += got;
            }

            if (BufferNeeded != null)
            {
                FillIQ();
                BufferNeeded(_iqBuffer, _audioBuffer);
                FillLR();
            }

            System.Runtime.InteropServices.Marshal.Copy(_outBuffer, 0, data, size);
        }

        private void RecorderFiller(IntPtr data, int size)
        {
            if (_audioStream.Length > MaxQueue)
            {
                return;
            }
            if (_inBuffer == null || _inBuffer.Length != size)
                _inBuffer = new byte[size];
            System.Runtime.InteropServices.Marshal.Copy(data, _inBuffer, 0, size);
            _audioStream.Write(_inBuffer, 0, _inBuffer.Length);
        }

        public void Stop()
        {
            if (_player != null)
            {
                try
                {
                    _player.Dispose();
                }
                finally
                {
                    _player = null;
                }
            }
            if (_recorder != null)
            {
                try
                {
                    _recorder.Dispose();
                }
                finally
                {
                    _recorder = null;
                }
            }
            if (_audioStream != null)
            {
                try
                {
                    _audioStream.Close();
                    _audioStream.Dispose();
                }
                finally
                {
                    _audioStream = null;
                }
            }
            _format = null;
            _source = null;
            _sampleRate = 0;
        }

        public bool Play()
        {
            if (_audioStream != null && _player == null && _recorder == null)
            {
                if (_audioStream is WaveStream)
                {
                    _audioStream.Position = 0;
                }
                else if (_audioStream is FifoStream)
                {
                    _recorder = new WaveInRecorder(_inputDevice, _format, BufferSize, BufferCount, RecorderFiller);
                }
                _player = new WaveOutPlayer(_outputDevice, _format, BufferSize, BufferCount, PlayerFiller);
                return true;
            }
            return false;
        }

        public void OpenDevice(int inputDevice, int outputDevice, int sampleRate)
        {
            Stop();

            _inputDevice = inputDevice;
            _outputDevice = outputDevice;
            _sampleRate = sampleRate;

            try
            {
                _source = "Sound card";
                _audioStream = new FifoStream();
                _format = new WaveFormat(_sampleRate, 32, 2);
                _isPCM = _format.wFormatTag == 1;
                var bytesPerSample = _isPCM ? 4 : 8;
                var nSamples = _outBuffer.Length / bytesPerSample;
                _iqBuffer = new Complex[nSamples];
                _audioBuffer = new double[nSamples];
            }
            catch (Exception)
            {
                Stop();
            }
        }

        public void OpenFile(string filename)
        {
            Stop();

            try
            {
                _source = filename;
                var stream = new WaveStream(filename);
                if (stream.Length <= 0)
                    throw new Exception("Invalid WAV file");
                if (stream.Format.wFormatTag != (short)WaveFormats.Pcm &&
                    stream.Format.wFormatTag != (short)WaveFormats.Float)
                    throw new Exception("Only PCM files are supported");
                _audioStream = stream;
                _format = stream.Format;
                _sampleRate = _format.nSamplesPerSec;
                _isPCM = _format.wFormatTag == 1;
                var bytesPerSample = _isPCM ? 4 : 8;
                var nSamples = _outBuffer.Length / bytesPerSample;
                _iqBuffer = new Complex[nSamples];
                _audioBuffer = new double[nSamples];
            }
            catch (Exception)
            {
                Stop();
            }
        }

        private unsafe void FillIQ()
        {
            var numReads = _iqBuffer.Length;

            fixed (Complex *iqPtr = _iqBuffer)
            fixed (byte* rawPtr = _outBuffer)
            {
                if (_isPCM)
                {
                    for (int i = 0; i < numReads; i++)
                    {
                        iqPtr[i].Real = *(Int16*)(rawPtr + i * 4) / 32767.0 * InputGain;
                        iqPtr[i].Imag = *(Int16*)(rawPtr + i * 4 + 2) / 32767.0 * InputGain;
                    }
                }
                else
                {
                    for (int i = 0; i < numReads; i++)
                    {
                        iqPtr[i].Real = *(float*)(rawPtr + i * 8) * InputGain;
                        iqPtr[i].Imag = *(float*)(rawPtr + i * 8 + 4) * InputGain;
                    }
                }
                for (int i = 0; i < numReads; i++)
                {
                    iqPtr[i].Real *= IGain;
                    iqPtr[i].Imag *= QGain;
                    if (SwapIQ)
                    {
                        var tmp = iqPtr[i].Real;
                        iqPtr[i].Real = iqPtr[i].Imag;
                        iqPtr[i].Imag = tmp;
                    }
                }
            }
        }

        private unsafe void FillLR()
        {
            double audioGain = Math.Pow(AudioGain / 10.0, 10);

            fixed (double* sourcePtr = _audioBuffer)
            fixed (byte* bufferPtr = _outBuffer)
            {
                if (_isPCM)
                {
                    for (var i = 0; i < _audioBuffer.Length; i++)
                    {
                        var i16 = (Int16) (_audioBuffer[i] * audioGain * 1000.0);
                        *(Int16*)(bufferPtr + i * 4) = i16;
                        *(Int16*)(bufferPtr + i * 4 + 2) = i16;
                    }
                }
                else
                {
                    for (var i = 0; i < _audioBuffer.Length; i++)
                    {
                        var f = (float)(sourcePtr[i] * audioGain);
                        *(float*)(bufferPtr + i * 8) = f;
                        *(float*)(bufferPtr + i * 8 + 4) = f;
                    }
                }
            }
        }
    }
}