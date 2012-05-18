using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Intermediate;
using Microsoft.Xna.Framework.Graphics;

namespace TouhouSpring.Particle
{
	partial class Main
	{
		private string m_fileName;
		private ParticleSystem m_particleSystem;
		private bool m_modified = false;

		public void New()
		{
			if (!CloseDocument())
			{
				return;
			}

			m_fileName = null;
			m_particleSystem = new ParticleSystem();

			var effect = new Effect { Capacity = 1000 };
			effect.Name = "New Effect";
			effect.EmissionRate = 50.0f;
			effect.DefaultParticleLifetime = 3.0f;
			effect.DefaultParticleSize = new Microsoft.Xna.Framework.Vector2(1, 1);
			effect.DefaultParticleColor = new Microsoft.Xna.Framework.Color(255, 255, 255, 128);
			effect.ModifiersOnEmit.Add(new Modifiers.RandomPositionInSphere { Radius = 5 });
			effect.ModifiersOnUpdate.Add(new Modifiers.Accelerate { Acceleration = Microsoft.Xna.Framework.Vector3.UnitZ * 10 });

			m_particleSystem.Effects.Add(effect);

			OnDocumentOpen();
			SetModified(true);
		}

		public void Open()
		{
			if (!CloseDocument() || openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			var fileName = openFileDialog.FileName;
			using (XmlReader xr = XmlReader.Create(fileName))
			{
				m_fileName = fileName;
				m_particleSystem = IntermediateSerializer.Deserialize<ParticleSystem>(xr, Path.GetDirectoryName(fileName));
			}

			OnDocumentOpen();
			SetModified(false);
		}

		public bool Save()
		{
			if (m_fileName == null)
			{
				if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
				{
					return false;
				}

				m_fileName = saveFileDialog.FileName;
			}

			using (XmlWriter xw = XmlWriter.Create(m_fileName, new XmlWriterSettings() { Indent = true }))
			{
				IntermediateSerializer.Serialize(xw, m_particleSystem, Path.GetDirectoryName(m_fileName));
			}

			SetModified(false);
			return true;
		}

		public bool CloseDocument()
		{
			if (m_modified)
			{
				var result = MessageBox.Show("File has been changed. Do you want to save?", "Particle Workshop", MessageBoxButtons.YesNoCancel);
				if (result == DialogResult.Cancel
					|| result == DialogResult.Yes && !Save())
				{
					return false;
				}
			}

			return true;
		}

		private void SetModified(bool modified)
		{
			m_modified = modified;

			var fileTitle = m_fileName != null ? Path.GetFileNameWithoutExtension(m_fileName) : "Untitled";
			if (m_modified)
			{
				fileTitle += "*";
			}
			Text = fileTitle + " - Particle Workshop";
			treeView.Nodes[0].Text = fileTitle;
		}

		private void OnDocumentOpen()
		{
			canvas.System = m_particleSystem;

			var rootNode = new TreeNode(m_fileName != null ? Path.GetFileNameWithoutExtension(m_fileName) : "Untitled");
			rootNode.Tag = m_particleSystem;

			treeView.Nodes.Clear();
			treeView.Nodes.Add(rootNode);

			foreach (var effect in m_particleSystem.Effects)
			{
				var node = AddEffectNode(effect);
				var n1 = node.Nodes[0];
				var n2 = node.Nodes[1];
				effect.ModifiersOnEmit.ForEach(mod => AddModifierNode(mod, n1));
				effect.ModifiersOnUpdate.ForEach(mod => AddModifierNode(mod, n2));
			}
			rootNode.ExpandAll();

			saveToolStripMenuItem.Enabled = true;
			saveAsToolStripMenuItem.Enabled = true;
			treeView.Enabled = true;
			toolStrip.Enabled = true;
			propertyGrid.Enabled = true;
		}
	}
}
