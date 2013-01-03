using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SDRSharp.FrequencyEdit
{
    public delegate void OnDigitClickDelegate(object sender, FrequencyEditDigitClickEventArgs args);

    internal sealed class FrequencyEditDigit : UserControl, IRenderable
    {
        private const float MaskedDigitTransparency = 0.3f;

        public event OnDigitClickDelegate OnDigitClick;

        private bool _masked;
        private int _displayedDigit;
        private long _weight;
        private bool _renderNeeded;
        private bool _cursorInside;
        private bool _highlight;
        private int _lastMouseY;
        private bool _lastIsUpperHalf;
        private bool _isUpperHalf;
        private int _tickCount;
        private ImageList _imageList;
        private readonly int _digitIndex;
        private readonly Timer _tickTimer = new Timer();
        private readonly ImageAttributes _maskedAttributes = new ImageAttributes();
                               
        public ImageList ImageList
        {
            get { return _imageList; }
            set { _imageList = value; }
        }

        public bool Highlight
        {
            get { return _highlight; }
            set
            {
                _highlight = value;
                _renderNeeded = true;
            }
        }

        public bool CursorInside
        {
            get { return _cursorInside; }
        }

        public int DisplayedDigit
        {
            get { return _displayedDigit; }
            set
            {
                if (value >= 0 && value <= 9)
                {
                    if (_displayedDigit != value)
                    {
                        _displayedDigit = value;
                        _renderNeeded = true;
                    }
                }
            }
        }

        public int DigitIndex
        {
            get { return _digitIndex; }
        }

        public bool Masked
        {
            get { return _masked; }
            set
            {
                if (_masked != value)
                {
                    _masked = value;
                    _renderNeeded = true;
                }
            }
        }

        public long Weight
        {
            get { return _weight; }
            set { _weight = value; }
        }
        
        public FrequencyEditDigit(int digitIndex)
        {
            DoubleBuffered = true;
            BackColor = Color.Transparent;
            _tickTimer.Tick += timer_Tick;
            UpdateStyles();
            _digitIndex = digitIndex;

            var cm = new ColorMatrix();
            cm.Matrix33 = MaskedDigitTransparency;
            _maskedAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_imageList != null)
            {
                if (_displayedDigit < _imageList.Images.Count)
                {
                    var image = _imageList.Images[_displayedDigit];                                                
                    var attributes = ((_masked && !_cursorInside) || !Parent.Enabled) ? _maskedAttributes: null;
                                        
                    e.Graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height), 
                                        0.0f, 0.0f, image.Width, image.Height, 
                                        GraphicsUnit.Pixel, attributes);
                }
            }

            if(_cursorInside && !((FrequencyEdit)Parent).EntryModeActive)
            {
                var isUpperHalf = (_lastMouseY <= ClientRectangle.Height / 2);

                var transparentColor = Color.FromArgb(100, isUpperHalf ? Color.Red : Color.Blue);
                using (var transparentBrush = new SolidBrush(transparentColor))
                {
                    if (isUpperHalf)
                    {
                        e.Graphics.FillRectangle(transparentBrush, new Rectangle(0, 0, ClientRectangle.Width, ClientRectangle.Height / 2));
                    }
                    else
                    {
                        e.Graphics.FillRectangle(transparentBrush, new Rectangle(0, ClientRectangle.Height / 2, ClientRectangle.Width, ClientRectangle.Height));
                    }
                }
            }

            if (_highlight)
            {
                var transparentColor = new SolidBrush(Color.FromArgb(25, Color.Red));
                e.Graphics.FillRectangle(transparentColor,new Rectangle(0,0,ClientRectangle.Width,ClientRectangle.Height));
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            
            _isUpperHalf = (e.Y <= ClientRectangle.Height / 2);
            //Cursor = _isUpperHalf ? Cursors.PanNorth : Cursors.PanSouth;

            _lastMouseY = e.Y;

            if (_isUpperHalf != _lastIsUpperHalf)
            {
                _renderNeeded = true;
                _tickCount = 0;
            }

            _lastIsUpperHalf = _isUpperHalf;
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _cursorInside = true;
            _renderNeeded = true;
            Focus();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _cursorInside = false;
            _renderNeeded = true;            
        }
       
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            _isUpperHalf = (e.Y <= ClientRectangle.Height / 2);
            var evt = OnDigitClick;
            if (evt != null)
            {
                OnDigitClick(this, new FrequencyEditDigitClickEventArgs(_isUpperHalf, e.Button));
            }
            _tickCount = 1;
            _tickTimer.Interval = 300;
            _tickTimer.Enabled = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            _tickTimer.Enabled = false;
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            var evt = OnDigitClick;
            if (evt != null)
            {
                OnDigitClick(this, new FrequencyEditDigitClickEventArgs((e.Delta > 0), e.Button));
            }
        }

        public void Render()
        {
            if (_renderNeeded)
            {
                Invalidate();
                _renderNeeded = false;
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            var evt = OnDigitClick;
            if (evt != null)
            {
                OnDigitClick(this, new FrequencyEditDigitClickEventArgs(_isUpperHalf, MouseButtons.Left));
            }
            _tickCount++;
            switch (_tickCount)
            {
                case 10:
                    _tickTimer.Interval = 200;
                    break;

                case 20:
                    _tickTimer.Interval = 100;
                    break;

                case 50:
                    _tickTimer.Interval = 50;
                    break;

                case 100:
                    _tickTimer.Interval = 20;
                    break;
            }
        }        
    }

    public class FrequencyEditDigitClickEventArgs
    {
        public bool IsUpperHalf;
        public MouseButtons Button;

        public FrequencyEditDigitClickEventArgs(bool isUpperHalf, MouseButtons button)
        {
            IsUpperHalf = isUpperHalf;
            Button = button;
        }
    }
}
