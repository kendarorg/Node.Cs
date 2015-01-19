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



using CoroutinesLib.Shared;
using GenericHelpers;
using Http.Shared.Contexts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Http.Shared.PathProviders
{
	public class ResourcesPathProvider:IPathProvider
	{
		private readonly HashSet<string> _resources;
		private readonly Dictionary<string, byte[]> _files;
		private readonly HashSet<string> _dirs;
		private Assembly _asm;

		public ResourcesPathProvider(Assembly asm = null)
		{
			if (asm == null) asm = Assembly.GetCallingAssembly();
			_resources = new HashSet<string>(asm.GetManifestResourceNames(),StringComparer.OrdinalIgnoreCase);
			_files = new Dictionary<string, byte[]>(StringComparer.OrdinalIgnoreCase);
			_dirs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			_asm = asm;
		}

		public void RegisterPath(string resourceId, string path)
		{
			path = path.Replace('/', Path.DirectorySeparatorChar);
			path = path.Trim(Path.DirectorySeparatorChar);
			var resourceName = _resources.FirstOrDefault(r => r.EndsWith(resourceId, StringComparison.OrdinalIgnoreCase));
			if(resourceName==null)
			{
				throw new FileLoadException(resourceId);
			}
			var content = ResourceContentLoader.LoadBytes(resourceName, _asm);
			_files.Add(path,content);
			var splitted = path.Split(Path.DirectorySeparatorChar);
			var entry = string.Empty;
			for (int i = 0; i < (splitted.Length-2); i++)
			{
				entry += Path.DirectorySeparatorChar + splitted[i];
				entry = entry.Trim(Path.DirectorySeparatorChar);
				if (!_dirs.Contains(entry))
				{
					_dirs.Add(entry);
				}
			}
		}


		public IEnumerable<string> FindFiles(string dir)
		{
			return new List<string>();
		}

		public IEnumerable<string> FindDirs(string dir)
		{
			return new List<string>();
		}

		public bool Exists(string relativePath,out bool isDir)
		{
			relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
			relativePath = relativePath.Trim(Path.DirectorySeparatorChar);
			if (_dirs.Contains(relativePath))
			{
				isDir = true;
				return true;
			}
			isDir = false;
			return _files.ContainsKey(relativePath);
		}

		public IEnumerable<ICoroutineResult> GetStream(string relativePath, IHttpContext context)
		{
			relativePath = relativePath.Replace('/', Path.DirectorySeparatorChar);
			relativePath = relativePath.Trim(Path.DirectorySeparatorChar);
			yield return CoroutineResult.Return(new StreamResult(DateTime.UtcNow,_files[relativePath]));
		}

		public void ShowDirectoryContent(bool showDirectoryContent)
		{
			
		}
	}
}
