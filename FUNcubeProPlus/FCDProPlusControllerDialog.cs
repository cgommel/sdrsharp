using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.FUNcubeProPlus
{
    public partial class FCDProPlusControllerDialog : Form
    {
        private bool _updating;
        private readonly FunCubeProPlusIO _owner;

        public FCDProPlusControllerDialog(FunCubeProPlusIO owner)
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
            lnaEnableCb.Checked = true;
            mixerGainEnableCb.Checked = true;
            biasTEnabledCb.Checked = true;
            ifGainNumbericUpDown.Value = 0;
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
                frequencyCorrectionNumericUpDown.Value = (decimal) _owner.FrequencyCorrection;
                mixerGainEnableCb.Checked = _owner.MixerGainEnabled;
                lnaEnableCb.Checked = _owner.LNAEnabled;
                biasTEnabledCb.Checked = _owner.BiasTeeEnabled;
                ifGainNumbericUpDown.Value = _owner.IFGain;

                var rfFilter = (int)_owner.RFFilter;
                if(rfFilter < rfFilterCombo.Items.Count)
                {
                    rfFilterCombo.SelectedIndex = rfFilter;
                }

                var ifFilter = (int)_owner.IFFilter;
                if (ifFilter < ifFilterCombo.Items.Count)
                {
                    ifFilterCombo.SelectedIndex = ifFilter;
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
            Utils.SaveSetting("funcubeProPlusFrequencyCorrection", _owner.FrequencyCorrection.ToString(CultureInfo.InvariantCulture));
                        
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

        private void ifGainNumbericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.IFGain = (int)ifGainNumbericUpDown.Value;
                }
                catch
                {

                }
                finally
                {
                    _updating = false;
                }
            }
        }

        private void mixerGainEnableCb_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.MixerGainEnabled = mixerGainEnableCb.Checked;
                }
                catch
                {
                }
                finally
                {
                    _updating = false;
                }

            }
        }

        private void lnaEnableCb_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.LNAEnabled = lnaEnableCb.Checked;
                }
                catch
                {
                }
                finally
                {
                    _updating = false;
                }

            }
        }

        private void biasTEnabledCb_CheckedChanged(object sender, EventArgs e)
        {
            if (!_updating)
            {
                _updating = true;
                try
                {
                    _owner.BiasTeeEnabled = biasTEnabledCb.Checked;
                }
                catch
                {
                }
                finally
                {
                    _updating = false;
                }
            }
        }
    }
}
