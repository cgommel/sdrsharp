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

    public enum EntryMode
    {
        None,
        Direct,
        Arrow
    };

    public sealed class FrequencyEdit : UserControl
    {
        private const int DigitCount = 10;
        private const int DigitImageSplitCount = 12;
        private const int DigitSeperatorCount = DigitCount - 1 / 3;
        
        public event EventHandler FrequencyChanged;
        public event EventHandler<FrequencyChangingEventArgs> FrequencyChanging;

        private readonly FrequencyEditDigit[] _digitControls = new FrequencyEditDigit[DigitCount];
        private readonly FrequencyEditSeparator[] _separatorControls = new FrequencyEditSeparator[DigitSeperatorCount];
        private readonly ImageList _imageList = new ImageList();
        private readonly Image _digitImages;
        private readonly Timer _renderTimer = new Timer();
        private readonly FrequencyChangingEventArgs _frequencyChangingEventArgs = new FrequencyChangingEventArgs();
        private long _frequency;
        private long _newFrequency;      
        private int _stepSize;        
        private int _editModePosition;
        private bool _changingEntryMode;
        private EntryMode _currentEntryMode;

        #region Public Properties

        public int StepSize
        {
            get { return _stepSize; }
            set { _stepSize = value; }
        }

        public bool EntryModeActive
        {
            get { return _currentEntryMode != EntryMode.None; }
        }

        public long Frequency
        {
            get { return _frequency; }
            set
            {
                if (value != _frequency)
                {
                    _frequencyChangingEventArgs.Accept = true;
                    _frequencyChangingEventArgs.Frequency = value;
                    if (FrequencyChanging != null)
                    {
                        FrequencyChanging(this, _frequencyChangingEventArgs);
                    }
                    if (_frequencyChangingEventArgs.Accept)
                    {
                        _frequency = _frequencyChangingEventArgs.Frequency;
                        UpdateDigitsValues();
                        if (FrequencyChanged != null)
                        {
                            FrequencyChanged(this, EventArgs.Empty);
                        }
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
                frequencyEditDigit.MouseLeave += DigitMouseLeave;
                                                
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

            UpdateDigitMask();
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
            
            if (_currentEntryMode != EntryMode.None)
            {
                LeaveEntryMode();
                return;
            }

            var digit = (FrequencyEditDigit)sender;
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
                        IncrementDigit(digit.DigitIndex,true);
                    }
                    else
                    {
                        DecrementDigit(digit.DigitIndex,true);
                    }
                }

                if (_newFrequency != _frequency)
                {
                    _frequencyChangingEventArgs.Accept = true;
                    _frequencyChangingEventArgs.Frequency = _newFrequency;
                    if (FrequencyChanging != null)
                    {
                        FrequencyChanging(this, _frequencyChangingEventArgs);
                    }
                    if (_frequencyChangingEventArgs.Accept)
                    {
                        _frequency = _frequencyChangingEventArgs.Frequency;
                        UpdateDigitsValues();
                        if (FrequencyChanged != null)
                        {
                            FrequencyChanged(this, EventArgs.Empty);
                        }
                    }
                    else
                    {
                        UpdateDigitsValues();
                    }
                }
            }
        }

        private void IncrementDigit(int index, bool updateDigit)
        {
            var digit = _digitControls[index];
            if (digit != null)
            {
                var oldDigit = digit.DisplayedDigit;
                var newDigit = digit.DisplayedDigit == 9 ? 0 : digit.DisplayedDigit + 1;
            
                var newFrequency = (_newFrequency - (oldDigit * digit.Weight)) + (newDigit * digit.Weight);

                if (updateDigit)
                {
                    digit.DisplayedDigit = newDigit;
                }
                _newFrequency = newFrequency;

                if (oldDigit == 9 && index < DigitCount - 1)
                {
                    IncrementDigit(index + 1, updateDigit);
                }
            }
        }

        private void DecrementDigit(int index, bool updateDigit)
        {
            var digit = _digitControls[index];
            if (digit != null)
            {
                var oldDigit = digit.DisplayedDigit;
                var newDigit = digit.DisplayedDigit == 0 ? 9 : digit.DisplayedDigit - 1;
                var newFrequency = (_newFrequency - (oldDigit * digit.Weight)) + (newDigit * digit.Weight);

                if (updateDigit)
                {
                    digit.DisplayedDigit = newDigit;
                }
                _newFrequency = newFrequency;

                if (oldDigit == 0 && index < DigitCount - 1)
                {                                        
                    var needDecrement = (_newFrequency > Math.Pow(10, index + 1));                                       
                    if (needDecrement)
                    {
                        DecrementDigit(index + 1, updateDigit);
                    }
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
                _digitControls[i].DisplayedDigit = (int)digit;
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

        private void DigitMouseLeave(object sender, EventArgs e)
        {
            if (!ClientRectangle.Contains(PointToClient(MousePosition)) && _currentEntryMode != EntryMode.None)
            {
                AbortEntryMode();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (!ClientRectangle.Contains(PointToClient(MousePosition)) && _currentEntryMode != EntryMode.None)
            {
                AbortEntryMode();
            }
        }

        private long GetFrequencyValue()
        {
            var newFrequency = 0L;
            for (var i = 0; i < _digitControls.Length; i++)
            {
                newFrequency += _digitControls[i].Weight * _digitControls[i].DisplayedDigit;
            }
            return newFrequency;
        }

        private void SetFrequencyValue(long newFrequency)
        {
            if (newFrequency != _frequency)
            {
                _frequencyChangingEventArgs.Accept = true;
                _frequencyChangingEventArgs.Frequency = newFrequency;
                if (FrequencyChanging != null)
                {
                    FrequencyChanging(this, _frequencyChangingEventArgs);
                }
                if (_frequencyChangingEventArgs.Accept)
                {
                    _frequency = _frequencyChangingEventArgs.Frequency;
                    UpdateDigitsValues();
                    if (FrequencyChanged != null)
                    {
                        FrequencyChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        #region Keyboard Handling

        #region Direct Entry Mode

        private void EnterDirectMode()
        {
            if (_changingEntryMode)
            {
                return;
            }
            _changingEntryMode = true;
            for (var i = 0; i < _digitControls.Length; i++)
            {
                if (_digitControls[i] != null)
                {
                    _digitControls[i].Masked = false;
                    if (_digitControls[i].CursorInside)
                    {
                        _editModePosition = i;
                        _digitControls[i].Highlight = true;
                    }
                }
            }
            
            ZeroDigits(_digitControls.Length - 1);            
            _currentEntryMode = EntryMode.Direct;
            _changingEntryMode = false;
        }
        
        
        
        private void DirectModeHandler(KeyEventArgs args)
        {
            switch (args.KeyCode)
            {
                case Keys.D0:
                case Keys.D1:
                case Keys.D2:
                case Keys.D3:
                case Keys.D4:
                case Keys.D5:
                case Keys.D6:
                case Keys.D7:
                case Keys.D8:
                case Keys.D9:
                case Keys.NumPad0:
                case Keys.NumPad1:
                case Keys.NumPad2:
                case Keys.NumPad3:
                case Keys.NumPad4:
                case Keys.NumPad5:
                case Keys.NumPad6:
                case Keys.NumPad7:
                case Keys.NumPad8:
                case Keys.NumPad9:
                    var newValue = (args.KeyCode >= Keys.D0 && args.KeyCode <= Keys.D9) ? (args.KeyCode - Keys.D0) : (args.KeyCode - Keys.NumPad0);
                    _digitControls[_editModePosition].DisplayedDigit = newValue;
                    if (_editModePosition > 0)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition--;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    else
                    {
                        LeaveEntryMode();
                    }
                    break;
                case Keys.Left:
                    if (_editModePosition < _digitControls.Length - 1)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition++;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    break;
                case Keys.Right:
                    if (_editModePosition > 0)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition--;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    break;
                case Keys.Back:
                    _digitControls[_editModePosition].DisplayedDigit = 0;
                    if (_editModePosition < _digitControls.Length - 1)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition++;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    break;
                case Keys.Tab:                
                case Keys.Decimal:
                case Keys.OemPeriod:
                    _digitControls[_editModePosition].Highlight = false;
                    _editModePosition -= (_editModePosition % 3) + 1;
                    if (_editModePosition < 2)
                    {
                        if (args.KeyCode == Keys.Tab)
                        {
                            _editModePosition = _digitControls.Length - 1;
                        }
                        else
                        {
                            _editModePosition = 0;
                            LeaveEntryMode();
                            break;
                        }
                    }
                    _digitControls[_editModePosition].Highlight = true;
                    break;
                case Keys.Escape:
                    AbortEntryMode();
                    break;
                case Keys.Enter:
                    LeaveEntryMode();
                    break;
            }            
        }

        #endregion

        #region Arrow Key Mode

        private void EnterArrowMode()
        {
            if (_changingEntryMode)
            {
                return;
            }
            _changingEntryMode = true;
            for (var i = 0; i < _digitControls.Length; i++)
            {
                if (_digitControls[i] != null)
                {
                    _digitControls[i].Masked = false;
                    if (_digitControls[i].CursorInside)
                    {
                        _editModePosition = i;
                        _digitControls[i].Highlight = true;            
                    }
                }
            }            
            _currentEntryMode = EntryMode.Arrow;
            _changingEntryMode = false;
        }

        private void ArrowModeHandler(KeyEventArgs args)
        {            
            switch (args.KeyCode)
            {
                case Keys.Up:
                    _newFrequency = _frequency;
                    IncrementDigit(_editModePosition,false);
                    SetFrequencyValue(_newFrequency);
                    break;
                case Keys.Down:
                    _newFrequency = _frequency;
                    DecrementDigit(_editModePosition,false);
                    SetFrequencyValue(_newFrequency);
                    break;
                case Keys.Left:
                    if (_editModePosition < _digitControls.Length - 1)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition++;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    break;
                case Keys.Right:
                    if (_editModePosition > 0)
                    {
                        _digitControls[_editModePosition].Highlight = false;
                        _editModePosition--;
                        _digitControls[_editModePosition].Highlight = true;
                    }
                    break;
                case Keys.Tab:
                    _digitControls[_editModePosition].Highlight = false;
                    _editModePosition -= (_editModePosition % 3) + 1;
                    if (_editModePosition < 2)
                    {
                        _editModePosition = _digitControls.Length - 1;
                    }
                    _digitControls[_editModePosition].Highlight = true;
                    break;
                case Keys.Enter:
                case Keys.Escape:
                    AbortEntryMode();
                    break;
            }
        }

        #endregion

        private void AbortEntryMode()
        {            
            if (_changingEntryMode)
            {
                return;
            }
            _changingEntryMode = true;
            _digitControls[_editModePosition].Highlight = false;
            UpdateDigitsValues();
            _currentEntryMode = EntryMode.None;
            _changingEntryMode = false;
        }

        private void LeaveEntryMode()
        {            
            if (_changingEntryMode)
            {
                return;
            }
            _changingEntryMode = true;
            _digitControls[_editModePosition].Highlight = false;
            if (_currentEntryMode == EntryMode.Direct)
            {
                var newFrequency = GetFrequencyValue();
                SetFrequencyValue(newFrequency);
            }
            _currentEntryMode = EntryMode.None;
            _changingEntryMode = false;
        }

        private bool DigitKeyHandler(KeyEventArgs args)
        {                     
            if (!ClientRectangle.Contains(PointToClient(MousePosition)) || _changingEntryMode)
            {
                return false;
            }
            
            if (_currentEntryMode != EntryMode.None)
            {
                switch (_currentEntryMode)
                {
                    case EntryMode.Direct:
                        DirectModeHandler(args);
                        break;
                    case EntryMode.Arrow:
                        ArrowModeHandler(args);
                        break;
                }
                return true;
            }

            if ((args.KeyCode >= Keys.D0 && args.KeyCode <= Keys.D9) ||
                (args.KeyCode >= Keys.NumPad0 && args.KeyCode <= Keys.NumPad9))
            {

                EnterDirectMode();
                DirectModeHandler(args);
                return true;
            }

            if (args.KeyCode == Keys.Up || args.KeyCode == Keys.Down ||
                args.KeyCode == Keys.Left || args.KeyCode == Keys.Right)
            {
                EnterArrowMode();
                ArrowModeHandler(args);
                return true;
            }

            if (args.Modifiers == Keys.Control)
            {
                if (args.KeyCode == Keys.C)
                {
                    var frequency = string.Format("{0}", GetFrequencyValue());
                    Clipboard.SetText(frequency, TextDataFormat.Text);
                    return true;
                }
                if (args.KeyCode == Keys.V)
                {
                    var newFrequency = 0L;
                    var result = long.TryParse(Clipboard.GetText(), out newFrequency);
                    if (result)
                    {
                        SetFrequencyValue(newFrequency);           
                    }
                    return true;
                }
            }
            
            return false;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {        
            const int WM_KEYDOWN = 0x100;
            const int WM_SYSKEYDOWN = 0x104;

            if ((msg.Msg == WM_KEYDOWN) || (msg.Msg == WM_SYSKEYDOWN))
            {
                return DigitKeyHandler(new KeyEventArgs(keyData));
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        
        #endregion
    }

    public class FrequencyChangingEventArgs : EventArgs
    {
        public long Frequency { get; set; }
        public bool Accept { get; set; }
    }
}
