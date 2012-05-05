using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class UnsafeBuffer : IDisposable
    {
        private readonly void* _ptr;
        private readonly GCHandle _handle;
        private int _length;
        private Array _buffer;

        private UnsafeBuffer(Array buffer, int realLength)
        {
            _buffer = buffer;
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (void*) _handle.AddrOfPinnedObject();
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
            var buffer = new byte[length * sizeOfElement];
            return new UnsafeBuffer(buffer, length);
        }

        public static UnsafeBuffer Create(Array buffer)
        {
            return new UnsafeBuffer(buffer, buffer.Length);
        }
    }
}
