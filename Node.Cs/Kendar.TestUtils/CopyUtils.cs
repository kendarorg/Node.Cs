using System.IO;
using System.Reflection;

namespace Kendar.TestUtils
{
	public static class CopyUtils
	{
		public static void CopyDllOnTarget(string externalDllName, string basePath, params string[] externalProjectPath)
		{
#if DEBUG
			const string dest = "Debug";
#else
			const string dest = "Release";
#endif
			var srcDll = PathResolver.GetSolutionRoot();
			foreach (var item in externalProjectPath)
			{
				srcDll = Path.Combine(srcDll, item);
			}
			srcDll = Path.Combine(srcDll, "bin", dest,
				externalDllName + ".dll");

			//var srcDll = Path.Combine(PathResolver.GetSolutionRoot(), "test", "mockProjects", externalDllName, 
			srcDll = PathResolver.FindByPath(srcDll, Assembly.GetCallingAssembly());
			if (string.IsNullOrWhiteSpace(srcDll) || !File.Exists(srcDll))
			{

				srcDll = PathResolver.FindByPath(Path.Combine(PathResolver.GetAssemblyPath(Assembly.GetCallingAssembly()), externalDllName + ".dll"), Assembly.GetCallingAssembly());
			}
			var destDll = Path.Combine(basePath, externalDllName + ".dll");
			if (srcDll == destDll)
			{
				return;
			}

			if (File.Exists(destDll))
			{
				File.Delete(destDll);
			}
			File.Copy(srcDll, destDll);
		}
	}
}