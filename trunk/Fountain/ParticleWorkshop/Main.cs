using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SystemColor = System.Drawing.Color;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace TouhouSpring.Particle
{
	public partial class Main : Form
	{
		private int m_framesDrawn = 0;

		public Main()
		{
			InitializeComponent();

			CustomTypeConverters.CurveNameProxy = new CurveSelector();
			CustomTypeConverters.TextureNameProxy = new TextureSelector();
			CustomTypeConverters.UVBoundsProxy = new SubTextureSelector();
			Application.Idle += (s, e) => { ++m_framesDrawn; };

			foreach (var t in AssemblyReflection.GetTypesDerivedFrom<Modifier>().Where(t => !t.IsAbstract).OrderBy(t => t.Name))
			{
				var item = new ToolStripMenuItem(t.Name);
				item.Tag = t;
				item.Click += new EventHandler(NewModifier_Click);
				toolStripDropDownButtonNewModifier.DropDownItems.Add(item);
			}

			openFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
			saveFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
		}

		private void timerFps_Tick(object sender, EventArgs e)
		{
			toolStripStatusLabelNumParticles.Text = String.Format("Nb Particles: {0}", m_particleSystem != null ? m_particleSystem.TotalLiveParticles : 0);
			toolStripStatusLabelFps.Text = String.Format("FPS: {0:.##}", m_framesDrawn * (1000f / timerFps.Interval));
			m_framesDrawn = 0;
		}

		private void toolStripStatusColor_Click(object sender, EventArgs e)
		{
			colorDialog.Color = SystemColor.FromArgb(canvas.ClearColor.A, canvas.ClearColor.R, canvas.ClearColor.G, canvas.ClearColor.B);
			if (colorDialog.ShowDialog() == DialogResult.OK)
			{
				toolStripStatusColor.BackColor = colorDialog.Color;
				canvas.ClearColor = new XnaColor(colorDialog.Color.R, colorDialog.Color.G, colorDialog.Color.B, colorDialog.Color.A);
			}
		}

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			if (e.ChangedItem.PropertyDescriptor.Name == "Name"
				&& e.ChangedItem.PropertyDescriptor.ComponentType == typeof(Effect))
			{
				var effectNode = treeView.SelectedNode.Tag is Effect
								 ? treeView.SelectedNode
								 : treeView.SelectedNode.Parent;
				effectNode.Text = (string)e.ChangedItem.Value;
			}

			SetModified(true);
		}

		private void toolStripButtonPlayPause_Click(object sender, EventArgs e)
		{
			toolStripButtonPlayPause.Image = toolStripButtonPlayPause.Checked ? Properties.Resources.Pause_16x16 : Properties.Resources.Play_16x16;
			canvas.KeepUpdating = toolStripButtonPlayPause.Checked;
		}

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			New();
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Open();
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void fileDialog_FileOk(object sender, CancelEventArgs e)
		{
			var fileDialog = sender as FileDialog;
			var filePath = fileDialog.FileName;
			if (!Path.IsPathRooted(filePath) || !filePath.StartsWith(fileDialog.InitialDirectory, true, null))
			{
				MessageBox.Show("Please select a file under the content root.", "Particle Workshop");
				e.Cancel = true;
			}
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !CloseDocument();
		}
	}
}
