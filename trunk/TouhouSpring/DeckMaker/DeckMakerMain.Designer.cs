namespace TouhouSpring
{
    partial class DeckMakerMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeckMakerMain));
            this.lbCardLibrary = new System.Windows.Forms.ListBox();
            this.lbCardDeck = new System.Windows.Forms.ListBox();
            this.btnAddAssist = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openCardLibraryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openDeckToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lbAssistLibrary = new System.Windows.Forms.ListBox();
            this.lbAssistDeck = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddCard = new System.Windows.Forms.Button();
            this.cbDeckSelector = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnMake1stDeck = new System.Windows.Forms.Button();
            this.btnMake2ndDeck = new System.Windows.Forms.Button();
            this.btnDeleteDeck = new System.Windows.Forms.Button();
            this.btnNewDeck = new System.Windows.Forms.Button();
            this.cbProfileSelector = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.lblCardId = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCardName = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCardDescription = new System.Windows.Forms.TextBox();
            this.lblAttackText = new System.Windows.Forms.Label();
            this.lblCardAttack = new System.Windows.Forms.Label();
            this.Defense = new System.Windows.Forms.Label();
            this.lblCardLife = new System.Windows.Forms.Label();
            this.btnSaveDeck = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCardLibrary
            // 
            this.lbCardLibrary.FormattingEnabled = true;
            this.lbCardLibrary.Location = new System.Drawing.Point(365, 459);
            this.lbCardLibrary.Name = "lbCardLibrary";
            this.lbCardLibrary.Size = new System.Drawing.Size(268, 225);
            this.lbCardLibrary.TabIndex = 0;
            this.lbCardLibrary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbCardLibrary_MouseMove);
            // 
            // lbCardDeck
            // 
            this.lbCardDeck.FormattingEnabled = true;
            this.lbCardDeck.Location = new System.Drawing.Point(752, 459);
            this.lbCardDeck.Name = "lbCardDeck";
            this.lbCardDeck.Size = new System.Drawing.Size(268, 225);
            this.lbCardDeck.TabIndex = 0;
            this.lbCardDeck.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbCardDeck_MouseUp);
            // 
            // btnAddAssist
            // 
            this.btnAddAssist.Image = ((System.Drawing.Image)(resources.GetObject("btnAddAssist.Image")));
            this.btnAddAssist.Location = new System.Drawing.Point(669, 266);
            this.btnAddAssist.Name = "btnAddAssist";
            this.btnAddAssist.Size = new System.Drawing.Size(49, 49);
            this.btnAddAssist.TabIndex = 1;
            this.btnAddAssist.UseVisualStyleBackColor = true;
            this.btnAddAssist.Click += new System.EventHandler(this.btnAddAssist_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1059, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openCardLibraryToolStripMenuItem,
            this.openDeckToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openCardLibraryToolStripMenuItem
            // 
            this.openCardLibraryToolStripMenuItem.Name = "openCardLibraryToolStripMenuItem";
            this.openCardLibraryToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openCardLibraryToolStripMenuItem.Text = "Open Card Library";
            this.openCardLibraryToolStripMenuItem.Click += new System.EventHandler(this.openCardLibraryToolStripMenuItem_Click);
            // 
            // openDeckToolStripMenuItem
            // 
            this.openDeckToolStripMenuItem.Name = "openDeckToolStripMenuItem";
            this.openDeckToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.openDeckToolStripMenuItem.Text = "Open Deck";
            this.openDeckToolStripMenuItem.Click += new System.EventHandler(this.openDeckToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // lbAssistLibrary
            // 
            this.lbAssistLibrary.FormattingEnabled = true;
            this.lbAssistLibrary.Location = new System.Drawing.Point(365, 177);
            this.lbAssistLibrary.Name = "lbAssistLibrary";
            this.lbAssistLibrary.Size = new System.Drawing.Size(268, 225);
            this.lbAssistLibrary.TabIndex = 0;
            this.lbAssistLibrary.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lbAssistLibrary_MouseMove);
            // 
            // lbAssistDeck
            // 
            this.lbAssistDeck.FormattingEnabled = true;
            this.lbAssistDeck.Location = new System.Drawing.Point(752, 177);
            this.lbAssistDeck.Name = "lbAssistDeck";
            this.lbAssistDeck.Size = new System.Drawing.Size(268, 225);
            this.lbAssistDeck.TabIndex = 0;
            this.lbAssistDeck.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lbAssistDeck_MouseUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(362, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(37, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Assist:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(362, 427);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(91, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Warrior and Spell:";
            // 
            // btnAddCard
            // 
            this.btnAddCard.Image = ((System.Drawing.Image)(resources.GetObject("btnAddCard.Image")));
            this.btnAddCard.Location = new System.Drawing.Point(669, 555);
            this.btnAddCard.Name = "btnAddCard";
            this.btnAddCard.Size = new System.Drawing.Size(49, 49);
            this.btnAddCard.TabIndex = 1;
            this.btnAddCard.UseVisualStyleBackColor = true;
            this.btnAddCard.Click += new System.EventHandler(this.btnAddCard_Click);
            // 
            // cbDeckSelector
            // 
            this.cbDeckSelector.FormattingEnabled = true;
            this.cbDeckSelector.Location = new System.Drawing.Point(798, 86);
            this.cbDeckSelector.Name = "cbDeckSelector";
            this.cbDeckSelector.Size = new System.Drawing.Size(121, 21);
            this.cbDeckSelector.TabIndex = 5;
            this.cbDeckSelector.SelectedIndexChanged += new System.EventHandler(this.cbDeckSelector_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(723, 89);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Select Deck:";
            // 
            // btnMake1stDeck
            // 
            this.btnMake1stDeck.Location = new System.Drawing.Point(752, 118);
            this.btnMake1stDeck.Name = "btnMake1stDeck";
            this.btnMake1stDeck.Size = new System.Drawing.Size(120, 23);
            this.btnMake1stDeck.TabIndex = 7;
            this.btnMake1stDeck.Text = "Make 1st Deck";
            this.btnMake1stDeck.UseVisualStyleBackColor = true;
            this.btnMake1stDeck.Click += new System.EventHandler(this.btnMake1stDeck_Click);
            // 
            // btnMake2ndDeck
            // 
            this.btnMake2ndDeck.Location = new System.Drawing.Point(900, 118);
            this.btnMake2ndDeck.Name = "btnMake2ndDeck";
            this.btnMake2ndDeck.Size = new System.Drawing.Size(120, 23);
            this.btnMake2ndDeck.TabIndex = 7;
            this.btnMake2ndDeck.Text = "Be 2nd Deck";
            this.btnMake2ndDeck.UseVisualStyleBackColor = true;
            this.btnMake2ndDeck.Click += new System.EventHandler(this.btnMake2ndDeck_Click);
            // 
            // btnDeleteDeck
            // 
            this.btnDeleteDeck.Location = new System.Drawing.Point(900, 148);
            this.btnDeleteDeck.Name = "btnDeleteDeck";
            this.btnDeleteDeck.Size = new System.Drawing.Size(120, 23);
            this.btnDeleteDeck.TabIndex = 7;
            this.btnDeleteDeck.Text = "Delete Deck";
            this.btnDeleteDeck.UseVisualStyleBackColor = true;
            this.btnDeleteDeck.Click += new System.EventHandler(this.btnDeleteDeck_Click);
            // 
            // btnNewDeck
            // 
            this.btnNewDeck.Location = new System.Drawing.Point(752, 147);
            this.btnNewDeck.Name = "btnNewDeck";
            this.btnNewDeck.Size = new System.Drawing.Size(120, 23);
            this.btnNewDeck.TabIndex = 7;
            this.btnNewDeck.Text = "New Deck";
            this.btnNewDeck.UseVisualStyleBackColor = true;
            this.btnNewDeck.Click += new System.EventHandler(this.btnNewDeck_Click);
            // 
            // cbProfileSelector
            // 
            this.cbProfileSelector.FormattingEnabled = true;
            this.cbProfileSelector.Location = new System.Drawing.Point(798, 52);
            this.cbProfileSelector.Name = "cbProfileSelector";
            this.cbProfileSelector.Size = new System.Drawing.Size(121, 21);
            this.cbProfileSelector.TabIndex = 5;
            this.cbProfileSelector.SelectedIndexChanged += new System.EventHandler(this.cbProfileSelector_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(720, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Select Profile:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 52);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Id:";
            // 
            // lblCardId
            // 
            this.lblCardId.AutoSize = true;
            this.lblCardId.Location = new System.Drawing.Point(74, 52);
            this.lblCardId.Name = "lblCardId";
            this.lblCardId.Size = new System.Drawing.Size(0, 13);
            this.lblCardId.TabIndex = 9;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(13, 86);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Name:";
            // 
            // lblCardName
            // 
            this.lblCardName.AutoSize = true;
            this.lblCardName.Location = new System.Drawing.Point(74, 86);
            this.lblCardName.Name = "lblCardName";
            this.lblCardName.Size = new System.Drawing.Size(0, 13);
            this.lblCardName.TabIndex = 11;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(13, 177);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(60, 13);
            this.label9.TabIndex = 12;
            this.label9.Text = "Description";
            // 
            // txtCardDescription
            // 
            this.txtCardDescription.Enabled = false;
            this.txtCardDescription.Location = new System.Drawing.Point(77, 177);
            this.txtCardDescription.Multiline = true;
            this.txtCardDescription.Name = "txtCardDescription";
            this.txtCardDescription.Size = new System.Drawing.Size(208, 127);
            this.txtCardDescription.TabIndex = 13;
            // 
            // lblAttackText
            // 
            this.lblAttackText.AutoSize = true;
            this.lblAttackText.Location = new System.Drawing.Point(10, 330);
            this.lblAttackText.Name = "lblAttackText";
            this.lblAttackText.Size = new System.Drawing.Size(38, 13);
            this.lblAttackText.TabIndex = 14;
            this.lblAttackText.Text = "Attack";
            // 
            // lblCardAttack
            // 
            this.lblCardAttack.AutoSize = true;
            this.lblCardAttack.Location = new System.Drawing.Point(74, 330);
            this.lblCardAttack.Name = "lblCardAttack";
            this.lblCardAttack.Size = new System.Drawing.Size(0, 13);
            this.lblCardAttack.TabIndex = 15;
            // 
            // Defense
            // 
            this.Defense.AutoSize = true;
            this.Defense.Location = new System.Drawing.Point(10, 357);
            this.Defense.Name = "Defense";
            this.Defense.Size = new System.Drawing.Size(24, 13);
            this.Defense.TabIndex = 16;
            this.Defense.Text = "Life";
            // 
            // lblCardLife
            // 
            this.lblCardLife.AutoSize = true;
            this.lblCardLife.Location = new System.Drawing.Point(74, 357);
            this.lblCardLife.Name = "lblCardLife";
            this.lblCardLife.Size = new System.Drawing.Size(0, 13);
            this.lblCardLife.TabIndex = 17;
            // 
            // btnSaveDeck
            // 
            this.btnSaveDeck.Location = new System.Drawing.Point(938, 52);
            this.btnSaveDeck.Name = "btnSaveDeck";
            this.btnSaveDeck.Size = new System.Drawing.Size(82, 55);
            this.btnSaveDeck.TabIndex = 18;
            this.btnSaveDeck.Text = "Save";
            this.btnSaveDeck.UseVisualStyleBackColor = true;
            this.btnSaveDeck.Click += new System.EventHandler(this.btnSaveDeck_Click);
            // 
            // DeckMakerMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1059, 696);
            this.Controls.Add(this.btnSaveDeck);
            this.Controls.Add(this.lblCardLife);
            this.Controls.Add(this.Defense);
            this.Controls.Add(this.lblCardAttack);
            this.Controls.Add(this.lblAttackText);
            this.Controls.Add(this.txtCardDescription);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.lblCardName);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.lblCardId);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.btnNewDeck);
            this.Controls.Add(this.btnDeleteDeck);
            this.Controls.Add(this.btnMake2ndDeck);
            this.Controls.Add(this.btnMake1stDeck);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cbProfileSelector);
            this.Controls.Add(this.cbDeckSelector);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnAddCard);
            this.Controls.Add(this.btnAddAssist);
            this.Controls.Add(this.lbCardDeck);
            this.Controls.Add(this.lbAssistDeck);
            this.Controls.Add(this.lbAssistLibrary);
            this.Controls.Add(this.lbCardLibrary);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "DeckMakerMain";
            this.Text = "Form1";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbCardLibrary;
        private System.Windows.Forms.ListBox lbCardDeck;
        private System.Windows.Forms.Button btnAddAssist;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openCardLibraryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openDeckToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ListBox lbAssistLibrary;
        private System.Windows.Forms.ListBox lbAssistDeck;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddCard;
        private System.Windows.Forms.ComboBox cbDeckSelector;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnMake1stDeck;
        private System.Windows.Forms.Button btnMake2ndDeck;
        private System.Windows.Forms.Button btnDeleteDeck;
        private System.Windows.Forms.Button btnNewDeck;
        private System.Windows.Forms.ComboBox cbProfileSelector;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label lblCardId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblCardName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtCardDescription;
        private System.Windows.Forms.Label lblAttackText;
        private System.Windows.Forms.Label lblCardAttack;
        private System.Windows.Forms.Label Defense;
        private System.Windows.Forms.Label lblCardLife;
        private System.Windows.Forms.Button btnSaveDeck;
    }
}

