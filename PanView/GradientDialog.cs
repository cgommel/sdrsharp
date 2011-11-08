using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SDRSharp.PanView
{
    public partial class GradientDialog : Form
    {
        private GradientDialog()
        {
            InitializeComponent();
        }

        public static ColorBlend GetGradient(ColorBlend originalBlend)
        {
            using (var form = new GradientDialog())
            {
                form.SetColorBlend(originalBlend);
                if (form.ShowDialog() == DialogResult.OK)
                {
                    return form.GetColorBlend();
                }
            }
            return null;
        }

        private ColorBlend GetColorBlend()
        {
            var result = new ColorBlend(colorListBox.Items.Count);
            var distance = 1f / (result.Positions.Length - 1);
            for (var i = 0; i < result.Positions.Length; i++)
            {
                result.Positions[i] = i * distance;
                result.Colors[i] = (Color)colorListBox.Items[i];
            }

            return result;
        }

        private void SetColorBlend(ColorBlend colorBlend)
        {
            for (var i = 0; i < colorBlend.Positions.Length; i++)
            {
                colorListBox.Items.Add(colorBlend.Colors[i]);
            }
        }

        private void colorListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
            {
                return;
            }
            var color = (Color) colorListBox.Items[e.Index];
            if ((e.State & DrawItemState.Selected) == 0)
            {
                using (var brush = new SolidBrush(color))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds.Left + 1, e.Bounds.Top + 1, e.Bounds.Width - 2, e.Bounds.Height - 1);
                }
            }
            else
            {
                using (var brush = new HatchBrush(HatchStyle.Percent70, color, Color.Gray))
                {
                    e.Graphics.FillRectangle(brush, e.Bounds.Left + 1, e.Bounds.Top + 1, e.Bounds.Width - 2, e.Bounds.Height - 1);
                }
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            var index = colorListBox.SelectedIndex;
            if (index > 0)
            {
                var obj = colorListBox.Items[index];
                colorListBox.Items.RemoveAt(index);
                colorListBox.Items.Insert(index - 1, obj);
                colorListBox.SelectedIndex = index - 1;
                gradientPictureBox.Invalidate();
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            var index = colorListBox.SelectedIndex;
            if (index >= 0 && index < colorListBox.Items.Count - 1)
            {
                var obj = colorListBox.Items[index];
                colorListBox.Items.RemoveAt(index);
                colorListBox.Items.Insert(index + 1, obj);
                colorListBox.SelectedIndex = index + 1;
                gradientPictureBox.Invalidate();
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog(this) == DialogResult.OK)
            {
                colorListBox.Items.Add(colorDialog.Color);
                gradientPictureBox.Invalidate();
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            var index = colorListBox.SelectedIndex;
            if (index >= 0 && colorListBox.Items.Count > 2)
            {
                colorListBox.Items.RemoveAt(index);
                gradientPictureBox.Invalidate();
            }
        }

        private void gradientPictureBox_Paint(object sender, PaintEventArgs e)
        {
            var colorBlend = GetColorBlend();
            using (var gradientBrush = new LinearGradientBrush(gradientPictureBox.ClientRectangle, Color.White, Color.Black, LinearGradientMode.Vertical))
            {
                gradientBrush.InterpolationColors = colorBlend;
                e.Graphics.FillRectangle(gradientBrush, e.ClipRectangle);
            }
        }
    }
}
