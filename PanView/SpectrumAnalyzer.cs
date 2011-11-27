using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SDRSharp.PanView
{
    public class SpectrumAnalyzer : UserControl
    {
        private const int AxisMargin = 30;
        private const int CarrierPenWidth = 1;

        private bool _performNeeded;
        private double[] _spectrum;
        private Bitmap _buffer;
        private Graphics _graphics;
        private int _spectrumWidth;
        private int _centerFrequency;
        private bool _highDefinition;

        private Bitmap _cursor;
        private BandType _bandType;
        private int _filterBandwidth;
        private int _offset;
        private float _xIncrement;
        private int _frequency;
        private float _lower;
        private float _upper;
        private int _delta;
        private bool _changingFrequency;
        private bool _changingCenterFrequency;

        public SpectrumAnalyzer()
        {
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
                if (_highDefinition != value)
                {
                    _highDefinition = value;
                    _performNeeded = true;
                }
            }
        }

        public void Perform()
        {
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

        private void Draw()
        {
            #region Draw only if needed

            if (ClientRectangle.Width <= AxisMargin || ClientRectangle.Height <= AxisMargin)
            {
                return;
            }

            #endregion

            #region Draw grid

            using (var bkgBrush = new SolidBrush(Color.Black))
            using (var fontBrush = new SolidBrush(Color.Silver))
            using (var squarePen = new Pen(Color.FromArgb(80, 80, 80)))
            using (var axisPen = new Pen(Color.DarkGray))
            using (var font = new Font("Arial", 8f))
            {
                // Background
                _graphics.FillRectangle(bkgBrush, ClientRectangle);

                // Axis
                _graphics.DrawLine(axisPen, AxisMargin, AxisMargin, AxisMargin, ClientRectangle.Height - AxisMargin);
                _graphics.DrawLine(axisPen, AxisMargin, ClientRectangle.Height - AxisMargin, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin);

                // Grid
                squarePen.DashStyle = DashStyle.Dash;
                var xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / 10.0f;
                for (var i = 1; i <= 10; i++)
                {
                    _graphics.DrawLine(squarePen, AxisMargin + xIncrement * i, AxisMargin, AxisMargin + xIncrement * i, ClientRectangle.Height - AxisMargin);
                }
                var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / 12.0f;
                for (var i = 1; i <= 12; i++)
                {
                    _graphics.DrawLine(squarePen, AxisMargin, ClientRectangle.Height - AxisMargin - i * yIncrement, ClientRectangle.Width - AxisMargin, ClientRectangle.Height - AxisMargin - i * yIncrement);
                }
                
                // Decibel line
                for (var i = 0; i <= 12; i++)
                {
                    var db = (-(12 - i) * 10).ToString();
                    var sizeF = _graphics.MeasureString(db, font);
                    var width = sizeF.Width;
                    var height = sizeF.Height;
                    _graphics.DrawString(db, font, fontBrush, AxisMargin - width - 5, ClientRectangle.Height - AxisMargin - i * yIncrement - height / 2f);
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
                        else if (frequency % 1000000 == 0)
                        {
                            f = (frequency / 1000000) + "MHz";
                        }
                        else if (frequency % 1000 == 0)
                        {
                            f = (frequency / 1000) + "kHz";
                        }
                        else if (frequency % 100 == 0)
                        {
                            f = (frequency / 1000.0) + "kHz";
                        }
                        else
                        {
                            f = frequency.ToString();
                        }
                        var sizeF = _graphics.MeasureString(f, font);
                        var width = sizeF.Width;

                        _graphics.DrawString(f, font, fontBrush, AxisMargin + xIncrement * i - width / 2f, ClientRectangle.Height - AxisMargin + 5f);
                    }
                }

            }

            #endregion

            if (_spectrum == null || _spectrum.Length == 0)
            {
                return;
            }

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

            #region Draw Cursor

            _graphics.DrawImage(_cursor, _lower, 0f);

            #endregion
        }

        private void DrawSpectrumHD()
        {
            var x = 0f;
            var y = 0f;
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
                    if (y == 0)
                    {
                        y = newY;
                    }
                    _graphics.DrawLine(spectrumPen,
                                       AxisMargin + x,
                                       y,
                                       AxisMargin + newX,
                                       newY);
                    x = newX;
                    y = newY;
                }
            }
        }

        private void DrawSpectrumLD()
        {
            var x = 0;
            var y = 0;
            var xPixelCount = ClientRectangle.Width - 2 * AxisMargin;
            var xPixelsPerFFTBins = _spectrum.Length / (float)xPixelCount;
            var yIncrement = (ClientRectangle.Height - 2 * AxisMargin) / 120f;

            _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float) _spectrumWidth;

            using (var spectrumPen = new Pen(Color.LimeGreen))
            {
                for (var i = 0; i < xPixelCount; i++)
                {
                    var strenght = (float) _spectrum[(int)(i * xPixelsPerFFTBins)] + 120f;
                    strenght = Math.Max(strenght, 0);
                    strenght = Math.Min(strenght, 120f);

                    var newX = i;
                    var newY = ClientRectangle.Height - AxisMargin - (int)(strenght * yIncrement);
                    if (y == 0)
                    {
                        y = newY;
                    }
                    _graphics.DrawLine(spectrumPen,
                                       AxisMargin + x,
                                       y,
                                       AxisMargin + newX,
                                       newY);
                    x = newX;
                    y = newY;
                }
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.DrawImage(_buffer, 0, 0);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (Width > 0 && Height > 0)
            {
                _buffer.Dispose();
                _graphics.Dispose();
                _buffer = new Bitmap(Width, Height);
                _graphics = Graphics.FromImage(_buffer);
                if (_spectrumWidth > 0)
                {
                    _xIncrement = (ClientRectangle.Width - 2 * AxisMargin) / (float)_spectrumWidth;
                }
                GenerateCursor();
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
            else
            {
                _delta = (int)(AxisMargin - _spectrumWidth * _xIncrement / 2 + e.X);
                //_changingCenterFrequency = true;
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
                var f = (e.X + _delta) * _spectrumWidth / (ClientRectangle.Width - 2 * AxisMargin) + _centerFrequency - _spectrumWidth / 2;
                UpdateCenterFrequency(f);
            }
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            UpdateFrequency(_frequency + e.Delta / 10);
        }
    }
}