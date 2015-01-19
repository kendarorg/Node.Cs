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
		public const string NUGET_ORG = "https://www.nuget.org/api/v2/";

		private readonly INugetArchiveList _archiveList;
		private readonly INodeExecutionContext _context;
		private readonly INugetVersionVerifier _versionVerifier;
		private readonly IWebClient _client;

		public NugetPackagesDownloader(INugetArchiveList archiveList, INodeExecutionContext context, INugetVersionVerifier versionVerifier,
			IWebClient client = null)
		{
			_archiveList = archiveList;
			_context = context;
			_versionVerifier = versionVerifier;
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
			if (_archiveList.Check(packageName, version))
			{
				var packageDescriptor = _archiveList.Get(packageName, version);
				foreach (var dll in packageDescriptor.Dlls)
				{
					var dllPath = Path.Combine(_context.NodeCsPackagesDirectory, dll);
					yield return new NugetDll(dll, File.ReadAllBytes(dllPath));
				}
			}
			foreach (var dll in DownloadPackageInternal(framework, packageName, version, allowPreRelease, duplicateDetector))
			{
				yield return dll;
			}
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
			var deps = new List<NugetPackageDependency>();
			foreach (var depDll in nuspec.DescendantsByTag("dependency"))
			{
				var depId = depDll.Attribute("id").Value;
				var depVersion = depDll.Attribute("version") != null ? depDll.Attribute("version").Value : null;

				deps.Add(new NugetPackageDependency(depId, depVersion ?? string.Empty));

				if (duplicateDetector.Contains(depId)) continue;
				duplicateDetector.Add(depId);

				foreach (var depItem in DownloadPackageInternal(package.Framework, depId, depVersion, package.Pre, duplicateDetector))
				{
					yield return depItem;
				}
			}
			var dlls = new List<string>();
			foreach (var locDll in LoadDlls(package))
			{
				dlls.Add(locDll.Name);
				yield return locDll;
			}
			_archiveList.Add(package.Id, package.Version, dlls, deps);
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
		private byte[] DownloadSingleItem(string server, string packageName, string version, bool allowPreRelease)
		{
			server = server.TrimEnd('/');
			var versionQuery = _versionVerifier.BuildODataQuery(packageName, null);
			var remoteUri = string.Format("{0}/Packages()?$orderby=LastUpdated desc&$filter={1}", server, versionQuery);
			var client = _client ?? new BaseWebClient();
			var queryResult = client.DownloadData(remoteUri);

			var queryXml = XmlExtension.LoadDocument(queryResult);
			foreach (var xNode in queryXml.DescendantsByTag("entry"))
			{
				var foundedVersion = xNode.DescendantsByTag("version").First().Value;
				if (_versionVerifier.IsVersionMatching(foundedVersion, version))
				{
					var foundedVersionAddress = xNode.DescendantsByTag("content").First().Attribute("src").Value;
					return client.DownloadData(foundedVersionAddress);
				}
			}
			return null;
		}
	}
}
