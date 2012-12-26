using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace SDRSharp.FrequencyEdit
{
    internal sealed class FrequencyEditSeparator : UserControl, IRenderable
    {
        private const float MaskedDigitTransparency = 0.3f;

        private Image _image;
        private bool _masked;
        private bool _renderNeeded;
        private readonly ImageAttributes _maskedAttributes = new ImageAttributes();

        public Image Image
        {
            get { return _image; }
            set { _image = value; }
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

        public FrequencyEditSeparator()
        {
            DoubleBuffered = true;
            UpdateStyles();

            var cm = new ColorMatrix();
            cm.Matrix33 = MaskedDigitTransparency;
            _maskedAttributes.SetColorMatrix(cm, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);
        }

        public void Render()
        {
            if (_renderNeeded)
            {
                Invalidate();
                _renderNeeded = false;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_image != null)
            {
                var attributes = (_masked || !Parent.Enabled) ? _maskedAttributes : null;
                e.Graphics.DrawImage(_image, new Rectangle(0, 0, Width, Height),
                                     0.0f, 0.0f, _image.Width, _image.Height,
                                     GraphicsUnit.Pixel, attributes);
            }
        }
    }
}
