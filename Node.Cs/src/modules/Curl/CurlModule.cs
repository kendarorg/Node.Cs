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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NodeCs.Shared;

namespace Curl
{
	public class CurlModule : NodeModuleBase
	{
		public override void Initialize()
		{
			var main = ServiceLocator.Locator.Resolve<IMain>();
			main.RegisterCommand("curl.read", new CommandDefinition((Action<string, string>)CurlModule.Curl, typeof(string), typeof(string)));
			main.RegisterCommand("curl.read", new CommandDefinition((Action<string>)CurlModule.Curl, typeof(string)));
			main.RegisterCommand("curl.result", new CommandDefinition((Action<string, string>)CurlModule.Curlh, typeof(string), typeof(string)));
			main.RegisterCommand("curl.result", new CommandDefinition((Action<string>)CurlModule.Curlh, typeof(string)));
		}

		protected override void Dispose(bool disposing)
		{
			var main = ServiceLocator.Locator.Resolve<IMain>();
			main.UnregisterCommand("curl.read", 2);
			main.UnregisterCommand("curl.read", 1);
			main.UnregisterCommand("curl.result", 2);
			main.UnregisterCommand("curl.result", 1);
		}

		protected static void Curl(string url)
		{
			Curl("GET", url);
		}

		protected static void Curl(string verb, string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = verb;
			var response = (HttpWebResponse)request.GetResponse();
			NodeRoot.CWriteLine(string.Format("Retrieved '{0}' with verb '{1}':", url, verb));
			if (response.StatusCode != HttpStatusCode.OK)
			{
				NodeRoot.CWriteLine(string.Format("Status code '{0}'.", response.StatusCode));
			}
			var stream = request.GetResponse().GetResponseStream();
			if (stream == null)
			{
				NodeRoot.CWriteLine("No data");
				return;
			}
			var reader = new StreamReader(stream); ;
			string sLine = "";
			int i = 0;

			while (sLine != null)
			{
				i++;
				sLine = reader.ReadLine();
				if (sLine != null)
				{
					NodeRoot.CWriteLine(string.Format("{0}:{1}", NodeRoot.Lpad(i, 5), sLine));
				}
			}
		}

		protected static void Curlh(string url)
		{
			Curlh("GET", url);
		}

		protected static void Curlh(string verb, string url)
		{
			var request = (HttpWebRequest)WebRequest.Create(url);
			request.Method = verb;
			var response = (HttpWebResponse)request.GetResponse();
			NodeRoot.CWriteLine(string.Format("Retrieved '{0}' with verb '{1}':", url, verb));
			if (response.StatusCode != HttpStatusCode.OK)
			{
				NodeRoot.CWriteLine(string.Format("Status code '{0}'.", response.StatusCode));
			}
			var stream = request.GetResponse().GetResponseStream();
			if (stream == null)
			{
				NodeRoot.CWriteLine("No data");
				return;
			}
			var reader = new StreamReader(stream); ;
			string sLine = "";
			int i = 0;
			var length = 0;
			while (sLine != null)
			{
				i++;
				sLine = reader.ReadLine();
				if (sLine != null)
				{
					length += sLine.Length;
				}
			}
			NodeRoot.CWriteLine(string.Format("Read {0} bytes.", length));
		}
	}
}
