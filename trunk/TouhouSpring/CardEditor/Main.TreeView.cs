using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring
{
    partial class Main
    {
        private void treeViewCards_AfterSelect(object sender, TreeViewEventArgs e)
        {
            propertyGrid.SelectedObject = e.Node != null ? e.Node.Tag : null;

            toolStripDropDownButtonNewBehavior.Visible =
                e.Node != null && (e.Node.Tag is EditorCardModel || e.Node.Tag is Behaviors.BehaviorModel);
            toolStripButtonDelete.Visible = e.Node != null && e.Node.Tag != null;
            toolStripSeparator.Visible = toolStripButtonDelete.Visible;
        }

        private TreeNodeCollection CreateNodeParent(string category)
        {
            TreeNodeCollection nodeCollection = treeViewCards.Nodes;
            if (category == null || category == String.Empty)
            {
                return nodeCollection;
            }

            var pathTokens = category.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < pathTokens.Length; ++i)
            {
                var token = pathTokens[i];

                if (nodeCollection.ContainsKey(token))
                {
                    nodeCollection = nodeCollection[token].Nodes;
                }
                else
                {
                    var tn = nodeCollection.Add(token, token, "Folder_Closed", "Folder_Closed");
                    nodeCollection = tn.Nodes;
                }
            }

            return nodeCollection;
        }

        private void MoveNode(TreeNode cardModelNode)
        {
            for (var n = cardModelNode; n != null; )
            {
                var p = n.Parent;
                var pNodes = p != null ? p.Nodes : treeViewCards.Nodes;
                pNodes.Remove(n);
                if (pNodes.Count != 0)
                {
                    break;
                }
                n = p;
            }

            CreateNodeParent((cardModelNode.Tag as EditorCardModel).Category).Add(cardModelNode);
            ShowNode(cardModelNode, true);
        }

        private void ShowNode(TreeNode node, bool select)
        {
            for (var n = node.Parent; n != null; n = n.Parent)
            {
                n.Expand();
            }
            if (select)
            {
                treeViewCards.SelectedNode = node;
            }
        }
    }
}
