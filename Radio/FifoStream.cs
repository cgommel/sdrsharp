using System;
using System.Collections;
using System.Threading;

namespace SDRSharp.Radio
{
    public unsafe sealed class ComplexFifoStream : IDisposable
    {
        private const int BlockSize = 65536;
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private bool _terminated;
        private readonly AutoResetEvent _event;
        private readonly Stack _usedBlocks = new Stack();
        private readonly ArrayList _blocks = new ArrayList();

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

        public void Dispose()
        {
            if (_event != null)
            {
                _terminated = true;
                _event.Close();
            }
        }

        private Complex[] AllocBlock()
        {
            Complex[] result = _usedBlocks.Count > 0 ? (Complex[])_usedBlocks.Pop() : new Complex[BlockSize];
            return result;
        }

        private void FreeBlock(Complex[] block)
        {
            if (_usedBlocks.Count < MaxBlocksInCache)
                _usedBlocks.Push(block);
        }

        private Complex[] GetWBlock()
        {
            Complex[] result;
            if (_writePos < BlockSize && _blocks.Count > 0)
                result = (Complex[])_blocks[_blocks.Count - 1];
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
                foreach (Complex[] block in _blocks)
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
                while (_size < count && !_terminated)
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

        public int Read(Complex[] buf, int count)
        {
            return Read(buf, 0, count);
        }

        public int Read(Complex[] buf, int ofs, int count)
        {
            if (_event != null)
            {
                while (_size < count && !_terminated)
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
                    //Array.Copy(buf, ofs + count - left, GetWBlock(), _writePos, toWrite);
                    var block = GetWBlock();
                    fixed (Complex* blockPtr = block)
                    {
                        Utils.Memcpy(blockPtr + _writePos, buf + ofs + count - left, toWrite * sizeof(Complex));
                    }
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
                        FreeBlock((Complex[])_blocks[0]);
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

        public int Peek(Complex[] buf, int ofs, int count)
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
                    Array.Copy((Complex[])_blocks[currentBlock], tempBlockPos, buf, ofs + count - sizeLeft, toFeed);
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
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
                    //Array.Copy((Complex[])_blocks[currentBlock], tempBlockPos, buf, ofs + count - sizeLeft, toFeed);
                    var block = (Complex[]) _blocks[currentBlock];
                    fixed (Complex* blockPtr = block)
                    {
                        Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(Complex));
                    }
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
        private const int BlockSize = 65536;
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private int _maxSize;
        private bool _terminated;
        private readonly AutoResetEvent _event;
        private readonly Stack _usedBlocks = new Stack();
        private readonly ArrayList _blocks = new ArrayList();

        public FloatFifoStream() : this(0)
        {
        }

        public FloatFifoStream(int maxSize)
        {
            if (maxSize > 0)
            {
                _maxSize = maxSize;
                _event = new AutoResetEvent(true);
            }                
        }

        public void Dispose()
        {
            if (_event != null)
            {
                _terminated = true;
                _event.Close();
            }
        }

        private float[] AllocBlock()
        {
            float[] result = _usedBlocks.Count > 0 ? (float[])_usedBlocks.Pop() : new float[BlockSize];
            return result;
        }

        private void FreeBlock(float[] block)
        {
            if (_usedBlocks.Count < MaxBlocksInCache)
                _usedBlocks.Push(block);
        }

        private float[] GetWBlock()
        {
            float[] result;
            if (_writePos < BlockSize && _blocks.Count > 0)
                result = (float[])_blocks[_blocks.Count - 1];
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
                _event.Set();
                _terminated = true;
            }
        }

        public void Flush()
        {
            lock (this)
            {
                foreach (float[] block in _blocks)
                    FreeBlock(block);
                _blocks.Clear();
                _readPos = 0;
                _writePos = 0;
                _size = 0;
            }
        }

        public int Read(float[] buf, int count)
        {
            return Read(buf, 0, count);
        }

        public int Read(float[] buf, int ofs, int count)
        {
            int result;
            lock (this)
            {
                result = Peek(buf, ofs, count);
                Advance(result);
            }
            if (_event != null)
            {
                _event.Set();
            }
            return result;
        }

        public int Read(float* buf, int ofs, int count)
        {
            int result;
            lock (this)
            {
                result = Peek(buf, ofs, count);
                Advance(result);
            }
            if (_event != null)
            {
                _event.Set();
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
                    //Array.Copy(buf, ofs + count - left, GetWBlock(), _writePos, toWrite);
                    var block = GetWBlock();
                    fixed (float* blockPtr = block)
                    {
                        Utils.Memcpy(blockPtr + _writePos, buf + ofs + count - left, toWrite * sizeof(float));
                    }
                    _writePos += toWrite;
                    left -= toWrite;
                }
                _size += count;
            }
        }

        public void Write(float[] buf, int count)
        {
            Write(buf, 0, count);
        }

        public void Write(float[] buf, int ofs, int count)
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
                    Array.Copy(buf, ofs + count - left, GetWBlock(), _writePos, toWrite);
                    _writePos += toWrite;
                    left -= toWrite;
                }
                _size += count;
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
                        FreeBlock((float[])_blocks[0]);
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

        public int Peek(float[] buf, int ofs, int count)
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
                    Array.Copy((float[])_blocks[currentBlock], tempBlockPos, buf, ofs + count - sizeLeft, toFeed);
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
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
                    //Array.Copy((float[])_blocks[currentBlock], tempBlockPos, buf, ofs + count - sizeLeft, toFeed);
                    var block = (float[]) _blocks[currentBlock];
                    fixed (float* blockPtr = block)
                    {
                        Utils.Memcpy(buf + ofs + count - sizeLeft, blockPtr + tempBlockPos, toFeed * sizeof(float));
                    }
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
                }
                return count - sizeLeft;
            }
        }
    }
}
