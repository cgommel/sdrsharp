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
        private const float TrackingFontSize = 16.0f;
        private const int AxisMargin = 30;
        private const int CarrierPenWidth = 1;
        private const int GradientAlpha = 180;

        public const float DefaultCursorHeight = 32.0f;

        private readonly static Color _spectrumColor = Utils.GetColorSetting("spectrumAnalyzerColor", Color.DarkGray);
        private readonly static bool _fillSpectrumAnalyzer = Utils.GetBooleanSetting("fillSpectrumAnalyzer");

        private double _attack;
        private double _decay;
        private bool _performNeeded;
        private bool _drawBackgroundNeeded;
        private byte[] _maxSpectrum;
        private byte[] _spectrum;
        private bool[] _peaks;
        private byte[] _temp;
        private byte[] _scaledPowerSpectrum;
        private Bitmap _bkgBuffer;
        private Bitmap _buffer;
        private Graphics _graphics;
        private long _spectrumWidth;
        private long _centerFrequency;
        private long _displayCenterFrequency;
        private Point[] _points;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _filterOffset;
        private int _stepSize = 1000;
        private float _xIncrement;
        private long _frequency;
        private float _lower;
        private float _upper;
        private int _zoom;
        private float _scale = 1f;
        private int _oldX;
        private int _trackingX;
        private int _trackingY;
        private long _trackingFrequency;
        private int _oldFilterBandwidth;
        private long _oldFrequency;
        private long _oldCenterFrequency;
        private bool _changingBandwidth;
        private bool _changingFrequency;
        private bool _changingCenterFrequency;
        private bool _useSmoothing;
        private bool _enableFilter = true;
        private bool _hotTrackNeeded;
        private bool _useSnap;
        private bool _markPeaks;
        private bool _showMaxLine;
        private float _trackingPower;
        private string _statusText;
        private int _displayRange = 130;
        private int _displayOffset;
        private LinearGradientBrush _gradientBrush;
        private ColorBlend _gradientColorBlend = Utils.GetGradientBlend(GradientAlpha, "spectrumAnalyzerGradient");

        public SpectrumAnalyzer()
        {
            _bkgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _graphics = Graphics.FromImage(_buffer);
            _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin, AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin), Color.White, Color.Black, LinearGradientMode.Vertical);
            _gradientBrush.InterpolationColors = _gradientColorBlend;
             
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
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
                return (int) _spectrumWidth;
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
                    _displayCenterFrequency += value - _centerFrequency;
                    _centerFrequency = value;
                    _drawBackgroundNeeded = true;
                }
            }
        }

        public int DisplayRange
        {
            get { return _displayRange; }
            set
            {
                if (_displayRange != value)
                {
                    _displayRange = value;
                    _drawBackgroundNeeded = true;
                }
            }
        }

        public int DisplayOffset
        {
            get { return _displayOffset; }
            set
            {
                if (_displayOffset != value)
                {
                    _displayOffset = value;
                    _drawBackgroundNeeded = true;
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

        public bool EnableFilter
        {
            get { return _enableFilter; }
            set
            {
                _enableFilter = value;
                _performNeeded = true;
            }
        }

        public string StatusText
        {
            get { return _statusText; }
            set
            {
                _statusText = value;
                _performNeeded = true;
            }
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
            }
        }

        public double Attack
        {
            get { return _attack; }
            set { _attack = value; }
        }

        public double Decay
        {
            get { return _decay; }
            set { _decay = value; }
        }

        public int StepSize
        {
            get { return _stepSize; }
            set
            {
                if (_stepSize != value)
                {
                    _stepSize = value;
                    _drawBackgroundNeeded = true;
                    _performNeeded = true;
                }
            }
        }

        public bool UseSnap
        {
            get { return _useSnap; }
            set { _useSnap = value; }
        }

        public bool MarkPeaks
        {
            get { return _markPeaks; }
            set { _markPeaks = value; }
        }

        public bool ShowMaxLine
        {
            get { return _showMaxLine; }
            set { _showMaxLine = value; }
        }

        private void ApplyZoom()
        {
            _scale = (float) Math.Pow(10, _zoom * Waterfall.MaxZoom / 100.0f);
            if (_spectrumWidth > 0)
            {
                _displayCenterFrequency = GetDisplayCenterFrequency();
                _xIncrement = _scale * (ClientRectangle.Width - 2 * AxisMargin) / _spectrumWidth;
                _drawBackgroundNeeded = true;
            }
        }

        public void CenterZoom()
        {
            _displayCenterFrequency = GetDisplayCenterFrequency();
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
                DrawBackground();
            }
            if (_performNeeded || _drawBackgroundNeeded)
            {
                DrawForeground();
                Invalidate();
            }
            _performNeeded = false;
            _drawBackgroundNeeded = false;
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

            using (var transparentBackground = new SolidBrush(Color.FromArgb(80, Color.DarkGray)))
            using (var redPen = new Pen(Color.Red))
            using (var graphics = Graphics.FromImage(_buffer))
            using (var fontFamily = new FontFamily("Arial"))
            using (var path = new GraphicsPath())
            using (var outlinePen = new Pen(Color.Black))
            {
                if (_enableFilter && cursorWidth < ClientRectangle.Width)
                {
                    var carrierPen = redPen;
                    carrierPen.Width = CarrierPenWidth;
                    graphics.FillRectangle(transparentBackground, (int) _lower + 1, 0, (int) bandpassWidth, ClientRectangle.Height);
                    if (xCarrier >= AxisMargin && xCarrier <= ClientRectangle.Width - AxisMargin)
                    {
                        graphics.DrawLine(carrierPen, xCarrier, 0f, xCarrier, ClientRectangle.Height);
                    }
                }
                if (_markPeaks && _spectrumWidth > 0)
                {
                    var windowSize = (int) bandpassWidth;
                    windowSize = Math.Max(windowSize, 10);
                    windowSize = Math.Min(windowSize, _spectrum.Length);
                    PeakDetector.GetPeaks(_spectrum, _peaks, windowSize);
                    var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / (float) byte.MaxValue;
                    for (var i = 0; i < _peaks.Length; i++)
                    {
                        if (_peaks[i])
                        {
                            var y = (int) (ClientRectangle.Height - AxisMargin - _spectrum[i] * yIncrement);
                            var x = i + AxisMargin;
                            graphics.DrawEllipse(Pens.Yellow, x - 5, y - 5, 10, 10);
                        }
                    }
                }

                if (_hotTrackNeeded && _trackingX >= AxisMargin && _trackingX <= ClientRectangle.Width - AxisMargin &&
                    _trackingY >= AxisMargin && _trackingY <= ClientRectangle.Height - AxisMargin)
                {
                    if (_spectrum != null && !_changingFrequency && !_changingCenterFrequency && !_changingBandwidth)
                    {
                        var index = _trackingX - AxisMargin;
                        if (_useSnap)
                        {
                            // Todo: snap the index
                        }
                        if (index > 0 && index < _spectrum.Length)
                        {
                            graphics.DrawLine(redPen, _trackingX, 0, _trackingX, ClientRectangle.Height);
                        }
                    }
                    string fstring;
                    if (_changingFrequency)
                    {
                        fstring = "VFO = " + GetFrequencyDisplay(_frequency);
                    }
                    else if (_changingBandwidth)
                    {
                        fstring = "BW = " + GetFrequencyDisplay(_filterBandwidth);
                    }
                    else if (_changingCenterFrequency)
                    {
                        fstring = "Center Freq. = " + GetFrequencyDisplay(_centerFrequency);
                    }
                    else
                    {
                        fstring = string.Format("{0}\r\n{1:0.##}dB", GetFrequencyDisplay(_trackingFrequency), _trackingPower);
                    }

                    path.AddString(fstring, fontFamily, (int)FontStyle.Regular, TrackingFontSize, Point.Empty, StringFormat.GenericTypographic);
                    var stringSize = path.GetBounds();
                    var currentCursor = Cursor.Current;
                    var xOffset = _trackingX + 15.0f;
                    var yOffset = _trackingY + (currentCursor == null ? DefaultCursorHeight : currentCursor.Size.Height) - 8.0f;
                    xOffset = Math.Min(xOffset, ClientRectangle.Width - stringSize.Width - 5);
                    yOffset = Math.Min(yOffset, ClientRectangle.Height - stringSize.Height - 5);
                    path.Reset();
                    path.AddString(fstring, fontFamily, (int)FontStyle.Regular, TrackingFontSize, new Point((int)xOffset, (int)yOffset), StringFormat.GenericTypographic);
                    var smoothingMode = graphics.SmoothingMode;
                    var interpolationMode = graphics.InterpolationMode;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    outlinePen.Width = 2;
                    graphics.DrawPath(outlinePen, path);
                    graphics.FillPath(Brushes.White, path);
                    graphics.SmoothingMode = smoothingMode;
                    graphics.InterpolationMode = interpolationMode;
                }
            }
        }

        public unsafe void Render(float* powerSpectrum, int length)
        {
            if (_scaledPowerSpectrum == null || _scaledPowerSpectrum.Length != length)
            {
                _scaledPowerSpectrum = new byte[length];
            }
            fixed (byte* scaledPowerSpectrumPtr = _scaledPowerSpectrum)
            {
                var displayOffset = _displayOffset / 10 * 10;
                var displayRange = _displayRange / 10 * 10;
                Fourier.ScaleFFT(powerSpectrum, scaledPowerSpectrumPtr, length, displayOffset - displayRange, displayOffset);
            }
            var scaledLength = (int) (length / _scale);
            var offset = (int) ((length - scaledLength) / 2.0 + length * (double) (_displayCenterFrequency - _centerFrequency) / _spectrumWidth);
            if (_useSmoothing)
            {
                Fourier.SmoothCopy(_scaledPowerSpectrum, _temp, length, _scale, offset);
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var ratio = _spectrum[i] < _temp[i] ? Attack : Decay;
                    _spectrum[i] = (byte) Math.Round(_spectrum[i] * (1 - ratio) + _temp[i] * ratio);
                }
            }
            else
            {
                Fourier.SmoothCopy(_scaledPowerSpectrum, _spectrum, length, _scale, offset);
            }

            for (var i = 0; i < _spectrum.Length; i++)
            {
                if (_maxSpectrum[i] < _spectrum[i])
                    _maxSpectrum[i] = _spectrum[i];
            }
            _performNeeded = true;
        }

        private void DrawBackground()
        {
            using (var fontBrush = new SolidBrush(Color.Silver))
            using (var gridPen = new Pen(Color.FromArgb(80, 80, 80)))
            using (var axisPen = new Pen(Color.DarkGray))
            using (var font = new Font("Arial", 8f))
            using (var graphics = Graphics.FromImage(_bkgBuffer))
            {
                ConfigureGraphics(graphics);

                // Background
                graphics.Clear(Color.Black);

                if (_spectrumWidth > 0)
                {
                    #region Frequency markers

                    var baseLabelLength = (int)graphics.MeasureString("1,000,000.000kHz", font).Width;
                    var frequencyStep = (int)(_spectrumWidth / _scale * baseLabelLength / (ClientRectangle.Width - 2 * AxisMargin));
                    int stepSnap = _stepSize;
                    frequencyStep = frequencyStep / stepSnap * stepSnap + stepSnap;
                    var lineCount = (int)(_spectrumWidth / _scale / frequencyStep) + 4;
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

                    for (var i = -lineCount / 2; i < lineCount / 2; i++)
                    {
                        var frequency = _displayCenterFrequency + i * frequencyStep - _displayCenterFrequency % frequencyStep;
                        var fstring = GetFrequencyDisplay(frequency);
                        var sizeF = graphics.MeasureString(fstring, font);
                        var width = sizeF.Width;
                        var x = (ClientRectangle.Width - 2 * AxisMargin) / 2 + AxisMargin + xIncrement * i - centerShift;
                        if (x >= AxisMargin && x <= ClientRectangle.Width - AxisMargin)
                        {
                            x -= width / 2f;
                            graphics.DrawString(fstring, font, fontBrush, x, ClientRectangle.Height - AxisMargin + 8f);
                        }
                    }

                    #endregion
                }

                #region Grid

                gridPen.DashStyle = DashStyle.Dash;

                var powerMarkerCount = _displayRange / 10;

                // Power axis
                var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / (float) powerMarkerCount;
                for (var i = 1; i <= powerMarkerCount; i++)
                {
                    graphics.DrawLine(gridPen, AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement), ClientRectangle.Width - AxisMargin, (int)(ClientRectangle.Height - AxisMargin - i * yIncrement));
                }

                var displayOffset = _displayOffset / 10 * 10;
                for (var i = 0; i <= powerMarkerCount; i++)
                {
                    var db = (displayOffset - (powerMarkerCount - i) * 10).ToString();
                    var sizeF = graphics.MeasureString(db, font);
                    var width = sizeF.Width;
                    var height = sizeF.Height;
                    graphics.DrawString(db, font, fontBrush, AxisMargin - width - 3, ClientRectangle.Height - AxisMargin - i * yIncrement - height / 2f);
                }

                // Axis
                graphics.DrawLine(axisPen, AxisMargin, AxisMargin, AxisMargin, ClientRectangle.Height - AxisMargin);
                graphics.DrawLine(axisPen, AxisMargin, ClientRectangle.Height - AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin);

                #endregion
            }
        }

        public static string GetFrequencyDisplay(long frequency)
        {
            string result;
            if (frequency == 0)
            {
                result = "DC";
            }
            else if (Math.Abs(frequency) > 1500000000)
            {
                result = string.Format("{0:#,0.000 000}GHz", frequency / 1000000000.0);
            }
            else if (Math.Abs(frequency) > 30000000)
            {
                result = string.Format("{0:0,0.000#}MHz", frequency / 1000000.0);
            }
            else if (Math.Abs(frequency) > 1000)
            {
                result = string.Format("{0:#,#.###}kHz", frequency / 1000.0);
            }
            else
            {
                result = string.Format("{0}Hz", frequency);
            }
            return result;
        }

        public static void ConfigureGraphics(Graphics graphics)
        {
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.SmoothingMode = SmoothingMode.None;
            graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.High;
        }

        private void DrawForeground()
        {
            if (ClientRectangle.Width <= AxisMargin || ClientRectangle.Height <= AxisMargin)
            {
                return;
            }

            CopyBackground();

            DrawSpectrum();

            DrawStatusText();

            DrawCursor();
        }

        private unsafe void CopyBackground()
        {
            var data1 = _buffer.LockBits(ClientRectangle, ImageLockMode.WriteOnly, _buffer.PixelFormat);
            var data2 = _bkgBuffer.LockBits(ClientRectangle, ImageLockMode.ReadOnly, _bkgBuffer.PixelFormat);
            Utils.Memcpy((void*) data1.Scan0, (void*) data2.Scan0, Math.Abs(data1.Stride) * data1.Height);
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

            if (_showMaxLine != false)
            {
                using (var spectrumPen = new Pen(Color.Red))
                {
                    for (var i = 0; i < _spectrum.Length; i++)
                    {
                        var strenght = _maxSpectrum[i];
                        var x = (int)(AxisMargin + i * xIncrement);
                        var y = (int)(ClientRectangle.Height - AxisMargin - strenght * yIncrement);
                        _points[i + 1].X = x;
                        _points[i + 1].Y = y;
                    }
                    //if (_fillSpectrumAnalyzer)
                    //{
                    //    _points[0].X = AxisMargin;
                    //    _points[0].Y = ClientRectangle.Height - AxisMargin + 1;
                    //    _points[_points.Length - 1].X = ClientRectangle.Width - AxisMargin;
                    //    _points[_points.Length - 1].Y = ClientRectangle.Height - AxisMargin + 1;
                    //    _graphics.FillPolygon(_gradientBrush, _points);
                    //}

                    _points[0] = _points[1];
                    _points[_points.Length - 1] = _points[_points.Length - 2];
                    _graphics.DrawLines(spectrumPen, _points);
                }
            }


            using (var spectrumPen = new Pen(_spectrumColor))
            {
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var strenght = _spectrum[i];
                    var x = (int) (AxisMargin + i * xIncrement);
                    var y = (int) (ClientRectangle.Height - AxisMargin - strenght * yIncrement);
                    _points[i + 1].X = x;
                    _points[i + 1].Y = y;
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

        private void DrawStatusText()
        {
            if (string.IsNullOrEmpty(_statusText))
            {
                return;
            }
            using (var font = new Font("Lucida Console", 9))
            {
                _graphics.DrawString(_statusText, font, Brushes.White, AxisMargin, 10);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            ConfigureGraphics(e.Graphics);
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
                ConfigureGraphics(_graphics);

                _bkgBuffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
                var length = ClientRectangle.Width - 2 * AxisMargin;
                var oldSpectrum = _spectrum;
                _maxSpectrum = new byte[length];
                _spectrum = new byte[length];
                _peaks = new bool[length];
                if (oldSpectrum != null)
                {
                    Fourier.SmoothCopy(oldSpectrum, _spectrum, oldSpectrum.Length, 1, 0);
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
                Perform();
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

        private void UpdateFrequency(long f, FrequencyChangeSource source)
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

            if (_useSnap)
            {
                f = (f + Math.Sign(f) * _stepSize / 2) / _stepSize * _stepSize;
            }

            if (f != _frequency)
            {
                var args = new FrequencyEventArgs(f, source);
                OnFrequencyChanged(args);
                if (!args.Cancel)
                {
                    _frequency = args.Frequency;
                    _performNeeded = true;
                }
            }
        }

        private void UpdateCenterFrequency(long f, FrequencyChangeSource source)
        {
            if (f < 0)
            {
                f = 0;
            }

            if (_useSnap)
            {
                f = (f + Math.Sign(f) * _stepSize / 2) / _stepSize * _stepSize;
            }

            if (f != _centerFrequency)
            {
                var args = new FrequencyEventArgs(f, source);
                OnCenterFrequencyChanged(args);
                if (!args.Cancel)
                {
                    var delta = args.Frequency - _centerFrequency;
                    _displayCenterFrequency += delta;

                    _centerFrequency = args.Frequency;
                    _drawBackgroundNeeded = true;
                }
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
                var args = new BandwidthEventArgs(bw);
                OnBandwidthChanged(args);
                if (!args.Cancel)
                {
                    _filterBandwidth = args.Bandwidth;
                    _performNeeded = true;
                }
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButtons.Left)
            {
                var cursorWidth = Math.Max((_filterBandwidth + _filterOffset) * _xIncrement, 2);
                if (e.X > _lower && e.X < _upper && cursorWidth < ClientRectangle.Width)
                {
                    _oldX = e.X;
                    _oldFrequency = _frequency;
                    _changingFrequency = true;
                }
                else if (_enableFilter &&
                    ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                    (_bandType == BandType.Center || _bandType == BandType.Lower))
                    ||
                    (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                    (_bandType == BandType.Center || _bandType == BandType.Upper))))
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
            else if (e.Button == MouseButtons.Right)
            {
                UpdateFrequency(_frequency / Waterfall.RightClickSnapDistance * Waterfall.RightClickSnapDistance, FrequencyChangeSource.Click);
                for(int i=0;i<_maxSpectrum.Length;i++)
                    _maxSpectrum[i] = 0;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_changingCenterFrequency && e.X == _oldX)
            {
                var f = (long)((_oldX - ClientRectangle.Width / 2) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _displayCenterFrequency);
                UpdateFrequency(f, FrequencyChangeSource.Click);
            }
            _changingCenterFrequency = false;
            _drawBackgroundNeeded = true;
            _changingBandwidth = false;
            _changingFrequency = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            _trackingX = e.X;
            _trackingY = e.Y;
            _trackingFrequency = (long)((e.X - ClientRectangle.Width / 2) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _displayCenterFrequency);
            if (_useSnap)
            {
                _trackingFrequency = (_trackingFrequency + Math.Sign(_trackingFrequency) * _stepSize / 2) / _stepSize * _stepSize;
            }
            var displayRange = _displayRange / 10 * 10;
            var displayOffset = _displayOffset / 10 * 10;
            var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / (float) displayRange;
            _trackingPower = displayOffset - displayRange - (_trackingY + AxisMargin - ClientRectangle.Height) / yIncrement;


            if (_changingFrequency)
            {
                var f = (long) ((e.X - _oldX) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _oldFrequency);
                UpdateFrequency(f, FrequencyChangeSource.Drag);
            }
            else if (_changingCenterFrequency)
            {
                var f = (long) ((_oldX - e.X) * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _oldCenterFrequency);
                UpdateCenterFrequency(f, FrequencyChangeSource.Drag);
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
                bw = (int) (bw * _spectrumWidth / _scale / (ClientRectangle.Width - 2 * AxisMargin) + _oldFilterBandwidth);
                UpdateBandwidth(bw);
            }
            else if (_enableFilter &&
                ((Math.Abs(e.X - _lower + Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Lower))
                ||
                (Math.Abs(e.X - _upper - Waterfall.CursorSnapDistance) <= Waterfall.CursorSnapDistance &&
                (_bandType == BandType.Center || _bandType == BandType.Upper))))
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.Default;
            }
            _performNeeded = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateFrequency(_frequency + _stepSize * Math.Sign(e.Delta), FrequencyChangeSource.Scroll);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hotTrackNeeded = false;
            _performNeeded = true;
        }

        protected override void  OnMouseEnter(EventArgs e)
        {
            Focus();
 	        base.OnMouseEnter(e);
            _hotTrackNeeded = true;
        }
    }
}