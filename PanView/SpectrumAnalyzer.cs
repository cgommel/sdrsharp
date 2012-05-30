using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.PanView
{
    public class SpectrumAnalyzer : UserControl
    {
        private const int AxisMargin = 30;
        private const int CarrierPenWidth = 1;
        private const int GradientAlpha = 180;

        private readonly static double _attack = Utils.GetDoubleSetting("spectrumAnalyzerAttack", 0.9);
        private readonly static double _decay = Utils.GetDoubleSetting("spectrumAnalyzerDecay", 0.3);
        private readonly static Color _spectrumColor = Utils.GetColorSetting("spectrumAnalyzerColor", Color.DarkGray);
        private readonly static bool _fillSpectrumAnalyzer = Utils.GetBooleanSetting("fillSpectrumAnalyzer");

        private bool _performNeeded;
        private bool _drawBackgroundNeeded;
        private byte[] _spectrum;
        private byte[] _temp;
        private Bitmap _bkgBuffer;
        private Bitmap _buffer;
        private Graphics _graphics;
        private int _spectrumWidth;
        private long _centerFrequency;
        private long _displayCenterFrequency;
        private Point[] _points;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _filterOffset;
        private float _xIncrement;
        private long _frequency;
        private float _lower;
        private float _upper;
        private int _zoom;
        private float _scale = 1.01f;
        private int _oldX;
        private int _oldFilterBandwidth;
        private long _oldFrequency;
        private long _oldCenterFrequency;
        private bool _changingBandwidth;
        private bool _changingFrequency;
        private bool _changingCenterFrequency;
        private bool _useSmoothing;
        private bool _hotTrackNeeded;
        private LinearGradientBrush _gradientBrush;
        private ColorBlend _gradientColorBlend = Utils.GetGradientBlend(GradientAlpha, "spectrumAnalyzerGradient");

        public SpectrumAnalyzer()
        {
            _bkgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _graphics = Graphics.FromImage(_buffer);
            _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin, AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin), Color.White, Color.Black, LinearGradientMode.Vertical);
            _gradientBrush.InterpolationColors = _gradientColorBlend;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, false);
            SetStyle(ControlStyles.DoubleBuffer, false);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            UpdateStyles();
        }

        ~SpectrumAnalyzer()
        {
            _buffer.Dispose();
            _graphics.Dispose();
            _gradientBrush.Dispose();
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
                    ApplyZoom();
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
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public int FilterOffset
        {
            get
            {
                return _filterOffset;
            }
            set
            {
                if (_filterOffset != value)
                {
                    _filterOffset = value;
                    _drawBackgroundNeeded = true;
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
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public long Frequency
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
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public long CenterFrequency
        {
            get
            {
                return _centerFrequency;
            }
            set
            {
                if (_centerFrequency != value)
                {
                    var delta = value - _centerFrequency;
                    _displayCenterFrequency += delta;
                    
                    _centerFrequency = value;
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public int Zoom
        {
            get
            {
                return _zoom;
            }
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    ApplyZoom();
                }
            }
        }

        public bool UseSmoothing
        {
            get { return _useSmoothing; }
            set { _useSmoothing = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ColorBlend GradientColorBlend
        {
            get
            {
                return _gradientColorBlend;
            }
            set
            {
                _gradientColorBlend = new ColorBlend(value.Colors.Length);
                for (var i = 0; i < value.Colors.Length; i++)
                {
                    _gradientColorBlend.Colors[i] = Color.FromArgb(GradientAlpha, value.Colors[i]);
                    _gradientColorBlend.Positions[i] = value.Positions[i];
                }
                _gradientBrush.Dispose();
                _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin, AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin), Color.White, Color.Black, LinearGradientMode.Vertical);
                _gradientBrush.InterpolationColors = _gradientColorBlend;

                _drawBackgroundNeeded = true;
                _performNeeded = true;
            }
        }

        private void ApplyZoom()
        {
            _scale = 0.01f + (float) Math.Pow(10, _zoom * Waterfall.MaxZoom / 100.0f);
            if (_spectrumWidth > 0)
            {
                _displayCenterFrequency = GetDisplayCenterFrequency();
                _xIncrement = _scale * (ClientRectangle.Width - 2 * AxisMargin) / _spectrumWidth;
                _drawBackgroundNeeded = true;
                _performNeeded = true;
            }
        }

        private long GetDisplayCenterFrequency()
        {
            var f = _frequency;
            switch (_bandType)
            {
                case BandType.Lower:
                    f -= _filterBandwidth / 2 + _filterOffset;
                    break;

                case BandType.Upper:
                    f += _filterBandwidth / 2 + _filterOffset;
                    break;
            }
            var lowerLeadingSpectrum = (long) ((_centerFrequency - _spectrumWidth / 2) - (f - _spectrumWidth / _scale / 2));
            if (lowerLeadingSpectrum > 0)
            {
                f += lowerLeadingSpectrum + 10;
            }

            var upperLeadingSpectrum = (long) ((f + _spectrumWidth / _scale / 2) - (_centerFrequency + _spectrumWidth / 2));
            if (upperLeadingSpectrum > 0)
            {
                f -= upperLeadingSpectrum + 10;
            }

            return f;
        }

        public void Perform()
        {
            if (_drawBackgroundNeeded)
            {
                Draw();
            }
            if (_performNeeded)
            {
                _performNeeded = false;
                Invalidate();
            }
        }

        private void DrawCursor()
        {
            _lower = 0f;
            float bandpassOffset;
            var bandpassWidth = 0f;
            var cursorWidth = Math.Max((_filterBandwidth + _filterOffset) * _xIncrement, 2);
            var xCarrier = (float) ClientRectangle.Width / 2 + (_frequency - _displayCenterFrequency) * _xIncrement;

            switch (_bandType)
            {
                case BandType.Upper:
                    bandpassOffset = _filterOffset * _xIncrement;
                    bandpassWidth = cursorWidth - bandpassOffset;
                    _lower = xCarrier + bandpassOffset;
                    break;

                case BandType.Lower:
                    bandpassOffset = _filterOffset * _xIncrement;
                    bandpassWidth = cursorWidth - bandpassOffset;
                    _lower = xCarrier - bandpassOffset - bandpassWidth;
                    break;

                case BandType.Center:
                    _lower = xCarrier - cursorWidth / 2;
                    bandpassWidth = cursorWidth;
                    break;
            }
            _upper = _lower + bandpassWidth;

            if (cursorWidth < ClientRectangle.Width)
            {
                using (var transparentBrush = new SolidBrush(Color.FromArgb(80, Color.White)))
                using (var carrierPen = new Pen(Color.Red))
                using (var hotTrackPen = new Pen(Color.Red))
                using (var graphics = Graphics.FromImage(_buffer))
                {
                    carrierPen.Width = CarrierPenWidth;
                    graphics.FillRectangle(transparentBrush, _lower, 0, bandpassWidth, ClientRectangle.Height);
                    if (xCarrier >= AxisMargin && xCarrier <= ClientRectangle.Width - AxisMargin)
                    {
                        graphics.DrawLine(carrierPen, xCarrier, 0f, xCarrier, ClientRectangle.Height);
                    }

                    if (_hotTrackNeeded && _oldX >= AxisMargin && _oldX <= ClientRectangle.Width - AxisMargin)
                    {
                        graphics.DrawLine(hotTrackPen, _oldX, 0f, _oldX, ClientRectangle.Height);
                    }
                }
            }
        }

        public void Render(byte[] spectrum, int length)
        {
            var scaledLength = (int)(length / _scale);
            var offset = (int)((length - scaledLength) / 2.0 + length * (double) (_displayCenterFrequency - _centerFrequency) / _spectrumWidth);
            if (_useSmoothing)
            {
                Waterfall.SmoothCopy(spectrum, _temp, length, _scale, offset);
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var ratio = _spectrum[i] < _temp[i] ? _attack : _decay;
                    _spectrum[i] = (byte) (_spectrum[i] * (1 - ratio) + _temp[i] * ratio);
                }
            }
            else
            {
                Waterfall.SmoothCopy(spectrum, _spectrum, length, _scale, offset);
            }
            Draw();
            _performNeeded = true;
        }

        private void DrawBackground()
        {
            #region Draw grid

            using (var bkgBrush = new SolidBrush(Color.Black))
            using (var fontBrush = new SolidBrush(Color.Silver))
            using (var gridPen = new Pen(Color.FromArgb(80, 80, 80)))
            using (var axisPen = new Pen(Color.DarkGray))
            using (var font = new Font("Arial", 8f))
            using (var graphics = Graphics.FromImage(_bkgBuffer))
            {
                graphics.SmoothingMode = SmoothingMode.HighSpeed;

                // Background
                graphics.FillRectangle(bkgBrush, ClientRectangle);

                // Grid
                gridPen.DashStyle = DashStyle.Dash;

                // Decibels
                var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / 13.0f;
                for (var i = 1; i <= 13; i++)
                {
                    graphics.DrawLine(gridPen, AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement), ClientRectangle.Width - AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement));
                }
                for (var i = 0; i <= 13; i++)
                {
                    var db = (-(13 - i) * 10).ToString();
                    var sizeF = graphics.MeasureString(db, font);
                    var width = sizeF.Width;
                    var height = sizeF.Height;
                    graphics.DrawString(db, font, fontBrush, AxisMargin - width - 5, ClientRectangle.Height - AxisMargin - i * yIncrement - height / 2f);
                }

                // Axis
                graphics.DrawLine(axisPen, AxisMargin, AxisMargin, AxisMargin, ClientRectangle.Height - AxisMargin);
                graphics.DrawLine(axisPen, AxisMargin, ClientRectangle.Height - AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin);

                if (_spectrumWidth <= 0)
                {
                    return;
                }

                // Frequencies
                var baseLabelLength = (int) graphics.MeasureString("1,000,000.000kHz", font).Width;
                var frequencyStep = (int) (_spectrumWidth / _scale * baseLabelLength / (ClientRectangle.Width - 2 * AxisMargin));
                int stepSnap = 1000;
                frequencyStep = frequencyStep / stepSnap * stepSnap + stepSnap;
                var lineCount = (int) (_spectrumWidth / _scale / frequencyStep) + 4;
                var xIncrement = (ClientRectangle.Width - 2.0f * AxisMargin) * frequencyStep * _scale / _spectrumWidth;
                var centerShift = (int)((_displayCenterFrequency % frequencyStep) * (ClientRectangle.Width - 2.0 * AxisMargin) * _scale / _spectrumWidth);
                for (var i = -lineCount / 2; i < lineCount / 2; i++)
                {
                    var x = (ClientRectangle.Width - 2 * AxisMargin) / 2 + AxisMargin + xIncrement * i - centerShift;
                    if (x >= AxisMargin && x <= ClientRectangle.Width - AxisMargin)
                    {
                        graphics.DrawLine(gridPen, x, AxisMargin, x, ClientRectangle.Height - AxisMargin);
                    }
                }

                // Frequency line
                for (var i = -lineCount / 2; i < lineCount / 2; i++)
                {
                    var frequency = _displayCenterFrequency + i * frequencyStep - _displayCenterFrequency % frequencyStep;
                    string f;
                    if (frequency == 0)
                    {
                        f = "DC";
                    }
                    else if (Math.Abs(frequency) > 1500000000)
                    {
                        f = string.Format("{0:#,0.000 000}GHz", frequency / 1000000000.0);
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
                    var x = (ClientRectangle.Width - 2 * AxisMargin) / 2 + AxisMargin + xIncrement * i - centerShift;
                    if (x >= AxisMargin && x <= ClientRectangle.Width - AxisMargin)
                    {
                        x -= width / 2f;
                        graphics.DrawString(f, font, fontBrush, x, ClientRectangle.Height - AxisMargin + 5f);
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

            if (_drawBackgroundNeeded)
            {
                _drawBackgroundNeeded = false;
                DrawBackground();
            }

            CopyBackground();

            DrawSpectrum();

            DrawCursor();
        }

        private unsafe void CopyBackground()
        {
            var data1 = _buffer.LockBits(ClientRectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            var data2 = _bkgBuffer.LockBits(ClientRectangle, ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            Utils.Memcpy((void*) data1.Scan0, (void*) data2.Scan0, data1.Width * data1.Height * 4);
            _buffer.UnlockBits(data1);
            _bkgBuffer.UnlockBits(data2);
        }

        private void DrawSpectrum()
        {
            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }

            var xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float) _spectrum.Length;
            var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / (float) byte.MaxValue;

            using (var spectrumPen = new Pen(_spectrumColor))
            {
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var strenght = _spectrum[i];
                    var newX = (int) (i * xIncrement);
                    var newY = (int) (ClientRectangle.Height - AxisMargin - strenght * yIncrement);
                    
                    _points[i + 1].X = AxisMargin + newX;
                    _points[i + 1].Y = newY;
                }
                if (_fillSpectrumAnalyzer)
                {
                    _points[0].X = AxisMargin;
                    _points[0].Y = ClientRectangle.Height - AxisMargin + 1;
                    _points[_points.Length - 1].X = ClientRectangle.Width - AxisMargin;
                    _points[_points.Length - 1].Y = ClientRectangle.Height - AxisMargin + 1;
                    _graphics.FillPolygon(_gradientBrush, _points);
                }

                _points[0] = _points[1];
                _points[_points.Length - 1] = _points[_points.Length - 2];
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
            if (ClientRectangle.Width > 0 && ClientRectangle.Height > 0)
            {
                _buffer.Dispose();
                _graphics.Dispose();
                _bkgBuffer.Dispose();
                _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
                _graphics = Graphics.FromImage(_buffer);
                _graphics.SmoothingMode = SmoothingMode.HighSpeed;
                _bkgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
                var length = ClientRectangle.Width - 2 * AxisMargin;
                var oldSpectrum = _spectrum;
                _spectrum = new byte[length];
                if (oldSpectrum != null)
                {
                    Waterfall.SmoothCopy(oldSpectrum, _spectrum, oldSpectrum.Length, 1, 0);
                }
                else
                {
                    for (var i = 0; i < _spectrum.Length; i++)
                    {
                        _spectrum[i] = 0;
                    }
                }
                _temp = new byte[length];
                _points = new Point[length + 2];
                if (_spectrumWidth > 0)
                {
                    _xIncrement = _scale * (ClientRectangle.Width - 2 * AxisMargin) / _spectrumWidth;
                }

                _gradientBrush.Dispose();
                _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin, AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin), Color.White, Color.Black, LinearGradientMode.Vertical);
                _gradientBrush.InterpolationColors = _gradientColorBlend;

                _drawBackgroundNeeded = true;
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

        private void UpdateFrequency(long f)
        {
            var min = (long) (_displayCenterFrequency - _spectrumWidth / _scale / 2);
            if (f < min)
            {
                f = min;
            }
            var max = (long) (_displayCenterFrequency + _spectrumWidth / _scale / 2);
            if (f > max)
            {
                f = max;
            }

            f = 10 * (f / 10);

            if (f != _frequency)
            {
                OnFrequencyChanged(new FrequencyEventArgs(f));
            }
        }

        private void UpdateCenterFrequency(long f)
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
            var cursorWidth = Math.Max((_filterBandwidth + _filterOffset) * _xIncrement, 2);
            if (e.X > _lower && e.X < _upper && cursorWidth < ClientRectangle.Width)
            {
                _oldX = e.X;
                _oldFrequency = _frequency;
                _changingFrequency = true;
            }
            else if ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Lower))
                ||
                (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
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

            if (_changingCenterFrequency && e.X == _oldX)
            {
                _hotTrackNeeded = false;
                _drawBackgroundNeeded = true;
                _performNeeded = true;
                var f = (long)((_oldX - ClientRectangle.Width / 2) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _displayCenterFrequency);
                UpdateFrequency(f);
            }
            _changingBandwidth = false;
            _changingFrequency = false;
            _changingCenterFrequency = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            _hotTrackNeeded = false;
            if (_changingFrequency)
            {
                var f = (long)((e.X - _oldX) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _oldFrequency);
                UpdateFrequency(f);
            }
            else if (_changingCenterFrequency)
            {
                var f = (long)((_oldX - e.X) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _oldCenterFrequency);
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
                bw = (int)(bw * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin)) + _oldFilterBandwidth;
                UpdateBandwidth(bw);
            }
            else if ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Lower))
                ||
                (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Upper)))
            {
                Cursor = Cursors.SizeWE;
                _drawBackgroundNeeded = true;
                _performNeeded = true;
            }
            else
            {
                Cursor = Cursors.Default;
                if (e.X < _lower - Waterfall.CursorSnapDistance || e.X > _upper + Waterfall.CursorSnapDistance)
                {
                    _oldX = e.X;
                    _hotTrackNeeded = true;
                }
                else
                {
                    _hotTrackNeeded = false;
                }
                _drawBackgroundNeeded = true;
                _performNeeded = true;
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateFrequency(_frequency + e.Delta / 10);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hotTrackNeeded = false;
            _drawBackgroundNeeded = true;
            _performNeeded = true;
        }
    }
}