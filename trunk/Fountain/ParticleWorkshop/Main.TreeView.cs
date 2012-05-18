using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring.Particle
{
	partial class Main
	{
		private TreeNode AddEffectNode(Effect effect)
		{
			var node = new TreeNode(effect.Name);
			node.Tag = effect;

			var modOnEmitNode = new TreeNode("Modifiers on Emit");
			modOnEmitNode.Tag = effect.ModifiersOnEmit;
			node.Nodes.Add(modOnEmitNode);

			var modOnUpdateNode = new TreeNode("Modifiers on Update");
			modOnUpdateNode.Tag = effect.ModifiersOnUpdate;
			node.Nodes.Add(modOnUpdateNode);

			treeView.Nodes[0].Nodes.Add(node);
			return node;
		}

		private TreeNode AddModifierNode(Modifier modifier, TreeNode modifierListNode)
		{
			var node = new TreeNode(modifier.GetType().Name);
			node.Tag = modifier;
			modifierListNode.Nodes.Add(node);
			return node;
		}

		private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertyGrid.SelectedObject = e.Node.Tag is ModifierList ? (e.Node.Tag as ModifierList).Effect : e.Node.Tag;

			toolStripDropDownButtonNewModifier.Visible = e.Node.Tag is Modifier || e.Node.Tag is ModifierList;
			toolStripButtonDelete.Visible = e.Node.Tag is Effect || e.Node.Tag is Modifier;

			toolStripButtonMoveUp.Visible = false;
			toolStripButtonMoveDown.Visible = false;
			if (e.Node.Tag is Effect)
			{
				var effect = (Effect)e.Node.Tag;
				var index = effect.System.Effects.IndexOf(effect);
				toolStripButtonMoveUp.Visible = index != 0;
				toolStripButtonMoveDown.Visible = index != effect.System.Effects.Count - 1;
			}
			else if (e.Node.Tag is Modifier)
			{
				var mod = (Modifier)e.Node.Tag;
				var modList = (ModifierList)e.Node.Parent.Tag;
				var index = modList.IndexOf(mod);
				toolStripButtonMoveUp.Visible = index != 0;
				toolStripButtonMoveDown.Visible = index != modList.Count - 1;
			}
		}

		private void toolStripButtonNewEffect_Click(object sender, EventArgs e)
		{
			var effect = new Effect { Capacity = 1000 };
			effect.Name = "New Effect";
			(treeView.Nodes[0].Tag as ParticleSystem).Effects.Add(effect);

			var node = AddEffectNode(effect);
			treeView.SelectedNode = node;
			node.Expand();

			SetModified(true);
		}

		private void NewModifier_Click(object sender, EventArgs e)
		{
			var modListNode = treeView.SelectedNode.Tag is ModifierList
							 ? treeView.SelectedNode
							 : treeView.SelectedNode.Parent;
			var modList = modListNode.Tag as ModifierList;
			var type = (sender as ToolStripMenuItem).Tag as Type;
			var mod = type.Assembly.CreateInstance(type.FullName) as Modifier;
			modList.Add(mod);
			treeView.SelectedNode = AddModifierNode(mod, modListNode);

			SetModified(true);
		}

		private void toolStripButtonDelete_Click(object sender, EventArgs e)
		{
			if (treeView.SelectedNode.Tag is Effect)
			{
				var effect = treeView.SelectedNode.Tag as Effect;
				effect.System.Effects.Remove(effect);
			}
			else if (treeView.SelectedNode.Tag is Modifier)
			{
				var mod = treeView.SelectedNode.Tag as Modifier;
				var modList = treeView.SelectedNode.Parent.Tag as ModifierList;
				modList.Remove(mod);
			}

			treeView.SelectedNode.Remove();
			SetModified(true);
		}

		private void toolStripButtonMoveUp_Click(object sender, EventArgs e)
		{
			if (treeView.SelectedNode.Tag is Effect)
			{
				var effect = treeView.SelectedNode.Tag as Effect;
				var effectList = effect.System.Effects;
				var index = effectList.IndexOf(effect);

				effectList.RemoveAt(index);
				effectList.Insert(index - 1, effect);

				var node = treeView.SelectedNode;
				var parentNode = node.Parent;
				parentNode.Nodes.RemoveAt(index);
				parentNode.Nodes.Insert(index - 1, node);

				treeView.SelectedNode = node;
			}
			else if (treeView.SelectedNode.Tag is Modifier)
			{
				var mod = treeView.SelectedNode.Tag as Modifier;
				var modList = treeView.SelectedNode.Parent.Tag as ModifierList;
				var index = modList.IndexOf(mod);

				modList.RemoveAt(index);
				modList.Insert(index - 1, mod);

				var node = treeView.SelectedNode;
				var parentNode = node.Parent;
				parentNode.Nodes.RemoveAt(index);
				parentNode.Nodes.Insert(index - 1, node);

				treeView.SelectedNode = node;
			}

			SetModified(true);
		}

		private void toolStripButtonMoveDown_Click(object sender, EventArgs e)
		{
			if (treeView.SelectedNode.Tag is Effect)
			{
				var effect = treeView.SelectedNode.Tag as Effect;
				var effectList = effect.System.Effects;
				var index = effectList.IndexOf(effect);

				effectList.RemoveAt(index);
				effectList.Insert(index + 1, effect);

				var node = treeView.SelectedNode;
				var parentNode = node.Parent;
				parentNode.Nodes.RemoveAt(index);
				parentNode.Nodes.Insert(index + 1, node);

				treeView.SelectedNode = node;
			}
			else if (treeView.SelectedNode.Tag is Modifier)
			{
				var mod = treeView.SelectedNode.Tag as Modifier;
				var modList = treeView.SelectedNode.Parent.Tag as ModifierList;
				var index = modList.IndexOf(mod);

				modList.RemoveAt(index);
				modList.Insert(index + 1, mod);

				var node = treeView.SelectedNode;
				var parentNode = node.Parent;
				parentNode.Nodes.RemoveAt(index);
				parentNode.Nodes.Insert(index + 1, node);

				treeView.SelectedNode = node;
			}

			SetModified(true);
		}
	}
}
