using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring.TextureAtlas
{
	partial class Main
	{
		private Document m_document;
		private View m_view;
		private bool m_isModified = false;

		private bool IsDocOpened
		{
			get { return m_document != null; }
		}

		private void Open(Document doc)
		{
			if (IsDocOpened)
			{
				throw new InvalidOperationException("Another document is opened.");
			}

			m_document = doc;
			m_view = new View();

			treeViewSubTextures.Nodes.Clear();
			propertyGrid.SelectedObject = null;

			foreach (var kvp in m_document.Atlas.SubTextures)
			{
				var node = new TreeNode(kvp.Key);
				node.Tag = kvp.Value;
				treeViewSubTextures.Nodes.Add(node);
			}

			saveToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
			propertiesToolStripMenuItem.Enabled = true;
			packToolStripMenuItem.Enabled = true;
			toolStripSubTextures.Enabled = true;
			treeViewSubTextures.Enabled = true;
			propertyGrid.Enabled = true;

			UpdateTitle();
			Redraw(true);
		}

		private bool Save()
		{
			if (!IsDocOpened)
			{
				throw new InvalidOperationException("No document is opened.");
			}

			if (m_document.FileName == null)
			{
				if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
				{
					return false;
				}

				m_document.SaveAs(saveFileDialog.FileName);
			}
			else
			{
				m_document.SaveAs(m_document.FileName);
			}

			m_isModified = false;
			UpdateTitle();

			return true;
		}
	}
}
