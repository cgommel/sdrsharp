using System;
using System.Text;
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

        private void FCDControllerDialog_Load(object sender, EventArgs e)
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
                samplerateComboBox.Enabled = !_owner.IsStreaming;
                deviceComboBox.Enabled = !_owner.IsStreaming;

                var devices = DeviceDisplay.GetActiveDevices();

                deviceComboBox.Items.Clear();
                deviceComboBox.Items.AddRange(devices);

                var found = false;
                for (var i = 0; i < deviceComboBox.Items.Count; i++)
                {
                    var item = (DeviceDisplay) deviceComboBox.Items[i];
                    if (item.Index == _owner.DeviceIndex)
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
            samplerateComboBox.Enabled = !_owner.IsStreaming;
            deviceComboBox.Enabled = !_owner.IsStreaming;
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deviceComboBox.SelectedItem != null)
            {
                _owner.DeviceIndex = ((DeviceDisplay) deviceComboBox.SelectedItem).Index;

                tunerTypeLabel.Text = _owner.TunerType.ToString();
                rfGainTrackBar.Maximum = _owner.SupportedGains == null ? 1 : (_owner.SupportedGains.Length - 1);
                samplerateComboBox_SelectedIndexChanged(null, null);
                gainModeCheckBox_CheckedChanged(null, null);
                rfGainTrackBar_Scroll(null, null);
                frequencyCorrectionNumericUpDown_ValueChanged(null, null);
            }
        }

        private void rfGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (_owner.SupportedGains != null)
            {
                var gain = _owner.SupportedGains[rfGainTrackBar.Value];
                _owner.Gain = gain;
                gainLabel.Text = gain/10.0 + " dB";
            }
        }

        private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var samplerateString = samplerateComboBox.Items[samplerateComboBox.SelectedIndex].ToString().Split(' ')[0];
            var sampleRate = double.Parse(samplerateString, CultureInfo.InvariantCulture);
            _owner.Samplerate = sampleRate * 1000000.0;
        }

        private void gainModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            rfGainTrackBar.Enabled = !gainModeCheckBox.Checked;
            _owner.UseAutomaticGain = gainModeCheckBox.Checked;
            if (!gainModeCheckBox.Checked)
            {
                rfGainTrackBar_Scroll(null, null);
            }
        }

        private void frequencyCorrectionNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            _owner.FrequencyCorrection = (int) frequencyCorrectionNumericUpDown.Value;
            Utils.SaveSetting("RTLFrequencyCorrection", frequencyCorrectionNumericUpDown.Value.ToString());
        }

        private class DeviceDisplay
        {
            public uint Index { get; private set; }
            public string Name { get; set; }
            public string Manufacturer { get; set; }
            public string Product { get; set; }
            public string Serial { get; set; }
            public RtlSdrTunerType Tuner { get; set; }

            public static DeviceDisplay[] GetActiveDevices()
            {
                var count = NativeMethods.rtlsdr_get_device_count();
                var result = new DeviceDisplay[count];

                for (var i = 0u; i < count; i++)
                {
                    var manufact = new StringBuilder(1024);
                    var product = new StringBuilder(1024);
                    var serial = new StringBuilder(1024);
                    NativeMethods.rtlsdr_get_device_usb_strings(i, manufact, product, serial);
                    var name = NativeMethods.rtlsdr_get_device_name(i);

                    result[i] = new DeviceDisplay
                                    {
                                        Index = i,
                                        Name = name,
                                        Manufacturer = manufact.ToString(),
                                        Product = product.ToString(),
                                        Serial = serial.ToString()
                                    };
                }

                return result;
            }

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
