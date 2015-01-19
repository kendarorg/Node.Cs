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
using System.Net;

namespace Http.Alternatives
{
	public class MyHttpServer : HttpServer
	{
		public MyHttpServer(IPAddress address,int port)
			: base(address,port)
		{
		}

		public override void HandleGetRequest(HttpProcessor p)
		{

			if (p.http_url.Equals("/Test.png"))
			{
				Stream fs = File.Open("../../Test.png", FileMode.Open);

				p.writeSuccess("image/png");
				fs.CopyTo(p.outputStream.BaseStream);
				p.outputStream.BaseStream.Flush();
			}

			Console.WriteLine("request: {0}", p.http_url);
			p.writeSuccess();
			p.outputStream.WriteLine("<html><body><h1>test server</h1>");
			p.outputStream.WriteLine("Current Time: " + DateTime.Now.ToString());
			p.outputStream.WriteLine("url : {0}", p.http_url);

			p.outputStream.WriteLine("<form method=post action=/form>");
			p.outputStream.WriteLine("<input type=text name=foo value=foovalue>");
			p.outputStream.WriteLine("<input type=submit name=bar value=barvalue>");
			p.outputStream.WriteLine("</form>");
		}

		public override void HandlePostRequest(HttpProcessor p, StreamReader inputData)
		{
			Console.WriteLine("POST request: {0}", p.http_url);
			string data = inputData.ReadToEnd();

			p.writeSuccess();
			p.outputStream.WriteLine("<html><body><h1>test server</h1>");
			p.outputStream.WriteLine("<a href=/test>return</a><p>");
			p.outputStream.WriteLine("postbody: <pre>{0}</pre>", data);


		}
	}
}