using System;
using System.Collections.Generic;
using System.Globalization;
using System.ComponentModel;

using System.Windows.Forms;

namespace SDRSharp.SDRIQ
{
    public partial class SDRIQControllerDialog : Form
    {
        private SdrIqIO _owner;
        private readonly bool _initialized;
        
        public SDRIQControllerDialog(SdrIqIO owner)
        {
            InitializeComponent();

            _owner = owner;

            var devices = DeviceDisplay.GetActiveDevices();
            deviceComboBox.Items.Clear();
            deviceComboBox.Items.AddRange(devices);

            samplerateComboBox.SelectedIndex = 5;
            ifGainTrackBar.Value = 5;
            rfGainTrackBar.Value = 2;

            _initialized = true;
        }

        private void SDRIQControllerDialog_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        public void ConfigureGUI()
        {
            for (var i = 0; i < deviceComboBox.Items.Count; i++)
            {
                var deviceDisplay = (DeviceDisplay)deviceComboBox.Items[i];
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

            ifGainTrackBar_Scroll(null, null);
            rfGainTrackBar_Scroll(null, null);
        }

        private void samplerateComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var samplerateString = samplerateComboBox.Items[samplerateComboBox.SelectedIndex].ToString().Split(' ')[0];
            var sampleRate = uint.Parse(samplerateString, CultureInfo.InvariantCulture);
            _owner.Device.Samplerate = (uint)sampleRate;
        }

        private void rfGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var gain = (rfGainTrackBar.Maximum - rfGainTrackBar.Value) * -10;
            _owner.Device.RfGain = (sbyte)gain;
            rfGainLabel.Text = gain + " dB";
        }

        private void ifGainTrackBar_Scroll(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var gain = (sbyte)ifGainTrackBar.Value * 6;
            _owner.Device.IfGain = (sbyte)gain;
            ifGainLabel.Text = gain +" dB";
        }

        private void deviceComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!_initialized)
            {
                return;
            }
            var deviceDisplay = (DeviceDisplay)deviceComboBox.SelectedItem;
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

        private void SDRIQControllerDialog_VisibleChanged(object sender, EventArgs e)
        {
            refreshTimer.Enabled = Visible;
            if (Visible)
            {
                samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
                deviceComboBox.Enabled = !_owner.Device.IsStreaming;
            }
        }

        private void refreshTimer_Tick(object sender, EventArgs e)
        {
            samplerateComboBox.Enabled = !_owner.Device.IsStreaming;
            deviceComboBox.Enabled = !_owner.Device.IsStreaming;
        }

        
    }

    public class DeviceDisplay
    {
        public uint Index { get; private set; }
        public string Name { get; set; }

        public static DeviceDisplay[] GetActiveDevices()
        {
            var count = NativeMethods.sdriq_get_device_count();
            var result = new DeviceDisplay[count];

            for (var i = 0u; i < count; i++)
            {               
                var name = "SDR-IQ #"+i+" S/N: " +NativeMethods.sdriq_get_serial_number(i);                
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
