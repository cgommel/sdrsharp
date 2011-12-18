using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SDRSharp.PanView
{
    public class SpectrumAnalyzer : UserControl
    {
        private const int AxisMargin = 30;
        private const int CarrierPenWidth = 1;

        private bool _performNeeded;
        private bool _drawBackgroundNeeded;
        private double[] _spectrum;
        private Bitmap _bkgBuffer;
        private Bitmap _buffer;
        private Graphics _graphics;
        private int _spectrumWidth;
        private int _centerFrequency;
        private PointF[] _points;
        private Bitmap _cursor;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _offset;
        private float _xIncrement;
        private int _frequency;
        private float _lower;
        private float _upper;
        private int _delta;
        private int _oldX;
        private int _oldFilterBandwidth;
        private int _oldCenterFrequency;
        private bool _changingBandwidth;
        private bool _changingFrequency;
        private bool _changingCenterFrequency;

        public SpectrumAnalyzer()
        {
            _bkgBuffer = new Bitmap(Width, Height);
            _buffer = new Bitmap(Width, Height);
            _graphics = Graphics.FromImage(_buffer);
            _cursor = new Bitmap(10, 10);
        }

        ~SpectrumAnalyzer()
        {
            _buffer.Dispose();
            _graphics.Dispose();
            _cursor.Dispose();
        }

        public event ManualFrequencyChange FrequencyChanged;

        public event ManualFrequencyChange CenterFrequencyChanged;

        public event ManualBandwidthChange BandwidthChanged;

        public int SpectrumWidth
        {
            get
            {
                return _spectrumWidth;
            }
            set
            {
                if (_spectrumWidth != value)
                {
                    _spectrumWidth = value;
                    if (_spectrumWidth > 0)
                    {
                        _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float)_spectrumWidth;
                    }
                    GenerateCursor();
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public int FilterBandwidth
        {
            get
            {
                return _filterBandwidth;
            }
            set
            {
                if (_filterBandwidth != value)
                {
                    _filterBandwidth = value;
                    GenerateCursor();
                    _performNeeded = true;
                }
            }
        }

        public int Offset
        {
            get
            {
                return _offset;
            }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    GenerateCursor();
                    _performNeeded = true;
                }
            }
        }

        public BandType BandType
        {
            get
            {
                return _bandType;
            }
            set
            {
                if (_bandType != value)
                {
                    _bandType = value;
                    GenerateCursor();
                    _performNeeded = true;
                }
            }
        }

        public int Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                if (_frequency != value)
                {
                    _frequency = value;
                    PositionCursor();
                    _performNeeded = true;
                }
            }
        }

        public int CenterFrequency
        {
            get
            {
                return _centerFrequency;
            }
            set
            {
                if (_centerFrequency != value)
                {
                    _centerFrequency = value;
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public void Perform()
        {
            if (_drawBackgroundNeeded)
            {
                _drawBackgroundNeeded = false;
                DrawBackground();
            }
            if (_performNeeded)
            {
                _performNeeded = false;
                Draw();
                Invalidate();
            }
        }

        private void GenerateCursor()
        {
            var cursorWidth = Math.Max(_filterBandwidth * _xIncrement, 2);
            var bandpassWidth = cursorWidth;
            var bandpassOffset = _offset * _xIncrement;
            var bandpassLow = 0f;
            var xCarrier = 0f;

            switch (_bandType)
            {
                case BandType.Upper:
                    bandpassLow = bandpassOffset;
                    cursorWidth += CarrierPenWidth + bandpassOffset;
                    xCarrier = CarrierPenWidth;
                    break;

                case BandType.Lower:
                    cursorWidth += CarrierPenWidth + bandpassOffset;
                    xCarrier = bandpassWidth + bandpassOffset - CarrierPenWidth;
                    break;

                case BandType.Center:
                    xCarrier = cursorWidth / 2f;
                    break;
            }

            _cursor.Dispose();
            _cursor = new Bitmap((int)cursorWidth, ClientRectangle.Height);

            using (var g = Graphics.FromImage(_cursor))
            using (var transparentBrush = new SolidBrush(Color.FromArgb(80, Color.White)))
            using (var carrierPen = new Pen(Color.Red))
            {
                carrierPen.Width = CarrierPenWidth;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.FillRectangle(transparentBrush, bandpassLow, 0, bandpassWidth, _cursor.Height);
                g.DrawLine(carrierPen, xCarrier, 0f, xCarrier, _cursor.Height);
            }

            PositionCursor();
        }

        private void PositionCursor()
        {
            var cursorWidth = Math.Max(_filterBandwidth * _xIncrement, 2);
            var relativeOffset = _offset * _xIncrement;
            var xCarrier = AxisMargin + (_frequency - _centerFrequency + _spectrumWidth / 2) * _xIncrement;

            switch (_bandType)
            {
                case BandType.Upper:
                    cursorWidth += relativeOffset;
                    _lower = xCarrier - CarrierPenWidth;
                    break;

                case BandType.Lower:
                    cursorWidth += relativeOffset;
                    _lower = xCarrier - cursorWidth + CarrierPenWidth;
                    break;

                case BandType.Center:
                    _lower = xCarrier - cursorWidth / 2;
                    break;
            }
            _upper = _lower + cursorWidth;
        }

        public void Render(double[] spectrum, int length)
        {
            if (_spectrum == null || _spectrum.Length != length)
            {
                _points = new PointF[length];
                _spectrum = new double[length];
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    _spectrum[i] = spectrum[i];
                }
            }
            const double attack = 0.9;
            const double decay = 0.2;
            for (var i = 0; i < _spectrum.Length; i++)
            {
                var ratio = _spectrum[i] < spectrum[i] ? attack : decay;
                _spectrum[i] = _spectrum[i] * (1 - ratio) + spectrum[i] * ratio;
            }
            _performNeeded = true;
        }

        private void DrawBackground()
        {
            #region Draw grid

            using (var bkgBrush = new SolidBrush(Color.Black))
            using (var fontBrush = new SolidBrush(Color.Silver))
            using (var squarePen = new Pen(Color.FromArgb(80, 80, 80)))
            using (var axisPen = new Pen(Color.DarkGray))
            using (var font = new Font("Arial", 8f))
            using (var graphics = Graphics.FromImage(_bkgBuffer))
            {
                // Background
                graphics.FillRectangle(bkgBrush, ClientRectangle);

                // Axis
                graphics.DrawLine(axisPen, AxisMargin, AxisMargin, AxisMargin, ClientRectangle.Height - AxisMargin);
                graphics.DrawLine(axisPen, AxisMargin, ClientRectangle.Height - AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin);

                // Grid
                squarePen.DashStyle = DashStyle.Dash;
                var xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / 10.0f;
                for (var i = 1; i <= 10; i++)
                {
                    graphics.DrawLine(squarePen, (int)(AxisMargin + xIncrement * i), AxisMargin, (int)(AxisMargin + xIncrement * i), ClientRectangle.Height - AxisMargin);
                }
                var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / 12.0f;
                for (var i = 1; i <= 12; i++)
                {
                    graphics.DrawLine(squarePen, AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement), ClientRectangle.Width - AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement));
                }

                // Decibel line
                for (var i = 0; i <= 12; i++)
                {
                    var db = (-(12 - i) * 10).ToString();
                    var sizeF = graphics.MeasureString(db, font);
                    var width = sizeF.Width;
                    var height = sizeF.Height;
                    graphics.DrawString(db, font, fontBrush, AxisMargin - width - 5, ClientRectangle.Height - AxisMargin - i * yIncrement - height / 2f);
                }

                // Frequency line
                if (SpectrumWidth > 0)
                {
                    for (var i = 0; i <= 10; i++)
                    {

                        var frequency = _centerFrequency + (i - 5) * SpectrumWidth / 10;
                        string f;
                        if (frequency == 0)
                        {
                            f = "DC";
                        }
                        else if (Math.Abs(frequency) > 30000000)
                        {
                            f = string.Format("{0:0,0.000}MHz", frequency / 1000000.0);
                        }
                        else if (Math.Abs(frequency) > 1000)
                        {
                            f = string.Format("{0:0,0}kHz", frequency / 1000.0);
                        }
                        else
                        {
                            f = frequency.ToString();
                        }
                        var sizeF = graphics.MeasureString(f, font);
                        var width = sizeF.Width;

                        graphics.DrawString(f, font, fontBrush, AxisMargin + xIncrement * i - width / 2f, ClientRectangle.Height - AxisMargin + 5f);
                    }
                }

            }

            #endregion
        }

        private void Draw()
        {
            if (ClientRectangle.Width <= AxisMargin || ClientRectangle.Height <= AxisMargin)
            {
                return;
            }

            var data1 = _buffer.LockBits(ClientRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            var data2 = _bkgBuffer.LockBits(ClientRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            Waterfall.Memcpy(data1.Scan0, data2.Scan0, new UIntPtr((ulong) (data1.Width * data1.Height * 4)));
            _buffer.UnlockBits(data1);
            _bkgBuffer.UnlockBits(data2);

            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }

            DrawSpectrum();
            _graphics.DrawImage(_cursor, (int) _lower, 0);
        }

        private void DrawSpectrum()
        {
            var xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float)_spectrum.Length;
            var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / 120f;

            using (var spectrumPen = new Pen(Color.LimeGreen))
            {
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var strenght = (float) _spectrum[i] + 120f;
                    strenght = Math.Max(strenght, 0);
                    strenght = Math.Min(strenght, 120f);
                    var newX = i*xIncrement;
                    var newY = ClientRectangle.Height - AxisMargin - strenght*yIncrement;
                    
                    _points[i].X = AxisMargin + newX;
                    _points[i].Y = newY;
                }
                _graphics.DrawLines(spectrumPen, _points);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                _buffer.Dispose();
                _graphics.Dispose();
                _bkgBuffer.Dispose();
                _buffer = new Bitmap(Width, Height);
                _graphics = Graphics.FromImage(_buffer);
                _bkgBuffer = new Bitmap(Width, Height);
                if (_spectrumWidth > 0)
                {
                    _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float)_spectrumWidth;
                }
                GenerateCursor();
                DrawBackground();
                Draw();
                Invalidate();
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            // Prevent default background painting
        }

        protected virtual void OnFrequencyChanged(FrequencyEventArgs e)
        {
            if (FrequencyChanged != null)
            {
                FrequencyChanged(this, e);
            }
        }

        protected virtual void OnCenterFrequencyChanged(FrequencyEventArgs e)
        {
            if (CenterFrequencyChanged != null)
            {
                CenterFrequencyChanged(this, e);
            }
        }

        protected virtual void OnBandwidthChanged(BandwidthEventArgs e)
        {
            if (BandwidthChanged != null)
            {
                BandwidthChanged(this, e);
            }
        }

        private void UpdateFrequency(int f)
        {
            if (f < _centerFrequency - _spectrumWidth / 2)
            {
                f = _centerFrequency - _spectrumWidth / 2;
            }
            if (f > _centerFrequency + _spectrumWidth / 2)
            {
                f = _centerFrequency + _spectrumWidth / 2;
            }

            f = 10 * (f / 10);

            if (f != _frequency)
            {
                OnFrequencyChanged(new FrequencyEventArgs(f));
            }
        }

        private void UpdateCenterFrequency(int f)
        {
            if (f < 0)
            {
                f = 0;
            }

            f = 10 * (f / 10);

            if (f != _frequency)
            {
                OnCenterFrequencyChanged(new FrequencyEventArgs(f));
            }
        }

        private void UpdateBandwidth(int bw)
        {
            bw = 10 * (bw / 10);

            if (bw < 10)
            {
                bw = 10;
            }

            if (bw != _filterBandwidth)
            {
                OnBandwidthChanged(new BandwidthEventArgs(bw));
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.X >= _lower && e.X <= _upper)
            {
                switch (_bandType)
                {
                    case BandType.Lower:
                        _delta = (int)(_upper - e.X);
                        break;

                    case BandType.Upper:
                        _delta = (int)(_lower - e.X);
                        break;

                    case BandType.Center:
                        _delta = (int)((_lower + _upper) / 2 - e.X);
                        break;
                }

                if (_spectrumWidth > 0)
                {
                    _changingFrequency = true;
                }
            }
            else if ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) < Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Lower))
                ||
                (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) < Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Upper)))
            {
                _oldX = e.X;
                _oldFilterBandwidth = _filterBandwidth;
                _changingBandwidth = true;
            }
            else
            {
                _oldX = e.X;
                _oldCenterFrequency = _centerFrequency;
                _changingCenterFrequency = true;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _changingBandwidth = false;
            _changingFrequency = false;
            _changingCenterFrequency = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_changingFrequency)
            {
                var f = (e.X + _delta - AxisMargin) * _spectrumWidth / (ClientRectangle.Width - 2 * AxisMargin) + _centerFrequency - _spectrumWidth / 2;
                UpdateFrequency(f);
            }
            else if (_changingCenterFrequency)
            {
                var f = (_oldX - e.X) * _spectrumWidth / (ClientRectangle.Width - 2 * AxisMargin) + _oldCenterFrequency;
                UpdateCenterFrequency(f);
            }
            else if (_changingBandwidth)
            {
                var bw = 0;
                switch (_bandType)
                {
                    case BandType.Upper:
                        bw = e.X - _oldX;
                        break;

                    case BandType.Lower:
                        bw = _oldX - e.X;
                        break;

                    case BandType.Center:
                        bw = (_oldX > (_lower + _upper) / 2 ? e.X - _oldX : _oldX - e.X) * 2;
                        break;
                }
                bw = bw * _spectrumWidth / (ClientRectangle.Width - 2 * AxisMargin) + _oldFilterBandwidth;
                UpdateBandwidth(bw);
            }
            if ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) < Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Lower))
                ||
                (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) < Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Upper)))
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateFrequency(_frequency + e.Delta / 10);
        }
    }
}