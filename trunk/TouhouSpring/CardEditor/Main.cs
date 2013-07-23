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
        private List<Type> m_behaviorModelTypes = new List<Type>();

        public Main()
        {
            InitializeComponent();
            m_defaultTitle = Text;

            Font = SystemFonts.MessageBoxFont;

            foreach (var t in AssemblyReflection.GetTypesImplements<Behaviors.IBehaviorModel>().Where(t => !t.IsAbstract))
            {
                var bhvAttr = t.GetAttribute<Behaviors.BehaviorModelAttribute>();
                if (bhvAttr == null || bhvAttr.HideFromEditor)
                {
                    continue;
                }

                m_behaviorModelTypes.Add(t);
            }

            m_behaviorModelTypes.Sort((t1, t2) =>
            {
                var bhvAttr1 = t1.GetAttribute<Behaviors.BehaviorModelAttribute>();
                var bhvAttr2 = t2.GetAttribute<Behaviors.BehaviorModelAttribute>();
                if (bhvAttr1.Category != bhvAttr2.Category)
                {
                    return String.Compare(bhvAttr1.Category, bhvAttr2.Category, false, System.Globalization.CultureInfo.CurrentCulture);
                }
                return String.Compare(bhvAttr1.DefaultName, bhvAttr2.DefaultName, false, System.Globalization.CultureInfo.CurrentCulture);
            });

            CardModelReferenceTypeConverter.CardModelIterator = () => m_document == null ? Enumerable.Empty<ICardModel>() : m_document.Cards;
            BehaviorEditor.BehaviorTypes = m_behaviorModelTypes;

            TypeDescriptor.AddAttributes(
                typeof(CardModelReference),
                new TypeConverterAttribute(typeof(CardModelReferenceTypeConverter)));

            TypeDescriptor.AddAttributes(
                typeof(BehaviorModelReference),
                new TypeConverterAttribute(typeof(BehaviorModelReferenceEditor.CustomTypeConverter)));

            TypeDescriptor.AddAttributes(
                typeof(BehaviorModelReference),
                new EditorAttribute(typeof(BehaviorModelReferenceEditor.TypeEditor), typeof(System.Drawing.Design.UITypeEditor)));

            imageList.Images.Add("Card", Properties.Resources.Card_16x16);
            imageList.Images.Add("Folder_Closed", Properties.Resources.Folder_Closed_16x16);
            imageList.Images.Add("Behavior", Properties.Resources.Behavior_16x16);

            openFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
            saveFileDialog.InitialDirectory = Program.PathUtils.ContentRootDirectory;
        }

        private void AddBehaviorToCurrentCardNode(Behaviors.IBehaviorModel bhvModel)
        {
            var cardNode = treeViewCards.SelectedNode.Tag is EditorCardModel
                           ? treeViewCards.SelectedNode
                           : treeViewCards.SelectedNode.Parent;
            var cardModel = cardNode.Tag as EditorCardModel;

            cardModel.Behaviors.Add(bhvModel);
            AddBehavior(bhvModel, cardNode, true);

            m_isModified = true;
            UpdateTitle();
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
            var bhvEditor = new BehaviorEditor(false, null);
            if (bhvEditor.ShowDialog() == System.Windows.Forms.DialogResult.OK
                && bhvEditor.EditedBehaviorModel != null)
            {
                AddBehaviorToCurrentCardNode(bhvEditor.EditedBehaviorModel);
            }
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
            else if (node.Tag is Behaviors.IBehaviorModel)
            {
                var behaviorModel = node.Tag as Behaviors.IBehaviorModel;
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
            else if (e.ChangedItem.PropertyDescriptor.Name == "Name"
                     && e.ChangedItem.PropertyDescriptor.ComponentType == typeof(Behaviors.IBehaviorModel))
            {
                var bhvNode = treeViewCards.SelectedNode;
                bhvNode.Text = (string)e.ChangedItem.Value;
            }

            m_isModified = true;
            UpdateTitle();
        }

        private void toolStripButtonMakeDeck_Click(object sender, EventArgs e)
        {
            if (m_deckMakerForm == null)
                m_deckMakerForm = new DeckMaker();
            m_deckMakerForm.OpenCardLibrary(m_document);
            m_deckMakerForm.Show();
        }
    }
}
