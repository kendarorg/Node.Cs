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



using Newtonsoft.Json;
using Node.Cs.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs.Nugets
{
	public class NugetArchiveList : INugetArchiveList
	{
		private readonly string _path;
		private readonly NugetPackages _packages;
		
		public NugetArchiveList(INodeExecutionContext context)
		{
			_path = Path.Combine(context.NodeCsPackagesDirectory, "packages.json");
			_packages = new NugetPackages();
		}

		public void Add(string id, string version, IEnumerable<string> dllNames, IEnumerable<NugetPackageDependency> deps)
		{
			EnsurePackages();
			if (!Check(id))
			{
				_packages.Packages.Add(new NugetPackage(id, version, dllNames,deps));
			}
			CommitChanges();
		}

		public NugetPackage Get(string id, string version = null)
		{
			EnsurePackages();
			if (_packages.Packages.Count == 0) return null;

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var index = 0; index < _packages.Packages.Count; index++)
			{
				var package = _packages.Packages[index];
				if (string.Equals(package.Id, id, StringComparison.InvariantCultureIgnoreCase))
				{
					if (version == null) return package;
					if (version == package.Version) return package;
				}
			}

			return null;
		}

		public bool Check(string id, string version = null)
		{
			return Get(id, version)!=null;
		}

		private void CommitChanges()
		{
			var serialized = JsonConvert.SerializeObject(_packages);
			File.WriteAllText(_path, serialized);
		}

		private void EnsurePackages()
		{
			if (!File.Exists(_path))
			{
				var serialized = JsonConvert.SerializeObject(_packages);
				File.WriteAllText(_path,serialized);
			}
		}


		public void Remove(string id)
		{
			EnsurePackages();
		}

	}
}
