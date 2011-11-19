using System;
using System.Threading;
using System.Collections.Generic;
using SDRSharp.Radio;

namespace SDRSharp.SoftRock
{
    public class SoftRockIO : IFrontendController, IDisposable
    {
        private readonly Queue<int> _frequencyQueue = new Queue<int>();
        private Thread _worker;
        private IntPtr _srHandle;
        private int _frequency;
        private bool _terminated;

        public void Dispose()
        {
            Close();
        }

        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            _srHandle = NativeUsb.srOpen(
                NativeUsb.Vid,
                NativeUsb.Pid,
                string.Empty,
                string.Empty,
                string.Empty);

            _frequency = Frequency;

            if (IsOpen)
            {
                _terminated = false;
                _worker = new Thread(SetFrequency);
                _worker.Start();
            }
        }

        public void Close()
        {
            if (IsOpen)
            {
                NativeUsb.srClose();
                _srHandle = IntPtr.Zero;
                _terminated = true;
                if (_worker != null)
                {
                    _worker.Join();
                    _worker = null;
                }
            }
        }

        public bool IsOpen
        {
            get
            {
                return _srHandle != IntPtr.Zero;
            }
        }

        public int Frequency
        {
            get
            {
                return GetSi570Frequency();
            }
            set
            {
                _frequencyQueue.Enqueue(value);
            }
        }

        private void SetFrequency(object parameter)
        {
            while (!_terminated)
            {
                if (_frequencyQueue.Count > 0)
                {
                    var frequency = _frequencyQueue.Dequeue();

                    #region Change the frequency slowly

                    if (frequency != _frequency)
                    {
                        var diff = frequency - _frequency;
                        if (Math.Abs(diff) > 10000)
                        {
                            SetSi570Frequency(frequency);
                        }
                        else
                        {
                            var start = DateTime.Now;
                            var direction = Math.Sign(diff);
                            var maxStep = 200;//frequency / 1000000;
                            for (var f = _frequency; direction > 0 ? f < frequency : f > frequency; f += direction * maxStep)
                            {
                                SetSi570Frequency(f);
                            }
                            SetSi570Frequency(frequency);
                            var ts = (DateTime.Now - start).TotalMilliseconds;
                            Console.WriteLine("Changed frequency in " + ts + "ms");
                        }
                        _frequency = frequency;
                    }

                    #endregion
                }
                else
                {
                    Thread.Sleep(100);
                }
            }
        }

        private static int GetSi570Frequency()
        {
            double mhz;
            NativeUsb.srGetFreq(out mhz);
            return (int)(mhz * 1e6 / 4);
        }

        private static void SetSi570Frequency(int frequency)
        {
            var mhz = frequency / 1e6 * 4;
            NativeUsb.srSetFreq(mhz, NativeUsb.I2CAddr);
        }
    }
}
