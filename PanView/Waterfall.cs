using System;
using System.ComponentModel;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;

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
        private const float MinimumLevel = 120.0f;

        private Bitmap _buffer;
        private Bitmap _cursor;
        private Graphics _graphics;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _frequencyOffset;
        private float _xIncrement;
        private double[] _spectrum = new double[0];
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
        private ColorBlend _gradientColorBlend = GetGradientBlend();

        public Waterfall()
        {
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            _cursor = new Bitmap(10, 10);
            _graphics = Graphics.FromImage(_buffer);
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
            _cursor.Dispose();
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
                _gradientColorBlend = value;

                _graphics.SmoothingMode = SmoothingMode.None;
                DrawGradient();
                _graphics.SmoothingMode = SmoothingMode.AntiAlias;

                Invalidate();
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
                _centerFrequency = value;
                PositionCursor();
                Invalidate();
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
                _spectrumWidth = value;
                if (_spectrumWidth > 0)
                {
                    _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float) _spectrumWidth;
                }
                GenerateCursor();
                Invalidate();
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
                _frequency = value;
                PositionCursor();
                Invalidate();
                Update();
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
                _filterBandwidth = value;
                GenerateCursor();
                Invalidate();
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
                _frequencyOffset = value;
                GenerateCursor();
                Invalidate();
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
                _bandType = value;
                GenerateCursor();
                Invalidate();
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
                Draw();
                Invalidate();
            }
        }

        public void Render(double[] spectrum, int length)
        {
            if (_spectrum == null || _spectrum.Length != length)
            {
                _spectrum = new double[length];
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    _spectrum[i] = spectrum[i];
                }
            }
            const double attack = 0.9;
            const double decay = 0.4;
            for (var i = 0; i < _spectrum.Length; i++)
            {
                var ratio = _spectrum[i] < spectrum[i] ? attack : decay;
                _spectrum[i] = _spectrum[i] * (1 - ratio) + spectrum[i] * ratio;
            }
            Draw();
            Invalidate();
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

            // Yes, GDI is such an abomination in .NET ...
            using (var bmp = (Bitmap) _buffer.Clone())
            {
                var rect = new Rectangle(AxisMargin, 2, _buffer.Width - 2 * AxisMargin, _buffer.Height - 2);
                _graphics.DrawImage(bmp, rect, AxisMargin, 1, _buffer.Width - 2 * AxisMargin, _buffer.Height - 2, GraphicsUnit.Pixel);
            }

            #endregion

            #region Draw Spectrum

            if (_highDefinition)
            {
                DrawSpectrumHD();
            }
            else
            {
                DrawSpectrumLD();
            }

            #endregion
        }

        private void DrawSpectrumHD()
        {
            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }

            var x = 0f;
            var xIncrement = (ClientRectangle.Width - 2f * AxisMargin) / _spectrum.Length;
            var yIncrement = (ClientRectangle.Height - AxisMargin) / MinimumLevel;
            using (var pen = new Pen(Color.Black))
            {
                for (var i = 0; i < _spectrum.Length; i++)
                {
                    var strenght = MinimumLevel + _spectrum[i];
                    strenght = Math.Max(strenght, 0f);
                    strenght = Math.Min(strenght, MinimumLevel);

                    var newX = i * xIncrement + 1;
                    var newY = (int)(ClientRectangle.Height - AxisMargin / 2 - strenght * yIncrement) + 2;

                    var color = _buffer.GetPixel(ClientRectangle.Width - AxisMargin / 2, newY);
                    pen.Color = color;
                    _graphics.DrawLine(pen, AxisMargin + x, 1, AxisMargin + newX, 1);
                    x = newX;
                }
            }
        }

        private void DrawSpectrumLD()
        {
            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }
            var xPixelCount = ClientRectangle.Width - 2 * AxisMargin;
            var xFFTBinsPerPixels = _spectrum.Length / (float) xPixelCount;
            var yIncrement = (ClientRectangle.Height - AxisMargin) / MinimumLevel;
            for (var i = 0; i < xPixelCount; i++)
            {
                var strenght = _spectrum[(int) (i * xFFTBinsPerPixels)];

                strenght = MinimumLevel + strenght;
                strenght = Math.Max(strenght, 0f);
                strenght = Math.Min(strenght, MinimumLevel);

                var newX = i;
                var newY = (int)(ClientRectangle.Height - AxisMargin / 2 - strenght * yIncrement) + 2;

                var color = _buffer.GetPixel(ClientRectangle.Width - AxisMargin / 2, newY);
                _buffer.SetPixel(AxisMargin + newX, 1, color);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_mouseIn)
            {
                using (var bmp = (Bitmap) _buffer.Clone())
                using (var g = Graphics.FromImage(bmp))
                {
                    g.DrawImage(_buffer, 0, 0);

                    if (_spectrumWidth > 0)
                    {
                        g.DrawImage(_cursor, _lower, 0f);
                    }
                    e.Graphics.DrawImage(bmp, 0, 0);
                }
            }
            else
            {
                e.Graphics.DrawImage(_buffer, 0, 0);
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
            _cursor = new Bitmap((int) cursorWidth, ClientRectangle.Height);

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
            var oldBuffer = _buffer;
            _buffer = new Bitmap(ClientRectangle.Width, ClientRectangle.Height);
            _graphics.Dispose();
            _graphics = Graphics.FromImage(_buffer);
            using (var bkgBrush = new SolidBrush(Color.Black))
            {
                _graphics.FillRectangle(bkgBrush, ClientRectangle);
            }
            var rect = new Rectangle(AxisMargin, 1, _buffer.Width - 2 * AxisMargin, _buffer.Height);
            _graphics.DrawImage(oldBuffer, rect, AxisMargin, 1, oldBuffer.Width - 2 * AxisMargin, oldBuffer.Height, GraphicsUnit.Pixel);
            oldBuffer.Dispose();
            if (_spectrumWidth > 0)
            {
                _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float) _spectrumWidth;
            }
            DrawGradient();

            _graphics.SmoothingMode = SmoothingMode.AntiAlias;
            GenerateCursor();
            Invalidate();
        }

        private void DrawGradient()
        {
            using (var gradientBrush = new LinearGradientBrush(new Rectangle(AxisMargin / 2, AxisMargin / 2, Width - AxisMargin / 2, Height - AxisMargin / 2), Color.White, Color.Black, LinearGradientMode.Vertical))
            {
                gradientBrush.InterpolationColors = _gradientColorBlend;
                using (var pen = new Pen(gradientBrush))
                {
                    pen.Width = 10;
                    _graphics.DrawLine(pen,
                                       ClientRectangle.Width - AxisMargin/2,
                                       ClientRectangle.Height - AxisMargin/2,
                                       ClientRectangle.Width - AxisMargin/2,
                                       AxisMargin / 2);
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
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _mouseIn = true;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _mouseIn = false;
            Invalidate();
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