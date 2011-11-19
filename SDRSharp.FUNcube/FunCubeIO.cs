using System;
using SDRSharp.Radio;

namespace SDRSharp.FUNcube
{
    public class FunCubeIO : IFrontendController, IDisposable
    {
        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public bool IsOpen
        {
            get { throw new NotImplementedException(); }
        }

        public int Frequency
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
