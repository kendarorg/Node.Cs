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


using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using System.Xml.XPath;
using Ionic.Zip;
using Node.Cs.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs
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

		public IEnumerable<NugetDll> DownloadPackage(string framework, string packageName, string version, bool allowPreRelease)
		{
			version = version ?? string.Empty;
			var allServers = new List<string>(_servers);
			if (allServers.Count == 0)
			{
				allServers.Add(NUGET_ORG);
			}

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
					return ExtractDlls(package);
				}
			}
			throw new NugetDownloadException("Unable to find package '{0}'.", packageName);
		}

		private IEnumerable<NugetDll> ExtractDlls(NugetDescriptor package)
		{

			var nuspec = ExtractNuspec(package);
			XNamespace ns = "http://schemas.microsoft.com/packaging/2012/06/nuspec.xsd";
			foreach (var xNode in nuspec.DescendantNodes().Where(el =>
			{
				var xe = el as XElement;
				if (xe == null) return false;
				return String.Compare(xe.Name.LocalName, "dependency", StringComparison.OrdinalIgnoreCase) == 0;
			}))
			{
				var depDll = (XElement) xNode;
				var depId = depDll.Attribute("id").Value;
				var depVersion = depDll.Attribute("version") != null ? depDll.Attribute("version").Value : null;
				foreach (var depItem in DownloadPackage(package.Framework, depId, depVersion, package.Pre))
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
			var items = new SortedDictionary<string,List<string>>();
			using (var zip = ZipFile.Read(package.Data))
			{
				foreach (ZipEntry e in zip)
				{
					LoadDllDataAndFramework(e, items);
				}
				foreach (var itemKey in items.Keys.Reverse())
				{
					if(String.Compare(itemKey, package.Framework, StringComparison.OrdinalIgnoreCase)>0) continue;
					return ExtractDllsForFramework(zip, items[itemKey]);
				}
				if (items.ContainsKey("net00"))
				{
					return ExtractDllsForFramework(zip, items["net00"]);
				}
				throw new DllNotFoundException(
					string.Format("Unable to find a suitable framework version for package '{0}.{1}'.",package.Id,package.Version));
			}
		}

		private IEnumerable<NugetDll> ExtractDllsForFramework(ZipFile zip, List<string> list)
		{
			foreach (var dll in list)
			{
				var dllUnderstandablePath = dll.ToPath();
				var fileName = Path.GetFileName(dllUnderstandablePath);
				var result = new MemoryStream();
				var zipItem = zip[dll];
				zipItem.Extract(result);
				yield return new NugetDll(fileName,result.ToArray());
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
				////var e = zip.First(r => r.FileName.Equals(package.Id + ".nuspec",StringComparison.OrdinalIgnoreCase));
				var e = zip[package.Id+".nuspec"];
				var outStream = new MemoryStream();
				e.Extract(outStream);
				outStream.Seek(0, SeekOrigin.Begin);
				return XDocument.Load(outStream);
			}
		}
		/*
		private IEnumerable<object> LoadNugetDlls(MemoryStream stream)
		{
			stream.Seek(0, SeekOrigin.Begin);
			var files = nuspec.XPathSelectElements("//dependencies/dependency");
			while (resultsCount == 0 && framework != null)
			{
				foreach (var item in ExtractDllsBase(bytes, framework))
				{
					resultsCount++;
					yield return item;
				}
				framework = DowngradeFramework(framework);
			}
		}

		private XDocument ExtractNuspec(MemoryStream stream, string packageName)
		{
			stream.Seek(0, SeekOrigin.Begin);
			using (var zip = ZipFile.Read(stream))
			{

				zip.ExtractSelectedEntries();
			}
			return XDocument.Load(stream);
		}

		private string DowngradeFramework(string framework)
		{
			framework = framework.ToLower();
			switch (framework)
			{
				case ("net45"): return "net40";
				case ("net40"): return "net35";
				case ("net35"): return "net30";
				case ("net30"): return "net20";
				case ("net20"): return "";
				default:
					return null;
			}
		}

		private static string DOUBLE_SEPARATOR = Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString();

		private IEnumerable<NugetDll> ExtractDllsBase(byte[] bytes, string framework)
		{
			using (var stream = new MemoryStream(bytes))
			{
				stream.Seek(0, SeekOrigin.Begin);
				using (var zip = ZipFile.Read(stream))
				{
					// here, we extract every entry, but we could extract conditionally
					// based on entry name, size, date, checkbox status, etc.  
					foreach (ZipEntry e in zip)
					{
						var path = e.FileName.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);


						path = path.Replace(DOUBLE_SEPARATOR, Path.DirectorySeparatorChar.ToString());
						var pointer = "lib" + Path.DirectorySeparatorChar + framework + Path.DirectorySeparatorChar;
						pointer = pointer.Replace(DOUBLE_SEPARATOR, Path.DirectorySeparatorChar.ToString());
						var ext = Path.GetExtension(path);
						var isRealPath = path
							.StartsWith(pointer) && !string.IsNullOrWhiteSpace(ext) && ext.ToLowerInvariant() == ".dll";

						if (isRealPath)
						{
							var ms = new MemoryStream();
							e.Extract(ms);
							yield return new NugetDll(Path.GetFileName(path), ms.ToArray());
						}
					}
				}
			}
		}
		*/
		private byte[] DownloadSingleItem(string format, string packageName, string version, bool allowPreRelease)
		{
			var remoteUri = string.Format(format, packageName, version).Trim('/');
			try
			{
				var client = _client ?? new BaseWebClient();
				return client.DownloadData(remoteUri);
			}
			catch (Exception)
			{
				return null;
			}

		}
	}
}
