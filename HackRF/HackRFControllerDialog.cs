using System;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.HackRF
{
    public partial class HackRFControllerDialog : Form
    {
        private const int LNAGainStep = 8;
        private const int VGAGainStep = 2;
        private readonly HackRFIO _owner;
        private bool _initialized;

        public HackRFControllerDialog(HackRFIO owner)
        {
            InitializeComponent();

            _owner = owner;
            var devices = DeviceDisplay.GetActiveDevices();
            deviceComboBox.Items.Clear();
            deviceComboBox.Items.AddRange(devices);

            frequencyCorrectionNumericUpDown.Value = (decimal)Utils.GetDoubleSetting("HackRFFrequencyCorrection", 0);
            samplerateComboBox.SelectedIndex = Utils.GetIntSetting("HackRFSampleRate", 3);
            lnaGainTrackBar.Value = Utils.GetIntSetting("HackRFLNAGain", 0);
            vgaGainTrackBar.Value = Utils.GetIntSetting("HackRFVGAGain", 0);
            externalAmpCb.Checked = Utils.GetBooleanSetting("HackRFExternalAmp");
                                    
            _initialized = true;
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void HackRFControllerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void HackRFControllerDialog_VisibleChanged(object sender, EventArgs e)
        {
            refreshTimer.Enabled = Visible;
            if (Visible)
            {
                samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
                deviceComboBox.Enabled = !_owner.Device.IsStreaming;
                lnaGainTrackBar.Enabled = _owner.Device.IsStreaming;
                vgaGainTrackBar.Enabled = _owner.Device.IsStreaming;
                externalAmpCb.Enabled = _owner.Device.IsStreaming;
                vgaGainLabel.Visible = _owner.Device.IsStreaming;
                lnaGainLabel.Visible = _owner.Device.IsStreaming;

                if (!_owner.Device.IsStreaming)
                {
                    var devices = DeviceDisplay.GetActiveDevices();
                    deviceComboBox.Items.Clear();
                    deviceComboBox.Items.AddRange(devices);

                    for (var i = 0; i < devices.Length; i++)
                    {
                        if (devices[i].Index == ((DeviceDisplay) deviceComboBox.Items[i]).Index)
                        {
                            _initialized = false;
                            deviceComboBox.SelectedIndex = i;
                            _initialized = true;
                            break;
                        }
                    }
                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
            deviceComboBox.Enabled = !_owner.Device.IsStreaming;
            lnaGainTrackBar.Enabled = _owner.Device.IsStreaming;
            vgaGainTrackBar.Enabled = _owner.Device.IsStreaming;
            externalAmpCb.Enabled = _owner.Device.IsStreaming;
            vgaGainLabel.Visible = _owner.Device.IsStreaming;
            lnaGainLabel.Visible = _owner.Device.IsStreaming;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var deviceDisplay = (DeviceDisplay) deviceComboBox.SelectedItem;
            if (deviceDisplay != null)
            {
                try
                {
                    _owner.SelectDevice(deviceDisplay.Index);
                }
                catch (Exception ex)
                {
                    deviceComboBox.SelectedIndex = -1;
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void lnaGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            
            _owner.Device.LNAGain = (uint)lnaGainTrackBar.Value * LNAGainStep;
            lnaGainLabel.Text = _owner.Device.LNAGain + " dB";
            Utils.SaveSetting("HackRFLNAGain", lnaGainTrackBar.Value);
        }

        private void vgaGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }

            _owner.Device.VGAGain = (uint)vgaGainTrackBar.Value * VGAGainStep;
            vgaGainLabel.Text = _owner.Device.VGAGain + " dB";
            Utils.SaveSetting("HackRFVGAGain", lnaGainTrackBar.Value);
        }

        private void externalAmpCb_CheckedChanged(object sender, EventArgs e)
        {
            if (_initialized)
            {
                return;
            }
            _owner.Device.EnableAmp = externalAmpCb.Checked;
            Utils.SaveSetting("HackRFExternalAmp", externalAmpCb.Checked);
        }      

        private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var samplerateString = samplerateComboBox.Items[samplerateComboBox.SelectedIndex].ToString().Split(' ')[0];
            var sampleRate = double.Parse(samplerateString, CultureInfo.InvariantCulture);
            _owner.Device.SampleRate = sampleRate * 1000000.0;
            Utils.SaveSetting("HackRFSampleRate", samplerateComboBox.SelectedIndex);
        }
       
        private void frequencyCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            _owner.FrequencyCorrection = (double) frequencyCorrectionNumericUpDown.Value;
            Utils.SaveSetting("HackRFFrequencyCorrection", frequencyCorrectionNumericUpDown.Value.ToString());
        }

        public void ConfigureGUI()
        {            
            for (var i = 0; i < deviceComboBox.Items.Count; i++)
            {
                var deviceDisplay = (DeviceDisplay) deviceComboBox.Items[i];
                if (deviceDisplay.Index == _owner.Device.Index)
                {
                    deviceComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        public void ConfigureDevice()
        {
            samplerateComboBox_SelectedIndexChanged(null, null);
            frequencyCorrectionNumericUpDown_ValueChanged(null, null);
            lnaGainTrackBar_Scroll(null, null);
            vgaGainTrackBar_Scroll(null, null);
            externalAmpCb_CheckedChanged(null, null);
        }        
    }

    public class DeviceDisplay
    {
        public uint Index { get; private set; }
        public string Name { get; set; }

        public static DeviceDisplay[] GetActiveDevices()
        {                       
            var count = 1;
            var result = new DeviceDisplay[count];
			
			var name = NativeMethods.hackrf_board_id_name(1);
            result[0] = new DeviceDisplay { Index = 0, Name = name };
            /*
            for (var i = 0u; i < count; i++)
            {
                var name = NativeMethods.rtlsdr_get_device_name(i);
                result[i] = new DeviceDisplay { Index = i, Name = name };
            }
            */
            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
