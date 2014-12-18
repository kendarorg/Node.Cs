﻿// ===========================================================
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
using System.Linq;
using System.Xml.Linq;
using Ionic.Zip;
using Node.Cs.Exceptions;
using Node.Cs.Utils;

namespace Node.Cs.Nugets
{
	public class NugetPackagesDownloader : INugetPackagesDownloader
	{
		internal struct NugetDescriptor
		{
			public MemoryStream Data;
			public string Framework;
			public string Version;
			public string Id;
			public bool Pre;
		}
		public const string NUGET_ORG = "https://www.nuget.org/api/v2/package/{0}/{1}";

		private readonly IWebClient _client;

		public NugetPackagesDownloader(IWebClient client = null)
		{
			_client = client;
		}

		private readonly List<string> _servers = new List<string>();

		public void AddPackageSource(string packageSourceFormat)
		{
			_servers.Add(packageSourceFormat);
		}

		public IEnumerable<NugetDll> DownloadPackage(string framework, string packageName, string version,
			bool allowPreRelease)
		{
			var duplicateDetector = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
			return DownloadPackageInternal(framework, packageName, version, allowPreRelease, duplicateDetector);
		}
		public IEnumerable<NugetDll> DownloadPackageInternal(string framework, string packageName, string version, bool allowPreRelease, HashSet<string> duplicateDetector)
		{
			version = version ?? string.Empty;
			var allServers = new List<string>(_servers);
			if (allServers.Count == 0)
			{
				allServers.Add(NUGET_ORG);
			}

			// ReSharper disable once ForCanBeConvertedToForeach
			for (var i = 0; i < allServers.Count; i++)
			{
				byte[] result = DownloadSingleItem(allServers[i], packageName, version, allowPreRelease);
				if (result != null && result.Length > 0)
				{
					var package = new NugetDescriptor
					{
						Data = new MemoryStream(result),
						Id = packageName,
						Version = version,
						Framework = framework,
						Pre = allowPreRelease
					};
					return ExtractDlls(package, duplicateDetector);
				}
			}
			throw new NugetDownloadException("Unable to find package '{0}'.", packageName);
		}

		private IEnumerable<NugetDll> ExtractDlls(NugetDescriptor package, HashSet<string> duplicateDetector)
		{

			var nuspec = ExtractNuspec(package);

			foreach (var xNode in nuspec.DescendantNodes().Where(el =>
			{
				var xe = el as XElement;
				if (xe == null) return false;
				return String.Compare(xe.Name.LocalName, "dependency", StringComparison.OrdinalIgnoreCase) == 0;
			}))
			{
				var depDll = (XElement)xNode;
				var depId = depDll.Attribute("id").Value;
				if (duplicateDetector.Contains(depId)) continue;
				duplicateDetector.Add(depId);
				var depVersion = depDll.Attribute("version") != null ? depDll.Attribute("version").Value : null;
				foreach (var depItem in DownloadPackageInternal(package.Framework, depId, depVersion, package.Pre, duplicateDetector))
				{
					yield return depItem;
				}
			}
			foreach (var locDll in LoadDlls(package))
			{
				yield return locDll;
			}
		}

		private IEnumerable<NugetDll> LoadDlls(NugetDescriptor package)
		{
			package.Data.Seek(0, SeekOrigin.Begin);
			var items = new SortedDictionary<string, List<string>>();
			using (var zip = ZipFile.Read(package.Data))
			{
				foreach (ZipEntry e in zip)
				{
					LoadDllDataAndFramework(e, items);
				}
				foreach (var itemKey in items.Keys.Reverse())
				{
					if (String.Compare(itemKey, package.Framework, StringComparison.OrdinalIgnoreCase) > 0) continue;
					return ExtractDllsForFramework(zip, items[itemKey]);
				}
				throw new DllNotFoundException(
					string.Format("Unable to find a suitable framework version for package '{0}.{1}'.", package.Id, package.Version));
			}
		}

		private IEnumerable<NugetDll> ExtractDllsForFramework(ZipFile zip, IEnumerable<string> list)
		{
			foreach (var dll in list)
			{
				var dllUnderstandablePath = dll.ToPath();
				var fileName = Path.GetFileName(dllUnderstandablePath);
				var result = new MemoryStream();
				var zipItem = zip[dll];
				zipItem.Extract(result);
				yield return new NugetDll(fileName, result.ToArray());
			}
		}

		private void LoadDllDataAndFramework(ZipEntry e, SortedDictionary<string, List<string>> items)
		{
			var path = e.FileName;
			var splPath = path.ToPath().Split(Path.DirectorySeparatorChar);
			if (splPath.First() == "lib" && splPath.Last().ToLowerInvariant().EndsWith(".dll"))
			{
				var fw = "net00";
				if (splPath.Length == 3)
				{
					fw = splPath[1];
				}
				if (!items.ContainsKey(fw)) items.Add(fw, new List<string>());
				items[fw].Add(e.FileName);
			}
		}

		private XDocument ExtractNuspec(NugetDescriptor package)
		{
			package.Data.Seek(0, SeekOrigin.Begin);
			using (var zip = ZipFile.Read(package.Data))
			{
				var e = zip[package.Id + ".nuspec"];
				var outStream = new MemoryStream();
				e.Extract(outStream);
				outStream.Seek(0, SeekOrigin.Begin);
				return XDocument.Load(outStream);
			}
		}

		// ReSharper disable once UnusedParameter.Local
		private byte[] DownloadSingleItem(string format, string packageName, string version, bool allowPreRelease)
		{
			var remoteUri = string.Format(format, packageName, version).Trim('/');

			var client = _client ?? new BaseWebClient();
			return client.DownloadData(remoteUri);

		}
	}
}