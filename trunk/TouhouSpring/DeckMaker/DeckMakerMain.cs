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
    public partial class DeckMakerMain : Form
    {
        private Document m_document;
        private const int ASSIST_MAXNUM = 3;
        private const int CARD_MAXNUM = 3;
        private bool m_isDeckModified = false;

        private bool IsDocumentCardLibraryOpened
        {
            get
            {
                return m_document.CardLibraryCards != null;
            }
        }

        private bool IsDocumentDeckOpened
        {
            get
            {
                return m_document.DeckCards != null;
            }
        }


        public DeckMakerMain()
        {
            InitializeComponent();
            btnAddAssist.Enabled = false;
            btnAddCard.Enabled = false;
            m_document = new Document();
            fileToolStripMenuItem.DropDownItems["openDeckToolStripMenuItem"].Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void openCardLibraryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                m_document.Open(openFileDialog.FileName, Document.DocType.CardLibrary);
                lbAssistLibrary.Items.Clear();
                lbAssistDeck.Items.Clear();
                foreach (var card in m_document.CardLibraryCards)
                {
                    if (card.CardType == DeckMaker.CardModel.CardTypeEnum.Assist)
                    {
                        lbAssistLibrary.Items.Add(card.Name);
                    }
                    else
                    {
                        lbCardLibrary.Items.Add(card.Name);
                    }
                }
            }
            fileToolStripMenuItem.DropDownItems["openDeckToolStripMenuItem"].Enabled = true;
            if (IsDocumentCardLibraryOpened && IsDocumentDeckOpened)
            {
                btnAddAssist.Enabled = true;
                btnAddCard.Enabled = true;
                fileToolStripMenuItem.DropDownItems["saveToolStripMenuItem"].Enabled = true;
            }
        }

        private void openDeckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                cbProfileSelector.Items.Clear();
                cbDeckSelector.Items.Clear();
                m_document.Open(openFileDialog.FileName, Document.DocType.Deck);
                lbAssistDeck.Items.Clear();
                lbCardDeck.Items.Clear();
                cbProfileSelector.DataSource = m_document.DeckProfileElements.Keys.ToList();
                cbProfileSelector.SelectedIndex = 0;
                m_document.LoadDeck(cbProfileSelector.SelectedValue.ToString());
                cbDeckSelector.DataSource = m_document.DeckElements.Keys.ToList();
                cbDeckSelector.SelectedIndex = 0;
                m_document.LoadDeckCards(cbDeckSelector.SelectedValue.ToString());
            }
            if (IsDocumentCardLibraryOpened && IsDocumentDeckOpened)
            {
                btnAddAssist.Enabled = true;
                btnAddCard.Enabled = true;
            }
        }

        private void cbProfileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_document.LoadDeck(cbProfileSelector.SelectedValue.ToString());
            cbDeckSelector.DataSource = m_document.DeckElements.Keys.ToList();
            cbDeckSelector.SelectedIndex = 0;
            m_document.LoadDeckCards(cbDeckSelector.SelectedValue.ToString());
        }

        private void cbDeckSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbAssistDeck.Items.Clear();
            lbCardDeck.Items.Clear();
            m_document.LoadDeckCards(cbDeckSelector.SelectedValue.ToString());
            foreach (var card in m_document.DeckCards)
            {
                if (card.CardType == DeckMaker.CardModel.CardTypeEnum.Assist)
                {
                    lbAssistDeck.Items.Add(card.Name);
                }
                else
                {
                    lbCardDeck.Items.Add(card.Name);
                }
            }
        }

        private void btnAddAssist_Click(object sender, EventArgs e)
        {
            if (lbAssistLibrary.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a card.");
                return;
            }
            string selectedAssist = lbAssistLibrary.SelectedItem.ToString();
            if (lbAssistDeck.Items.Contains(selectedAssist))
            {
                MessageBox.Show("This assist is already added.");
                return;
            }
            if (lbAssistDeck.Items.Count >= ASSIST_MAXNUM)
            {
                MessageBox.Show("Assist number has already reached maximum.");
                return;
            }
            DeckMaker.CardModel cardToAdd=(from card in m_document.CardLibraryCards
                                           where card.Name == selectedAssist
                                                        select card).FirstOrDefault();
            m_document.DeckCards.Add((DeckMaker.CardModel)cardToAdd.Clone());
            lbAssistDeck.Items.Add(cardToAdd.Name);
        }

        private void btnAddCard_Click(object sender, EventArgs e)
        {
            if (lbCardLibrary.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a card.");
                return;
            }
            string selectedCard = lbCardLibrary.SelectedItem.ToString();
            int selectedCardCount = (from cardName in lbCardDeck.Items.Cast<string>().ToList()
                                     where cardName == selectedCard
                                     select cardName).Count();
            if (selectedCardCount >= 3)
            {
                MessageBox.Show("Card number in deck has reached maximum.");
                return;
            }
            DeckMaker.CardModel cardToAdd = (from card in m_document.CardLibraryCards
                                             where card.Name == selectedCard
                                             select card).FirstOrDefault();
            m_document.DeckCards.Add((DeckMaker.CardModel)cardToAdd.Clone());
            lbCardDeck.Items.Add(cardToAdd.Name);
        }

        private void lbAssistDeck_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = lbAssistDeck.IndexFromPoint(e.Location);
                if (item >= 0)
                {
                    string cardName = lbAssistDeck.Items[item].ToString();
                    DeckMaker.CardModel cardToRemove = (from card in m_document.DeckCards
                                                        where card.Name == cardName
                                                        select card).FirstOrDefault();
                    m_document.DeckCards.Remove(cardToRemove);
                    lbAssistDeck.Items.Remove(cardName);
                }
            }
        }

        private void lbCardDeck_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var item = lbCardDeck.IndexFromPoint(e.Location);
                if (item >= 0)
                {
                    string cardName = lbCardDeck.Items[item].ToString();
                    DeckMaker.CardModel cardToRemove = (from card in m_document.DeckCards
                                                        where card.Name == cardName
                                                        select card).FirstOrDefault();
                    m_document.DeckCards.Remove(cardToRemove);
                    lbCardDeck.Items.Remove(cardName);
                }
            }
        }

        private void btnMake1stDeck_Click(object sender, EventArgs e)
        {

        }

        private void btnMake2ndDeck_Click(object sender, EventArgs e)
        {

        }

        private void btnNewDeck_Click(object sender, EventArgs e)
        {

        }

        private void btnDeleteDeck_Click(object sender, EventArgs e)
        {

        }

        private void RefreshCardViewer(DeckMaker.CardModel cardModel)
        {
            if (cardModel == null)
            {
                lblCardId.Text = string.Empty;
                lblCardName.Text = string.Empty;
                txtCardDescription.Text = string.Empty;
                lblCardAttack.Text = string.Empty;
                lblCardLife.Text = string.Empty;
                return;
            }
            lblCardId.Text = cardModel.Id;
            lblCardName.Text = cardModel.Name;
            txtCardDescription.Text = cardModel.Description;
            lblCardAttack.Text = cardModel.Attack.ToString();
            lblCardLife.Text = cardModel.Life.ToString();
        }

        private void lbAssistLibrary_MouseMove(object sender, MouseEventArgs e)
        {
            System.Drawing.Point point = lbAssistLibrary.PointToClient(Cursor.Position);
            int index = lbAssistLibrary.IndexFromPoint(point);
            if (index < 0)
            {
                RefreshCardViewer(null);
            }
            else
            {
                DeckMaker.CardModel pointedCard = (from card in m_document.CardLibraryCards
                                                   where card.Name == lbAssistLibrary.Items[index].ToString()
                                                   select card).FirstOrDefault();
                RefreshCardViewer(pointedCard);
            }
        }

        private void lbCardLibrary_MouseMove(object sender, MouseEventArgs e)
        {
            System.Drawing.Point point = lbCardLibrary.PointToClient(Cursor.Position);
            int index = lbCardLibrary.IndexFromPoint(point);
            if (index < 0)
            {
                RefreshCardViewer(null);
            }
            else
            {
                DeckMaker.CardModel pointedCard = (from card in m_document.CardLibraryCards
                                                   where card.Name == lbCardLibrary.Items[index].ToString()
                                                   select card).FirstOrDefault();
                RefreshCardViewer(pointedCard);
            }
        }

        private void btnSaveDeck_Click(object sender, EventArgs e)
        {
            m_document.Save(cbProfileSelector.SelectedItem.ToString(), cbDeckSelector.SelectedItem.ToString());
        }
    }
}
