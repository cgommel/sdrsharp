using System;
using System.Threading;

using SDRSharp.Radio;

namespace SDRSharp.WavRecorder
{
    public enum RecordingMode
    {
        Baseband,
        Audio
    }

    public unsafe class SimpleRecorder: IDisposable
    {
        private const int DefaultAudioGain = 30;
        
        private static readonly int _bufferCount = Utils.GetIntSetting("RecordingBufferCount", 8);
        private readonly float _audioGain = (float) Math.Pow(DefaultAudioGain / 10.0, 10);

        private readonly SharpEvent _bufferEvent = new SharpEvent(false);
        
        private readonly UnsafeBuffer[] _circularBuffers = new UnsafeBuffer[_bufferCount];
        private readonly Complex*[] _complexCircularBufferPtrs = new Complex*[_bufferCount];
        private readonly float*[] _floatCircularBufferPtrs = new float*[_bufferCount];
        
        private int _circularBufferTail;
        private int _circularBufferHead;
        private int _circularBufferLength;
        
        private bool _diskWriterRunning;        
        private string _fileName;
        private double _sampleRate;
        private RecordingMode _recordingMode;

        private WavSampleFormat _wavSampleFormat;
        private SimpleWavWriter _wavWriter;
        private Thread _diskWriter;

        private readonly RecordingIQObserver _iQObserver;
        private readonly RecordingAudioProcessor _audioProcessor;

        public bool IsRecording
        {
            get { return _diskWriterRunning; }
        }

        public bool IsStreamFull
        {
            get { return _wavWriter == null ? false : _wavWriter.IsStreamFull; }
        }
        
        public long BytesWritten
        {
            get { return _wavWriter == null ? 0L : _wavWriter.Length; }            
        }

        public RecordingMode Mode
        {
            get { return _recordingMode; }
            set
            {
                if (_diskWriterRunning)
                {
                    throw new ArgumentException("Recording mode cannot set be while recording");
                }
                _recordingMode = value;
            }
        }
        
        public WavSampleFormat Format
        {
            get { return _wavSampleFormat; }
            set
            {
                if (_diskWriterRunning)
                {
                    throw new ArgumentException("Format cannot be set while recording");
                }
                _wavSampleFormat = value; 
            }
        }

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                if (_diskWriterRunning)
                {
                    throw new ArgumentException("SampleRate cannot be set while recording");
                }

