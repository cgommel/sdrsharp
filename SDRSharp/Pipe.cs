namespace SDRSharp
{
    public class Pipe<T> where T: class
    {
        private int _headPtr;
        private int _tailPtr;
        private readonly T[] _buffer;

        public Pipe(int capacity)
        {
            _buffer = new T[capacity];
        }

        public void AdvanceRead()
        {
            if (_tailPtr == _headPtr)
            {
                return;
            }
            _tailPtr++;
            if (_tailPtr >= _buffer.Length)
            {
                _tailPtr = 0;
            }
        }

        public void AdvanceWrite()
        {
            _headPtr++;
            if (_headPtr >= _buffer.Length)
            {
                _headPtr = 0;
            }
            if (_headPtr == _tailPtr)
            {
                _tailPtr++;
                if (_tailPtr >= _buffer.Length)
                {
                    _tailPtr = 0;
                }
            }
        }

        public T Head
        {
            get { return _buffer[_headPtr]; }
            set { _buffer[_headPtr] = value; }
        }

        public T Tail
        {
            get
            {
                return _buffer[_tailPtr];
            }
        }
    }
}
