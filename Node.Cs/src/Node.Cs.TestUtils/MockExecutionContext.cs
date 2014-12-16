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
                string nodeCsPackagesDirectory = null,
                string imageRuntimeMessage = "net45",
				Assembly caller = null)
			: base(args, version,
					nodeCsExecutablePath ?? RelativeToTest("node.cs.exe", caller ?? Assembly.GetExecutingAssembly()),
					nodeCsExtraBinDirecotry ?? RelativeToTest("bin", caller ?? Assembly.GetExecutingAssembly()),
					currentDirectory ?? RelativeToTest("", caller ?? Assembly.GetExecutingAssembly()),
					tempPath ?? RelativeToTest("tmp", caller ?? Assembly.GetExecutingAssembly()),
                    nodeCsPackagesDirectory ?? RelativeToTest("packages", caller ?? Assembly.GetExecutingAssembly()),
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