                _sampleRate = value;
            }
        }

        public string FileName
        {
            get { return _fileName; }
            set
            {
                if (_diskWriterRunning)
                {
                    throw new ArgumentException("FileName cannot be set while recording");
                }
                _fileName = value;                
            }
        }

        #region Initialization and Termination

        public SimpleRecorder(RecordingIQObserver iQObserver, RecordingAudioProcessor audioProcessor)
        {
            _iQObserver = iQObserver;
            _audioProcessor = audioProcessor;
        }

        ~SimpleRecorder()
        {
            Dispose();
        }

        public void Dispose()
        {            
            FreeBuffers();
        }

        #endregion

        #region IQ Event Handler

        public void IQSamplesIn(Complex* buffer, int length)
        {
            
            #region Buffers

            if (_circularBufferLength != length)
            {
                FreeBuffers();
                CreateBuffers(length);

                _circularBufferTail = 0;
                _circularBufferHead = 0;
            }
            
            #endregion

            Utils.Memcpy(_complexCircularBufferPtrs[_circularBufferHead], buffer, length * sizeof(Complex));
            _circularBufferHead++;
            _circularBufferHead &= (_bufferCount - 1);
            _bufferEvent.Set();
        }

        #endregion

        #region Audio Event / Scaling

        public void AudioSamplesIn(float* audio, int length)
        {
            #region Buffers
            
            var sampleCount = length / 2;
            if (_circularBufferLength != sampleCount)
            {
                FreeBuffers();
                CreateBuffers(sampleCount);

                _circularBufferTail = 0;
                _circularBufferHead = 0;
            }

            #endregion

            Utils.Memcpy(_floatCircularBufferPtrs[_circularBufferHead], audio, length * sizeof(float));
            _circularBufferHead++;
            _circularBufferHead &= (_bufferCount - 1);
            _bufferEvent.Set();
        }

        public void ScaleAudio(float* audio, int length)
        {
            for (var i = 0; i < length; i++)
            {
                audio[i] *= _audioGain;
            }
        }

        #endregion

        #region Worker Thread

        private void DiskWriterThread()
        {
            if (_recordingMode == RecordingMode.Baseband)
            {
                _iQObserver.IQReady += IQSamplesIn;
                _iQObserver.Enabled = true;
            }
            else
            {
                _audioProcessor.AudioReady += AudioSamplesIn;
                _audioProcessor.Bypass = false;             
            }

            while (_diskWriterRunning && !_wavWriter.IsStreamFull)
            {
                if (_circularBufferTail == _circularBufferHead)
                {
                    _bufferEvent.WaitOne();
                }

                if (_diskWriterRunning && _circularBufferTail != _circularBufferHead)
                {
                    if (_recordingMode == RecordingMode.Audio)
                    {
                        ScaleAudio(_floatCircularBufferPtrs[_circularBufferTail], _circularBuffers[_circularBufferTail].Length * 2);
                    }

                    _wavWriter.Write(_floatCircularBufferPtrs[_circularBufferTail], _circularBuffers[_circularBufferTail].Length);

                    _circularBufferTail++;
                    _circularBufferTail &= (_bufferCount - 1);
                }
            }

            while (_circularBufferTail != _circularBufferHead)
            {
                if(_floatCircularBufferPtrs[_circularBufferTail] != null)
                {
                    _wavWriter.Write(_floatCircularBufferPtrs[_circularBufferTail], _circularBuffers[_circularBufferTail].Length);
                }
                _circularBufferTail++;
                _circularBufferTail &= (_bufferCount - 1);
            }

            if (_recordingMode == RecordingMode.Baseband)
            {
                _iQObserver.Enabled = false;
                _iQObserver.IQReady -= IQSamplesIn;
            }
            else
            {
                _audioProcessor.Bypass = true;
                _audioProcessor.AudioReady -= AudioSamplesIn;                
            }

            _diskWriterRunning = false;
        }

        private void Flush()
        {
            if (_wavWriter != null)
            {
                _wavWriter.Close();
            }
        }

        private void CreateBuffers(int size)
        {
            for (var i = 0; i < _bufferCount; i++)
            {
                _circularBuffers[i] = UnsafeBuffer.Create(size, sizeof(Complex));
                _complexCircularBufferPtrs[i] = (Complex*)_circularBuffers[i];
                _floatCircularBufferPtrs[i] = (float*)_circularBuffers[i];
            }

            _circularBufferLength = size;
        }

        private void FreeBuffers()
        {
            _circularBufferLength = 0;
            for (var i = 0; i < _bufferCount; i++)
            {
                if (_circularBuffers[i] != null)
                {
                    _circularBuffers[i].Dispose();
                    _circularBuffers[i] = null;
                    _complexCircularBufferPtrs[i] = null;
                    _floatCircularBufferPtrs[i] = null;
                }
            }
        }

        #endregion

        #region Public Methods

        public void StartRecording()
        {
            if (_diskWriter == null)
            {
                _circularBufferHead = 0;
                _circularBufferTail = 0;
                
                _bufferEvent.Reset();

                _wavWriter = new SimpleWavWriter(_fileName, _wavSampleFormat, (uint) _sampleRate);
                _wavWriter.Open();
                
                _diskWriter = new Thread(DiskWriterThread);
                
                _diskWriterRunning = true;
                _diskWriter.Start();
            }            
        }

        public void StopRecording()
        {
            _diskWriterRunning = false;

            if (_diskWriter != null)
            {
                _bufferEvent.Set();
                _diskWriter.Join();
            }

            Flush();
            FreeBuffers();

            _diskWriter = null;
            _wavWriter = null;
        }

        #endregion
    }
}
