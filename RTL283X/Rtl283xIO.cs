using System;
using System.Windows.Forms;
using System.Threading;
using SDRSharp.Radio;

namespace SDRSharp.RTL283X
{
    public unsafe class Rtl283xIO : IFrontendController, IDisposable
    {
        private SamplesAvailableDelegate _callback;

        public Rtl283xIO()
        {
        }

        ~Rtl283xIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
            var r = NativeMethods.RTK_BDAFilterInit(IntPtr.Zero);

            if (!r)
            {
                throw new ApplicationException("No compatible devices detected");
            }

            r = NativeMethods.RTK_DevicePowerON();

            if (!r)
            {
                throw new ApplicationException("No compatible devices detected");
            }

            //_samplesEvent = new AutoResetEvent(false);

            //NativeMethods.RTK_SetDABEventHandle(_samplesEvent.Handle);
            //NativeMethods.RTK_SetDABEventHandle(IntPtr.Zero);


            DoInitFromRTKFM();

        }

        public void Close()
        {
            NativeMethods.RTK_DevicePowerOFF();
        }

        public void Start(SamplesAvailableDelegate callback)
        {
            _callback = callback;
        }

        public void Stop()
        {
        }

        public bool IsSoundCardBased
        {
            get { return false; }
        }

        public string SoundCardHint
        {
            get { return string.Empty; }
        }

        public void ShowSettingGUI(IWin32Window parent)
        {
        }

        public void HideSettingGUI()
        {
        }

        public double Samplerate
        {
            get { return 2048000; }
        }

        public long Frequency
        {
            get { return 0; }
            set { }
        }

        #region More Hacks
        static byte[] _magicDataSYS0 = { 0xf8 };

        static byte[] _magicDataSYS1 = { 0x98 };

        static byte[] _magicDataSYS2 = { 0x3 };

        static byte[] _magicDataSYS3 = { 0xdc };

        static byte[] _magicData4 = { 0x0, 0x0 };

        static byte[] _magicData5 = { 0x0 };

        static byte[] _magicData6 = { 0x0, 0x0, 0x0 };

        static byte[] _magicData7 = { 0x0, 0x0, 0x0 };

        static byte[] _magicData8 = { 0x3, 0x84, 0x0, 0x0 };

        static byte[] _magicData9 = { 0xca };

        static byte[] _magicDataa = { 0xdc };

        static byte[] _magicDatab = { 0xd7 };

        static byte[] _magicDatac = { 0xd8 };

        static byte[] _magicDatad = { 0xe0 };

        static byte[] _magicDatae = { 0xf2 };

        static byte[] _magicDataf = { 0xe };

        static byte[] _magicData10 = { 0x35 };

        static byte[] _magicData11 = { 0x6 };

        static byte[] _magicData12 = { 0x50 };

        static byte[] _magicData13 = { 0x9c };

        static byte[] _magicData14 = { 0xd };

        static byte[] _magicData15 = { 0x71 };

        static byte[] _magicData16 = { 0x11 };

        static byte[] _magicData17 = { 0x14 };

        static byte[] _magicData18 = { 0x71 };

        static byte[] _magicData19 = { 0x74 };

        static byte[] _magicData1a = { 0x19 };

        static byte[] _magicData1b = { 0x41 };

        static byte[] _magicData1c = { 0xa5 };

        static byte[] _magicData1d = { 0x11 };

        static byte[] _magicData1e = { 0x10 };

        static byte[] _magicData1f = { 0x21 };

        static byte[] _magicData20 = { 0xff };

        static byte[] _magicData21 = { 0x1 };

        static byte[] _magicData22 = { 0x6 };

        static byte[] _magicData23 = { 0xd };

        static byte[] _magicData24 = { 0x16 };

        static byte[] _magicData25 = { 0x1b };

        static byte[] _magicData26 = { 0x0 };

        static byte[] _magicData27 = { 0xf0 };

        static byte[] _magicData28 = { 0xf };

        static byte[] _magicData29 = { 0x60 };

        static byte[] _magicData2a = { 0x80 };

        static byte[] _magicData2b = { 0x5a };

        static byte[] _magicData2c = { 0x40 };

        static byte[] _magicData2d = { 0x5a };

        static byte[] _magicData2e = { 0x30 };

        static byte[] _magicData2f = { 0xd0 };

        static byte[] _magicData30 = { 0xbe };

        static byte[] _magicData31 = { 0x18 };

        static byte[] _magicData32 = { 0x35 };

        static byte[] _magicData33 = { 0x21 };

        static byte[] _magicData34 = { 0x21 };

        static byte[] _magicData35 = { 0x0 };

        static byte[] _magicData36 = { 0x40 };

        static byte[] _magicData37 = { 0x10 };

        static byte[] _magicData38 = { 0x10 };

        static byte[] _magicData39 = { 0x80 };

        static byte[] _magicData3a = { 0x7f };

