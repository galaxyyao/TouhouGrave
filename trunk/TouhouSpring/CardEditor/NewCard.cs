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
    partial class NewCard : Form
    {
        private Document m_document;

        public string CardId
        {
            get { return textBoxId.Text; }
            set { textBoxId.Text = value; }
        }

        public NewCard(Document document)
        {
            if (document == null)
            {
                throw new ArgumentNullException("document");
            }

            InitializeComponent();
            m_document = document;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();

            if (textBoxId.Text == String.Empty)
            {
                errorProvider.SetError(textBoxId, "Can't be empty.");
                DialogResult = DialogResult.None;
            }
            else if (m_document.Cards.Any(m => m.Id == textBoxId.Text))
            {
                errorProvider.SetError(textBoxId, String.Format("Id '{0}' is taken.", textBoxId.Text));
                DialogResult = DialogResult.None;
            }
        }
    }
}
