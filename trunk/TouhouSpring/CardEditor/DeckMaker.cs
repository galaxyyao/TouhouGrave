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
    partial class DeckMaker : Form
    {
        private const int ASSIST_MAXNUM = 3;
        private const int SAMECARD_MAXNUM = 3;
        private Deck m_currentDeck;

        public DeckMaker()
        {
            InitializeComponent();
            btnAddAssist.Enabled = false;
            btnAddCard.Enabled = false;
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        public void OpenCardLibrary(Document cardLibraryDocument)
        {
            lbAssistLibrary.Items.Clear();
            lbAssistDeck.Items.Clear();
            fileToolStripMenuItem.DropDownItems["openDeckToolStripMenuItem"].Enabled = true;
            m_document = cardLibraryDocument;
            foreach (var card in m_document.Cards)
            {
                var s = card.Behaviors;
                if (card.Behaviors.FirstOrDefault(bhv => bhv is Behaviors.Assist.ModelType) != null)
                {
                    lbAssistLibrary.Items.Add(card.Name);
                }
                else
                {
                    lbCardLibrary.Items.Add(card.Name);
                }
            }
        }

        private void openDeckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cbProfileSelector.Items.Clear();
            cbDeckSelector.Items.Clear();
            lbAssistDeck.Items.Clear();
            lbCardDeck.Items.Clear();

            Settings.Instance.LoadSettings();
            cbProfileSelector.DataSource = (from profile in Settings.Instance.MyAppSettings.Profiles.ProfileList
                                            select profile.Uid).ToList();
            cbProfileSelector.SelectedIndex = 0;
            cbDeckSelector.DataSource = (from deck in Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks
                                         select deck.Name).ToList();
            cbDeckSelector.SelectedIndex = 0;

            btnAddAssist.Enabled = true;
            btnAddCard.Enabled = true;
        }

        private void LoadDeck()
        {
            m_currentDeck = Settings.Instance.MyAppSettings.Profiles
                .ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks[cbDeckSelector.SelectedIndex];
            foreach (var cardId in m_currentDeck.DeckAssistIdList.Model)
            {
                var editorCardModel = (from card in m_document.Cards
                                 where card.Id == cardId
                                 select card).FirstOrDefault();
                lbAssistDeck.Items.Add(editorCardModel.Name);
            }
            foreach (var cardId in m_currentDeck.DeckCardIdList.Model)
            {
                var editorCardModel = (from card in m_document.Cards
                                 where card.Id == cardId
                                 select card).FirstOrDefault();
                lbCardDeck.Items.Add(editorCardModel.Name);
            }
        }

        private void cbProfileSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            cbDeckSelector.DataSource = (from deck in Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks
                                         select deck.Name).ToList();
            cbDeckSelector.SelectedIndex = 0;
        }

        private void cbDeckSelector_SelectedIndexChanged(object sender, EventArgs e)
        {
            lbAssistDeck.Items.Clear();
            lbCardDeck.Items.Clear();
            LoadDeck();
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
            EditorCardModel cardToAdd = (from card in m_document.Cards
                                         where card.Name == selectedAssist
                                         select card).FirstOrDefault();
            m_currentDeck.DeckAssistIdList.Model.Add(cardToAdd.Id);
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
            EditorCardModel cardToAdd = (from card in m_document.Cards
                                         where card.Name == selectedCard
                                         select card).FirstOrDefault();
            m_currentDeck.DeckCardIdList.Model.Add(cardToAdd.Id);
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
                    lbAssistDeck.Items.Remove(cardName);
                    ICardModel cardModelToRemove = (from card in m_currentDeck.Assists
                                                    where card.Name == cardName
                                                    select card).FirstOrDefault();
                    m_currentDeck.Assists.Remove(cardModelToRemove);
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
                    lbCardDeck.Items.Remove(cardName);
                    ICardModel cardModelToRemove = (from card in m_currentDeck.Cards
                                                    where card.Name == cardName
                                                    select card).FirstOrDefault();
                    m_currentDeck.Cards.Remove(cardModelToRemove);
                }
            }
        }

        private void btnMake1stDeck_Click(object sender, EventArgs e)
        {
            Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Deck1Id =
                Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks[cbDeckSelector.SelectedIndex].Id;
        }

        private void btnMake2ndDeck_Click(object sender, EventArgs e)
        {
            Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Deck2Id =
                Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks[cbDeckSelector.SelectedIndex].Id;
        }

        private void btnNewDeck_Click(object sender, EventArgs e)
        {
            Deck deckToAdd=new Deck();
            deckToAdd.Id = Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MaxDeckId + 1;
            deckToAdd.Name = string.Format("Deck{0}", deckToAdd.Id);
            Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks.Add(deckToAdd);
        }

        private void btnDeleteDeck_Click(object sender, EventArgs e)
        {
            if (m_currentDeck.Id == Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Deck1Id
                || m_currentDeck.Id == Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Deck2Id)
            {
                MessageBox.Show("Cannot delete 1st deck and 2nd deck");
                return;
            }
            Settings.Instance.MyAppSettings.Profiles.ProfileList[cbProfileSelector.SelectedIndex].Decks.MyDecks.Remove(m_currentDeck);
        }

        private void RefreshCardViewer(EditorCardModel cardModel)
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
            if (cardModel.Behaviors.FirstOrDefault(bhv => bhv is Behaviors.Warrior.ModelType) != null)
            {
                Behaviors.Warrior.ModelType warrior = (Behaviors.Warrior.ModelType)
                    cardModel.Behaviors.FirstOrDefault(bhv => bhv is Behaviors.Warrior.ModelType);
                lblCardAttack.Text = warrior.Attack.ToString();
                lblCardLife.Text = warrior.Life.ToString();
            }
            else
            {
                lblCardAttack.Text = string.Empty;
                lblCardLife.Text = string.Empty;
            }
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
                EditorCardModel pointedCard = (from card in m_document.Cards
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
                EditorCardModel pointedCard = (from card in m_document.Cards
                                               where card.Name == lbCardLibrary.Items[index].ToString()
                                               select card).FirstOrDefault();
                RefreshCardViewer(pointedCard);
            }
        }

        private void btnSaveDeck_Click(object sender, EventArgs e)
        {
            Settings.Instance.SaveSettings();
            MessageBox.Show("Save succeeded");
        }
    }
}
