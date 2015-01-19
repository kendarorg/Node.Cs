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
using System.Reflection;
using System.ServiceProcess;

namespace NodeCs
{
	class Program
	{
		static void Main(string[] args)
		{
			var helpMessage = ResourceContentLoader.LoadText("Help.txt", Assembly.GetExecutingAssembly());
			var clp = new CommandLineParser(args, helpMessage);
			var runner = new NodeCsService();
			
			if (clp.Has("service"))
			{
				if (clp.Has("serviceSHOULDSTART REALLY"))
				{
					var servicesToRun = new ServiceBase[] { };
					ServiceBase.Run(servicesToRun);
				}
				else { throw new NotImplementedException(); }
			}
			else
			{
				runner.Run(args,helpMessage);
				while (true)
				{
					Shared.NodeRoot.Write();
					var result = Console.ReadLine();
					if (!string.IsNullOrWhiteSpace(result))
					{
						if (result.Trim().ToLowerInvariant() == "close")
						{
							break;
						}
						else
						{
							if (!runner.Execute(result))
							{
								break;
							}
						}
					}
				}


				Shared.NodeRoot.WriteLine("Stopping.");
				runner.Terminate();
				Shared.NodeRoot.WriteLine("Stopped.");
			}
		}
	}
}
