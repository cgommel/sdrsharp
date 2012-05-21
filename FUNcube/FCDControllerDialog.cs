using System;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.FUNcube
{
    public partial class FCDControllerDialog : Form
    {
        private bool _updating;
        private readonly FunCubeIO _owner;

        public FCDControllerDialog(FunCubeIO owner)
        {
            _owner = owner;
            InitializeComponent();
        }

        private void FCDControllerDialog_Load(object sender, EventArgs e)
        {
            ReadDeviceValues();
        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            SetDefaultValue();
        }

        private void SetDefaultValue()
        {
            BandComboBox.SelectedIndex = (int) TunerBand.TBE_VHF2;
            LNAGainComboBox.SelectedIndex = (int) TunerLNAGain.P20_0DB - 2;
            RFFilterComboBox.SelectedIndex = (int) TunerRFFilter.TRFE_LPF268MHZ;
            mixerGainComboBox.SelectedIndex = (int) TunerMixerGain.TMGE_P12_0DB;
            mixerFilterComboBox.SelectedIndex = (int) TunerMixerFilter.TMFE_1_9MHZ - 15;
            IFGain1ComboBox.SelectedIndex = (int) TunerIFGain1.TIG1E_P6_0DB;
            IFRCFilterComboBox.SelectedIndex = (int) TunerIFRCFilter.TIRFE_1_0MHZ - 15;
            IFGain2ComboBox.SelectedIndex = (int) TunerIFGain2.TIG2E_P0_0DB;
            IFGain3ComboBox.SelectedIndex = (int) TunerIFGain3.TIG3E_P0_0DB;
            IFGain4ComboBox.SelectedIndex = (int) TunerIFGain4.TIG4E_P0_0DB;
            IFFilterComboBox.SelectedIndex = (int) TunerIFFilter.TIFE_2_15MHZ - 31;
            IFGain5ComboBox.SelectedIndex = (int) TunerIFGain5.TIG5E_P3_0DB;
            IFGain6ComboBox.SelectedIndex = (int) TunerIFGain6.TIG6E_P3_0DB;
            LNAEnhanceComboBox.SelectedIndex = (int) TunerLNAEnhance.TLEE_OFF;
            BiasCurrentComboBox.SelectedIndex = (int) TunerBiasCurrent.TBCE_VUBAND;
            IFGainModeComboBox.SelectedIndex = (int) TunerIFGainMode.TIGME_LINEARITY;
        }

        private void LNAGainComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    if (LNAGainComboBox.SelectedIndex > 1)
                    {
                        _owner.LNAGain = (TunerLNAGain) (LNAGainComboBox.SelectedIndex + 2);
                    }
                    else
                    {
                        _owner.LNAGain = (TunerLNAGain) LNAGainComboBox.SelectedIndex;
                    }

                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }

        }

        private void RFFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.RFFilter = RFFilterComboBox.SelectedIndex == 0 ? TunerRFFilter.TRFE_LPF268MHZ : TunerRFFilter.TRFE_LPF299MHZ;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }

        }

        private void mixerGainComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.MixerGain = (TunerMixerGain) mixerGainComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }

        }

        private void mixerFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    if (mixerFilterComboBox.SelectedIndex < 8)
                    {
                        _owner.MixerFilter = (TunerMixerFilter) (15 - mixerFilterComboBox.SelectedIndex);
                    }
                    else
                    {
                        _owner.MixerFilter = (TunerMixerFilter) (mixerFilterComboBox.SelectedIndex - 8);
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain1ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain1 = (TunerIFGain1) IFGain1ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFRCFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFRCFilter = (TunerIFRCFilter) (15 - IFRCFilterComboBox.SelectedIndex);
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain2ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain2 = (TunerIFGain2) IFGain2ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain3ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain3 = (TunerIFGain3) IFGain3ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain4ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain4 = (TunerIFGain4) IFGain4ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain5ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain5 = (TunerIFGain5) IFGain5ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFGain6ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain6 = (TunerIFGain6) IFGain6ComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void IFFilterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFFilter = (TunerIFFilter) (31 - IFFilterComboBox.SelectedIndex);
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void LNAEnhanceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    if (LNAEnhanceComboBox.SelectedIndex > 0)
                    {
                        _owner.LNAEnhance = (TunerLNAEnhance) (2 * LNAEnhanceComboBox.SelectedIndex - 1);
                    }
                    else
                    {
                        _owner.LNAEnhance = TunerLNAEnhance.TLEE_OFF;
                    }
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void BiasCurrentComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.BiasCurrent = (TunerBiasCurrent) BiasCurrentComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }

        }

        private void IFGainModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGainMode = (TunerIFGainMode) IFGainModeComboBox.SelectedIndex;
                }
                catch (Exception)
                {

                }
                finally
                {
                    _updating = false;
                }
            }

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            ReadDeviceValues();
        }

        private void ReadDeviceValues()
        {
            _updating = true;
            try
            {
                if (!LNAGainComboBox.DroppedDown)
                {
                    if ((int)_owner.LNAGain >= 4)
                    {
                        LNAGainComboBox.SelectedIndex = (int) _owner.LNAGain - 2;
                    }
                    else
                    {
                        LNAGainComboBox.SelectedIndex = (int) _owner.LNAGain;
                    }

                }
                if (!RFFilterComboBox.DroppedDown)
                {
                    RFFilterComboBox.SelectedIndex = _owner.RFFilter == TunerRFFilter.TRFE_LPF268MHZ ? 0 : 1;
                }
                if (!mixerGainComboBox.DroppedDown)
                {
                    mixerGainComboBox.SelectedIndex = (int) _owner.MixerGain;
                }
                if (!mixerFilterComboBox.DroppedDown)
                {
                    if (mixerFilterComboBox.SelectedIndex < 8)
                    {
                        mixerFilterComboBox.SelectedIndex = 15 - (int) _owner.MixerFilter;
                    }
                    else
                    {
                        mixerFilterComboBox.SelectedIndex = 8 - (int) _owner.MixerFilter;
                    }
                }
                if (!IFGain1ComboBox.DroppedDown)
                {
                    IFGain1ComboBox.SelectedIndex = (int) _owner.IFGain1;
                }
                if (!IFGain2ComboBox.DroppedDown)
                {
                    IFGain2ComboBox.SelectedIndex = (int) _owner.IFGain2;
                }
                if (!IFGain3ComboBox.DroppedDown)
                {
                    IFGain3ComboBox.SelectedIndex = (int) _owner.IFGain3;
                }
                if (!IFGain4ComboBox.DroppedDown)
                {
                    IFGain4ComboBox.SelectedIndex = (int) _owner.IFGain4;
                }
                if (!IFGain5ComboBox.DroppedDown)
                {
                    IFGain5ComboBox.SelectedIndex = (int) _owner.IFGain5;
                }
                if (!IFGain6ComboBox.DroppedDown)
                {
                    IFGain6ComboBox.SelectedIndex = (int) _owner.IFGain6;
                }
                if (!IFRCFilterComboBox.DroppedDown)
                {
                    IFRCFilterComboBox.SelectedIndex = 15 - (int) _owner.IFRCFilter;
                }
                if (!BandComboBox.DroppedDown)
                {
                    BandComboBox.SelectedIndex = (int) _owner.Band;
                }
                if (!IFFilterComboBox.DroppedDown)
                {
                    IFFilterComboBox.SelectedIndex = 31 - (int)_owner.IFFilter;
                }
                if (!LNAEnhanceComboBox.DroppedDown)
                {
                    if (_owner.LNAEnhance == TunerLNAEnhance.TLEE_OFF)
                    {
                        LNAEnhanceComboBox.SelectedIndex = 0;
                    }
                    else
                    {
                        LNAEnhanceComboBox.SelectedIndex = ((int) _owner.LNAEnhance + 1) / 2;
                    }
                }
                if (!BiasCurrentComboBox.DroppedDown)
                {
                    BiasCurrentComboBox.SelectedIndex = (int) _owner.BiasCurrent;
                }
                if (!IFGainModeComboBox.DroppedDown)
                {
                    IFGainModeComboBox.SelectedIndex = (int) _owner.IFGainMode;
                }
                if (!LNAGainComboBox.Enabled)
                {
                    LNAGainComboBox.Enabled = true;
                    RFFilterComboBox.Enabled = true;
                    mixerGainComboBox.Enabled = true;
                    mixerFilterComboBox.Enabled = true;
                    IFGain1ComboBox.Enabled = true;
                    IFRCFilterComboBox.Enabled = true;
                    IFGain2ComboBox.Enabled = true;
                    IFGain3ComboBox.Enabled = true;
                    IFGain4ComboBox.Enabled = true;
                    IFFilterComboBox.Enabled = true;
                    IFGain5ComboBox.Enabled = true;
                    IFGain6ComboBox.Enabled = true;
                    LNAEnhanceComboBox.Enabled = true;
                    BiasCurrentComboBox.Enabled = true;
                    IFGainModeComboBox.Enabled = true;    
                }

                frequencyCorrectionNumericUpDown.Value = (decimal) _owner.FrequencyCorrection;
            }
            catch (Exception)
            {
                if (LNAGainComboBox.Enabled)
                {
                    LNAGainComboBox.Enabled = false;
                    RFFilterComboBox.Enabled = false;
                    mixerGainComboBox.Enabled = false;
                    mixerFilterComboBox.Enabled = false;
                    IFGain1ComboBox.Enabled = false;
                    IFRCFilterComboBox.Enabled = false;
                    IFGain2ComboBox.Enabled = false;
                    IFGain3ComboBox.Enabled = false;
                    IFGain4ComboBox.Enabled = false;
                    IFFilterComboBox.Enabled = false;
                    IFGain5ComboBox.Enabled = false;
                    IFGain6ComboBox.Enabled = false;
                    LNAEnhanceComboBox.Enabled = false;
                    BiasCurrentComboBox.Enabled = false;
                    IFGainModeComboBox.Enabled = false;    
                }
            }
            finally
            {
                _updating = false;
            }
        }

        private void frequencyCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.FrequencyCorrection = (double) frequencyCorrectionNumericUpDown.Value;
                }
                catch (Exception)
                {
                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void FCDControllerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            Utils.SaveSetting("funcubeFrequencyCorrection", _owner.FrequencyCorrection.ToString(CultureInfo.InvariantCulture));
            e.Cancel = true;
            Hide();
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FCDControllerDialog_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                refreshTimer.Enabled = true;
            }
            else
            {
                refreshTimer.Enabled = false;
            }
        }
    }
}
