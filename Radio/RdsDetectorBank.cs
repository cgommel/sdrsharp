using System;

namespace SDRSharp.Radio
{
    public class RdsDetectorBank
    {
        private readonly RdsDumpGroups _dumpGroups;
        private readonly SyndromeDetector _detector;

        public string RadioText
        {
            get { return _dumpGroups.RadioText; }
        }

        public string ProgramService
        {
            get { return _dumpGroups.ProgramService; }
        }

        public ushort PICode
        {
            get { return _dumpGroups.PICode; }
        }

        public RdsDetectorBank()
        {
            _dumpGroups = new RdsDumpGroups();
            _detector = new SyndromeDetector(_dumpGroups);
        }

        public void Process(bool b)
        {
            _detector.Clock(b);
        }

        public void Reset()
        {
            _dumpGroups.Reset();
        }
    }

    public class SyndromeDetector
    {
        [Flags]
        protected enum BlockSequence
        {
            WaitBitSync,
            GotBitSync,
            GotA,
            GotB,
            GotC,
            GotD
        }

        private BlockSequence _sequence = BlockSequence.WaitBitSync;
        private UInt32 _raw;
        private readonly RdsDumpGroups _dumpGroups;
        private readonly bool _useFec = Utils.GetBooleanSetting("rdsUseFEC");
        private UInt16 _syndrome;
        private int _count;

        public SyndromeDetector(RdsDumpGroups dumpGroups)
        {
            _dumpGroups = dumpGroups;
        }

        public void Clock(bool b)
        {
            _raw <<= 1;
            _raw |= b ? 1u : 0u;
            _count++;

            if (_sequence == BlockSequence.WaitBitSync)
            {
                _syndrome = BuildSyndrome(_raw);
                _syndrome ^= 0x3d8;
                _sequence = _syndrome == 0 ? BlockSequence.GotA : BlockSequence.WaitBitSync;
                Block1 = (UInt16)((_raw >> 10) & 0xffff);
                _count = 0;
            }

            if (_count == 26)
            {
                ProcessSyndrome();
                if (_sequence == BlockSequence.GotD)
                {
                    _dumpGroups.AnalyseFrames(Block1, Block2, Block3, Block4);
                    _sequence = BlockSequence.GotBitSync;
                }

                _count = 0;
            }
        }

        private void ProcessSyndrome()
        {
            _syndrome = BuildSyndrome(_raw);
            switch (_sequence)
            {
                case BlockSequence.GotBitSync:
                    _syndrome ^= 0x3d8;
                    _sequence = BlockSequence.GotA;
                    break;
                case BlockSequence.GotA:
                    _syndrome ^= 0x3d4;
                    _sequence = BlockSequence.GotB;
                    break;
                case BlockSequence.GotB:
                    _syndrome ^= (UInt16)(Block2 & 0x800) == 0 ? (UInt16)0x25c : (UInt16)0x3cc;
                    _sequence = BlockSequence.GotC;
                    break;
                case BlockSequence.GotC:
                    _syndrome ^= 0x258;
                    _sequence = BlockSequence.GotD;
                    break;
            }
            if (_syndrome != 0)
            {
                if (_useFec)
                {
                    var corrected = ApplyFEC();
                    if (_syndrome != 0 || corrected > 2)
                    {
                        _sequence = BlockSequence.WaitBitSync;
                    }
                    else
                    {
                        SetBlock(_sequence, (UInt16)(_raw & 0xffff));
                    }
                }
                else
                {
                    _sequence = BlockSequence.WaitBitSync;
                }
            }
            else
            {
                SetBlock(_sequence, (UInt16)((_raw >> 10) & 0xffff));
            }
        }

        private void SetBlock(BlockSequence sequence, UInt16 val)
        {
            switch (_sequence)
            {
                case BlockSequence.GotA:
                    Block1 = val;
                    break;
                case BlockSequence.GotB:
                    Block2 = val;
                    break;
                case BlockSequence.GotC:
                    Block3 = val;
                    break;
                case BlockSequence.GotD:
                    Block4 = val;
                    break;
                default:
                    break;
            }
        }

        private int ApplyFEC()
        {
            const int poly = 0x5b9;
            const int errorMask = (1 << 5);

            var correction = (uint)(1 << 25);
            var correctedBitsCount = 0;

            _raw >>= 10;
            for (var i = 0; i < 16; i++)
            {
                var st = ((_syndrome & 0x200) == 0x200);
                var bitError = (_syndrome & errorMask) == 0;
                _raw ^= (st && bitError) ? correction : 0;
                _syndrome <<= 1;
                _syndrome ^= (UInt16)((st && !bitError) ? poly : 0);
                correctedBitsCount += (st && bitError) ? 1 : 0;
                correction >>= 1;
            }

            _syndrome &= 0x3ff;
            return correctedBitsCount;
        }

        private static UInt16 BuildSyndrome(uint raw)
        {
            UInt16[] Parity = 
            {
                0x2DC,
                0x16E,
                0x0B7,
                0x287,
                0x39F,
                0x313,
                0x355,
                0x376,
                0x1BB,
                0x201,
                0x3DC,
                0x1EE,
                0x0F7,
                0x2A7,
                0x38F,
                0x31B
            };

            var block = raw & 0x3FFFFFF;
            var syndrome = (UInt16)(block >> 16);
            for (var i = 0; i < 16; i++)
            {
                syndrome ^= (UInt16)(((block & 0x8000) == 0x8000) ? Parity[i] : 0);
                block <<= 1;
            }

            return syndrome;
        }

        public override string ToString()
        {
            return string.Format("st {0:d02} sy {1:x04} r {2:x08}", _count, _syndrome, _raw);
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
            get;
            private set;
        }

        public ushort Block2
        {
            get;
            private set;
        }

        public ushort Block3
        {
            get;
            private set;
        }

        public ushort Block4
        {
            get;
            private set;
        }
    }
}
