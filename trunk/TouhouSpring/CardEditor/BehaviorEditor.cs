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
            PopulateTreeView(String.Empty);
        }

        private void textBoxSearchBox_TextChanged(object sender, EventArgs e)
        {
            PopulateTreeView(textBoxSearchBox.Text);
        }

        private void PopulateTreeView(string searchString)
        {
            treeViewBehaviors.Nodes.Clear();

            foreach (var t in m_bhvTypes)
            {
                var bhvAttr = t.GetAttribute<Behaviors.BehaviorModelAttribute>();

                if (CultureInfo.CurrentCulture.CompareInfo.IndexOf(bhvAttr.DefaultName, searchString, CompareOptions.IgnoreCase) == -1
                    && CultureInfo.CurrentCulture.CompareInfo.IndexOf(t.FullName, searchString, CompareOptions.IgnoreCase) == -1)
                {
                    continue;
                }

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

            treeViewBehaviors.ExpandAll();
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
