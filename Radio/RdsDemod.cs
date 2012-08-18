using System;

namespace SDRSharp.Radio
{
    unsafe class RdsDemod
    {
        bool _lastRDSSample;

        private double _sampleClock = 8;

        private double _symbolPeriod;
        private double _sampleRate;
        private int _count;

        public double SampleRate
        {
            get { return _sampleRate; }
            set
            {
                _sampleRate = value;
                _symbolPeriod = 2.0 * Math.PI * 1187.5 / _sampleRate;
            }
        }

        private readonly RdsDetectorBank _rdsDetectorBank;

        public RdsDemod()
        {
            _rdsDetectorBank = new RdsDetectorBank(BlockDelegate1);
        }

        public void ProcessSamples(float* samples, int length)
        {
            for (var i = 0; i < length; i++)
            {
                bool b = samples[i] >= 0;
                ProcessSample(b);
            }
        }

        public void ProcessSample(bool thisDataSample)
        {
            _count += thisDataSample ? 1 : -1;

            _sampleClock += _symbolPeriod;

            if (_sampleClock >= 2 * Math.PI) // completed a 1187.5 Hz clock period
            {
                _sampleClock -= 2 * Math.PI;

                bool d = DifferentialDecode(_count > 0);
                _rdsDetectorBank.ProcessBitStream(d);

                _count = 0;
            }
        }

        // section 1.6 
        // IEC 62106:1999
        // Standard EN50067: 1998,

        private bool DifferentialDecode(bool thisDataSample)
        {
            bool thisConversion = thisDataSample ^ _lastRDSSample;
            _lastRDSSample = thisDataSample;
            return thisConversion;
        }

        void BlockDelegate1(GroupSequencer g)
        {
            Console.WriteLine(g.GroupCode);
            RdsDumpGroups.AnalyseFrames(g.Block1, g.Block2, g.Block3, g.Block4);
        }
    }
}
