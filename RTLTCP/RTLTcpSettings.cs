using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SDRSharp.RTLTCP
{
    public partial class RTLTcpSettings : Form
    {
        private RtlTcpIO _owner;
        public RTLTcpSettings(RtlTcpIO owner)
        {
            _owner = owner;
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _owner.hostName = hostBox.Text;
            _owner.port = Convert.ToUInt16(portBox.Text);
            _owner.Samplerate = Convert.ToDouble(srBox.Text);
            if (autoRB.Checked)
            {
                _owner.GainMode = RtlTcpIO.GAIN_MODE_AUTO;
            }
            else
            {
                _owner.GainMode = RtlTcpIO.GAIN_MODE_MANUAL;
                _owner.Gain = Convert.ToInt32(gainBox.Text);
            }
        }

        private void RTLTcpSettings_Load(object sender, EventArgs e)
        {
            hostBox.Text = _owner.hostName;
            portBox.Text = _owner.port.ToString();
            srBox.Text = _owner.Samplerate.ToString();
            gainBox.Text = _owner.Gain.ToString();
            uint gainMode = _owner.GainMode;
            if (gainMode == RtlTcpIO.GAIN_MODE_AUTO)
            {
                autoRB.Checked = true;
                manualRB.Checked = false;
                gainBox.Enabled = false;
            }
            else
            {
                autoRB.Checked = false;
                manualRB.Checked = true;
                gainBox.Enabled = true;
            }
        }

        void updateRB()
        {
            gainBox.Enabled = manualRB.Checked;
        }

        private void manualRB_CheckedChanged(object sender, EventArgs e)
        {
            updateRB();
        }

        private void autoRB_CheckedChanged(object sender, EventArgs e)
        {
            updateRB();
        }
    }
}
