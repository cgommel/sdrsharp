using System;
using System.Collections.Generic;

namespace SDRSharp.Radio
{
    public enum BlockMode
    {
        None,
        BlockingRead,
        BlockingWrite,
        BlockingReadWrite
    }

    public unsafe sealed class ComplexFifoStream : IDisposable
    {
        private const int BlockSize = 65536 / 8; // 64k / sizeof(Complex)
        private const int MaxBlocksInCache = (4 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private bool _terminated;
        private readonly int _maxSize;  
        private readonly SharpEvent _writeEvent;
        private readonly SharpEvent _readEvent;
        private readonly Stack<UnsafeBuffer> _usedBlocks = new Stack<UnsafeBuffer>();
        private readonly List<UnsafeBuffer> _blocks = new List<UnsafeBuffer>();

        public ComplexFifoStream() : this(BlockMode.None)
        {
        }

        public ComplexFifoStream(BlockMode blockMode) : this(blockMode, 0)
        {
        }

        public ComplexFifoStream(BlockMode blockMode, int maxSize)
        {
            if (blockMode == BlockMode.BlockingRead || blockMode == BlockMode.BlockingReadWrite)
            {
                _readEvent = new SharpEvent(false);
            }

            if (blockMode == BlockMode.BlockingWrite || blockMode == BlockMode.BlockingReadWrite)
            {
                if (maxSize <= 0)
                {
                    throw new ArgumentException("MaxSize should be greater than zero when in blocking write mode", "maxSize");
                }
                _writeEvent = new SharpEvent(false);
            }

            _maxSize = maxSize;
        }

        ~ComplexFifoStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
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
            _terminated = true;
            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }
            if (_readEvent != null)
            {
                _readEvent.Set();
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

        public int Read(Complex* buf, int ofs, int count)
        {
            if (_readEvent != null)
            {
                while (_size == 0 && !_terminated)
                {
                    _readEvent.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }

            int result;

            lock (this)
            {
                result = DoPeek(buf, ofs, count);
                DoAdvance(result);
            }

            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }

            return result;
        }

        public void Write(Complex* buf, int ofs, int count)
        {
            if (_writeEvent != null)
            {
                while (_size >= _maxSize && !_terminated)
                {
                    _writeEvent.WaitOne();
                }
                if (_terminated)
                {
                    return;
                }
            }

            lock (this)
            {
                var left = count;
                while (left > 0)
                {
                    int toWrite = Math.Min(BlockSize - _writePos, left);
                    var block = GetWBlock();
                    var blockPtr = (Complex*) block;
                    Utils.Memcpy(blockPtr + _writePos, buf + ofs + count - left, toWrite * sizeof(Complex));
                    _writePos += toWrite;
                    left -= toWrite;
                }
                _size += count;
            }

            if (_readEvent != null)
            {
                _readEvent.Set();
            }
        }

        public int Read(Complex* buf, int count)
        {
            return Read(buf, 0, count);
        }

        public void Write(Complex* buf, int count)
        {
            Write(buf, 0, count);
        }

        private int DoAdvance(int count)
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
            return count - sizeLeft;
        }

        public int Advance(int count)
        {
            if (_readEvent != null)
            {
                while (_size == 0 && !_terminated)
                {
                    _readEvent.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }

            int result;

            lock (this)
            {
                result = DoAdvance(count);
            }

            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }

            return result;
        }

        private int DoPeek(Complex* buf, int ofs, int count)
        {
            var sizeLeft = count;
            var tempBlockPos = _readPos;
            var tempSize = _size;

            var currentBlock = 0;
            while (sizeLeft > 0 && tempSize > 0)
            {
                if (tempBlockPos == BlockSize)
                {
                    tempBlockPos = 0;
                    currentBlock++;
                }
                var upper = currentBlock < _blocks.Count - 1 ? BlockSize : _writePos;
                var toFeed = Math.Min(upper - tempBlockPos, sizeLeft);
                var block = _blocks[currentBlock];
                var blockPtr = (Complex*) block;
                Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(Complex));
                sizeLeft -= toFeed;
                tempBlockPos += toFeed;
                tempSize -= toFeed;
            }
            return count - sizeLeft;
        }

