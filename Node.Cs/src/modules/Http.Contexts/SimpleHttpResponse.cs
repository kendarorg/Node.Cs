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
using System.Web.Caching;
using System.Web;
using System.Web.Routing;
using System.Threading;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using Http.Shared.Contexts;

namespace Http.Contexts
{
	public class SimpleHttpResponse : HttpResponseBase, IHttpResponse
	{
		public object SourceObject { get { return null; } }
		public override void AddCacheItemDependency(String cacheKey)
		{
		}
		
		public override void AddCacheItemDependencies(ArrayList cacheKeys)
		{
		}
		
		public override void AddCacheItemDependencies(String[] cacheKeys)
		{
		}
		
		public override void AddCacheDependency(params CacheDependency[] dependencies)
		{
		}
		
		public override void AddFileDependency(String filename)
		{
		}
		
		public override void AddFileDependencies(ArrayList filenames)
		{
		}
		
		public override void AddFileDependencies(String[] filenames)
		{
		}
		
		public override void AddHeader(String name, String value)
		{
			_headers.Add(name, value);
		}

		public void InitializeUnsettable()
		{
			
		}

		public override void AppendCookie(HttpCookie cookie)
		{
			_cookies.Add(cookie);
		}
		
		public override void AppendHeader(String name, String value)
		{
			_headers.Add(name, value);
		}
		
		public override void AppendToLog(String param)
		{
		}
		
		public override String ApplyAppPathModifier(String virtualPath)
		{
			return null;
		}
		
		public override IAsyncResult BeginFlush(AsyncCallback callback, Object state)
		{
			return null;
		}
		
		public override void BinaryWrite(Byte[] buffer)
		{
			_outputStream.Write(buffer, 0, buffer.Length);
		}
		
		public override void Clear()
		{
		}
		
		public override void ClearContent()
		{
		}
		
		public override void ClearHeaders()
		{
		}
		
		public override void Close()
		{
			if (_outputStream != null)
			{
				_outputStream.Close();
			}
		}
		
		public override void DisableKernelCache()
		{
		}
		
		public override void DisableUserCache()
		{
		}
		
		public override void End()
		{
		}
		
		public override void EndFlush(IAsyncResult asyncResult)
		{
		}
		
		public override void Flush()
		{
		}
		
		public override void Pics(String value)
		{
		}
		
		public override void Redirect(String url)
		{
		}
		
		public override void Redirect(String url, Boolean endResponse)
		{
		}
		
		public override void RedirectToRoute(Object routeValues)
		{
		}
		
		public override void RedirectToRoute(String routeName)
		{
		}
		
		public override void RedirectToRoute(RouteValueDictionary routeValues)
		{
		}
		
		public override void RedirectToRoute(String routeName, Object routeValues)
		{
		}
		
		public override void RedirectToRoute(String routeName, RouteValueDictionary routeValues)
		{
		}
		
		public override void RedirectToRoutePermanent(Object routeValues)
		{
		}
		
		public override void RedirectToRoutePermanent(String routeName)
		{
		}
		
		public override void RedirectToRoutePermanent(RouteValueDictionary routeValues)
		{
		}
		
		public override void RedirectToRoutePermanent(String routeName, Object routeValues)
		{
		}
		
		public override void RedirectToRoutePermanent(String routeName, RouteValueDictionary routeValues)
		{
		}
		
		public override void RedirectPermanent(String url)
		{
		}
		
		public override void RedirectPermanent(String url, Boolean endResponse)
		{
		}
		
		public override void RemoveOutputCacheItem(String path)
		{
		}
		
		public override void RemoveOutputCacheItem(String path, String providerName)
		{
		}
		
		public override void SetCookie(HttpCookie cookie)
		{
			_cookies.Set(cookie);
		}
		
		public override void TransmitFile(String filename)
		{
		}
		
		public override void TransmitFile(String filename, Int64 offset, Int64 length)
		{
		}
		
		public override void Write(Char ch)
		{
			if (ContentEncoding == null) ContentEncoding = Encoding.UTF8;
			var bytes = ContentEncoding.GetBytes(new[] { ch });
			_outputStream.Write(bytes, 0, bytes.Length);
		}
		
