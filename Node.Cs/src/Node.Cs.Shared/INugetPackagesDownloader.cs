using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs
{
	public interface IWebClient : IDisposable
	{
		byte[] DownloadData(string address);
	}

	public class NugetDll
	{
		public NugetDll(string name, byte[] data)
		{
			Name = name;
			Data = data;
		}

		public string Name { get; private set; }
		public byte[] Data { get; private set; }
	}

	public interface INugetPackagesDownloader
	{
		void AddPackageSource(string packageSourceFormat);
		IEnumerable<NugetDll> DownloadPackage(string framework, string packageName, string version, bool allowPreRelease);
	}
}