        public int Peek(Complex* buf, int ofs, int count)
        {
            lock (this)
            {
                return DoPeek(buf, ofs, count);
            }
        }
    }

    public unsafe sealed class FloatFifoStream : IDisposable
    {
        private const int BlockSize = 65536 / 4; // 64k / sizeof(float)
        private const int MaxBlocksInCache = (2 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private bool _terminated;
        private readonly int _maxSize;  
        private readonly SharpEvent _writeEvent;
        private readonly SharpEvent _readEvent;
        private readonly Stack<UnsafeBuffer> _usedBlocks = new Stack<UnsafeBuffer>();
        private readonly List<UnsafeBuffer> _blocks = new List<UnsafeBuffer>();

        public FloatFifoStream() : this(BlockMode.None)
        {
        }

        public FloatFifoStream(BlockMode blockMode) : this(blockMode, 0)
        {
        }

        public FloatFifoStream(BlockMode blockMode, int maxSize)
        {
            if (blockMode == BlockMode.BlockingRead || blockMode == BlockMode.BlockingReadWrite)
            {
                _readEvent = new SharpEvent(false);
            }

            if (blockMode == BlockMode.BlockingWrite || blockMode == BlockMode.BlockingReadWrite)
            {
                if (maxSize <= 0)
                {
                    throw new ArgumentException("MaxSize should be greater than zero when in blocking write mode", "maxSize");
                }
                _writeEvent = new SharpEvent(false);
            }

            _maxSize = maxSize;
        }

        ~FloatFifoStream()
        {
            Dispose();
        }

        public void Dispose()
        {
            Close();
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
            _terminated = true;
            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }
            if (_readEvent != null)
            {
                _readEvent.Set();
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
            if (_readEvent != null)
            {
                while (_size == 0 && !_terminated)
                {
                    _readEvent.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }

            int result;

            lock (this)
            {
                result = DoPeek(buf, ofs, count);
                DoAdvance(result);
            }

            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }

            return result;
        }

        public int Read(float* buf, int count)
        {
            return Read(buf, 0, count);
        }

        public void Write(float* buf, int ofs, int count)
        {
            if (_writeEvent != null)
            {
                while (_size >= _maxSize && !_terminated)
                {
                    _writeEvent.WaitOne();
                }
                if (_terminated)
                {
                    return;
                }
            }

            lock (this)
            {
                var left = count;
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
            }

            if (_readEvent != null)
            {
                _readEvent.Set();
            }
        }

        public void Write(float* buf, int count)
        {
            Write(buf, 0, count);
        }

        private int DoAdvance(int count)
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
            return count - sizeLeft;
        }

        public int Advance(int count)
        {
            if (_readEvent != null)
            {
                while (_size == 0 && !_terminated)
                {
                    _readEvent.WaitOne();
                }
                if (_terminated)
                {
                    return 0;
                }
            }

            int result;

            lock (this)
            {
                result = DoAdvance(count);
            }

            if (_writeEvent != null)
            {
                _writeEvent.Set();
            }

            return result;
        }

        private int DoPeek(float* buf, int ofs, int count)
        {
            var sizeLeft = count;
            var tempBlockPos = _readPos;
            var tempSize = _size;

            var currentBlock = 0;
            while (sizeLeft > 0 && tempSize > 0)
            {
                if (tempBlockPos == BlockSize)
                {
                    tempBlockPos = 0;
                    currentBlock++;
                }
                var upper = currentBlock < _blocks.Count - 1 ? BlockSize : _writePos;
                var toFeed = Math.Min(upper - tempBlockPos, sizeLeft);
                var block = _blocks[currentBlock];
                var blockPtr = (float*) block;
                Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(float));
                sizeLeft -= toFeed;
                tempBlockPos += toFeed;
                tempSize -= toFeed;
            }
            return count - sizeLeft;
        }

        public int Peek(float* buf, int ofs, int count)
        {
            lock (this)
            {
                return DoPeek(buf, ofs, count);
            }
        }
    }
}
