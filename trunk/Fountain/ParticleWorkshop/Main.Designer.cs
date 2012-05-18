namespace TouhouSpring.Particle
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
			this.components = new System.ComponentModel.Container();
			System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
			System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
			System.Windows.Forms.SplitContainer splitContainer1;
			System.Windows.Forms.StatusStrip statusStrip1;
			System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
			System.Windows.Forms.SplitContainer splitContainer2;
			System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
			System.Windows.Forms.MenuStrip menuStrip1;
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.canvas = new TouhouSpring.Particle.Canvas();
			this.toolStripStatusLabelNumParticles = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabelFps = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusColor = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonPlayPause = new System.Windows.Forms.ToolStripButton();
			this.treeView = new System.Windows.Forms.TreeView();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.toolStripButtonNewEffect = new System.Windows.Forms.ToolStripButton();
			this.toolStripDropDownButtonNewModifier = new System.Windows.Forms.ToolStripDropDownButton();
			this.toolStripButtonDelete = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMoveUp = new System.Windows.Forms.ToolStripButton();
			this.toolStripButtonMoveDown = new System.Windows.Forms.ToolStripButton();
			this.propertyGrid = new System.Windows.Forms.PropertyGrid();
			this.timerFps = new System.Windows.Forms.Timer(this.components);
			this.colorDialog = new System.Windows.Forms.ColorDialog();
			this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
			this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
			fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
			toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
			splitContainer1 = new System.Windows.Forms.SplitContainer();
			statusStrip1 = new System.Windows.Forms.StatusStrip();
			toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			splitContainer2 = new System.Windows.Forms.SplitContainer();
			toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			menuStrip1 = new System.Windows.Forms.MenuStrip();
			((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			statusStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(splitContainer2)).BeginInit();
			splitContainer2.Panel1.SuspendLayout();
			splitContainer2.Panel2.SuspendLayout();
			splitContainer2.SuspendLayout();
			this.toolStrip.SuspendLayout();
			menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            toolStripMenuItem1,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            toolStripMenuItem2,
            this.exitToolStripMenuItem});
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
			fileToolStripMenuItem.Text = "&File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.newToolStripMenuItem.Text = "&New";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.openToolStripMenuItem.Text = "&Open";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// toolStripMenuItem1
			// 
			toolStripMenuItem1.Name = "toolStripMenuItem1";
			toolStripMenuItem1.Size = new System.Drawing.Size(118, 6);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Enabled = false;
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.saveToolStripMenuItem.Text = "&Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Enabled = false;
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.saveAsToolStripMenuItem.Text = "Save &As";
			// 
			// toolStripMenuItem2
			// 
			toolStripMenuItem2.Name = "toolStripMenuItem2";
			toolStripMenuItem2.Size = new System.Drawing.Size(118, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
			this.exitToolStripMenuItem.Text = "E&xit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			splitContainer1.Location = new System.Drawing.Point(0, 25);
			splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(this.canvas);
			splitContainer1.Panel1.Controls.Add(statusStrip1);
			splitContainer1.Panel1.Controls.Add(this.toolStrip1);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(splitContainer2);
			splitContainer1.Size = new System.Drawing.Size(716, 463);
			splitContainer1.SplitterDistance = 510;
			splitContainer1.TabIndex = 1;
			// 
			// canvas
			// 
			this.canvas.ClearColor = new Microsoft.Xna.Framework.Color(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.canvas.Dock = System.Windows.Forms.DockStyle.Fill;
			this.canvas.GridColor = new Microsoft.Xna.Framework.Color(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.canvas.Location = new System.Drawing.Point(0, 25);
			this.canvas.Name = "canvas";
			this.canvas.Size = new System.Drawing.Size(510, 416);
			this.canvas.System = null;
			this.canvas.TabIndex = 0;
			this.canvas.Text = "canvas1";
			// 
			// statusStrip1
			// 
			statusStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Visible;
			statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelNumParticles,
            this.toolStripStatusLabelFps,
            toolStripStatusLabel1,
            this.toolStripStatusColor});
			statusStrip1.Location = new System.Drawing.Point(0, 441);
			statusStrip1.Name = "statusStrip1";
			statusStrip1.Size = new System.Drawing.Size(510, 22);
			statusStrip1.SizingGrip = false;
			statusStrip1.TabIndex = 1;
			statusStrip1.Text = "statusStrip1";
			// 
			// toolStripStatusLabelNumParticles
			// 
			this.toolStripStatusLabelNumParticles.Name = "toolStripStatusLabelNumParticles";
			this.toolStripStatusLabelNumParticles.Size = new System.Drawing.Size(85, 17);
			this.toolStripStatusLabelNumParticles.Text = "Nb Particles :";
			// 
			// toolStripStatusLabelFps
			// 
			this.toolStripStatusLabelFps.Name = "toolStripStatusLabelFps";
			this.toolStripStatusLabelFps.Size = new System.Drawing.Size(31, 17);
			this.toolStripStatusLabelFps.Text = "FPS:";
			// 
			// toolStripStatusLabel1
			// 
			toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			toolStripStatusLabel1.Size = new System.Drawing.Size(82, 17);
			toolStripStatusLabel1.Text = "Background:";
			// 
			// toolStripStatusColor
			// 
			this.toolStripStatusColor.BackColor = System.Drawing.Color.Black;
			this.toolStripStatusColor.Name = "toolStripStatusColor";
			this.toolStripStatusColor.Size = new System.Drawing.Size(24, 17);
			this.toolStripStatusColor.Text = "    ";
			this.toolStripStatusColor.Click += new System.EventHandler(this.toolStripStatusColor_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonPlayPause});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(510, 25);
			this.toolStrip1.TabIndex = 2;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// toolStripButtonPlayPause
			// 
			this.toolStripButtonPlayPause.Checked = true;
			this.toolStripButtonPlayPause.CheckOnClick = true;
			this.toolStripButtonPlayPause.CheckState = System.Windows.Forms.CheckState.Checked;
			this.toolStripButtonPlayPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonPlayPause.Image = global::TouhouSpring.Particle.Properties.Resources.Pause_16x16;
			this.toolStripButtonPlayPause.Name = "toolStripButtonPlayPause";
			this.toolStripButtonPlayPause.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonPlayPause.Click += new System.EventHandler(this.toolStripButtonPlayPause_Click);
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
			splitContainer2.Panel1.Controls.Add(this.treeView);
			splitContainer2.Panel1.Controls.Add(this.toolStrip);
			// 
			// splitContainer2.Panel2
			// 
			splitContainer2.Panel2.Controls.Add(this.propertyGrid);
			splitContainer2.Size = new System.Drawing.Size(202, 463);
			splitContainer2.SplitterDistance = 158;
			splitContainer2.TabIndex = 0;
			// 
			// treeView
			// 
			this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView.Enabled = false;
			this.treeView.HideSelection = false;
			this.treeView.Location = new System.Drawing.Point(0, 25);
			this.treeView.Name = "treeView";
			this.treeView.ShowRootLines = false;
			this.treeView.Size = new System.Drawing.Size(202, 133);
			this.treeView.TabIndex = 0;
			this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
			// 
			// toolStrip
			// 
			this.toolStrip.Enabled = false;
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButtonNewEffect,
            this.toolStripDropDownButtonNewModifier,
            toolStripSeparator1,
            this.toolStripButtonDelete,
            this.toolStripButtonMoveUp,
            this.toolStripButtonMoveDown});
			this.toolStrip.Location = new System.Drawing.Point(0, 0);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(202, 25);
			this.toolStrip.TabIndex = 1;
			this.toolStrip.Text = "toolStrip1";
			// 
			// toolStripButtonNewEffect
			// 
			this.toolStripButtonNewEffect.Image = global::TouhouSpring.Particle.Properties.Resources.Add_16x16;
			this.toolStripButtonNewEffect.Name = "toolStripButtonNewEffect";
			this.toolStripButtonNewEffect.Size = new System.Drawing.Size(90, 22);
			this.toolStripButtonNewEffect.Text = "New Effect";
			this.toolStripButtonNewEffect.Click += new System.EventHandler(this.toolStripButtonNewEffect_Click);
			// 
			// toolStripDropDownButtonNewModifier
			// 
			this.toolStripDropDownButtonNewModifier.Image = global::TouhouSpring.Particle.Properties.Resources.Add_16x16;
			this.toolStripDropDownButtonNewModifier.Name = "toolStripDropDownButtonNewModifier";
			this.toolStripDropDownButtonNewModifier.Size = new System.Drawing.Size(117, 21);
			this.toolStripDropDownButtonNewModifier.Text = "New Modifier";
			this.toolStripDropDownButtonNewModifier.Visible = false;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolStripButtonDelete
			// 
			this.toolStripButtonDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonDelete.Image = global::TouhouSpring.Particle.Properties.Resources.Delete_16x16;
			this.toolStripButtonDelete.Name = "toolStripButtonDelete";
			this.toolStripButtonDelete.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonDelete.Text = "Delete";
			this.toolStripButtonDelete.Visible = false;
			this.toolStripButtonDelete.Click += new System.EventHandler(this.toolStripButtonDelete_Click);
			// 
			// toolStripButtonMoveUp
			// 
			this.toolStripButtonMoveUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonMoveUp.Image = global::TouhouSpring.Particle.Properties.Resources.UpArrow_16x16;
			this.toolStripButtonMoveUp.Name = "toolStripButtonMoveUp";
			this.toolStripButtonMoveUp.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonMoveUp.Text = "Move Up";
			this.toolStripButtonMoveUp.Visible = false;
			this.toolStripButtonMoveUp.Click += new System.EventHandler(this.toolStripButtonMoveUp_Click);
			// 
			// toolStripButtonMoveDown
			// 
			this.toolStripButtonMoveDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.toolStripButtonMoveDown.Image = global::TouhouSpring.Particle.Properties.Resources.DownArrow_16x16;
			this.toolStripButtonMoveDown.Name = "toolStripButtonMoveDown";
			this.toolStripButtonMoveDown.Size = new System.Drawing.Size(23, 22);
			this.toolStripButtonMoveDown.Text = "Move Down";
			this.toolStripButtonMoveDown.Visible = false;
			this.toolStripButtonMoveDown.Click += new System.EventHandler(this.toolStripButtonMoveDown_Click);
			// 
			// propertyGrid
			// 
			this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGrid.Enabled = false;
			this.propertyGrid.Location = new System.Drawing.Point(0, 0);
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.Size = new System.Drawing.Size(202, 301);
			this.propertyGrid.TabIndex = 0;
			this.propertyGrid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid_PropertyValueChanged);
			// 
			// menuStrip1
			// 
			menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            fileToolStripMenuItem});
			menuStrip1.Location = new System.Drawing.Point(0, 0);
			menuStrip1.Name = "menuStrip1";
			menuStrip1.Size = new System.Drawing.Size(716, 25);
			menuStrip1.TabIndex = 0;
			menuStrip1.Text = "menuStrip1";
			// 
			// timerFps
			// 
			this.timerFps.Enabled = true;
			this.timerFps.Interval = 500;
			this.timerFps.Tick += new System.EventHandler(this.timerFps_Tick);
			// 
			// saveFileDialog
			// 
			this.saveFileDialog.Filter = "XML File (*.xml)|*.xml";
			this.saveFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialog_FileOk);
			// 
			// openFileDialog
			// 
			this.openFileDialog.Filter = "XML File (*.xml)|*.xml";
			this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.fileDialog_FileOk);
			// 
			// Main
			// 
			this.ClientSize = new System.Drawing.Size(716, 488);
			this.Controls.Add(splitContainer1);
			this.Controls.Add(menuStrip1);
			this.MainMenuStrip = menuStrip1;
			this.Name = "Main";
			this.Text = "Particle Workshop";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel1.PerformLayout();
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
			splitContainer1.ResumeLayout(false);
			statusStrip1.ResumeLayout(false);
			statusStrip1.PerformLayout();
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			splitContainer2.Panel1.ResumeLayout(false);
			splitContainer2.Panel1.PerformLayout();
			splitContainer2.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(splitContainer2)).EndInit();
			splitContainer2.ResumeLayout(false);
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			menuStrip1.ResumeLayout(false);
			menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid propertyGrid;
		private System.Windows.Forms.TreeView treeView;
		private Canvas canvas;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelFps;
		private System.Windows.Forms.Timer timerFps;
		private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusColor;
		private System.Windows.Forms.ColorDialog colorDialog;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelNumParticles;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.ToolStripButton toolStripButtonNewEffect;
		private System.Windows.Forms.ToolStripButton toolStripButtonDelete;
		private System.Windows.Forms.ToolStripButton toolStripButtonMoveUp;
		private System.Windows.Forms.ToolStripButton toolStripButtonMoveDown;
		private System.Windows.Forms.ToolStripDropDownButton toolStripDropDownButtonNewModifier;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton toolStripButtonPlayPause;
		private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
		private System.Windows.Forms.SaveFileDialog saveFileDialog;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
	}
}

