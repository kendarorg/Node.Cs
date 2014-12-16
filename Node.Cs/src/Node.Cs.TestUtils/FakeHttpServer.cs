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
using System.Collections.Generic;
using System.Net;
using System.Threading;

namespace Node.Cs.Test
{
	public class FakeHttpServer : IDisposable
	{
		public int Port { get; private set; }
		public string Prefix { get; private set; }
		private Thread _thread;
		private HttpListener _listener;

		public FakeHttpServer(int port = 12530, string prefix = "test")
		{
			Port = port;
			Prefix = prefix;
			Requests = new List<string>();
			Responses = new Dictionary<string, Action<HttpListenerContext, FakeHttpServer>>(StringComparer.OrdinalIgnoreCase);
		}

		public List<string> Requests { get; private set; }
		public Dictionary<string, Action<HttpListenerContext, FakeHttpServer>> Responses { get; private set; }

		public string Root
		{
			get { return "http://localhost:" + Port + "/" + Prefix.Trim('/') + "/"; }
		}

		public bool Start()
		{
			try
			{
				_listener = new HttpListener();
				_listener.Prefixes.Add(Root);
				_listener.AuthenticationSchemes = AuthenticationSchemes.Anonymous;
				_listener.Start();

				_thread = new Thread(Listen);
				_thread.Start();
				Thread.Sleep(100);
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		public void Dispose()
		{
			try
			{
				_listener.Abort();
				_thread.Abort();
			}
			catch
			{
				
			}
		}

		private void Listen()
		{
			try
			{
				while (true)
				{
					var context = _listener.GetContext();
					var requestUrl = context.Request.Url.ToString();
					if (Responses.ContainsKey(requestUrl))
					{
						Responses[requestUrl](context, this);
					}
					else
					{
						context.Response.StatusCode = 404;
						context.Response.Close();
					}
				}
			}
			catch
			{
				
			}
		}

		public void Respond(HttpListenerContext httpListenerContext, byte[] dllContent)
		{
			httpListenerContext.Response.OutputStream.Write(dllContent, 0, dllContent.Length);
			httpListenerContext.Response.Close();
		}

		public string CreateAddress(string subPath)
		{
			return Root + subPath.Trim('/');
		}
	}
}
