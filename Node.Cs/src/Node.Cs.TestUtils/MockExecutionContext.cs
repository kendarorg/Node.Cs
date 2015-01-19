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


using GenericHelpers;
using System;
using System.IO;
using System.Reflection;
using Node.Cs.Utils;

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
			if (baseDir == null) return path;
			return Path.Combine(baseDir, path);
		}
	}
}
