using System;
using System.Runtime.InteropServices;

namespace SDRSharp.Radio
{
    public unsafe class UnsafeBuffer : IDisposable
    {
        private readonly void* _ptr;
        private readonly GCHandle _handle;
        private readonly Array _buffer;

        private UnsafeBuffer(Array buffer)
        {
            _buffer = buffer;
            _handle = GCHandle.Alloc(_buffer, GCHandleType.Pinned);
            _ptr = (void*) _handle.AddrOfPinnedObject();
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
        }

        public void* Address
        {
            get { return _ptr; }
        }

        public int Length
        {
            get { return _buffer.Length; }
        }

        public static implicit operator void *(UnsafeBuffer unsafeBuffer)
        {
            return unsafeBuffer.Address;
        }

        public static UnsafeBuffer Create<T>(int size)
            where T : struct
        {
            var buffer = new T[size];
            return new UnsafeBuffer(buffer);
        }

        public static UnsafeBuffer Create<T>(T[] buffer)
            where T : struct
        {
            return new UnsafeBuffer(buffer);
        }
    }
}
