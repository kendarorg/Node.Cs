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
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Caching;
using System.Web.Routing;

namespace Http.Shared.Contexts
{
	public interface IHttpResponse
	{
		void AddCacheItemDependency(String cacheKey);
		void AddCacheItemDependencies(ArrayList cacheKeys);
		void AddCacheItemDependencies(String[] cacheKeys);
		void AddCacheDependency(params CacheDependency[] dependencies);
		void AddFileDependency(String filename);
		void AddFileDependencies(ArrayList filenames);
		void AddFileDependencies(String[] filenames);
		void AddHeader(String name, String value);
		void AppendHeader(String name, String value);
		void AppendToLog(String param);
		String ApplyAppPathModifier(String virtualPath);
		IAsyncResult BeginFlush(AsyncCallback callback, Object state);
		void BinaryWrite(Byte[] buffer);
		void Clear();
		void ClearContent();
		void ClearHeaders();
		void Close();
		void Close(byte[] data, bool willblock = false);
		void DisableKernelCache();
		void DisableUserCache();
		void End();
		void EndFlush(IAsyncResult asyncResult);
		void Flush();
		void Pics(String value);
		void Redirect(String url);
		void RedirectToRoute(Object routeValues);
		void RedirectToRoute(String routeName);
		void RedirectToRoute(RouteValueDictionary routeValues);
		void RedirectToRoute(String routeName, Object routeValues);
		void RedirectToRoute(String routeName, RouteValueDictionary routeValues);
		void RedirectToRoutePermanent(Object routeValues);
		void RedirectToRoutePermanent(String routeName);
		void RedirectToRoutePermanent(RouteValueDictionary routeValues);
		void RedirectToRoutePermanent(String routeName, Object routeValues);
		void RedirectToRoutePermanent(String routeName, RouteValueDictionary routeValues);
		void RedirectPermanent(String url);
		void RedirectPermanent(String url, Boolean endResponse);
		void RemoveOutputCacheItem(String path);
		void RemoveOutputCacheItem(String path, String providerName);
		void TransmitFile(String filename);
		void TransmitFile(String filename, Int64 offset, Int64 length);
		void Write(Char ch);
		void Write(Char[] buffer, Int32 index, Int32 count);
		void Write(Object obj);
		void Write(String s);
		void WriteFile(String filename);
		void WriteFile(String filename, Boolean readIntoMemory);
		void WriteFile(String filename, Int64 offset, Int64 size);
		void WriteFile(IntPtr fileHandle, Int64 offset, Int64 size);
		void WriteSubstitution(HttpResponseSubstitutionCallback callback);
		Boolean Buffer { set; get; }
		Boolean BufferOutput { set; get; }
		HttpCachePolicyBase Cache { get; }
		String CacheControl { set; get; }
		String Charset { set; get; }
		CancellationToken ClientDisconnectedToken { get; }
		Encoding ContentEncoding { set; get; }
		String ContentType { set; get; }
		Int32 Expires { set; get; }
		DateTime ExpiresAbsolute { set; get; }
		Stream Filter { set; get; }
		NameValueCollection Headers { get; }
		Encoding HeaderEncoding { set; get; }
		Boolean IsClientConnected { get; }
		Boolean IsRequestBeingRedirected { get; }
		TextWriter Output { set; get; }
		Stream OutputStream { get; }
		String RedirectLocation { set; get; }
		String Status { set; get; }
		Int32 StatusCode { set; get; }
		String StatusDescription { set; get; }
		Int32 SubStatusCode { set; get; }
		Boolean SupportsAsyncFlush { get; }
		Boolean SuppressContent { set; get; }
		Boolean SuppressFormsAuthenticationRedirect { set; get; }
		Boolean TrySkipIisCustomErrors { set; get; }
		HttpCookieCollection Cookies { get; }
		void SetCache(HttpCachePolicyBase val);
		void SetClientDisconnectedToken(CancellationToken val);
		void SetHeaders(NameValueCollection val);
		void SetIsClientConnected(Boolean val);
		void SetIsRequestBeingRedirected(Boolean val);
		void SetOutputStream(Stream val);
		void SetSupportsAsyncFlush(Boolean val);
		void InitializeUnsettable();
		void AppendCookie(HttpCookie cookie);
		void Redirect(String url, Boolean endResponse);
		void SetCookie(HttpCookie cookie);
		void SetCookies(HttpCookieCollection val);
	}
}