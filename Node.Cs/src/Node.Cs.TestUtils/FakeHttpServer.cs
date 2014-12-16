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
			_listener.Abort();
			_thread.Abort();
		}

		private void Listen()
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
