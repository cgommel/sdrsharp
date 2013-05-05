using System;
using SDRSharp.Radio;

namespace SDRSharp.DNR
{
    public unsafe abstract class FftProcessor
    {
        private const float OverlapRatio = 0.1f; // Must be less than 0.5f

        private readonly int _fftSize;
        private readonly int _halfSize;
        private readonly int _overlapSize;
        private readonly float _blendFactor;

        private UnsafeBuffer _fftBuffer;
        private Complex* _fftBufferPtr;
        private int _fftBufferPos;
                
        private UnsafeBuffer _overlapBuffer;
        private float* _overlapBufferPtr;

        private UnsafeBuffer _outOverlapBuffer;
        private float* _outOverlapPtr;

        private UnsafeBuffer _sampleBuffer;
        private float* _sampleBufferPtr;
        private int _sampleBufferHead;
        private int _sampleBufferTail;      
        
        public FftProcessor(int fftSize)
        {            
            _fftSize = fftSize;
            _halfSize = fftSize / 2;
            _overlapSize = (int) Math.Ceiling(_fftSize * OverlapRatio);
            _fftBufferPos = _halfSize;

            _blendFactor = 1.0f / _overlapSize;

            _fftBuffer = UnsafeBuffer.Create(fftSize, sizeof(Complex));
            _fftBufferPtr = (Complex*)_fftBuffer;

            _outOverlapBuffer = UnsafeBuffer.Create(_overlapSize, sizeof(float));
            _outOverlapPtr = (float*) _outOverlapBuffer;

            _overlapBuffer = UnsafeBuffer.Create(fftSize / 2, sizeof(float));
            _overlapBufferPtr = (float*)_overlapBuffer;

            _sampleBuffer = UnsafeBuffer.Create(fftSize, sizeof(float));
            _sampleBufferPtr = (float*)_sampleBuffer;
            _sampleBufferHead = _halfSize;
        }

        public void ProcessInterleaved(float *buffer, int length)
        {
            ProcessBuffer(buffer, length, 2);
        }

        public void Process(float* buffer, int length)
        {
            ProcessBuffer(buffer, length, 1);
        }

        
        private void ProcessBuffer(float* buffer, int length, int increment)
        {                                   
            var inOffset = 0;
            var outOffset = 0;            
            while (inOffset < length)
            {
                _fftBufferPtr[_fftBufferPos].Real = buffer[inOffset];
                _fftBufferPtr[_fftBufferPos++].Imag = 0.0f;

                if (_fftBufferPos == _fftSize)
                {
                    for (int i = _halfSize, j = 0; i < _fftSize; i++, j++)
                    {
                        _overlapBufferPtr[j] = _fftBufferPtr[i].Real;
                    }
                    
                    while (outOffset < length && _sampleBufferHead != _sampleBufferTail)
                    {
                        buffer[outOffset] = _sampleBufferPtr[_sampleBufferTail];
                        _sampleBufferTail = (_sampleBufferTail + 1) & (_fftSize - 1);                        
                        outOffset += increment;
                    }

                    FourierDNR.ForwardTransform(_fftBufferPtr, _fftSize);
                    
                    ProcessFft(_fftBufferPtr, _fftSize);
                    
                    FourierDNR.InverseTransform(_fftBufferPtr, _fftSize);

                    for (int i = 0, j = _halfSize - _overlapSize; i < _halfSize; i++, j++)
                    {
                        if (i < _overlapSize)
                        {
                            var alpha = i * _blendFactor;
                            _sampleBufferPtr[_sampleBufferHead] = alpha * _fftBufferPtr[j].Real + (1.0f - alpha) * _outOverlapPtr[i];
                        }
                        else
                        {
                            _sampleBufferPtr[_sampleBufferHead] = _fftBufferPtr[j].Real;
                        }

                        _sampleBufferHead = (_sampleBufferHead + 1) & (_fftSize - 1);
                    }

                    for (int i = 0, j = _fftSize - _overlapSize; i < _overlapSize; i++, j++)
                    {
                        _outOverlapPtr[i] = _fftBufferPtr[j].Real;
                    }

                    for (var i = 0; i < _halfSize; i++)
                    {
                        _fftBufferPtr[i].Real = _overlapBufferPtr[i];
                        _fftBufferPtr[i].Imag = 0.0f;
                    }                                                                                                   
                    _fftBufferPos = _halfSize;
                }

                inOffset += increment;                
            }

            while (outOffset < length && _sampleBufferHead != _sampleBufferTail)
            {
                buffer[outOffset] = _sampleBufferPtr[_sampleBufferTail];
                _sampleBufferTail = (_sampleBufferTail + 1) & (_fftSize - 1);                
                outOffset += increment;
            }                       
        }

        protected abstract void ProcessFft(Complex* buffer, int length);      
    }
}
