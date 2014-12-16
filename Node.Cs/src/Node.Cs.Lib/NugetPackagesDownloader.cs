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


using Ionic.Zip;
using Node.Cs.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Node.Cs
{
	public class NugetPackagesDownloader : INugetPackagesDownloader
	{
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
					return ExtractDlls(result, framework);
				}
			}
			throw new NugetDownloadException("Unable to find package '{0}'.", packageName);
		}

		private IEnumerable<NugetDll> ExtractDlls(byte[] bytes, string framework)
		{
			var resultsCount = 0;
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


						path = path.Replace(DOUBLE_SEPARATOR,Path.DirectorySeparatorChar.ToString());
						var pointer = "lib" + Path.DirectorySeparatorChar + framework + Path.DirectorySeparatorChar;
                        pointer = pointer.Replace(DOUBLE_SEPARATOR,Path.DirectorySeparatorChar.ToString());
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
