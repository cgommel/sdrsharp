using System.Collections.Generic;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe class StreamHookManager
    {

        private readonly List<IAudioProcessor> _audioProcessors = new List<IAudioProcessor>();
        private readonly LinkedList<IAudioInterceptor> _audioInterceptors = new LinkedList<IAudioInterceptor>();
        private readonly List<IIQObserver> _iqObservers = new List<IIQObserver>();
       
        private FloatFifoStream _firstAudioStream;
        private FloatFifoStream _lastAudioStream;

        //private double _inputSampleRate;
        //private double _outputSampleRate;

        private bool _iqObserverThreadRunning;
        private Thread _iqObserverThread;
        private readonly AutoResetEvent _iqObserverEvent = new AutoResetEvent(false);
        private UnsafeBuffer _iqObserverBuffer;
        private Complex* _iqObserverBufferPtr;
       
        public double OutputSampleRate
        {
            set { /*_outputSampleRate = value;*/  SetOutputSampleRate(value); }
        }

        public double InputSampleRate
        {
            set { /*_inputSampleRate = value;*/ SetInputSampleRate(value); }
        }
       
        public bool HaveIqObservers
        {
            get { return (_iqObservers.Count > 0); }
        }

        public bool HaveAudioInterceptors
        {
            get { return (_audioInterceptors.Count > 0); }
        }

        public bool HaveAudioProcessors
        {
            get { return (_audioProcessors.Count > 0); }
        }
        
        public FloatFifoStream FirstAudioStream
        {
            get { return _firstAudioStream; }
        }

        public FloatFifoStream LastAudioStream
        {
            get { return _lastAudioStream; }
        }
        
        
        public void InitStreams(int inBufferSize, int outBufferSize)
        {

            InitAudioInterceptorStreams(outBufferSize);

        }

        public void Stop()
        {
            StopAudioInterceptors();
        }

        public void Start()
        {
            StartAudioInterceptors();
        }

        public void CloseStreams()
        {
            CloseInterceptorStreams();
        }

        public void DisposeStreams()
        {
            DisposeInterceptorStreams();
        }

        public void RegisterStreamHook(object streamHook)
        {
            if (streamHook == null)
            {
                return;
            }

            if (streamHook is IIQObserver)
            {
                lock (_iqObservers)
                {
                    _iqObservers.Add((IIQObserver)streamHook);
                }
            }
            else if (streamHook is IAudioProcessor)
            {
                lock (_audioProcessors)
                {
                    _audioProcessors.Add((IAudioProcessor)streamHook);
                }
            }
            else if (streamHook is IAudioInterceptor)
            {
                lock (_audioInterceptors)
                {
                    _audioInterceptors.AddLast((IAudioInterceptor)streamHook);
                }
            }
        }

        public void UnregisterStreamHook(object streamHook)
        {
            if (streamHook == null)
            {
                return;
            }

            if (streamHook is IIQObserver)
            {
                lock (_iqObservers)
                {
                    var hook = (IIQObserver)streamHook;
                    if (_iqObservers.Contains(hook))
                    {
                        _iqObservers.Remove(hook);
                    }
                }
            }
            else if (streamHook is IAudioInterceptor)
            {
                lock (_audioInterceptors)
                {
                    var hook = (IAudioInterceptor)streamHook;
                    if (_audioInterceptors.Contains(hook))
                    {
                        _audioInterceptors.Remove(hook);
                    }
                }
            }
            else if (streamHook is IAudioProcessor)
            {
                lock (_audioProcessors)
                {
                    var hook = (IAudioProcessor)streamHook;
                    if (_audioProcessors.Contains(hook))
                    {
                        _audioProcessors.Remove(hook);
                    }
                }
            }
        }

        #region Private methods

        private void SetOutputSampleRate(double newSampleRate)
        {
            lock (_audioProcessors)
            {
                for (var i = 0; i < _audioProcessors.Count; i++)
                {
                    if (_audioProcessors[i] != null)
                    {
                        _audioProcessors[i].SampleRate = newSampleRate;
                    }
                }
            }

            lock (_audioInterceptors)
            {
                foreach (IAudioInterceptor interceptor in _audioInterceptors)
                {
                    if (interceptor != null)
                    {
                        interceptor.SampleRate = newSampleRate;
                    }
                }
            }
        }

        private void SetInputSampleRate(double newSampleRate)
        {
            lock (_iqObservers)
            {
                for (var i = 0; i < _iqObservers.Count; i++)
                {

                    if (_iqObservers[i] != null)
                    {
                        _iqObservers[i].SampleRate = newSampleRate;

                    }
                }
            }
        }

        #endregion

        #region IIqObserver

        public void ProcessIQHook(Complex* buffer, int length)
        {
            var copyBuffer = false;                        
            for (var i = 0; i < _iqObservers.Count; i++)
            {
                if (_iqObservers[i].Enabled)
                {
                    copyBuffer = true;
                    break;
                }
            }

            if (copyBuffer)
            {
                if (_iqObserverThread.ThreadState == ThreadState.WaitSleepJoin)
                {
                    Utils.Memcpy(_iqObserverBufferPtr, buffer, length * sizeof(Complex));
                    _iqObserverEvent.Set();
                }
            }
        }

        private void IQObserverThread()
        {
            while (_iqObserverThreadRunning)
            {
                _iqObserverEvent.WaitOne();
                
                for (var i = 0; i < _iqObservers.Count; i++)
                {
                    if (_iqObservers[i].Enabled)
                    {
                        _iqObservers[i].IQSamplesAvailable(_iqObserverBufferPtr, _iqObserverBuffer.Length);
                    }
                }

            }
        }

        public void StartIQObserverThread(int bufferSize)
        {
            if (_iqObserverBuffer == null || _iqObserverBuffer.Length != bufferSize)
            {

                if (_iqObserverBuffer != null)
                {
                    _iqObserverBuffer.Dispose();
                    _iqObserverBuffer = null;
                }

                _iqObserverBuffer = UnsafeBuffer.Create(bufferSize, sizeof(Complex));
                _iqObserverBufferPtr = (Complex*)_iqObserverBuffer;
            }
                
            _iqObserverThread = new Thread(IQObserverThread);
            _iqObserverThread.Priority = ThreadPriority.BelowNormal;
            _iqObserverThread.Name = "IQObserverThread";

            _iqObserverThreadRunning = true;
            _iqObserverThread.Start();            
        }

        public void StopIQObserverThread()
        {
            if (_iqObserverThread != null)
            {
                _iqObserverThreadRunning = false;
                _iqObserverEvent.Set();
                _iqObserverThread.Join();
                _iqObserverThread = null;
            }
        }

        #endregion

        #region IAudioProcessor

        public void ProcessAudio(float* buffer, int length)
        {
            for (var i = 0; i < _audioProcessors.Count; i++)
            {
                if (_audioProcessors[i] != null && !_audioProcessors[i].Bypass)
                {
                    _audioProcessors[i].Process(buffer, length);
                }
            }
        }

        #endregion

        #region IAudioInterceptor

        private void InitAudioInterceptorStreams(int size)
        {
            var chainHead = new FloatFifoStream(size);
            lock (_audioInterceptors)
            {
                if (_audioInterceptors.Count == 0)
                {
                    _firstAudioStream = chainHead;
                    _lastAudioStream = chainHead;                   
                }
                else
                {

                    var nextInputFifo = chainHead;
                    foreach (IAudioInterceptor interceptor in _audioInterceptors)
                    {

                        interceptor.Input = nextInputFifo;
                        interceptor.Output = new FloatFifoStream(size);

                        interceptor.OutputBufferSize = size;
                        
                        nextInputFifo = interceptor.Output;
                    }

                    _firstAudioStream = _audioInterceptors.First.Value.Input;
                    _lastAudioStream = _audioInterceptors.Last.Value.Output;

                }
            }
        }

        private void StartAudioInterceptors()
        {
            lock (_audioInterceptors)
            {
                foreach (IAudioInterceptor interceptor in _audioInterceptors)
                {
                    if (interceptor != null)
                    {
                        interceptor.Start();
                    }
                }
            }
        }

        private void StopAudioInterceptors()
        {
            lock (_audioInterceptors)
            {
                foreach (IAudioInterceptor interceptor in _audioInterceptors)
                {
                    if (interceptor != null)
                    {
                        interceptor.Stop();
                    }
                }
            }
        }

        private void CloseInterceptorStreams()
        {
            if (_firstAudioStream != null)
            {
                _firstAudioStream.Close();
            }

            lock (_audioInterceptors)
            {
                foreach (IAudioInterceptor interceptor in _audioInterceptors)
                {
                    if (interceptor.Output != null)
                    {
                        interceptor.Output.Close();
                    }
                }
            }
        }

        private void DisposeInterceptorStreams()
        {
            if (_firstAudioStream != null)
            {
                _firstAudioStream.Dispose();
                _firstAudioStream = null;
            }

            lock (_audioInterceptors)
            {
                foreach (IAudioInterceptor interceptor in _audioInterceptors)
                {
                    if (interceptor.Output != null)
                    {
                        interceptor.Output.Dispose();
                        interceptor.Output = null;
                    }
                }
            }
        }
    
        #endregion

    }
}
