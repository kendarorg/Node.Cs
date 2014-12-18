// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs
{
	public interface IAssemblySeeker
	{
		void AddSearchPath(string path);
		string FindAssembly(string dllPath);
	}

	public class AssemblySeeker : IAssemblySeeker
	{
		public AssemblySeeker(INodeExecutionContext context)
		{
			AddSearchPath(context.TempPath);
			AddSearchPath(context.NodeCsPackagesDirectory);
			AddSearchPath(context.NodeCsExtraBinDirectory);
			AddSearchPath(context.NodeCsExecutablePath);
			AddSearchPath(context.CurrentDirectory.Data);
		}

		private readonly List<string> _searchPaths = new List<string>();

		public void AddSearchPath(string path)
		{
			_searchPaths.Add(path);
		}

		public string FindAssembly(string dllPath)
		{
			var path = dllPath.ToPath().
					Trim(Path.DirectorySeparatorChar);

			if (Path.IsPathRooted(path))
			{
				if (File.Exists(path))
				{
					return path;
				}
				path = Path.GetFileName(path);
			}

			var ext = Path.GetExtension(path);
			if (ext.ToLowerInvariant() != ".dll")
			{
				return null;
			}
			if (ext.Length == 0)
			{
				path += ".dll";
			}

			// ReSharper disable once LoopCanBeConvertedToQuery
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int i = 0; i < _searchPaths.Count; i++)
			{
				var searchPath = _searchPaths[i];
				var fullPath = Path.Combine(searchPath, path);
				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}
			return null;
		}
	}
}
