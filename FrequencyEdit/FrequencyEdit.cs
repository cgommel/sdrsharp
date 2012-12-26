using System;
using System.Drawing;
using System.Windows.Forms;
using SDRSharp.FrequencyEdit.Properties;

namespace SDRSharp.FrequencyEdit
{
    internal interface IRenderable
    {
        void Render();
    }

    public sealed class FrequencyEdit : UserControl
    {
        private const int DigitCount = 10;
        private const int DigitImageSplitCount = 12;
        private const int DigitSeperatorCount = DigitCount - 1 / 3;
        
        public event EventHandler FrequencyChanged;

        private readonly FrequencyEditDigit[] _digitControls = new FrequencyEditDigit[DigitCount];
        private readonly FrequencyEditSeparator[] _separatorControls = new FrequencyEditSeparator[DigitSeperatorCount];
        private readonly ImageList _imageList = new ImageList();
        private readonly Image _digitImages;
        private readonly Timer _renderTimer = new Timer();
        private long _frequency;
        private long _newFrequency;
        private long _maximum, _minimum;        
        private int _stepSize;

        #region Public Properties

        public int StepSize
        {
            get { return _stepSize; }
            set { _stepSize = value; }
        }

        public long Frequency
        {
            get { return _frequency; }
            set
            {
                var f = Math.Min(value, _maximum);
                f = Math.Max(f, _minimum);
                if (_frequency != f)
                {
                    _frequency = f;
                    UpdateDigitsValues();
                    var evt = FrequencyChanged;
                    if (evt != null)
                    {
                        evt(this, EventArgs.Empty);
                    }
                }
            }
        }

        public long Maximum
        {
            get { return _maximum; }
            set
            {
                if (_maximum != value)
                {
                    _maximum = value;
                    if (_frequency > _maximum)
                    {
                        Frequency = _maximum;
                    }
                }
            }
        }

        public long Minimum
        {
            get { return _minimum; }
            set
            {
                if (_minimum != value)
                {
                    _minimum = value;
                    if (_frequency < _minimum)
                    {
                        Frequency = _minimum;
                    }
                }
            }
        }

        #endregion

        #region Initialization

        public FrequencyEdit()
        {
            DoubleBuffered = true;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            _digitImages = Resources.Numbers;
            _renderTimer.Interval = 30;
            _renderTimer.Tick += renderTimer_Tick;
            _renderTimer.Enabled = true;
            ConfigureComponent();
        }

        private void renderTimer_Tick(object sender, EventArgs e)
        {
            for (var i = 0; i < Controls.Count; i++)
            {
                if (Controls[i] is IRenderable)
                {
                    ((IRenderable) Controls[i]).Render();
                }
            }
        }

        private void ConfigureComponent()
        {
            BackColor = Color.Transparent;

            if (_digitImages != null)
            {
                for(var i = 0 ; i < DigitCount ; i++)
                {
                    if (_digitControls[i] != null && Controls.Contains(_digitControls[i]))
                    {
                        Controls.Remove(_digitControls[i]);
                        _digitControls[i] = null;
                    }
                }
                for(var i = 0 ; i < DigitSeperatorCount; i++)
                {
                    if(_separatorControls[i] != null && Controls.Contains(_separatorControls[i]))
                    {
                        Controls.Remove(_separatorControls[i]);
                        _separatorControls[i] = null;
                    }
                }

                SplitDigitImages();
            }

            if (_imageList.Images.Count == 0)
            {
                return;
            }

            var xPos = 0;
            var yPos = 0;

            var digitWidth = _imageList.ImageSize.Width;
            var digitHeight = _imageList.ImageSize.Height;

            for (var i = DigitCount-1; i >= 0; i--)
            {
                if ((i + 1) % 3 == 0 && i != DigitCount - 1)
                {
                    var separator = new FrequencyEditSeparator();
                    var seperatorWidth = digitWidth / 2;
                    var seperatorIndex = i / 3;

                    separator.Image = _imageList.Images[11];
                    separator.Width = seperatorWidth;
                    separator.Height = digitHeight;
                    separator.Location = new Point(xPos, yPos);
                    Controls.Add(separator);
                                        
                    _separatorControls[seperatorIndex] = separator;

                    xPos += seperatorWidth + 2;
                }

                var frequencyEditDigit = new FrequencyEditDigit(i);

                frequencyEditDigit.Location = new Point(xPos, yPos);
                frequencyEditDigit.OnDigitClick += DigitClickHandler;
                                                
                frequencyEditDigit.Width = digitWidth;
                frequencyEditDigit.Height = digitHeight;
                frequencyEditDigit.ImageList = _imageList;

                Controls.Add(frequencyEditDigit);
                _digitControls[i] = frequencyEditDigit;

                xPos += digitWidth + 2;
            }

            long weight = 1L;
            for (var i = 0; i < DigitCount; i++)
            {
                _digitControls[i].Weight = weight;                
                weight *= 10;                
            }
            Height = digitHeight;
        }

