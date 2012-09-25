using System;
using System.Threading;
using System.Collections.Generic;

namespace SDRSharp.Radio
{

    public enum FifoStreamBlockingMode
    {
        None,
        BlockingRead,
        BlockingWrite
    };

    public unsafe sealed class ComplexFifoStream : IDisposable
    {
        private const int BlockSize = 65536 / 8; // 64k / sizeof(Complex)
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private bool _terminated;
        private readonly AutoResetEvent _event;
        private readonly Stack<UnsafeBuffer> _usedBlocks = new Stack<UnsafeBuffer>();
        private readonly List<UnsafeBuffer> _blocks = new List<UnsafeBuffer>();
        
        public ComplexFifoStream() : this(false)
        {
        }

        public ComplexFifoStream(bool blockingRead)
        {
            if (blockingRead)
            {
                _event = new AutoResetEvent(false);
            }
        }

        ~ComplexFifoStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_event != null)
            {
                _terminated = true;
                _event.Close();
            }
            GC.SuppressFinalize(this);
        }

        private UnsafeBuffer AllocBlock()
        {
            var result = _usedBlocks.Count > 0 ? _usedBlocks.Pop() : UnsafeBuffer.Create(BlockSize, sizeof(Complex));
            return result;
        }

        private void FreeBlock(UnsafeBuffer block)
        {
            if (_usedBlocks.Count < MaxBlocksInCache)
                _usedBlocks.Push(block);
        }

        private UnsafeBuffer GetWBlock()
        {
            UnsafeBuffer result;
            if (_writePos < BlockSize && _blocks.Count > 0)
                result = _blocks[_blocks.Count - 1];
            else
            {
                result = AllocBlock();
                _blocks.Add(result);
                _writePos = 0;
            }
            return result;
        }

        public int Length
        {
            get
            {
                return _size;
            }
        }

        public void Close()
        {
            Flush();
            if (_event != null)
            {                
                _terminated = true;
                _event.Set();
            }
        }

        public void Flush()
        {
            lock (this)
            {
                foreach (var block in _blocks)
                    FreeBlock(block);
                _blocks.Clear();
                _readPos = 0;
                _writePos = 0;
                _size = 0;
            }
        }

        public int Read(Complex* buf, int count)
        {
            return Read(buf, 0, count);
        }

        public int Read(Complex* buf, int ofs, int count)
        {
            if (_event != null)
            {
                while (_size == 0 && !_terminated)
                {
                    _event.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }
            lock (this)
            {
                int result = Peek(buf, ofs, count);
                Advance(result);
                return result;
            }
        }
        
        public void Write(Complex* buf, int ofs, int count)
        {
            lock (this)
            {
                int left = count;
                while (left > 0)
                {
                    int toWrite = Math.Min(BlockSize - _writePos, left);
                    var block = GetWBlock();
                    var blockPtr = (Complex*) block.Address;
                    Utils.Memcpy(blockPtr + _writePos, buf + ofs + count - left, toWrite * sizeof(Complex));
                    _writePos += toWrite;
                    left -= toWrite;
                }
                _size += count;
                if (_event != null)
                {
                    _event.Set();
                }
            }
        }
        
        public void Write(Complex* buf, int count)
        {
            Write(buf, 0, count);
        }

        // extra stuff
        public int Advance(int count)
        {
            lock (this)
            {
                int sizeLeft = count;
                while (sizeLeft > 0 && _size > 0)
                {
                    if (_readPos == BlockSize)
                    {
                        _readPos = 0;
                        FreeBlock(_blocks[0]);
                        _blocks.RemoveAt(0);
                    }
                    int toFeed = _blocks.Count == 1 ? Math.Min(_writePos - _readPos, sizeLeft) : Math.Min(BlockSize - _readPos, sizeLeft);
                    _readPos += toFeed;
                    sizeLeft -= toFeed;
                    _size -= toFeed;
                }
                return count - sizeLeft;
            }
        }

        public int Peek(Complex* buf, int ofs, int count)
        {
            lock (this)
            {
                int sizeLeft = count;
                int tempBlockPos = _readPos;
                int tempSize = _size;

                int currentBlock = 0;
                while (sizeLeft > 0 && tempSize > 0)
                {
                    if (tempBlockPos == BlockSize)
                    {
                        tempBlockPos = 0;
                        currentBlock++;
                    }
                    int upper = currentBlock < _blocks.Count - 1 ? BlockSize : _writePos;
                    int toFeed = Math.Min(upper - tempBlockPos, sizeLeft);
                    var block = _blocks[currentBlock];
                    var blockPtr = (Complex*) block.Address;
                    Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(Complex));
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
                }
                return count - sizeLeft;
            }
        }
    }

    public unsafe sealed class FloatFifoStream : IDisposable
    {

        private const int BlockSize = 65536 / 4; // 64k / sizeof(float)
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private bool _terminated;
        private readonly int _maxSize;
        private readonly FifoStreamBlockingMode _blockingMode;   
        private readonly AutoResetEvent _event;
        private readonly Stack<UnsafeBuffer> _usedBlocks = new Stack<UnsafeBuffer>();
        private readonly List<UnsafeBuffer> _blocks = new List<UnsafeBuffer>();

        public FloatFifoStream() : this(0)
        {
        }

        public FloatFifoStream(int maxSize): this(maxSize,FifoStreamBlockingMode.BlockingWrite)
        {           
        }

        public FloatFifoStream(int maxSize, FifoStreamBlockingMode blockingMode)
        {

            if (blockingMode != FifoStreamBlockingMode.None)
            {
                _event = new AutoResetEvent(true);
            }

            _maxSize = maxSize;
            _blockingMode = blockingMode;
        }

        ~FloatFifoStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_event != null)
            {
                _terminated = true;
                _event.Close();
            }
            GC.SuppressFinalize(this);
        }

        private UnsafeBuffer AllocBlock()
        {
            var result = _usedBlocks.Count > 0 ? _usedBlocks.Pop() : UnsafeBuffer.Create(BlockSize, sizeof(float));
            return result;
        }

        private void FreeBlock(UnsafeBuffer block)
        {
            if (_usedBlocks.Count < MaxBlocksInCache)
                _usedBlocks.Push(block);
        }

        private UnsafeBuffer GetWBlock()
        {
            UnsafeBuffer result;
            if (_writePos < BlockSize && _blocks.Count > 0)
                result = _blocks[_blocks.Count - 1];
            else
            {
                result = AllocBlock();
                _blocks.Add(result);
                _writePos = 0;
            }
            return result;
        }

        public int Length
        {
            get
            {
                return _size;
            }
        }

        public void Close()
        {
            Flush();
            if (_event != null)
            {
                _terminated = true;
                _event.Set();
            }
        }

        public void Flush()
        {
            lock (this)
            {
                foreach (var block in _blocks)
                    FreeBlock(block);
                _blocks.Clear();
                _readPos = 0;
                _writePos = 0;
                _size = 0;
            }
        }

        public int Read(float* buf, int ofs, int count)
        {
            int result;
            
            if (_blockingMode == FifoStreamBlockingMode.BlockingRead && _event != null)
            {
                while (_size==0 && !_terminated)
                {
                    _event.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }                        
            lock (this)
            {
                result = Peek(buf, ofs, count);
                Advance(result);
            }
            return result;
        }

        public void Write(float* buf, int ofs, int count)
        {
            if (_event != null)
            {
                while (_size >= _maxSize && !_terminated)
                {
                    _event.WaitOne();
                }
                if (_terminated)
                {
                    return;
                }
            }
            lock (this)
            {
                int left = count;
                while (left > 0)
                {
                    int toWrite = Math.Min(BlockSize - _writePos, left);
                    var block = GetWBlock();
                    var blockPtr = (float*) block;
                    Utils.Memcpy(blockPtr + _writePos, buf + ofs + count - left, toWrite * sizeof(float));
                    _writePos += toWrite;
                    left -= toWrite;
                }
                _size += count;
                if (_blockingMode == FifoStreamBlockingMode.BlockingRead && _event != null)
                {
                    _event.Set();
                }
            }
        }

        public void Write(float* buf, int count)
        {
            Write(buf, 0, count);
        }

        // extra stuff
        public int Advance(int count)
        {
            lock (this)
            {
                int sizeLeft = count;
                while (sizeLeft > 0 && _size > 0)
                {
                    if (_readPos == BlockSize)
                    {
                        _readPos = 0;
                        FreeBlock(_blocks[0]);
                        _blocks.RemoveAt(0);
                    }
                    var toFeed = _blocks.Count == 1 ? Math.Min(_writePos - _readPos, sizeLeft) : Math.Min(BlockSize - _readPos, sizeLeft);
                    _readPos += toFeed;
                    sizeLeft -= toFeed;
                    _size -= toFeed;
                }
                if (_blockingMode == FifoStreamBlockingMode.BlockingWrite && _event != null)
                {
                    _event.Set();
                }
                return count - sizeLeft;
            }
        }

        public int Peek(float* buf, int ofs, int count)
        {
            lock (this)
            {
                int sizeLeft = count;
                int tempBlockPos = _readPos;
                int tempSize = _size;

                int currentBlock = 0;
                while (sizeLeft > 0 && tempSize > 0)
                {
                    if (tempBlockPos == BlockSize)
                    {
                        tempBlockPos = 0;
                        currentBlock++;
                    }
                    int upper = currentBlock < _blocks.Count - 1 ? BlockSize : _writePos;
                    int toFeed = Math.Min(upper - tempBlockPos, sizeLeft);
                    var block = _blocks[currentBlock];
                    var blockPtr = (float*) block;
                    Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(float));
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
                }
                return count - sizeLeft;
            }
        }
    }
}