        static byte[] _magicData3b = { 0x80 };

        static byte[] _magicData3c = { 0x7f };

        static byte[] _magicData3d = { 0xfc };

        static byte[] _magicData3e = { 0xfc };

        static byte[] _magicData3f = { 0xd4 };

        static byte[] _magicData40 = { 0xf0 };

        static byte[] _magicData41 = { 0x0 };

        static byte[] _magicData42 = { 0x0 };

        static byte[] _magicData43 = { 0x14 };

        static byte[] _magicData44 = { 0xec };

        static byte[] _magicData45 = { 0xc };

        static byte[] _magicData46 = { 0x2 };

        static byte[] _magicData47 = { 0x9 };

        static byte[] _magicData48 = { 0x83 };

        static byte[] _magicData49 = { 0x49 };

        static byte[] _magicData4a = { 0x87 };

        static byte[] _magicData4b = { 0x85 };

        static byte[] _magicData4c = { 0x2 };

        static byte[] _magicData4d = { 0xcd };

        static byte[] _magicData4e = { 0x1 };

        static byte[] _magicData4f = { 0x14 };

        static byte[] _magicData50 = { 0x10 };

        static byte[] _magicData51 = { 0x21 };

        static byte[] _magicDataSYS52 = { 0x3 };

        static byte[] _magicDataSYS53 = { 0xdc };

        static byte[] _magicDataSYS54 = { 0x88 };

        static byte[] _magicData55 = { 0x0, 0xe3, 0x8e };

        static byte[] _magicData56 = { 0x14 };

        static byte[] _magicData57 = { 0x10 };

        static byte[] _magicDataSYS58 = { 0x3 };

        static byte[] _magicDataSYS59 = { 0xdc };

        static byte[] _magicDataSYS5a = { 0x88 };


        private void SendSYSMagic(ushort offset, uint length, byte[] magic)
        {
            UnsafeBuffer data = UnsafeBuffer.Create(magic);
            byte* dataPtr = (byte*)data;
            NativeMethods.RTK_SYS_Byte_Write(offset, length, dataPtr);

            data.Dispose();
        }

        private void SendDemodMagic(byte page, ushort offset, uint length, byte[] magic)
        {
            UnsafeBuffer data = UnsafeBuffer.Create(magic);
            byte* dataPtr = (byte*)data;
            NativeMethods.RTK_Demod_Byte_Write(page, offset, length, dataPtr);

            data.Dispose();
        }

