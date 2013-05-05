using System;
using System.Windows.Forms;

namespace SDRSharp.DNR
{
    public partial class AudioProcessorPanel : UserControl
    {
        AudioProcessor _control;
       
        public AudioProcessorPanel(AudioProcessor control)
        {
            InitializeComponent();

            _control = control;
            DisableControls();
            thresholdTrackBar_Scroll(null, null);
        }
        
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _control.Bypass = !enableCheckBox.Checked;
            thresholdTrackBar_Scroll(null, null);
        }
        
        public void DisableControls()
        {
            enableCheckBox.Enabled = false;
            thresholdTrackBar.Enabled = false;
            thresholdLabel.Enabled = false;
            _control.Bypass = true;
        }

        public void EnableControls()
        {
            enableCheckBox.Enabled = true;
            thresholdTrackBar.Enabled = true;
            thresholdLabel.Enabled = true;
            _control.Bypass = !enableCheckBox.Checked;
        }

        private void thresholdTrackBar_Scroll(object sender, EventArgs e)
        {
            thresholdLabel.Text = thresholdTrackBar.Value + " dB";
            _control.NoiseThreshold = thresholdTrackBar.Value;
        }
    }
}
