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
using System.Collections;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace Http.Alternatives
{
	public class HttpProcessor
	{
		public TcpClient socket;
		public HttpServer srv;

		private Stream inputStream;
		public StreamWriter outputStream;

		public String http_method;
		public String http_url;
		public String http_protocol_versionstring;
		public Hashtable httpHeaders = new Hashtable();


		private static int MAX_POST_SIZE = 10 * 1024 * 1024; // 10MB

		public HttpProcessor(TcpClient s, HttpServer srv)
		{
			this.socket = s;
			this.srv = srv;
		}


		private string StreamReadLine(Stream inputStream)
		{
			int next_char;
			string data = "";
			while (true)
			{
				next_char = inputStream.ReadByte();
				if (next_char == '\n') { break; }
				if (next_char == '\r') { continue; }
				if (next_char == -1) { Thread.Sleep(1); continue; };
				data += Convert.ToChar(next_char);
			}
			return data;
		}


		public void Process()
		{
			// we can't use a StreamReader for input, because it buffers up extra data on us inside it's
			// "processed" view of the world, and we want the data raw after the headers
			inputStream = new BufferedStream(socket.GetStream());

			// we probably shouldn't be using a streamwriter for all output from handlers either
			outputStream = new StreamWriter(new BufferedStream(socket.GetStream()));
			try
			{
				parseRequest();
				readHeaders();
				if (http_method.Equals("GET"))
				{
					handleGETRequest();
				}
				else if (http_method.Equals("POST"))
				{
					handlePOSTRequest();
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception: " + e.ToString());
				writeFailure();
			}
			outputStream.Flush();
			// bs.Flush(); // flush any remaining output
			inputStream = null; outputStream = null; // bs = null;            
			socket.Close();
		}

		public void parseRequest()
		{
			String request = StreamReadLine(inputStream);
			string[] tokens = request.Split(' ');
			if (tokens.Length != 3)
			{
				throw new Exception("invalid http request line");
			}
			http_method = tokens[0].ToUpper();
			http_url = tokens[1];
			http_protocol_versionstring = tokens[2];

			Console.WriteLine("starting: " + request);
		}

		public void readHeaders()
		{
			Console.WriteLine("readHeaders()");
			String line;
			while ((line = StreamReadLine(inputStream)) != null)
			{
				if (line.Equals(""))
				{
					Console.WriteLine("got headers");
					return;
				}

				int separator = line.IndexOf(':');
				if (separator == -1)
				{
					throw new Exception("invalid http header line: " + line);
				}
				String name = line.Substring(0, separator);
				int pos = separator + 1;
				while ((pos < line.Length) && (line[pos] == ' '))
				{
					pos++; // strip any spaces
				}

				string value = line.Substring(pos, line.Length - pos);
				Console.WriteLine("header: {0}:{1}", name, value);
				httpHeaders[name] = value;
			}
		}

		public void handleGETRequest()
		{
			srv.HandleGetRequest(this);
		}

		private const int BUF_SIZE = 4096;
		public void handlePOSTRequest()
		{
			// this post data processing just reads everything into a memory stream.
			// this is fine for smallish things, but for large stuff we should really
			// hand an input stream to the request processor. However, the input stream 
			// we hand him needs to let him see the "end of the stream" at this content 
			// length, because otherwise he won't know when he's seen it all! 

			Console.WriteLine("get post data start");
			int content_len = 0;
			MemoryStream ms = new MemoryStream();
			if (this.httpHeaders.ContainsKey("Content-Length"))
			{
				content_len = Convert.ToInt32(this.httpHeaders["Content-Length"]);
				if (content_len > MAX_POST_SIZE)
				{
					throw new Exception(
						String.Format("POST Content-Length({0}) too big for this simple server",
							content_len));
				}
				byte[] buf = new byte[BUF_SIZE];
				int to_read = content_len;
				while (to_read > 0)
				{
					Console.WriteLine("starting Read, to_read={0}", to_read);

					int numread = this.inputStream.Read(buf, 0, Math.Min(BUF_SIZE, to_read));
					Console.WriteLine("read finished, numread={0}", numread);
					if (numread == 0)
					{
						if (to_read == 0)
						{
							break;
						}
						else
						{
							throw new Exception("client disconnected during post");
						}
					}
					to_read -= numread;
					ms.Write(buf, 0, numread);
				}
				ms.Seek(0, SeekOrigin.Begin);
			}
			Console.WriteLine("get post data end");
			srv.HandlePostRequest(this, new StreamReader(ms));

		}

		public void writeSuccess(string content_type = "text/html")
		{
			outputStream.WriteLine("HTTP/1.0 200 OK");
			outputStream.WriteLine("Content-Type: " + content_type);
			outputStream.WriteLine("Connection: close");
			outputStream.WriteLine("");
		}

		public void writeFailure()
		{
			outputStream.WriteLine("HTTP/1.0 404 File not found");
			outputStream.WriteLine("Connection: close");
			outputStream.WriteLine("");
		}
	}
}