// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs.Utils
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
