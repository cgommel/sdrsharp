using System;
using System.Globalization;
using System.Windows.Forms;
using SDRSharp.Radio;

namespace SDRSharp.RTLSDR
{
    public partial class RtlSdrControllerDialog : Form
    {
        private readonly RtlSdrIO _owner;

        public RtlSdrControllerDialog(RtlSdrIO owner)
        {
            _owner = owner;
            InitializeComponent();
        }

        private void RtlSdrControllerDialog_Load(object sender, EventArgs e)
        {
            samplerateComboBox.SelectedIndex = 3;
            frequencyCorrectionNumericUpDown.Value = Utils.GetIntSetting("RTLFrequencyCorrection", 0);
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void RtlSdrControllerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void RtlSdrControllerDialog_VisibleChanged(object sender, EventArgs e)
        {
            refreshTimer.Enabled = Visible;
            if (Visible)
            {
                samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
                deviceComboBox.Enabled = !_owner.Device.IsStreaming;

                var devices = DeviceDisplay.GetActiveDevices();

                deviceComboBox.Items.Clear();
                deviceComboBox.Items.AddRange(devices);

                var found = false;
                for (var i = 0; i < deviceComboBox.Items.Count; i++)
                {
                    var deviceDisplay = (DeviceDisplay) deviceComboBox.Items[i];
                    if (deviceDisplay.Index == _owner.Device.Index)
                    {
                        deviceComboBox.SelectedIndex = i;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    deviceComboBox.SelectedIndex = -1;
                }
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
            deviceComboBox.Enabled = !_owner.Device.IsStreaming;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var deviceDisplay = (DeviceDisplay) deviceComboBox.SelectedItem;
            if (deviceDisplay != null)
            {
                try
                {
                    _owner.SelectDevice(deviceDisplay.Index);

                    tunerTypeLabel.Text = _owner.Device.TunerType.ToString();
                    rfGainTrackBar.Maximum = _owner.Device.SupportedGains.Length - 1;
                    samplerateComboBox_SelectedIndexChanged(null, null);
                    gainModeCheckBox_CheckedChanged(null, null);
                    if (!gainModeCheckBox.Checked)
                    {
                        rfGainTrackBar_Scroll(null, null);
                    }
                    frequencyCorrectionNumericUpDown_ValueChanged(null, null);
                }
                catch (Exception ex)
                {
                    deviceComboBox.SelectedIndex = -1;
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void rfGainTrackBar_Scroll(object sender, EventArgs e)
        {
            var gain = _owner.Device.SupportedGains[rfGainTrackBar.Value];
            _owner.Device.Gain = gain;
            gainLabel.Text = gain / 10.0 + " dB";
        }

        private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var samplerateString = samplerateComboBox.Items[samplerateComboBox.SelectedIndex].ToString().Split(' ')[0];
            var sampleRate = double.Parse(samplerateString, CultureInfo.InvariantCulture);
            _owner.Device.Samplerate = (uint) (sampleRate * 1000000.0);
        }

        private void gainModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            rfGainTrackBar.Enabled = !gainModeCheckBox.Checked;
            _owner.Device.UseAutomaticGain = gainModeCheckBox.Checked;
            gainLabel.Visible = !gainModeCheckBox.Checked;
            if (!gainModeCheckBox.Checked)
            {
                rfGainTrackBar_Scroll(null, null);
            }
        }

        private void frequencyCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _owner.Device.FrequencyCorrection = (int) frequencyCorrectionNumericUpDown.Value;
            Utils.SaveSetting("RTLFrequencyCorrection", frequencyCorrectionNumericUpDown.Value.ToString());
        }
    }

    public class DeviceDisplay
    {
        public uint Index { get; private set; }
        public string Name { get; set; }

        public static DeviceDisplay[] GetActiveDevices()
        {
            var count = NativeMethods.rtlsdr_get_device_count();
            var result = new DeviceDisplay[count];

            for (var i = 0u; i < count; i++)
            {
                var name = NativeMethods.rtlsdr_get_device_name(i);
                result[i] = new DeviceDisplay { Index = i, Name = name };
            }

            return result;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
