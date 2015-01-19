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
