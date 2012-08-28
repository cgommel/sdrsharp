using System;

namespace SDRSharp.Radio
{
    public class RdsDetectorBank
    {
        private readonly RdsDumpGroups _dumpGroups = new RdsDumpGroups();
        private readonly SyndromeDetector[] _detectors = new SyndromeDetector[26];

        public string RadioText
        {
            get { return _dumpGroups.RadioText; }
        }

        public string ProgramService
        {
            get { return _dumpGroups.ProgramService; }
        }

        public RdsDetectorBank()
        {
            for (var i = 0; i < _detectors.Length; i++)
            {
                _detectors[i] = new SyndromeDetector(i, _dumpGroups);
            }
        }

        public void Process(bool b)
        {
            for (var i = 0; i < _detectors.Length; i++)
            {
                _detectors[i].Clock(b);
            }
        }

        public void Reset()
        {
            _dumpGroups.Reset();
        }
    }

    public class SyndromeDetector : GroupSequencer
    {
        private readonly RdsDumpGroups _dumpGroups;
        private UInt16 _syndrome;
        private int _state;

        public SyndromeDetector(int state, RdsDumpGroups dumpGroups)
        {
            _state = state;
            _dumpGroups = dumpGroups;
        }

        public bool Clock(bool b)
        {
            var result = false;
            _state++;
            if (_state == 26)
            {
                if (Sequence(_syndrome) == BlockSequence.Live)
                {
                    result = _dumpGroups.AnalyseFrames(Block1, Block2, Block3, Block4);
                }
                _syndrome = 0;
                _state = 0;
            }

            _raw = (_raw << 1);

            if (b)
            {
                _raw++;
            }

            if (_state < 16)
            {
                int gateA = ((_syndrome & 0x200) != 0) ^ b ? 0x1b9 : 0;
                _syndrome = (UInt16)((_syndrome << 1) ^ gateA);
            }
            else
            {
                int gateB = ((_syndrome & 0x200) != 0) ^ b ? 1 : 0;
                _syndrome = (UInt16)((_syndrome << 1) ^ gateB);
            }

            return result;
        }

        public override string ToString()
        {
            return string.Format("st {0:d02} sy {1:x04} r {2:x08}", _state, _syndrome, _raw);
        }
    }

    public class GroupSequencer 
    {
        protected enum BlockSequence
        {
            Broke = 0,
            GotA,
            GotB,
            GotC,
            Live
        } ;

        private BlockSequence _seq = BlockSequence.Broke;

        protected UInt32 _raw;

        protected BlockSequence Sequence(UInt16 syndrome)
        {
            if ((syndrome & 0x03ff) == 0x0fc)
            {
                //A              
                Block1 = (UInt16)((_raw >> 10) & 0xffff);
                _seq = BlockSequence.GotA;
                return _seq;
            }

            if (((syndrome & 0x03ff) == 0x198) && (_seq == BlockSequence.GotA))
            {
                //B             
                Block2 = (UInt16)((_raw >> 10) & 0xffff);
                _seq = BlockSequence.GotB;
                return _seq;
            }

            if (((syndrome & 0x03ff) == 0x168) && (_seq == BlockSequence.GotB))
            {
                //C             
                Block3 = (UInt16)((_raw >> 10) & 0xffff);
                _seq = BlockSequence.GotC;
                return _seq;
            }

            if (((syndrome & 0x03ff) == 0x350) && (_seq == BlockSequence.GotB))
            {
                // c                
                Block3 = (UInt16)((_raw >> 10) & 0xffff);
                _seq = BlockSequence.GotC;
                return _seq;
            }

            if (((syndrome & 0x03ff) == 0x1b4) && (_seq == BlockSequence.GotC))
            {
                //D                
                Block4 = (UInt16)((_raw >> 10) & 0xffff);
                _seq = BlockSequence.Live;
                return _seq;
            }
            _seq = BlockSequence.Broke;
            return _seq;
        }
    
        public string GroupCode
        {
            get
            {
                int group = Block2 >> 12;
                char groupType = ((Block2 & 0x0800) == 0) ? 'A' : 'B';
                return string.Format("{0:d}{1}", group, groupType);
            }
        }

        public ushort Block1
        {
            get; private set;
        }

        public ushort Block2
        {
            get; private set;
        }

        public ushort Block3
        {
            get; private set;
        }

        public ushort Block4
        {
            get; private set;
        }
    }
}
