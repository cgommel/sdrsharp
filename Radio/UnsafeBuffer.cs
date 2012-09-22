using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe sealed class UnsafeBuffer : IDisposable
    {
        private readonly GCHandle _handle;
        private void* _ptr;
        private int _length;
        private Array _buffer;

        private UnsafeBuffer(Array buffer, int realLength, bool aligned)
        {
            _buffer = buffer;
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (void*) _handle.AddrOfPinnedObject();
            if (aligned)
            {
                _ptr = (void*) (((long) _ptr + 15) & ~15);
            }
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

        public static UnsafeBuffer Create(int size)
        {
            return Create(1, size, true);
        }

        public static UnsafeBuffer Create(int length, int sizeOfElement)
        {
            return Create(length, sizeOfElement, true);
        }

        public static UnsafeBuffer Create(int length, int sizeOfElement, bool aligned)
        {
            var buffer = new byte[length * sizeOfElement + (aligned ? 16 : 0)];
            return new UnsafeBuffer(buffer, length, aligned);
        }

        public static UnsafeBuffer Create(Array buffer)
        {
            return new UnsafeBuffer(buffer, buffer.Length, false);
        }
    }
}
