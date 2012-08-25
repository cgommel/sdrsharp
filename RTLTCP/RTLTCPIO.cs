using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;
using SDRSharp.Radio;

namespace SDRSharp.RTLTCP
{
    public unsafe class RtlTcpIO : IFrontendController, IDisposable
    {
        private RTLTcpSettings _gui;
        private const float INPUT_GAIN = 0.001f;
        private const double DEFAULT_SAMPLE_RATE = 2048000.0; //1024000.0;
        private const long DEFAULT_FREQUENCY = 100000000;
        private const string DEFAULT_HOSTNAME = "192.168.0.11";
        private const int DEFAULT_PORT = 1234;
        private volatile Radio.SamplesAvailableDelegate _callback;
        Thread _sampleThread;
        private long _freq;
        private double _sr;
        private Socket _s;
        private const byte CMD_SET_FREQ = 1;
        private const byte CMD_SET_SAMPLE_RATE = 2;
        private const byte CMD_SET_GAIN_MODE = 3;
        private const byte CMD_SET_GAIN = 4;
        private const byte CMD_SET_FREQ_COR = 5;
        private string _host;
        private int _port;
        private uint _gainMode;
        private int _gainVal;
        private int _fCor;
        private UnsafeBuffer _b;

        public const uint GAIN_MODE_AUTO = 0;
        public const uint GAIN_MODE_MANUAL = 1;

        private const int BUFFER_SIZE = 16 * 1024;

        private bool _tunePlease = false;
        private const int MAX_TUNE_RATE = 20; //rtl_tcp seems to be limited to ~25
        private System.Timers.Timer _retuneTimer = new System.Timers.Timer(1000.0 / MAX_TUNE_RATE);

        public string hostName
        {
            get { return _host; }
            set { _host = value; }
        }

        public int port
        {
            get { return _port; }
            set { _port = value; }
        }

        private bool sendCommand(byte cmd, byte[] val)
        {
            if (null == _s) { return false; }
            if (val.Length < 4) { return false; }
            byte[] buffer = new byte[5];
            buffer[0] = cmd;
            buffer[1] = val[3]; //Network byte order
            buffer[2] = val[2];
            buffer[3] = val[1];
            buffer[4] = val[0];
            try
            {
                _s.Send(buffer);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        private bool sendCommand(byte cmd, UInt32 val)
        {
            byte[] valBytes = BitConverter.GetBytes(val);
            return sendCommand(cmd, valBytes);
        }

        private bool sendCommand(byte cmd, Int32 val)
        {
            byte[] valBytes = BitConverter.GetBytes(val);
            return sendCommand(cmd, valBytes);
        }

        public RtlTcpIO()
        {
            _freq = DEFAULT_FREQUENCY;
            _sr = DEFAULT_SAMPLE_RATE;
            _gainVal = 0;
            _gainMode = GAIN_MODE_AUTO;
            _host = DEFAULT_HOSTNAME;
            _port = DEFAULT_PORT;
            _retuneTimer.Elapsed += new System.Timers.ElapsedEventHandler(retuneNow);
            _retuneTimer.Start();
        }

        ~RtlTcpIO()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (_gui != null)
            {
                _gui.Dispose();
                _gui = null;
            }
            GC.SuppressFinalize(this);
        }

        public void Open()
        {
        }

        public void Close()
        {
            if (_s != null)
            {
                _s.Close();
                _s = null;
            }
        }

        public void Start(Radio.SamplesAvailableDelegate callback)
        {
            lock (this)
            {
                _callback = callback;
            }
            _s = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _s.Connect(_host, _port);
            _sampleThread = new Thread(() => { receiveSamples(); });
            _sampleThread.Start();
            sendCommand(CMD_SET_GAIN_MODE, (uint)_gainMode);
            sendCommand(CMD_SET_SAMPLE_RATE, (uint)_sr);
            sendCommand(CMD_SET_FREQ, (uint)_freq);
            sendCommand(CMD_SET_FREQ_COR, (int)_fCor);
        }

        public void Stop()
        {
            lock (this)
            {
                _callback = null;
            }
            Close();
            if (_sampleThread != null)
            {
                _sampleThread.Join();
            }
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
            if (null == _gui || _gui.IsDisposed)
            {
                _gui = new RTLTcpSettings(this);
            }
            _gui.Show();
        }

        public void HideSettingGUI()
        {
            if (null == _gui || _gui.IsDisposed)
            {
                return;
            }
            _gui.Hide();
        }

        public double Samplerate
        {
            get { return _sr; }
            set { _sr = value; sendCommand(CMD_SET_SAMPLE_RATE, (uint)_sr); }
        }

        public long Frequency
        {
            get { return _freq; }
            set { _freq = value; lock (_retuneTimer) { _tunePlease = true; } }
        }

        public int Gain
        {
            get { return _gainVal; }
            set { _gainVal = value; sendCommand(CMD_SET_GAIN, (int)_gainVal); } //Gain can be negative
        }

        public uint GainMode
        {
            get { return _gainMode; }
            set { _gainMode = value; sendCommand(CMD_SET_GAIN_MODE, (uint)_gainMode); }
        }

        public int FreqCorrection
        {
            get { return _fCor; }
            set { _fCor = value; sendCommand(CMD_SET_FREQ_COR, (int)_fCor); }
        }

        private void receiveSamples()
        {
            byte[] recBuffer = new byte[BUFFER_SIZE + 1024];
            int offs = 0;
            UInt64 sessionTotalB = 0;
            DateTime start = DateTime.Now;
            while (_callback != null && _s != null && _s.Connected)
            {
                try
                {
                    int bytesRec = _s.Receive(recBuffer, offs, BUFFER_SIZE, SocketFlags.None);
                    sessionTotalB += (UInt64)bytesRec;
                    int totalBytes = offs + bytesRec;
                    offs = totalBytes % 2; //Need to correctly handle the hypothetical case where we somehow get an odd number of bytes
                    beamUpThemSamples(recBuffer, totalBytes - offs); //This might work.
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    Close();
                    break;
                }
            }
            DateTime end = DateTime.Now;
            TimeSpan totalTime = end - start;
            double bps = (double)sessionTotalB / totalTime.TotalSeconds;
            double sps = bps / 2.0;
            Console.WriteLine(String.Format("Received {0} bytes over {1} seconds, which is {2} bps or {3} sps",
                sessionTotalB, totalTime.TotalSeconds, bps, sps));
        }

        private void beamUpThemSamples(byte[] buffer, int len)
        {
            var sampleCount = len / 2;
            Complex * bufPtr;
            if (_b == null || _b.Length < sampleCount)
            {
                _b = UnsafeBuffer.Create(sampleCount, sizeof(Complex));
            }
            bufPtr = (Complex*)_b;

            for (int i = 0; i < sampleCount; i++)
            {
                bufPtr[i].Real = (buffer[i * 2 + 1] - 128) / 128.0f * INPUT_GAIN;
                bufPtr[i].Imag = (buffer[i * 2] - 128) / 128.0f * INPUT_GAIN;
            }

            lock (this)
            {
                if (null == _callback) { return; }
                _callback(this, bufPtr, sampleCount);
            }
        }

        private void retuneNow(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (_retuneTimer)
            {
                if (_tunePlease)
                {
                    sendCommand(CMD_SET_FREQ, (uint)_freq);
                    _tunePlease = false;
                }
            }
        }
    }
}
