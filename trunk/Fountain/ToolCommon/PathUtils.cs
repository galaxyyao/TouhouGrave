using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TouhouSpring
{
	public class PathUtils
	{
		public string ContentRootDirectory
		{
			get; private set;
		}

		public PathUtils(string contentRootDirectory)
		{
			if (contentRootDirectory == null)
			{
				throw new ArgumentNullException("contentRootDirectory");
			}

			ContentRootDirectory = Path.GetFullPath(contentRootDirectory);

			if (!ContentRootDirectory.EndsWith("\\"))
			{
				ContentRootDirectory += "\\";
			}
		}

		public bool IsUnderContentRoot(string fullPath)
		{
			if (fullPath == null)
			{
				throw new ArgumentNullException("fullPath");
			}

			return Path.IsPathRooted(fullPath) && fullPath.StartsWith(ContentRootDirectory, true, null);
		}

		public bool IsValidContentAssetUri(string uri)
		{
			string path = Path.GetFullPath(Path.Combine(ContentRootDirectory, uri));
			return Path.GetExtension(path) == "" && IsUnderContentRoot(path);
		}

		public string ToContentAssetUri(string fullPath)
		{
			if (!IsUnderContentRoot(fullPath))
			{
				throw new ArgumentException("Path is not under content directory.");
			}

			string relative = fullPath.Substring(ContentRootDirectory.Length);
			return Path.ChangeExtension(relative, null).Replace(Path.DirectorySeparatorChar, '/');
		}

		public string ToDiskPath(string assetUri, string extension)
		{
			if (!IsValidContentAssetUri(assetUri))
			{
				throw new ArgumentException("Invalid asset URI.");
			}

			return Path.GetFullPath(Path.ChangeExtension(Path.Combine(ContentRootDirectory, assetUri), extension));
		}
	}
}
