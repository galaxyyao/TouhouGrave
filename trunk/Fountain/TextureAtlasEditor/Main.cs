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
	public partial class Main : Form
	{
		private string m_defaultTitle;

        private class ImageInfo : Mapper.IImageInfo
        {
            public int Width { get; set; }
            public int Height { get; set; }
            public SubTexture Ref { get; set; }
        }

		public Main()
		{
			InitializeComponent();
			m_defaultTitle = Text;

			addTextureDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
            openFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
            saveFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
		}

		private void Redraw(bool reload)
		{
			if (IsDocOpened)
			{
				if (reload)
				{
					m_view.Reload(m_document);
				}
				else
				{
					m_view.RedrawFinalImage();
				}
				pictureBox.Image = m_view.FinalComposedImage;
			}
		}

		private void UpdateTitle()
		{
			Text = m_defaultTitle;
			if (IsDocOpened)
			{
				Text += " - " + (m_document.FileName != null ? Path.GetFileName(m_document.FileName) : "Untitled");
				if (m_isModified)
				{
					Text += "*";
				}
			}
		}

		#region menu items

		private void newToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dlg = new NewFile
			{
                Text = "New File",
				ImageWidth = 512,
				ImageHeight = 512
			};

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				m_isModified = true;
				Open(new Document(dlg.ImageWidth, dlg.ImageHeight));
			}
		}

		private void openToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Open(new Document(openFileDialog.FileName));
			}
		}

		private void saveToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Save();
		}

		private void propertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var dlg = new NewFile
			{
                Text = "Properties",
				ImageWidth = m_document.Atlas.Width,
				ImageHeight = m_document.Atlas.Height
			};

			if (dlg.ShowDialog() == DialogResult.OK)
			{
				if (dlg.ImageWidth != m_document.Atlas.Width
					|| dlg.ImageHeight != m_document.Atlas.Height)
				{
					m_isModified = true;
					m_document.Atlas.Width = dlg.ImageWidth;
					m_document.Atlas.Height = dlg.ImageHeight;

					UpdateTitle();
					Redraw(true);
				}
			}
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void packToolStripMenuItem_Click(object sender, EventArgs e)
		{
            int w, h;
            if (m_document.Atlas.SubTextures.Count > 0)
            {
                var mapper = new Mapper.MapperOptimalEfficiency<PackResult>(new Mapper.Canvas());

                var imageInfoArray = new ImageInfo[m_document.Atlas.SubTextures.Count];
                int index = 0;
                foreach (var subTexture in m_document.Atlas.SubTextures.Values)
                {
                    imageInfoArray[index++] = new ImageInfo
                    {
                        Width = subTexture.Width,
                        Height = subTexture.Height,
                        Ref = subTexture
                    };
                }

                var result = mapper.Mapping(imageInfoArray);

                foreach (var i in result.MappedImages)
                {
                    (i.ImageInfo as ImageInfo).Ref.Left = i.X;
                    (i.ImageInfo as ImageInfo).Ref.Top = i.Y;
                }
                w = MathUtils.RoundToNextPowerOfTwo(result.Width);
                h = MathUtils.RoundToNextPowerOfTwo(result.Height);
            }
            else
            {
                w = 0;
                h = 0;
            }

            m_document.Atlas.Width = Math.Max(w, 16);
            m_document.Atlas.Height = Math.Max(h, 16);

			m_isModified = true;
			UpdateTitle();
			propertyGrid.Refresh();
			Redraw(true);
		}

		#endregion

		#region subtexture's treeview

		private void treeViewSubTextures_AfterSelect(object sender, TreeViewEventArgs e)
		{
			toolStripSubTextureRemove.Enabled = treeViewSubTextures.SelectedNode != null;
			m_view.SelectedSubTexture = treeViewSubTextures.SelectedNode != null ? (SubTexture)treeViewSubTextures.SelectedNode.Tag : null;
			propertyGrid.SelectedObject = treeViewSubTextures.SelectedNode != null ? treeViewSubTextures.SelectedNode.Tag : null;
			Redraw(false);
		}

		private void treeViewSubTextures_MouseUp(object sender, MouseEventArgs e)
		{
			if (treeViewSubTextures.GetNodeAt(e.X, e.Y) == null)
			{
				treeViewSubTextures.SelectedNode = null;
				m_view.SelectedSubTexture = null;
				propertyGrid.SelectedObject = null;
				Redraw(false);
			}

			toolStripSubTextureRemove.Enabled = treeViewSubTextures.SelectedNode != null;
		}

		#endregion

		#region subtexture's toolbar items

		private void addTextureDialog_FileOk(object sender, CancelEventArgs e)
		{
			if (!Program.PathUtils.IsUnderContentRoot(addTextureDialog.FileName))
			{
				MessageBox.Show("Please select a file under the content root.");
				e.Cancel = true;
			}
		}

        private void toolStripSubTextureAdd_Click(object sender, EventArgs e)
        {
            if (addTextureDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            var fileName = addTextureDialog.FileName;
            var id = Path.GetFileNameWithoutExtension(fileName);

            m_document.AddSubTexture(id, fileName);

            var node = new TreeNode(id);
            node.Tag = m_document.Atlas.SubTextures[id];
            treeViewSubTextures.Nodes.Add(node);

            Redraw(true);
            m_isModified = true;
            UpdateTitle();
        }

		private void toolStripSubTextureRemove_Click(object sender, EventArgs e)
		{
			if (treeViewSubTextures.SelectedNode == null)
			{
				toolStripSubTextureAdd.Enabled = false;
				return;
			}

            var id = treeViewSubTextures.SelectedNode.Text;
			var subTexture = (SubTexture)treeViewSubTextures.SelectedNode.Tag;
			if (m_view.SelectedSubTexture == subTexture)
			{
				m_view.SelectedSubTexture = null;
			}
			if (propertyGrid.SelectedObject == subTexture)
			{
				propertyGrid.SelectedObject = null;
			}
			treeViewSubTextures.Nodes.Remove(treeViewSubTextures.SelectedNode);

            m_document.RemoveSubTexture(id);

			Redraw(true);
			m_isModified = true;
			UpdateTitle();

			toolStripSubTextureRemove.Enabled = treeViewSubTextures.SelectedNode != null;
		}

		#endregion

		private void LeftPanel_SizeChanged(object sender, EventArgs e)
		{
			pictureBox.Left = Math.Max((pictureBox.Parent.ClientSize.Width - pictureBox.Width) / 2, 0);
			pictureBox.Top = Math.Max((pictureBox.Parent.ClientSize.Height - pictureBox.Height) / 2, 0);
		}

		private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			Redraw(true);
			m_isModified = true;
			UpdateTitle();
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (m_isModified)
			{
				var result = MessageBox.Show("File has been changed. Do you want to save?", m_defaultTitle, MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Cancel)
				{
					e.Cancel = true;
				}
				else if (result == DialogResult.Yes)
				{
					e.Cancel = !Save();
				}
			}
		}
	}
}
