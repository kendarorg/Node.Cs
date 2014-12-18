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

		public void Add(string id, string version,IEnumerable<string> dllNames)
		{
			EnsurePackages();
			if (!Check(id))
			{
				_packages.Packages.Add(new NugetPackage(id, version, dllNames));
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
