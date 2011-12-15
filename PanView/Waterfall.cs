using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace SDRSharp.PanView
{
    public enum BandType
    {
        Lower,
        Upper,
        Center
    }

    public delegate void ManualFrequencyChange(object sender, FrequencyEventArgs e);

    public class Waterfall : UserControl
    {
        private const int CarrierPenWidth = 1;
        private const int AxisMargin = 30;
        private const int CursorSnapDistance = 2;
        private const float MinimumLevel = 120.0f;

        private bool _performNeeded;
        private Bitmap _buffer;
        private Bitmap _buffer2;
        private Bitmap _cursor;
        private Graphics _graphics;
        private Graphics _graphics2;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _frequencyOffset;
        private float _xIncrement;
        private double[] _temp;
        private double[] _spectrum;
        private int _centerFrequency;
        private int _spectrumWidth;
        private int _frequency;
        private float _lower;
        private float _upper;
        private int _delta;
        private bool _changingFrequency;
        private bool _changingCenterFrequency;
        private bool _highDefinition;
        private bool _mouseIn;
        private int _oldX;
        private int _oldCenterFrequency;
        private int[] _gradientPixels;
        private LinearGradientBrush _gradientBrush;
        private ColorBlend _gradientColorBlend = GetGradientBlend();

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr Memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        public Waterfall()
        {
            _spectrum = new double[ClientRectangle.Width - 2 * AxisMargin];
            _temp = new double[_spectrum.Length];
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _buffer2 = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _cursor = new Bitmap(10, 10, PixelFormat.Format32bppPArgb);
            _graphics = Graphics.FromImage(_buffer);
            _graphics2 = Graphics.FromImage(_buffer2);
            _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin / 2, AxisMargin / 2, Width - AxisMargin / 2, Height - AxisMargin / 2), Color.White, Color.Black, LinearGradientMode.Vertical);
            _gradientBrush.InterpolationColors = _gradientColorBlend;
        }

        private static ColorBlend GetGradientBlend()
        {
            var colorBlend = new ColorBlend();
            
            string colorString;
            try
            {
                colorString = ConfigurationManager.AppSettings["gradient"] ?? string.Empty;
            }
            catch
            {
                colorString = string.Empty;
            }
            var colorPatterns = colorString.Split(',');
            if (colorPatterns.Length < 2)
            {
                //colorBlend.Colors = new[] { Color.White, Color.Yellow, Color.Red, Color.FromArgb(56, 3, 2), Color.Black };
                colorBlend.Colors = new[] { Color.White, Color.LightBlue, Color.DodgerBlue, Color.FromArgb(0, 0, 80), Color.Black, Color.Black };
                //colorBlend.Colors = new[] { Color.Red, Color.Orange, Color.Yellow, Color.Lime, Color.DodgerBlue, Color.DarkBlue, Color.Black, Color.Black };                
            }
            else
            {
                colorBlend.Colors = new Color[colorPatterns.Length];
                for (var i = 0; i < colorPatterns.Length; i++)
                {
                    var colorPattern = colorPatterns[i];
                    var r = int.Parse(colorPattern.Substring(0, 2), NumberStyles.HexNumber);
                    var g = int.Parse(colorPattern.Substring(2, 2), NumberStyles.HexNumber);
                    var b = int.Parse(colorPattern.Substring(4, 2), NumberStyles.HexNumber);
                    colorBlend.Colors[i] = Color.FromArgb(r, g, b);
                }
            }

            var positions = new float[colorBlend.Colors.Length];
            var distance = 1f / (positions.Length - 1);
            for (var i = 0; i < positions.Length; i++)
            {
                positions[i] = i * distance;
            }
            colorBlend.Positions = positions;
            return colorBlend;
        }

        ~Waterfall()
        {
            _buffer.Dispose();
            _graphics.Dispose();
            _graphics2.Dispose();
            _cursor.Dispose();
            _gradientBrush.Dispose();
        }

        public void Perform()
        {
            if (_performNeeded)
            {
                _performNeeded = false;
                Invalidate();
            }
        }

        public event ManualFrequencyChange FrequencyChanged;

        public event ManualFrequencyChange CenterFrequencyChanged;

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
                if (_gradientColorBlend != value)
                {
                    _gradientColorBlend = value;

                    _gradientBrush.Dispose();
                    _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin / 2, AxisMargin / 2, Width - AxisMargin / 2, Height - AxisMargin / 2), Color.White, Color.Black, LinearGradientMode.Vertical);
                    _gradientPixels = null;
                    _gradientBrush.InterpolationColors = _gradientColorBlend;
                    
                    DrawGradient();

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
                    PositionCursor();
                    _performNeeded = true;
                }
            }
        }

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
                return _frequencyOffset;
            }
            set
            {
                if (_frequencyOffset != value)
                {
                    _frequencyOffset = value;
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

        public bool HighDefinition
        {
            get
            {
                return _highDefinition;
            }
            set
            {
                _highDefinition = value;
            }
        }

        public void SmoothCopy(double[] source, double[] destination, int maxSource)
        {
            var r = maxSource / (double)destination.Length;
            if (r > 1.0)
            {
                var n = (int) r;
                for (var i = 0; i < destination.Length; i++)
                {
                    var k = (int) (i * r) - n / 2;
                    var sum = 0.0;
                    var count = 0;
                    for (var j = 0; j < n; j++)
                    {
                        var index = k + j;
                        if (index >= 0 && index < maxSource)
                        {
                            sum += source[index];
                            count++;
                        }
                    }
                    destination[i] = sum / count;
                }
            }
            else
            {
                for (var i = 0; i < destination.Length; i++)
                {
                    destination[i] = source[(int) (r * i)];
                }
            }
        }

        public void Render(double[] spectrum, int length)
        {
            SmoothCopy(spectrum, _temp, length);

            const double attack = 0.9;
            const double decay = 0.4;
            for (var i = 0; i < _spectrum.Length; i++)
            {
                var ratio = _spectrum[i] < _temp[i] ? attack : decay;
                _spectrum[i] = _spectrum[i] * (1 - ratio) + _temp[i] * ratio;
            }
            Draw();
            _performNeeded = true;
        }

        private void Draw()
        {
            #region Draw only if needed

            if (ClientRectangle.Width <= AxisMargin || ClientRectangle.Height <= AxisMargin)
            {
                return;
            }

            #endregion

            #region Shift image

            var bmpData = _buffer.LockBits(ClientRectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            var ptr = new IntPtr((long) bmpData.Scan0 + bmpData.Stride);
            Memcpy(ptr, bmpData.Scan0, new UIntPtr((ulong)((bmpData.Height - 1) * bmpData.Width * 4)));
            _buffer.UnlockBits(bmpData);

            DrawGradient();

            #endregion

            #region Draw Spectrum

            DrawSpectrum();

            #endregion
        }

        private unsafe void DrawSpectrum()
        {
            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }
            var bits = _buffer.LockBits(ClientRectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            var ptr = (int*) bits.Scan0 + AxisMargin;
            for (var i = 0; i < _spectrum.Length; i++)
            {
                var colorIndex = (int)((MinimumLevel + _spectrum[i]) * _gradientPixels.Length / MinimumLevel);
                colorIndex = Math.Max(colorIndex, 0);
                colorIndex = Math.Min(colorIndex, _gradientPixels.Length - 1);
                
                *ptr++ = _gradientPixels[colorIndex];
            }
            _buffer.UnlockBits(bits);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_mouseIn)
            {
                _graphics2.DrawImageUnscaled(_buffer, 0, 0);
                if (_spectrumWidth > 0)
                {
                    _graphics2.DrawImage(_cursor, (int)_lower, 0);
                }
                e.Graphics.DrawImageUnscaled(_buffer2, 0, 0);
            }
            else
            {
                e.Graphics.DrawImageUnscaled(_buffer, 0, 0);
            }
        }

        private void PositionCursor()
        {
            var cursorWidth = Math.Max(_filterBandwidth * _xIncrement, 2);
            var relativeOffset = _frequencyOffset * _xIncrement;
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

        private void GenerateCursor()
        {
            var cursorWidth = Math.Max(_filterBandwidth * _xIncrement, 2);
            var bandpassWidth = cursorWidth;
            var bandpassOffset = _frequencyOffset * _xIncrement;
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
            _cursor = new Bitmap((int) cursorWidth, ClientRectangle.Height, PixelFormat.Format32bppPArgb);

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

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (ClientRectangle.Width <= AxisMargin || ClientRectangle.Height <= AxisMargin)
            {
                return;
            }
            var temp = new double[ClientRectangle.Width - 2 * AxisMargin];
            SmoothCopy(_spectrum, temp, _spectrum.Length);
            _spectrum = temp;
            _temp = new double[_spectrum.Length];
            var oldBuffer = _buffer;
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            var oldBuffer2 = _buffer2;
            _buffer2 = new Bitmap(ClientRectangle.Width, ClientRectangle.Height, PixelFormat.Format32bppPArgb);
            _graphics.Dispose();
            _graphics = Graphics.FromImage(_buffer);
            _graphics2.Dispose();
            _graphics2 = Graphics.FromImage(_buffer2);
            using (var bkgBrush = new SolidBrush(Color.Black))
            {
                _graphics.FillRectangle(bkgBrush, ClientRectangle);
            }
            var rect = new Rectangle(AxisMargin, 1, _buffer.Width - 2 * AxisMargin, _buffer.Height);
            _graphics.DrawImage(oldBuffer, rect, AxisMargin, 1, oldBuffer.Width - 2 * AxisMargin, oldBuffer.Height, GraphicsUnit.Pixel);
            oldBuffer.Dispose();
            oldBuffer2.Dispose();
            if (_spectrumWidth > 0)
            {
                _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float) _spectrumWidth;
            }
            _gradientBrush.Dispose();
            _gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin / 2, AxisMargin / 2, Width - AxisMargin / 2, Height - AxisMargin / 2), Color.White, Color.Black, LinearGradientMode.Vertical);
            _gradientPixels = null;
            _gradientBrush.InterpolationColors = _gradientColorBlend;
            DrawGradient();
            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            _graphics2.SmoothingMode = SmoothingMode.HighSpeed;
            GenerateCursor();
            Invalidate();
        }

        private void DrawGradient()
        {
            _graphics.SmoothingMode = SmoothingMode.HighSpeed;
            using (var pen = new Pen(_gradientBrush, 10))
            {
                _graphics.FillRectangle(Brushes.Black,
                                   ClientRectangle.Width - AxisMargin,
                                   0,
                                   AxisMargin,
                                   ClientRectangle.Height);
                _graphics.DrawLine(pen,
                                   ClientRectangle.Width - AxisMargin / 2,
                                   ClientRectangle.Height - AxisMargin / 2,
                                   ClientRectangle.Width - AxisMargin / 2,
                                   AxisMargin / 2);

                if (_gradientPixels == null || _gradientPixels.Length != ClientRectangle.Height - AxisMargin)
                {
                    _gradientPixels = new int[ClientRectangle.Height - AxisMargin];
                }
                for (var i = 0; i < _gradientPixels.Length; i++)
                {
                    _gradientPixels[_gradientPixels.Length - i - 1] = _buffer.GetPixel(ClientRectangle.Width - AxisMargin / 2, i + AxisMargin / 2).ToArgb();
                }
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

            f = 1000 * (f / 1000);
            if (f != _centerFrequency)
            {
                OnCenterFrequencyChanged(new FrequencyEventArgs(f));
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
                _changingFrequency = true;
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
            if (Math.Abs(e.X - _lower + CursorSnapDistance) < CursorSnapDistance || Math.Abs(e.X - _upper - CursorSnapDistance) < CursorSnapDistance)
            {
                Cursor = Cursors.SizeWE;
            }
            else
            {
                Cursor = Cursors.Default;
            }
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseIn = true;
            _performNeeded = true;
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseIn = false;
            _performNeeded = true;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateFrequency(_frequency + e.Delta / 10);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Left:
                    UpdateCenterFrequency(_centerFrequency - 1000);
                    break;

                case Keys.Right:
                    UpdateCenterFrequency(_centerFrequency + 1000);
                    break;
            }
        }
    }

    public class FrequencyEventArgs : EventArgs
    {
        public int Frequency { get; set; }

        public FrequencyEventArgs(int frequency)
        {
            Frequency = frequency;
        }
    }
}