using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe sealed class UnsafeBuffer : IDisposable
    {
        const int Alignment = 32;

        private readonly GCHandle _handle;
        private void* _ptr;
        private int _length;
        private Array _buffer;

        private UnsafeBuffer(Array buffer, int realLength) : this(buffer, realLength, 1)
        {
        }

        private UnsafeBuffer(Array buffer, int realLength, int alignment)
        {
            _buffer = buffer;
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (void*) _handle.AddrOfPinnedObject();
            _ptr = (void*)((long)((byte*)_ptr + alignment - 1) & ~(alignment - 1));
            _length = realLength;
        }

        ~UnsafeBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_handle.IsAllocated)
            {
                _handle.Free();
            }
            _buffer = null;
            _ptr = null;
            _length = 0;
            GC.SuppressFinalize(this);
        }

        public void* Address
        {
            get { return _ptr; }
        }

        public int Length
        {
            get { return _length; }
        }

        public static implicit operator void*(UnsafeBuffer unsafeBuffer)
        {
            return unsafeBuffer.Address;
        }

        public static UnsafeBuffer Create(int length, int sizeOfElement)
        {
            var buffer = new byte[length * sizeOfElement + Alignment];
            return new UnsafeBuffer(buffer, length, Alignment);
        }

        public static UnsafeBuffer Create(Array buffer)
        {
            return new UnsafeBuffer(buffer, buffer.Length);
        }
    }
}
