using System;
using System.Collections;

namespace SDRSharp.Radio
{
    public unsafe class FifoStream
    {
        private const int BlockSize = 65536;
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private readonly Stack _usedBlocks = new Stack();
        private readonly ArrayList _blocks = new ArrayList();

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
        
        public int Read(Complex[] buf, int ofs, int count)
        {
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
            }
        }

        
        public void Write(Complex[] buf, int ofs, int count)
        {
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
    }
}
