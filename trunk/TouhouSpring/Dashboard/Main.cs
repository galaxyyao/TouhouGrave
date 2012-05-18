using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using IWshShortcut = IWshRuntimeLibrary.IWshShortcut;
using WshShell = IWshRuntimeLibrary.WshShell;

namespace TouhouSpring
{
	public partial class Main : Form
	{
		private string[] m_targets = new string[]
		{
			"Build",
			"Rebuild",
			"Clean"
		};

		public Main()
		{
			InitializeComponent();

			Font = System.Drawing.SystemFonts.MessageBoxFont;
			comboBoxConfiguration.SelectedIndex = 1;
			buttonBuild.Text = m_targets[Properties.Settings.Default.Target];
		}

		private void Main_FormClosing(object sender, FormClosingEventArgs e)
		{
			Properties.Settings.Default.Save();
		}

		private void buttonBrowse_Click(object sender, EventArgs e)
		{
			if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
			{
				textBoxProjectFolder.Text = folderBrowserDialog.SelectedPath;
			}
		}

		private void buttonBuild_Click(object sender, EventArgs e)
		{
			var buildThread = new Thread(new ParameterizedThreadStart(BuildThreadMain))
			{
				IsBackground = true
			};
			buildThread.Start(new string[]
			{
				comboBoxConfiguration.SelectedItem.ToString(),
				m_targets[Properties.Settings.Default.Target]
			});
		}

		private void BuildThreadMain(object arg)
		{
			string[] strArgs = (string[])arg;
			string solutionFileName = @".\trunk\TouhouSpring.sln";
			string solutionFilePath = Path.GetFullPath(Path.Combine(Properties.Settings.Default.ProjectFolder, solutionFileName));

			var buildRequestData = new BuildRequestData(
				solutionFilePath,
				new Dictionary<string, string>
				{
					{ "Configuration", strArgs[0] }
				},
				null,
				new string[] { strArgs[1] },
				null);

			var logFile = new StreamWriter("./buildlog.txt");

			var monitor = new BuildMonitor();
			monitor.LogReceived += (s, logEventArgs) =>
			{
				logFile.Write(logEventArgs.Message.Replace("\n", logFile.NewLine));
			};
			monitor.ProjectsBuilt += (s, eventArgs) =>
			{
				progressBarBuildProgress.BeginInvoke((Action)delegate()
				{
					progressBarBuildProgress.Maximum = monitor.Projects.Count;
					progressBarBuildProgress.Value = monitor.NumBuiltProjects;
				});
			};

			BeginInvoke((Action)BuildStarted);

			var buildResult = BuildManager.DefaultBuildManager.Build(new BuildParameters
			{
				Loggers = new ILogger[] { monitor }
			}, buildRequestData);

			BeginInvoke((Action)BuildFinished);

			logFile.Close();

			BeginInvoke((Action)delegate()
			{
				labelStep2.BackColor = buildResult.OverallResult == BuildResultCode.Success
									   ? System.Drawing.Color.Green : System.Drawing.Color.Red;
			});

			var startMenuFolder = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu);
			var projectFolder = Path.Combine(startMenuFolder, "Touhou Spring");

			if (Properties.Settings.Default.Target == 2)
			{
				if (Directory.Exists(projectFolder))
				{
					Directory.Delete(projectFolder, true);
				}
				return;
			}

			if (buildResult.OverallResult == BuildResultCode.Failure)
			{
				return;
			}

			string resourceFolder = Path.Combine(Properties.Settings.Default.ProjectFolder,
												 "trunk\\TouhouSpring\\THSXnaWindows\\THSXnaWindows\\Resources");
			string contentFolder = Path.Combine(Properties.Settings.Default.ProjectFolder,
												"trunk\\TouhouSpring\\THSXnaWindows\\THSXnaWindowsContent");

			WshShell shell = new WshShell();
			Directory.CreateDirectory(projectFolder);

			// patch the config files
			foreach (var proj in monitor.Projects)
			{
				if (proj.Value.OutputPath != null && String.Compare(Path.GetExtension(proj.Value.OutputPath), ".exe", true) == 0)
				{
					var configFile = proj.Value.OutputPath + ".config";
					if (!File.Exists(configFile))
					{
						continue;
					}

					XDocument configDoc = XDocument.Load(configFile);
					bool changed = false;
					foreach (var settings in configDoc.Descendants("setting").Where(elem => elem.Attribute("serializeAs").Value == "String"))
					{
						var valueElem = settings.Element("value");

						const string ResourceFolderLabel = "THSXnaWindows\\Resources\\";
						const string ContentFolderLabel = "THSXnaWindowsContent\\";

						if (valueElem.Value.Contains(ResourceFolderLabel))
						{
							int childPathStart = valueElem.Value.IndexOf(ResourceFolderLabel) + ResourceFolderLabel.Length;
							string childPath = valueElem.Value.Substring(childPathStart);
							valueElem.Value = Path.Combine(resourceFolder, childPath);
							if (!valueElem.Value.EndsWith(Path.DirectorySeparatorChar.ToString()))
							{
								valueElem.Value += Path.DirectorySeparatorChar;
							}
							changed = true;
						}
						else if (valueElem.Value.Contains(ContentFolderLabel))
						{
							int childPathStart = valueElem.Value.IndexOf(ContentFolderLabel) + ContentFolderLabel.Length;
							string childPath = valueElem.Value.Substring(childPathStart);
							valueElem.Value = Path.Combine(contentFolder, childPath);
							if (!valueElem.Value.EndsWith(Path.DirectorySeparatorChar.ToString()))
							{
								valueElem.Value += Path.DirectorySeparatorChar;
							}
							changed = true;
						}
					}

					if (changed)
					{
						configDoc.Save(configFile);
					}

					IWshShortcut shortCut = shell.CreateShortcut(Path.Combine(projectFolder, Path.GetFileNameWithoutExtension(proj.Key) + ".lnk"));
					shortCut.TargetPath = proj.Value.OutputPath;
					shortCut.WorkingDirectory = Path.GetDirectoryName(proj.Value.OutputPath);
					shortCut.Save();
				}
			}
		}

		private void buttonTargetLeft_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.Target = Properties.Settings.Default.Target == 0
												 ? m_targets.Length - 1
												 : Properties.Settings.Default.Target - 1;
			buttonBuild.Text = m_targets[Properties.Settings.Default.Target];
		}

		private void buttonTargetRight_Click(object sender, EventArgs e)
		{
			Properties.Settings.Default.Target = Properties.Settings.Default.Target == m_targets.Length - 1
												 ? 0 : Properties.Settings.Default.Target + 1;
			buttonBuild.Text = m_targets[Properties.Settings.Default.Target];
		}

		private void BuildStarted()
		{
			labelStep2.BackColor = System.Drawing.Color.Black;
			buttonBuild.Text = "Building...";
			buttonBuild.Enabled = false;
			buttonTargetLeft.Enabled = false;
			buttonTargetRight.Enabled = false;
			progressBarBuildProgress.Value = 0;
			progressBarBuildProgress.Maximum = 1;
			progressBarBuildProgress.Visible = true;
		}

		private void BuildFinished()
		{
			buttonBuild.Text = m_targets[Properties.Settings.Default.Target];
			buttonBuild.Enabled = true;
			buttonTargetLeft.Enabled = true;
			buttonTargetRight.Enabled = true;
		}
	}
}
