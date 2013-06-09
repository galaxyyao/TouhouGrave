using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring
{
    partial class Main
    {
        private string m_defaultTitle;

        private Document m_document;
        private bool m_isModified = false;

        private bool IsDocOpened
        {
            get { return m_document != null; }
        }

        private void Open(Document doc)
        {
            if (IsDocOpened)
            {
                DialogResult confirmResult = MessageBox.Show("当前的牌库尚未保存，你确认要打开另一个牌库么？", "关闭确认", MessageBoxButtons.OKCancel);
                if (confirmResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return;
                }
                m_document = null;
            }

            m_document = doc;

            treeViewCards.Nodes.Clear();
            propertyGrid.SelectedObject = null;

            m_document.Cards.ForEach(m => AddCard(m, false));

            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            toolStrip.Enabled = true;
            treeViewCards.Enabled = true;
            propertyGrid.Enabled = true;

            treeViewCards.ExpandAll();
            if (treeViewCards.Nodes.Count != 0)
            {
                treeViewCards.SelectedNode = treeViewCards.Nodes[0];
            }

            UpdateTitle();
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

        private void AddCard(EditorCardModel cardModel, bool select)
        {
            var node = new TreeNode(cardModel.Name)
            {
                Tag = cardModel,
                ImageKey = "Card",
                SelectedImageKey = "Card"
            };
            CreateNodeParent(cardModel.Category).Add(node);
            cardModel.Behaviors.ForEach(bhv => AddBehavior(bhv, node, select));

            if (select)
            {
                ShowNode(node, true);
            }
        }

        private void AddBehavior(Behaviors.IBehaviorModel behaviorModel, TreeNode cardNode, bool select)
        {
            var bhvAttr = behaviorModel.GetType().GetAttribute<Behaviors.BehaviorModelAttribute>();
            var node = new TreeNode(behaviorModel.Name)
            {
                Tag = behaviorModel,
                ImageKey = "Behavior",
                SelectedImageKey = "Behavior"
            };
            cardNode.Nodes.Add(node);

            if (select)
            {
                ShowNode(node, true);
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
    }
}
