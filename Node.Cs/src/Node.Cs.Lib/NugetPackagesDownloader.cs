using Ionic.Zip;
using Node.Cs.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs
{
	public class BaseWebClient : IWebClient
	{
		public byte[] DownloadData(string address)
		{
			using (var cli = new WebClient())
			{
				return cli.DownloadData(address);
			}
		}

		public void Dispose()
		{

		}
	}

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
			if (_servers.Count == 0)
			{
				byte[] result = DownloadSingleItem(NUGET_ORG, packageName, version, allowPreRelease);
				if (result != null)
				{
					return ExtractDlls(result, framework);
				}
			}
			for (var i = 0; i < _servers.Count; i++)
			{
				byte[] result = DownloadSingleItem(_servers[i], packageName, version, allowPreRelease);
				if (result != null)
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


						path = path.Replace(
								Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString(),
								Path.DirectorySeparatorChar.ToString());
						var pointer = "lib" + Path.DirectorySeparatorChar + framework + Path.DirectorySeparatorChar;
						pointer = pointer.Replace(
							Path.DirectorySeparatorChar.ToString() + Path.DirectorySeparatorChar.ToString(),
							Path.DirectorySeparatorChar.ToString());
						var isRealPath = path
							.StartsWith(pointer);

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
