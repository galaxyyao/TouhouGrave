using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring.TextureAtlas
{
	public partial class NewFile : Form
	{
		public int ImageWidth
		{
			get; set;
		}

		public int ImageHeight
		{
			get; set;
		}

		public NewFile()
		{
			InitializeComponent();
		}

		private void NewFile_Load(object sender, EventArgs e)
		{
			textBoxWidth.Text = ImageWidth.ToString();
			textBoxHeight.Text = ImageHeight.ToString();
		}

		private void buttonOK_Click(object sender, EventArgs e)
		{
			errorProvider.Clear();

			int v;
			if (!int.TryParse(textBoxWidth.Text, out v) || v <= 0)
			{
				errorProvider.SetError(textBoxWidth, "Should be a number greater than 0.");
				DialogResult = DialogResult.None;
			}
			if (!int.TryParse(textBoxHeight.Text, out v) || v <= 0)
			{
				errorProvider.SetError(textBoxHeight, "Should be a number greater than 0.");
				DialogResult = DialogResult.None;
			}

			if (DialogResult == DialogResult.OK)
			{
				ImageWidth = int.Parse(textBoxWidth.Text);
				ImageHeight = int.Parse(textBoxHeight.Text);
			}
		}
	}
}