        private void SplitDigitImages()
        {
            var digitHeight = _digitImages.Height;
            var digitWidth = (int) Math.Round(_digitImages.Width / 11.5f);

            _imageList.Images.Clear();
            _imageList.ImageSize = new Size(digitWidth, digitHeight);

            Bitmap newImage;
            var xPos = 0;
            for (var i = 0; i < DigitImageSplitCount - 1; i++)
            {
                newImage = new Bitmap(digitWidth, digitHeight);
                using (var g = Graphics.FromImage(newImage))
                {
                    g.DrawImage(_digitImages, new Rectangle(0, 0, digitWidth, digitHeight), new Rectangle(xPos, 0, digitWidth, digitHeight), GraphicsUnit.Pixel);
                }
                xPos += digitWidth;

                _imageList.Images.Add(newImage);
            }

            newImage = new Bitmap(digitWidth, digitHeight);
            using (var g = Graphics.FromImage(newImage))
            {
                g.DrawImage(_digitImages, new Rectangle(0, 0, digitWidth, digitHeight), new Rectangle(xPos, 0, digitWidth / 2, digitHeight), GraphicsUnit.Pixel);
            }
            _imageList.Images.Add(newImage);
        }

        #endregion

        private void DigitClickHandler(object sender, FrequencyEditDigitClickEventArgs args)
        {
            var digit = (FrequencyEditDigit) sender;

            if (digit != null)
            {
                _newFrequency = _frequency;
                if (args.Button == MouseButtons.Right)
                {
                    ZeroDigits(digit.DigitIndex);
                }
                else
                {
                    if (args.IsUpperHalf && _frequency >= 0)
                    {
                        IncrementDigit(digit.DigitIndex);
                    }
                    else
                    {
                        DecrementDigit(digit.DigitIndex);
                    }
                }

                if ((_newFrequency < _minimum) || (_newFrequency > _maximum))
                {
                    UpdateDigitsValues();
                    return;
                }

                if (_newFrequency != _frequency)
                {
                    _frequency = _newFrequency;
                    UpdateDigitMask();
                    var evt = FrequencyChanged;
                    if (evt != null)
                    {
                        evt(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void IncrementDigit(int index)
        {
            var digit = _digitControls[index];
            if (digit != null)
            {
                var oldDigit = digit.DisplayedDigit;
                var newDigit = digit.DisplayedDigit == 9 ? 0 : digit.DisplayedDigit + 1;
            
                var newFrequency = (_newFrequency - (oldDigit * digit.Weight)) + (newDigit * digit.Weight);

                digit.DisplayedDigit = newDigit;
                
                _newFrequency = newFrequency;

                if (oldDigit == 9 && index < DigitCount - 1)
                {
                    IncrementDigit(index + 1);
                }
            }
        }

        private void DecrementDigit(int index)
        {
            var digit = _digitControls[index];
            if (digit != null)
            {
                var oldDigit = digit.DisplayedDigit;
                var newDigit = digit.DisplayedDigit == 0 ? 9 : digit.DisplayedDigit - 1;
                var newFrequency = (_newFrequency - (oldDigit * digit.Weight)) + (newDigit * digit.Weight);

                digit.DisplayedDigit = newDigit;
                
                _newFrequency = newFrequency;

                if (oldDigit == 0 && index < DigitCount - 1)
                {
                    DecrementDigit(index + 1);
                }
            }
        }

        private void ZeroDigits(int index)
        {
            for (var i = 0; i <= index; i++)
            {
                _digitControls[i].DisplayedDigit = 0;
            }
            var res = (long) Math.Pow(10, index + 1);
            _newFrequency = _newFrequency / res * res;
        }
        
        private void UpdateDigitsValues()
        {
            if (_digitControls[0] == null)
                return;

            var currentFrequency = _frequency;
            
            for (var i = DigitCount - 1; i >= 0; i--)
            {
                var digit = currentFrequency / _digitControls[i].Weight;                
                _digitControls[i].DisplayedDigit = (int) digit;                
                currentFrequency -= (_digitControls[i].DisplayedDigit * _digitControls[i].Weight);                
            }

            UpdateDigitMask();
        }

        private void UpdateDigitMask()
        {
            var frequency = _frequency;
            
            if (frequency >= 0)
            {
                for (var i = 1; i < DigitCount; i++)
                {                    
                    if ((i + 1) % 3 == 0 && i != DigitCount - 1)
                    {                        
                        var separatorIndex = i / 3;
                        if (_separatorControls[separatorIndex] != null)
                        {
                            _separatorControls[separatorIndex].Masked = (_digitControls[i + 1].Weight > frequency);
                        }
                    }                    
                    if (_digitControls[i] != null)
                    {
                        _digitControls[i].Masked = (_digitControls[i].Weight > frequency);                        
                    }                    
                }
            }
        }
    }
}
