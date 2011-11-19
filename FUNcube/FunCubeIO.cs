using System;
using SDRSharp.Radio;

namespace SDRSharp.FUNcube
{
    public class FunCubeIO : IFrontendController, IDisposable
    {
        public void Open()
        {
        }

        public void Close()
        {
        }

        public bool IsOpen
        {
            get
            {
                return true;
            }
        }

        public int Frequency
        {
            get
            {
                return 0;
            }
            set
            {
                
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