        public void DoInitFromRTKFM()
        {
            SendSYSMagic(0x4, 0x1, _magicDataSYS0);

            SendSYSMagic(0x1, 0x1, _magicDataSYS1);

            SendSYSMagic(0x4, 0x1, _magicDataSYS2);

            SendSYSMagic(0x3, 0x1, _magicDataSYS3);

            SendDemodMagic(0x1, 0x3e, 0x2, _magicData4);

            SendDemodMagic(0x1, 0x15, 0x1, _magicData5);

            SendDemodMagic(0x1, 0x16, 0x3, _magicData6);

            SendDemodMagic(0x1, 0x19, 0x3, _magicData7);

            SendDemodMagic(0x1, 0x9f, 0x4, _magicData8);

            SendDemodMagic(0x1, 0x1c, 0x1, _magicData9);

            SendDemodMagic(0x1, 0x1d, 0x1, _magicDataa);

            SendDemodMagic(0x1, 0x1e, 0x1, _magicDatab);

            SendDemodMagic(0x1, 0x1f, 0x1, _magicDatac);

            SendDemodMagic(0x1, 0x20, 0x1, _magicDatad);

            SendDemodMagic(0x1, 0x21, 0x1, _magicDatae);

            SendDemodMagic(0x1, 0x22, 0x1, _magicDataf);

            SendDemodMagic(0x1, 0x23, 0x1, _magicData10);

            SendDemodMagic(0x1, 0x24, 0x1, _magicData11);

            SendDemodMagic(0x1, 0x25, 0x1, _magicData12);

            SendDemodMagic(0x1, 0x26, 0x1, _magicData13);

            SendDemodMagic(0x1, 0x27, 0x1, _magicData14);

            SendDemodMagic(0x1, 0x28, 0x1, _magicData15);

            SendDemodMagic(0x1, 0x29, 0x1, _magicData16);

            SendDemodMagic(0x1, 0x2a, 0x1, _magicData17);

            SendDemodMagic(0x1, 0x2b, 0x1, _magicData18);

            SendDemodMagic(0x1, 0x2c, 0x1, _magicData19);

            SendDemodMagic(0x1, 0x2d, 0x1, _magicData1a);

            SendDemodMagic(0x1, 0x2e, 0x1, _magicData1b);

            SendDemodMagic(0x1, 0x2f, 0x1, _magicData1c);

            SendDemodMagic(0x0, 0x17, 0x1, _magicData1d);

            SendDemodMagic(0x0, 0x18, 0x1, _magicData1e);

            SendDemodMagic(0x0, 0x19, 0x1, _magicData1f);

            SendDemodMagic(0x0, 0x1f, 0x1, _magicData20);

            SendDemodMagic(0x0, 0x1e, 0x1, _magicData21);

            SendDemodMagic(0x0, 0x1d, 0x1, _magicData22);

            SendDemodMagic(0x0, 0x1c, 0x1, _magicData23);

            SendDemodMagic(0x0, 0x1b, 0x1, _magicData24);

            SendDemodMagic(0x0, 0x1a, 0x1, _magicData25);

            SendDemodMagic(0x1, 0x92, 0x1, _magicData26);

            SendDemodMagic(0x1, 0x93, 0x1, _magicData27);

            SendDemodMagic(0x1, 0x94, 0x1, _magicData28);

            SendDemodMagic(0x0, 0x61, 0x1, _magicData29);

            SendDemodMagic(0x0, 0x6, 0x1, _magicData2a);

            SendDemodMagic(0x1, 0x12, 0x1, _magicData2b);

            SendDemodMagic(0x1, 0x2, 0x1, _magicData2c);

            SendDemodMagic(0x1, 0x3, 0x1, _magicData2d);

            SendDemodMagic(0x1, 0xc7, 0x1, _magicData2e);

            SendDemodMagic(0x1, 0x4, 0x1, _magicData2f);

            SendDemodMagic(0x1, 0x5, 0x1, _magicData30);

            SendDemodMagic(0x1, 0xc8, 0x1, _magicData31);

            SendDemodMagic(0x1, 0x6, 0x1, _magicData32);

            SendDemodMagic(0x1, 0xc9, 0x1, _magicData33);

            SendDemodMagic(0x1, 0xca, 0x1, _magicData34);

            SendDemodMagic(0x1, 0xcb, 0x1, _magicData35);

            SendDemodMagic(0x1, 0x7, 0x1, _magicData36);

            SendDemodMagic(0x1, 0xcd, 0x1, _magicData37);

            SendDemodMagic(0x1, 0xce, 0x1, _magicData38);

            SendDemodMagic(0x1, 0x8, 0x1, _magicData39);

            SendDemodMagic(0x1, 0x9, 0x1, _magicData3a);

            SendDemodMagic(0x1, 0xa, 0x1, _magicData3b);

            SendDemodMagic(0x1, 0xb, 0x1, _magicData3c);

            SendDemodMagic(0x0, 0xe, 0x1, _magicData3d);

            SendDemodMagic(0x0, 0xe, 0x1, _magicData3e);

            SendDemodMagic(0x0, 0x11, 0x1, _magicData3f);

            SendDemodMagic(0x1, 0xe5, 0x1, _magicData40);

            SendDemodMagic(0x1, 0xd9, 0x1, _magicData41);

            SendDemodMagic(0x1, 0xdb, 0x1, _magicData42);

            SendDemodMagic(0x1, 0xdd, 0x1, _magicData43);

            SendDemodMagic(0x1, 0xde, 0x1, _magicData44);

            SendDemodMagic(0x1, 0xd8, 0x1, _magicData45);

            SendDemodMagic(0x1, 0xe6, 0x1, _magicData46);

            SendDemodMagic(0x1, 0xd7, 0x1, _magicData47);

            SendDemodMagic(0x0, 0xd, 0x1, _magicData48);

            SendDemodMagic(0x0, 0x10, 0x1, _magicData49);

            SendDemodMagic(0x0, 0xd, 0x1, _magicData4a);

            SendDemodMagic(0x0, 0xd, 0x1, _magicData4b);

            SendDemodMagic(0x0, 0x13, 0x1, _magicData4c);

            SendDemodMagic(0x0, 0x8, 0x1, _magicData4d);

            SendDemodMagic(0x1, 0xb1, 0x1, _magicData4e);

            SendDemodMagic(0x1, 0x1, 0x1, _magicData4f);

            SendDemodMagic(0x1, 0x1, 0x1, _magicData50);

            SendDemodMagic(0x0, 0x19, 0x1, _magicData51);

            SendSYSMagic(0x4, 0x1, _magicDataSYS52);

            SendSYSMagic(0x3, 0x1, _magicDataSYS53);

            SendSYSMagic(0x1, 0x1, _magicDataSYS54);

            SendDemodMagic(0x1, 0x16, 0x3, _magicData55);

            SendDemodMagic(0x1, 0x1, 0x1, _magicData56);

            SendDemodMagic(0x1, 0x1, 0x1, _magicData57);

            SendSYSMagic(0x4, 0x1, _magicDataSYS58);

            SendSYSMagic(0x3, 0x1, _magicDataSYS59);

            SendSYSMagic(0x1, 0x1, _magicDataSYS5a);
        }

        #endregion
    }
}