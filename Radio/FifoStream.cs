using System;
using System.Collections;

namespace SDRSharp.Radio
{
    public class FifoStream<T>
    {
        private const int BlockSize = 65536;
        private const int MaxBlocksInCache = (3 * 1024 * 1024) / BlockSize;

        private int _size;
        private int _readPos;
        private int _writePos;
        private readonly Stack _usedBlocks = new Stack();
        private readonly ArrayList _blocks = new ArrayList();

        private T[] AllocBlock()
        {
            T[] result = _usedBlocks.Count > 0 ? (T[])_usedBlocks.Pop() : new T[BlockSize];
            return result;
        }
        private void FreeBlock(T[] block)
        {
            if (_usedBlocks.Count < MaxBlocksInCache)
                _usedBlocks.Push(block);
        }
        private T[] GetWBlock()
        {
            T[] result;
            if (_writePos < BlockSize && _blocks.Count > 0)
                result = (T[])_blocks[_blocks.Count - 1];
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
                foreach (T[] block in _blocks)
                    FreeBlock(block);
                _blocks.Clear();
                _readPos = 0;
                _writePos = 0;
                _size = 0;
            }
        }
        public int Read(T[] buf, int ofs, int count)
        {
            lock (this)
            {
                int result = Peek(buf, ofs, count);
                Advance(result);
                return result;
            }
        }
        public void Write(T[] buf, int ofs, int count)
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
                        FreeBlock((T[])_blocks[0]);
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
        public int Peek(T[] buf, int ofs, int count)
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
                    Array.Copy((T[])_blocks[currentBlock], tempBlockPos, buf, ofs + count - sizeLeft, toFeed);
                    sizeLeft -= toFeed;
                    tempBlockPos += toFeed;
                    tempSize -= toFeed;
                }
                return count - sizeLeft;
            }
        }
    }
}
