using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace TouhouSpring
{
	public class BuildMonitor : Logger
	{
		public class LogEventArgs : EventArgs
		{
			public string Message
			{
				get; private set;
			}

			public LogEventArgs(string message)
			{
				Message = message;
			}
		}

		public class Project
		{
			public string Configuration;
			public string Platform;
			public bool IsBuilt;
			public string OutputPath;
		}

		public Dictionary<string, Project> Projects
		{
			get; private set;
		}

		public int NumBuiltProjects
		{
			get { return Projects != null ? Projects.Values.Count(p => p.IsBuilt) : 0; }
		}

		public int NumSuccessfullyBuiltProjects
		{
			get; private set;
		}

		private string m_lastRegisteredProject = null;

		private string m_currentBuildingProject = null;
		private int m_numErrors = 0;
		private int m_numWarnings = 0;

		public event EventHandler<LogEventArgs> LogReceived;
		public event EventHandler<EventArgs> ProjectsRegistered;
		public event EventHandler<EventArgs> ProjectsBuilt;

		public BuildMonitor()
		{
			Projects = new Dictionary<string, Project>();
		}

		public override void Initialize(IEventSource eventSource)
		{
			eventSource.BuildFinished += (s, e) =>
			{
				Write("========== Build: {0} succeeded or up-to-date, {1} failed, 0 skipped ==========\n", NumSuccessfullyBuiltProjects, Projects.Count - NumSuccessfullyBuiltProjects);
			};

			eventSource.TargetFinished += (s, e) =>
			{
				string ext = Path.GetExtension(e.ProjectFile);
				if (String.Compare(ext, ".sln", true) == 0
					|| String.Compare(ext, ".tmp_proj", true) == 0)
				{
					return;
				}

				var projectFile = Path.GetFullPath(e.ProjectFile);
				if (!Projects.ContainsKey(projectFile))
				{
					Projects.Add(projectFile, new Project { Configuration = "", Platform = "", IsBuilt = false });
				}

				var project = Projects[projectFile];

				if (e.TargetName == "ResolveProjectReferences" && !project.IsBuilt)
				{
					Debug.Assert(m_currentBuildingProject == null);
					m_currentBuildingProject = projectFile;

					Write("----- Build started: Project: {0}", Path.GetFileNameWithoutExtension(projectFile));
					if (project.Configuration != "" || project.Platform != null)
					{
						Write(", Configuration: {0} {1}", project.Configuration, project.Platform);
					}
					Write(" -----\n");
				}
				else if ((e.TargetName == "Build" || e.TargetName == "_CleanRecordFileWrites")
						 && m_currentBuildingProject != null)
				{
					Debug.Assert(m_currentBuildingProject == projectFile);

					if (m_numErrors != 0 || m_numWarnings != 0)
					{
						Write("\nCompile complete -- {0} errors, {1} warnings\n", m_numErrors, m_numWarnings);
					}

					project.IsBuilt = true;
					if (m_numErrors == 0)
					{
						++NumSuccessfullyBuiltProjects;
					}

					if (ProjectsBuilt != null)
					{
						ProjectsBuilt(this, EventArgs.Empty);
					}

					m_numErrors = 0;
					m_numWarnings = 0;
					m_currentBuildingProject = null;
				}
			};

			eventSource.MessageRaised += (s, e) =>
			{
				if (e.Importance == MessageImportance.High
					&& e.SenderName != "Csc"
					&& e.SenderName != "ResGen")
				{
					Write("{0}{1}\n", e.SenderName == "BuildContent" ? "\t" : "", e.Message);

					if (e.SenderName == "Message" && e.Message.Contains("->") && m_currentBuildingProject != null)
					{
						var outputPath = Path.GetFullPath(e.Message.Substring(e.Message.IndexOf("->") + 3));
						Projects[m_currentBuildingProject].OutputPath = outputPath;
					}
				}

				const string RegisterToken = "Additional Properties for project \"";
				const string ConfigurationToken = "  Configuration=";
				const string PlatformToken = "  Platform=";

				if (e.SenderName == "MSBuild" && e.Message.StartsWith(RegisterToken))
				{
					string projectName = e.Message.Substring(RegisterToken.Length, e.Message.Length - RegisterToken.Length - 2);
					string projectFile = Path.GetFullPath(projectName);
					Projects.Add(projectFile, new Project { IsBuilt = false });
					m_lastRegisteredProject = projectFile;
				}
				else if (m_lastRegisteredProject != null
						 && e.SenderName == "MSBuild" && e.Message.StartsWith(ConfigurationToken))
				{
					Projects[m_lastRegisteredProject].Configuration = e.Message.Substring(ConfigurationToken.Length);
				}
				else if (m_lastRegisteredProject != null
						 && e.SenderName == "MSBuild" && e.Message.StartsWith(PlatformToken))
				{
					Projects[m_lastRegisteredProject].Platform = e.Message.Substring(PlatformToken.Length);
					if (ProjectsRegistered != null)
					{
						ProjectsRegistered(this, EventArgs.Empty);
					}
					m_lastRegisteredProject = null;
				}
			};

			eventSource.WarningRaised += (s, e) =>
			{
				if (m_currentBuildingProject != null)
				{
					Write("{0}({1},{2}): warning {3}: {4}\n", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
					++m_numWarnings;
				}
			};

			eventSource.ErrorRaised += (s, e) =>
			{
				if (m_currentBuildingProject != null)
				{
					Write("{0}({1},{2}): error {3}: {4}\n", e.File, e.LineNumber, e.ColumnNumber, e.Code, e.Message);
					++m_numErrors;
				}
			};
		}

		private void Write(string format, params object[] args)
		{
			if (LogReceived != null)
			{
				LogReceived(this, new LogEventArgs(String.Format(format, args)));
			}
		}

		/// <summary>
		/// Shutdown() is guaranteed to be called by MSBuild at the end of the build, after all 
		/// events have been raised.
		/// </summary>
		public override void Shutdown()
		{
		}
	}
}