		public override void Write(Char[] buffer, Int32 index, Int32 count)
		{
			if (ContentEncoding == null) ContentEncoding = Encoding.UTF8;
			var bytes = ContentEncoding.GetBytes(buffer);
			_outputStream.Write(bytes, index, count);
		}
		
		public override void Write(Object obj)
		{
		}
		
		public override void Write(String s)
		{
			if (ContentEncoding == null) ContentEncoding = Encoding.UTF8;
			var bytes = ContentEncoding.GetBytes(s);
			_outputStream.Write(bytes, 0, bytes.Length);
		}
		
		public override void WriteFile(String filename)
		{
		}
		
		public override void WriteFile(String filename, Boolean readIntoMemory)
		{
		}
		
		public override void WriteFile(String filename, Int64 offset, Int64 size)
		{
		}
		
		public override void WriteFile(IntPtr fileHandle, Int64 offset, Int64 size)
		{
		}
		
		public override void WriteSubstitution(HttpResponseSubstitutionCallback callback)
		{
		}
		public override Boolean Buffer { get; set; }
		
		public override Boolean BufferOutput { get; set; }
		
		private HttpCachePolicyBase _cache;
		
		public override HttpCachePolicyBase Cache { get {return _cache; } }
		
		public void SetCache(HttpCachePolicyBase val)
		{
			_cache=val;
		}
		
		public override String CacheControl { get; set; }
		
		public override String Charset { get; set; }
		
		private CancellationToken _clientDisconnectedToken;
		
		public override CancellationToken ClientDisconnectedToken { get {return _clientDisconnectedToken; } }
		
		public void SetClientDisconnectedToken(CancellationToken val)
		{
			_clientDisconnectedToken=val;
		}
		
		public override Encoding ContentEncoding { get; set; }
		
		public override String ContentType { get; set; }
		
		private HttpCookieCollection _cookies= new HttpCookieCollection();
		
		public override HttpCookieCollection Cookies { get {return _cookies; } }
		
		public void SetCookies(HttpCookieCollection val)
		{
			_cookies=val;
		}
		
		public override Int32 Expires { get; set; }
		
		public override DateTime ExpiresAbsolute { get; set; }
		
		public override Stream Filter { get; set; }
		
		private NameValueCollection _headers= new NameValueCollection();
		
		public override NameValueCollection Headers { get {return _headers; } }
		
		public void SetHeaders(NameValueCollection val)
		{
			_headers=val;
		}
		
		public override Encoding HeaderEncoding { get; set; }
		
		private Boolean _isClientConnected;
		
		public override Boolean IsClientConnected { get {return _isClientConnected; } }
		
		public void SetIsClientConnected(Boolean val)
		{
			_isClientConnected=val;
		}
		
		private Boolean _isRequestBeingRedirected;
		
		public override Boolean IsRequestBeingRedirected { get {return _isRequestBeingRedirected; } }
		
		public void SetIsRequestBeingRedirected(Boolean val)
		{
			_isRequestBeingRedirected=val;
		}
		
		public override TextWriter Output { get; set; }
		
		private Stream _outputStream= new MemoryStream();
		
		public override Stream OutputStream { get {return _outputStream; } }
		
		public void SetOutputStream(Stream val)
		{
			_outputStream=val;
		}
		
		public override String RedirectLocation { get; set; }
		
		public override String Status { get; set; }
		
		public override Int32 StatusCode { get; set; }
		
		public override String StatusDescription { get; set; }
		
		public override Int32 SubStatusCode { get; set; }
		
		private Boolean _supportsAsyncFlush;
		
		public override Boolean SupportsAsyncFlush { get {return _supportsAsyncFlush; } }
		
		public void SetSupportsAsyncFlush(Boolean val)
		{
			_supportsAsyncFlush=val;
		}
		
		public override Boolean SuppressContent { get; set; }
		
		public override Boolean SuppressFormsAuthenticationRedirect { get; set; }
		
		public override Boolean TrySkipIisCustomErrors { get; set; }

		public void Close(byte[] data, bool willblock = true)
		{
			_outputStream.Write(data, 0, data.Length);
		}
	}
}