using GenericHelpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs.Test
{
	public class MockExecutionContext : NodeExecutionContext
	{
		public MockExecutionContext(
				CommandLineParser args = null,
				Version version = default(Version),
				string nodeCsExecutablePath = null,
				string nodeCsExtraBinDirecotry = null,
				string currentDirectory = null,
			    string tempPath = null,
                string imageRuntimeMessage = "net45",
				Assembly caller = null)
			: base(args, version,
					nodeCsExecutablePath ?? RelativeToTest("node.cs.exe", caller ?? Assembly.GetExecutingAssembly()),
					nodeCsExtraBinDirecotry ?? RelativeToTest("bin", caller ?? Assembly.GetExecutingAssembly()),
					currentDirectory ?? RelativeToTest("", caller ?? Assembly.GetExecutingAssembly()),
					tempPath ?? RelativeToTest("tmp", caller ?? Assembly.GetExecutingAssembly()),
                    imageRuntimeMessage)
		{

		}

		private static string RelativeToTest(string path, Assembly caller)
		{
			var asmUri = new UriBuilder(caller.CodeBase);
			var asmPath = asmUri.Path;
			var baseDir = Path.GetDirectoryName(asmPath);
			return Path.Combine(baseDir, path);
		}
	}
}
