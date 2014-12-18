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


using System;
using System.IO;
using ConcurrencyHelpers.Containers;
using GenericHelpers;

namespace Node.Cs.Utils
{
	public class NodeExecutionContext : INodeExecutionContext
	{
		public NodeExecutionContext(
			CommandLineParser args,
			Version version,
			string nodeCsExecutablePath,
			string nodeCsExtraBinDirecotry,
			string currentDirectory,
			string tempPath,
            string nodeCsPackagesDirectory,
            string imageRuntimeVersion)
		{
			Args = args;
			Version = version;
			NodeCsExecutablePath = nodeCsExecutablePath;
			TempPath = CreateDirIfNotExists(tempPath);
			NodeCsExtraBinDirectory = CreateDirIfNotExists(nodeCsExtraBinDirecotry);
			CurrentDirectory = new LockFreeItem<string>(CreateDirIfNotExists(currentDirectory));
            ImageRuntimeVersion = imageRuntimeVersion;
            NodeCsPackagesDirectory = CreateDirIfNotExists(nodeCsPackagesDirectory);
		}

		private string CreateDirIfNotExists(string dir)
		{
			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			return dir;
		}

        public string NodeCsPackagesDirectory { get; protected set; }
        public CommandLineParser Args { get; protected set; }
        public String ImageRuntimeVersion { get; protected set; }
        public Version Version { get; protected set; }
        public string NodeCsExecutablePath { get; protected set; }
		public string NodeCsExtraBinDirectory { get; protected set; }
		public LockFreeItem<string> CurrentDirectory { get; set; }
		public string TempPath { get; protected set; }
	}
}