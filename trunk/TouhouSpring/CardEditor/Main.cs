using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TouhouSpring
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            m_defaultTitle = Text;

            CardModelReference.TypeConverter = new CardModelReferenceTypeConverter(() =>
            {
                return m_document == null
                       ? Enumerable.Empty<ICardModel>()
                       : m_document.Cards;
            });

            imageList.Images.Add("Card", Properties.Resources.Card_16x16);
            imageList.Images.Add("Folder_Closed", Properties.Resources.Folder_Closed_16x16);
            imageList.Images.Add("Behavior", Properties.Resources.Behavior_16x16);

            var itemList = new List<ToolStripMenuItem>();

            foreach (var t in AssemblyReflection.GetTypesDerivedFrom<Behaviors.BehaviorModel>().Where(t => !t.IsAbstract))
            {
                var bhvAttr = t.GetAttribute<Behaviors.BehaviorModelAttribute>();
                if (bhvAttr == null)
                {
                    continue;
                }

                var item = new ToolStripMenuItem(bhvAttr.Name);
                item.Tag = t;
                item.Click += new EventHandler(NewBehavior_Click);
                itemList.Add(item);
            }

            itemList.Sort((i1, i2) => String.Compare(i1.Text, i2.Text));
            toolStripDropDownButtonNewBehavior.DropDownItems.AddRange(itemList.ToArray());

            openFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
            saveFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
        }

        #region menu items

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            m_isModified = true;
            Open(new Document());
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

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        #endregion

        #region toolstrip items

        private void toolStripButtonNewCard_Click(object sender, EventArgs e)
        {
            var dlg = new NewCard(m_document);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var n = treeViewCards.SelectedNode;
                for (; n != null && n.Tag != null; n = n.Parent)
                { }
                var category = n != null ? n.FullPath : String.Empty;

                var model = new EditorCardModel
                {
                    Category = category,
                    Id = dlg.CardId,
                    Name = dlg.CardId,
                    Description = "",
                    ArtworkUri = ""
                };
                m_document.Cards.Add(model);
                AddCard(model, true);

                m_isModified = true;
                UpdateTitle();
            }
        }

        private void NewBehavior_Click(object sender, EventArgs e)
        {
            var cardNode = treeViewCards.SelectedNode.Tag is EditorCardModel
                           ? treeViewCards.SelectedNode
                           : treeViewCards.SelectedNode.Parent;
            var cardModel = cardNode.Tag as EditorCardModel;
            var bhvType = (sender as ToolStripMenuItem).Tag as Type;
            var bhv = bhvType.Assembly.CreateInstance(bhvType.FullName) as Behaviors.BehaviorModel;

            cardModel.Behaviors.Add(bhv);
            AddBehavior(bhv, cardNode, true);

            m_isModified = true;
            UpdateTitle();
        }

        private void toolStripButtonDelete_Click(object sender, EventArgs e)
        {
            var node = treeViewCards.SelectedNode;

            if (MessageBox.Show(String.Format("Delete {0} {1}?",
                                              node.Tag is EditorCardModel ? "card" : "behavior",
                                              node.Text),
                                "Card Editor", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }

            if (node.Tag is EditorCardModel)
            {
                var cardModel = node.Tag as EditorCardModel;
                m_document.Cards.Remove(cardModel);
            }
            else if (node.Tag is Behaviors.BehaviorModel)
            {
                var behaviorModel = node.Tag as Behaviors.BehaviorModel;
                (node.Parent.Tag as EditorCardModel).Behaviors.Remove(behaviorModel);
            }

            (node.Parent != null ? node.Parent.Nodes : treeViewCards.Nodes).Remove(node);
            if (treeViewCards.SelectedNode == null)
            {
                treeViewCards_AfterSelect(treeViewCards, new TreeViewEventArgs(null));
            }
            m_isModified = true;
            UpdateTitle();
        }

        #endregion

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

        private void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            if (e.ChangedItem.PropertyDescriptor.Name == "Name"
                && e.ChangedItem.PropertyDescriptor.ComponentType == typeof(CardModel))
            {
                var cardNode = treeViewCards.SelectedNode.Tag is EditorCardModel
                               ? treeViewCards.SelectedNode
                               : treeViewCards.SelectedNode.Parent;
                cardNode.Text = (string)e.ChangedItem.Value;
            }
            else if (e.ChangedItem.PropertyDescriptor.Name == "Category"
                     && e.ChangedItem.PropertyDescriptor.ComponentType == typeof(EditorCardModel))
            {
                var cardNode = treeViewCards.SelectedNode.Tag is EditorCardModel
                               ? treeViewCards.SelectedNode
                               : treeViewCards.SelectedNode.Parent;

                var oldCategory = (string)e.OldValue;
                var newCategory = (string)e.ChangedItem.Value;
                if (oldCategory != newCategory)
                {
                    MoveNode(cardNode);
                }
            }

            m_isModified = true;
            UpdateTitle();
        }
    }
}
