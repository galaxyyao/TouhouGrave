using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring
{
    public partial class BehaviorEditor : Form
    {
        private List<Type> m_bhvTypes;
        private string m_lastSearchString = null;

        public Behaviors.IBehaviorModel EditedBehaviorModel
        {
            get; private set;
        }

        public BehaviorEditor(List<Type> behaviorTypes)
        {
            InitializeComponent();

            Font = SystemFonts.MessageBoxFont;

            m_bhvTypes = behaviorTypes;
        }

        private void BehaviorEditor_Load(object sender, EventArgs e)
        {
            UpdateTreeView(String.Empty);
        }

        private void textBoxSearchBox_TextChanged(object sender, EventArgs e)
        {
            UpdateTreeView(textBoxSearchBox.Text);
        }

        private void UpdateTreeView(string searchString)
        {
            if (m_lastSearchString != null
                && CultureInfo.CurrentCulture.CompareInfo.IndexOf(searchString, m_lastSearchString, CompareOptions.IgnoreCase) != -1)
            {
                FilterTreeView(searchString, treeViewBehaviors.Nodes);
            }
            else
            {
                PopulateTreeView(searchString);
            }

            treeViewBehaviors.ExpandAll();
            m_lastSearchString = searchString;
        }

        private bool Match(Type bhvType, string searchString)
        {
            var bhvAttr = bhvType.GetAttribute<Behaviors.BehaviorModelAttribute>();

            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(bhvAttr.DefaultName, searchString, CompareOptions.IgnoreCase) >= 0
                   || CultureInfo.CurrentCulture.CompareInfo.IndexOf(bhvType.FullName, searchString, CompareOptions.IgnoreCase) >= 0;
        }

        private void PopulateTreeView(string searchString)
        {
            treeViewBehaviors.Nodes.Clear();

            foreach (var t in m_bhvTypes)
            {
                if (!Match(t, searchString))
                {
                    continue;
                }

                var bhvAttr = t.GetAttribute<Behaviors.BehaviorModelAttribute>();
                var category = bhvAttr.Category.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
                var root = treeViewBehaviors.Nodes;
                for (int i = 0; i < category.Length; ++i)
                {
                    var childNode = root.Find(category[i], false);
                    if (childNode.Length == 0)
                    {
                        var newNode = new TreeNode(category[i]);
                        newNode.Name = category[i];
                        root.Add(newNode);
                        root = newNode.Nodes;
                    }
                    else
                    {
                        root = childNode[0].Nodes;
                    }
                }

                root.Add(new TreeNode(bhvAttr.DefaultName)
                {
                    Tag = t
                });
            }
        }

        private void FilterTreeView(string searchString, TreeNodeCollection nodes)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                var node = nodes[i];
                if (node.Nodes.Count > 0)
                {
                    FilterTreeView(searchString, node.Nodes);
                }
                else
                {
                    var bhvType = node.Tag as Type;
                    if (bhvType != null
                        && !Match(bhvType, searchString))
                    {
                        nodes.RemoveAt(i--);
                    }
                }
            }
        }

        private void treeViewBehaviors_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var bhvType = e.Node.Tag as Type;
            if (bhvType != null)
            {
                EditedBehaviorModel = bhvType.Assembly.CreateInstance(bhvType.FullName) as Behaviors.IBehaviorModel;
                propertyGrid1.SelectedObject = EditedBehaviorModel;
            }
        }
    }
}
