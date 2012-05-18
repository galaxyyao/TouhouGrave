namespace TouhouSpring.TextureAtlas
{
	partial class Main
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
			System.Windows.Forms.SplitContainer splitContainer1;
			System.Windows.Forms.SplitContainer splitContainer2;
			System.Windows.Forms.MenuStrip mainMenuStrip;
			System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
			System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
			System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.treeViewSubTextures = new System.Windows.Forms.TreeView();
			this.toolStripSubTextures = new System.Windows.Forms.ToolStrip();
			this.toolStripSubTextureAdd = new System.Windows.Forms.ToolStripButton();
			this.toolStripSubTextureRemove = new System.Windows.Forms.ToolStripButton();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.propertiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			this.addTextureDialog = new System.Windows.Forms.OpenFileDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.packToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			splitContainer2 = new System.Windows.Forms.SplitContainer();
			mainMenuStrip = new System.Windows.Forms.MenuStrip();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
			exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			this.toolStripSubTextures.SuspendLayout();
			mainMenuStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 25);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.AutoScroll = true;
			splitContainer1.Panel1.Controls.Add(this.pictureBox);
			splitContainer1.Panel1.SizeChanged += new System.EventHandler(this.LeftPanel_SizeChanged);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Size = new System.Drawing.Size(608, 373);
			splitContainer1.SplitterDistance = 309;
			splitContainer1.TabIndex = 0;
			splitContainer1.TabStop = false;
			// 
			// pictureBox
			// 
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(0, 0);
			this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox.TabIndex = 0;
			this.pictureBox.TabStop = false;
			// 
			// splitContainer2
			// 
			splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer2.Location = new System.Drawing.Point(0, 0);
			splitContainer2.Name = "splitContainer2";
			splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer2.Panel1
			// 
			splitContainer2.Panel1.Controls.Add(this.treeViewSubTextures);
			splitContainer2.Panel1.Controls.Add(this.toolStripSubTextures);
			// 
			// splitContainer2.Panel2
			// 
			splitContainer2.Panel2.Controls.Add(this.propertyGrid);
			splitContainer2.Size = new System.Drawing.Size(293, 371);
			splitContainer2.SplitterDistance = 150;
			splitContainer2.TabIndex = 0;
			splitContainer2.TabStop = false;
			// 
			// treeViewSubTextures
			// 
			this.treeViewSubTextures.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeViewSubTextures.Enabled = false;
			this.treeViewSubTextures.HideSelection = false;
			this.treeViewSubTextures.Location = new System.Drawing.Point(0, 25);
			this.treeViewSubTextures.Name = "treeViewSubTextures";
			this.treeViewSubTextures.ShowRootLines = false;
			this.treeViewSubTextures.Size = new System.Drawing.Size(293, 125);
			this.treeViewSubTextures.TabIndex = 0;
			this.treeViewSubTextures.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewSubTextures_AfterSelect);
			this.treeViewSubTextures.MouseUp += new System.Windows.Forms.MouseEventHandler(this.treeViewSubTextures_MouseUp);
			// 
			// toolStripSubTextures
			// 
			this.toolStripSubTextures.Enabled = false;
			this.toolStripSubTextures.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripSubTextureAdd,
            this.toolStripSubTextureRemove});
			this.toolStripSubTextures.Location = new System.Drawing.Point(0, 0);
			this.toolStripSubTextures.Name = "toolStripSubTextures";
			this.toolStripSubTextures.Size = new System.Drawing.Size(293, 25);
			this.toolStripSubTextures.TabIndex = 1;
			// 
			// toolStripSubTextureAdd
			// 
			this.toolStripSubTextureAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripSubTextureAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSubTextureAdd.Name = "toolStripSubTextureAdd";
			this.toolStripSubTextureAdd.Size = new System.Drawing.Size(45, 22);
			this.toolStripSubTextureAdd.Text = "Add...";
			this.toolStripSubTextureAdd.Click += new System.EventHandler(this.toolStripSubTextureAdd_Click);
			// 
			// toolStripSubTextureRemove
			// 
			this.toolStripSubTextureRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
			this.toolStripSubTextureRemove.Enabled = false;
			this.toolStripSubTextureRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolStripSubTextureRemove.Name = "toolStripSubTextureRemove";
			this.toolStripSubTextureRemove.Size = new System.Drawing.Size(59, 22);
			this.toolStripSubTextureRemove.Text = "Remove";
			this.toolStripSubTextureRemove.Click += new System.EventHandler(this.toolStripSubTextureRemove_Click);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Enabled = false;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(293, 217);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// mainMenuStrip
			// 
			mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem,
            editToolStripMenuItem});
			mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			mainMenuStrip.Name = "mainMenuStrip";
			mainMenuStrip.Size = new System.Drawing.Size(608, 25);
			mainMenuStrip.TabIndex = 1;
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            newToolStripMenuItem,
            openToolStripMenuItem,
            toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            toolStripMenuItem2,
            this.propertiesToolStripMenuItem,
            toolStripMenuItem3,
            exitToolStripMenuItem});
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			newToolStripMenuItem.Text = "&New";
			newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			openToolStripMenuItem.Text = "&Open...";
			openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new System.Drawing.Size(142, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Enabled = false;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As...";
			// 
			// toolStripMenuItem2
			// 
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new System.Drawing.Size(142, 6);
			// 
			// propertiesToolStripMenuItem
			// 
			this.propertiesToolStripMenuItem.Enabled = false;
			this.propertiesToolStripMenuItem.Name = "propertiesToolStripMenuItem";
			this.propertiesToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			this.propertiesToolStripMenuItem.Text = "&Properties...";
			this.propertiesToolStripMenuItem.Click += new System.EventHandler(this.propertiesToolStripMenuItem_Click);
			// 
			// toolStripMenuItem3
			// 
			toolStripMenuItem3.Name = "toolStripMenuItem3";
			toolStripMenuItem3.Size = new System.Drawing.Size(142, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.Size = new System.Drawing.Size(145, 22);
			exitToolStripMenuItem.Text = "E&xit";
			exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "Texture Atlas files (*.xml)|*.xml";
			// 
			// addTextureDialog
			// 
			this.addTextureDialog.Filter = "Texture file (*.png)|*.png";
			this.addTextureDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.addTextureDialog_FileOk);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "Texture Atlas files (*.xml)|*.xml";
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.packToolStripMenuItem});
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new System.Drawing.Size(42, 21);
			editToolStripMenuItem.Text = "&Edit";
			// 
			// packToolStripMenuItem
			// 
			this.packToolStripMenuItem.Enabled = false;
			this.packToolStripMenuItem.Name = "packToolStripMenuItem";
			this.packToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
			this.packToolStripMenuItem.Text = "&Pack";
			this.packToolStripMenuItem.Click += new System.EventHandler(this.packToolStripMenuItem_Click);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(608, 398);
			this.Controls.Add(splitContainer1);
			this.Controls.Add(mainMenuStrip);
			this.MainMenuStrip = mainMenuStrip;
			this.Name = "Main";
			this.Text = "Texture Atlas Editor";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel1.PerformLayout();
			splitContainer2.Panel2.ResumeLayout(false);
			splitContainer2.ResumeLayout(false);
			this.toolStripSubTextures.ResumeLayout(false);
			this.toolStripSubTextures.PerformLayout();
			mainMenuStrip.ResumeLayout(false);
			mainMenuStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.TreeView treeViewSubTextures;
		private System.Windows.Forms.OpenFileDialog addTextureDialog;
		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem propertiesToolStripMenuItem;
		private System.Windows.Forms.ToolStrip toolStripSubTextures;
		private System.Windows.Forms.ToolStripButton toolStripSubTextureAdd;
		private System.Windows.Forms.ToolStripButton toolStripSubTextureRemove;
		private System.Windows.Forms.ToolStripMenuItem packToolStripMenuItem;

	}
}

