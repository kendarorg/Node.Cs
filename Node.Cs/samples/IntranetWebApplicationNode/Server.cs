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
using NodeCs.Shared;

namespace IntranetWebApplication
{

	public class RunHttpModule
	{
		public void Execute()
		{
			const string logFile = "SampleLog.txt";
			File.Delete(logFile);
			File.WriteAllText(logFile, "");
			NodeRoot.SetLoggingLevel("all");
			NodeRoot.SetLogFile(logFile);

			//Load curl to ease testing
			NodeRoot.LoadModule("curl");

			//Load Node caching
			var cacheModule = NodeRoot.LoadNamedModule("mainCache", "node.caching");

			//Load static path provider
			var staticPathProvider = NodeRoot.LoadNamedModule("root", "http.pathprovider.staticcontent");
			//Set the root where files will be taken
			staticPathProvider.SetParameter("connectionString", InferWebRootDir());
			staticPathProvider.SetParameter("cache", cacheModule);

			
			//Load cshtml renderer
			var razorModule = NodeRoot.LoadModule("http.renderer.razor");
			razorModule.SetParameter("cache", cacheModule);

			//Load routing engine
			NodeRoot.LoadModule("http.routing");
			NodeRoot.LoadModule("http.mvc");


			//Load the webserver
			NodeRoot.LoadModule("http");

			//Load the application dll
			NodeRoot.LoadDll("IntranetWebApplication");

			//Initialize application server
			var http = NodeRoot.GetModule("http");
			http.SetParameter("port", 8881);
			http.SetParameter("virtualDir", "nodecs");
			http.SetParameter("host", "localhost");
			http.SetParameter("showDirectoryContent", false);

			
			//Start everthing
			NodeRoot.StartModules();
		}

		private static string InferWebRootDir()
		{
			var rootDir = NodeRoot.Main.AssemblyDirectory;
			
			//Search all root directories
			while (!Directory.Exists(Path.Combine(rootDir, "App_Start")))
			{
				rootDir = Path.GetDirectoryName(rootDir);
			}
			//rootDir = Path.Combine(rootDir, "web");
			
			return rootDir;
		}
	}
}
