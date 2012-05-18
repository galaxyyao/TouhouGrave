namespace TouhouSpring
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
			System.Windows.Forms.Label label1;
			System.Windows.Forms.Label label2;
			System.Windows.Forms.Label label3;
			System.Windows.Forms.Label label4;
			System.Windows.Forms.Label label7;
			this.labelStep2 = new System.Windows.Forms.Label();
			this.textBoxProjectFolder = new System.Windows.Forms.TextBox();
			this.buttonBrowse = new System.Windows.Forms.Button();
			this.buttonBuild = new System.Windows.Forms.Button();
			this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
			this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
			this.progressBarBuildProgress = new System.Windows.Forms.ProgressBar();
			this.buttonTargetRight = new System.Windows.Forms.Button();
			this.buttonTargetLeft = new System.Windows.Forms.Button();
			this.comboBoxConfiguration = new System.Windows.Forms.ComboBox();
			label1 = new System.Windows.Forms.Label();
			label2 = new System.Windows.Forms.Label();
			label3 = new System.Windows.Forms.Label();
			label4 = new System.Windows.Forms.Label();
			label7 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
			this.SuspendLayout();
			// 
			// label1
			// 
			label1.AutoSize = true;
			label1.Location = new System.Drawing.Point(85, 88);
			label1.Name = "label1";
			label1.Size = new System.Drawing.Size(89, 12);
			label1.TabIndex = 0;
			label1.Text = "Project folder";
			// 
			// label2
			// 
			label2.BackColor = System.Drawing.Color.White;
			label2.Dock = System.Windows.Forms.DockStyle.Top;
			label2.Location = new System.Drawing.Point(0, 0);
			label2.Name = "label2";
			label2.Size = new System.Drawing.Size(465, 76);
			label2.TabIndex = 5;
			// 
			// label3
			// 
			label3.AutoSize = true;
			label3.BackColor = System.Drawing.Color.White;
			label3.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label3.Location = new System.Drawing.Point(12, 8);
			label3.Name = "label3";
			label3.Size = new System.Drawing.Size(322, 37);
			label3.TabIndex = 6;
			label3.Text = "TouhouSpring Dashboard";
			// 
			// label4
			// 
			label4.BackColor = System.Drawing.Color.Black;
			label4.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			label4.ForeColor = System.Drawing.Color.White;
			label4.Location = new System.Drawing.Point(0, 76);
			label4.Name = "label4";
			label4.Size = new System.Drawing.Size(79, 62);
			label4.TabIndex = 7;
			label4.Text = " 1";
			// 
			// labelStep2
			// 
			this.labelStep2.BackColor = System.Drawing.Color.Black;
			this.labelStep2.Font = new System.Drawing.Font("Segoe UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelStep2.ForeColor = System.Drawing.Color.White;
			this.labelStep2.Location = new System.Drawing.Point(0, 139);
			this.labelStep2.Name = "labelStep2";
			this.labelStep2.Size = new System.Drawing.Size(79, 62);
			this.labelStep2.TabIndex = 8;
			this.labelStep2.Text = " 2";
			// 
			// label7
			// 
			label7.AutoSize = true;
			label7.Location = new System.Drawing.Point(85, 115);
			label7.Name = "label7";
			label7.Size = new System.Drawing.Size(83, 12);
			label7.TabIndex = 13;
			label7.Text = "Configuration";
			// 
			// textBoxProjectFolder
			// 
			this.textBoxProjectFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TouhouSpring.Properties.Settings.Default, "ProjectFolder", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
			this.textBoxProjectFolder.Location = new System.Drawing.Point(180, 85);
			this.textBoxProjectFolder.Name = "textBoxProjectFolder";
			this.textBoxProjectFolder.Size = new System.Drawing.Size(188, 21);
			this.textBoxProjectFolder.TabIndex = 1;
			this.textBoxProjectFolder.Text = global::TouhouSpring.Properties.Settings.Default.ProjectFolder;
			// 
			// buttonBrowse
			// 
			this.buttonBrowse.Location = new System.Drawing.Point(378, 83);
			this.buttonBrowse.Name = "buttonBrowse";
			this.buttonBrowse.Size = new System.Drawing.Size(75, 21);
			this.buttonBrowse.TabIndex = 2;
			this.buttonBrowse.Text = "Browse...";
			this.buttonBrowse.UseVisualStyleBackColor = true;
			this.buttonBrowse.Click += new System.EventHandler(this.buttonBrowse_Click);
			// 
			// buttonBuild
			// 
			this.buttonBuild.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.buttonBuild.Location = new System.Drawing.Point(110, 147);
			this.buttonBuild.Name = "buttonBuild";
			this.buttonBuild.Size = new System.Drawing.Size(261, 40);
			this.buttonBuild.TabIndex = 3;
			this.buttonBuild.Text = "Build";
			this.buttonBuild.UseVisualStyleBackColor = true;
			this.buttonBuild.Click += new System.EventHandler(this.buttonBuild_Click);
			// 
			// folderBrowserDialog
			// 
			this.folderBrowserDialog.ShowNewFolderButton = false;
			// 
			// errorProvider
			// 
			this.errorProvider.ContainerControl = this;
			// 
			// progressBarBuildProgress
			// 
			this.progressBarBuildProgress.Location = new System.Drawing.Point(110, 188);
			this.progressBarBuildProgress.Name = "progressBarBuildProgress";
			this.progressBarBuildProgress.Size = new System.Drawing.Size(313, 9);
			this.progressBarBuildProgress.TabIndex = 10;
			this.progressBarBuildProgress.Visible = false;
			// 
			// buttonTargetRight
			// 
			this.buttonTargetRight.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.buttonTargetRight.Location = new System.Drawing.Point(396, 147);
			this.buttonTargetRight.Name = "buttonTargetRight";
			this.buttonTargetRight.Size = new System.Drawing.Size(27, 40);
			this.buttonTargetRight.TabIndex = 11;
			this.buttonTargetRight.Text = ">";
			this.buttonTargetRight.UseVisualStyleBackColor = true;
			this.buttonTargetRight.Click += new System.EventHandler(this.buttonTargetRight_Click);
			// 
			// buttonTargetLeft
			// 
			this.buttonTargetLeft.Font = new System.Drawing.Font("SimSun", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.buttonTargetLeft.Location = new System.Drawing.Point(370, 147);
			this.buttonTargetLeft.Name = "buttonTargetLeft";
			this.buttonTargetLeft.Size = new System.Drawing.Size(27, 40);
			this.buttonTargetLeft.TabIndex = 12;
			this.buttonTargetLeft.Text = "<";
			this.buttonTargetLeft.UseVisualStyleBackColor = true;
			this.buttonTargetLeft.Click += new System.EventHandler(this.buttonTargetLeft_Click);
			// 
			// comboBoxConfiguration
			// 
			this.comboBoxConfiguration.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxConfiguration.FormattingEnabled = true;
			this.comboBoxConfiguration.Items.AddRange(new object[] {
            "Debug",
            "Release"});
			this.comboBoxConfiguration.Location = new System.Drawing.Point(180, 112);
			this.comboBoxConfiguration.Name = "comboBoxConfiguration";
			this.comboBoxConfiguration.Size = new System.Drawing.Size(121, 20);
			this.comboBoxConfiguration.TabIndex = 14;
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(465, 222);
			this.Controls.Add(this.comboBoxConfiguration);
			this.Controls.Add(label7);
			this.Controls.Add(this.buttonTargetLeft);
			this.Controls.Add(this.buttonTargetRight);
			this.Controls.Add(this.progressBarBuildProgress);
			this.Controls.Add(this.labelStep2);
			this.Controls.Add(label4);
			this.Controls.Add(label3);
			this.Controls.Add(label2);
			this.Controls.Add(this.buttonBuild);
			this.Controls.Add(this.buttonBrowse);
			this.Controls.Add(this.textBoxProjectFolder);
			this.Controls.Add(label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "Main";
			this.Text = "Dashboard";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
			((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBoxProjectFolder;
		private System.Windows.Forms.Button buttonBrowse;
		private System.Windows.Forms.Button buttonBuild;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
		private System.Windows.Forms.ErrorProvider errorProvider;
		private System.Windows.Forms.ProgressBar progressBarBuildProgress;
		private System.Windows.Forms.Button buttonTargetRight;
		private System.Windows.Forms.Button buttonTargetLeft;
		private System.Windows.Forms.ComboBox comboBoxConfiguration;
		private System.Windows.Forms.Label labelStep2;

	}
}

