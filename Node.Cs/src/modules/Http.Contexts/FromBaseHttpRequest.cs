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


using System.Collections.Generic;
using System.Web;
using System;
using System.IO;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Collections.Specialized;
using System.Security.Principal;
using System.Web.Routing;
using System.Threading;
using Http.Shared.Contexts;

namespace Http.Contexts
{
	public class FromBaseHttpRequest:HttpRequestBase,IHttpRequest
	{
		public FromBaseHttpRequest() {}

		public object SourceObject { get { return _httpRequestBase; } }
		
		private readonly HttpRequestBase _httpRequestBase;
		
		public FromBaseHttpRequest(HttpRequestBase httpRequestBase)
		{
			_httpRequestBase=httpRequestBase;
			InitializeUnsettable();
		}

		private void InitializeUnsettable()
		{
			
		}

		public Dictionary<string, string> ItemDictionary { get { throw new NotImplementedException(); } }

		public override void Abort()
		{
			_httpRequestBase.Abort();
		}
		
		public override Byte[] BinaryRead(Int32 count)
		{
			return _httpRequestBase.BinaryRead(count);
		}
		
		public override Stream GetBufferedInputStream()
		{
			return _httpRequestBase.GetBufferedInputStream();
		}
		
		public override Stream GetBufferlessInputStream()
		{
			return _httpRequestBase.GetBufferlessInputStream();
		}
		
		public override Stream GetBufferlessInputStream(Boolean disableMaxRequestLength)
		{
			return _httpRequestBase.GetBufferlessInputStream(disableMaxRequestLength);
		}
		
		public override void InsertEntityBody()
		{
			_httpRequestBase.InsertEntityBody();
		}
		
		public override void InsertEntityBody(Byte[] buffer, Int32 offset, Int32 count)
		{
			_httpRequestBase.InsertEntityBody(buffer, offset, count);
		}
		
		public override Int32[] MapImageCoordinates(String imageFieldName)
		{
			return _httpRequestBase.MapImageCoordinates(imageFieldName);
		}
		
		public override Double[] MapRawImageCoordinates(String imageFieldName)
		{
			return _httpRequestBase.MapRawImageCoordinates(imageFieldName);
		}
		
		public override String MapPath(String virtualPath)
		{
			return _httpRequestBase.MapPath(virtualPath);
		}
		
		public override String MapPath(String virtualPath, String baseVirtualDir, Boolean allowCrossAppMapping)
		{
			return _httpRequestBase.MapPath(virtualPath, baseVirtualDir, allowCrossAppMapping);
		}
		
		public override void ValidateInput()
		{
			_httpRequestBase.ValidateInput();
		}
		
		public override void SaveAs(String filename, Boolean includeHeaders)
		{
			_httpRequestBase.SaveAs(filename, includeHeaders);
		}
		
		public override String[] AcceptTypes { get {return _httpRequestBase.AcceptTypes; } }

		public void SetAcceptTypes(String[] val)
		{
		}
		
		
		public override String ApplicationPath { get {return _httpRequestBase.ApplicationPath; } }
		
		public void SetApplicationPath(String val)
		{
		}
		
		
		public override String AnonymousID { get {return _httpRequestBase.AnonymousID; } }
		
		public void SetAnonymousID(String val)
		{
		}
		
		
		public override String AppRelativeCurrentExecutionFilePath { get {return _httpRequestBase.AppRelativeCurrentExecutionFilePath; } }
		
		public void SetAppRelativeCurrentExecutionFilePath(String val)
		{
		}
		
		
		public override HttpBrowserCapabilitiesBase Browser { get {return _httpRequestBase.Browser; } }
		
		public void SetBrowser(HttpBrowserCapabilitiesBase val)
		{
		}
		
		
		public override ChannelBinding HttpChannelBinding { get {return _httpRequestBase.HttpChannelBinding; } }
		
		public void SetHttpChannelBinding(ChannelBinding val)
		{
		}
		
		
		public override HttpClientCertificate ClientCertificate { get {return _httpRequestBase.ClientCertificate; } }
		
		public void SetClientCertificate(HttpClientCertificate val)
		{
		}
		
		public override Encoding ContentEncoding
		{
			set
			{
				_httpRequestBase.ContentEncoding=value;
			}
			get
			{
				return _httpRequestBase.ContentEncoding;
			}
		}
		
		
		public override Int32 ContentLength { get {return _httpRequestBase.ContentLength; } }
		
		public void SetContentLength(Int32 val)
		{
		}
		
		public override String ContentType
		{
			set
			{
				_httpRequestBase.ContentType=value;
			}
			get
			{
				return _httpRequestBase.ContentType;
			}
		}
		
		
		public override HttpCookieCollection Cookies { get {return _httpRequestBase.Cookies; } }
		
		public void SetCookies(HttpCookieCollection val)
		{
		}
		
		
		public override String CurrentExecutionFilePath { get {return _httpRequestBase.CurrentExecutionFilePath; } }
		
		public void SetCurrentExecutionFilePath(String val)
		{
		}
		
		
		public override String CurrentExecutionFilePathExtension { get {return _httpRequestBase.CurrentExecutionFilePathExtension; } }
		
		public void SetCurrentExecutionFilePathExtension(String val)
		{
		}
		
		
		public override String FilePath { get {return _httpRequestBase.FilePath; } }
		
		public void SetFilePath(String val)
		{
		}
		
		
		public override HttpFileCollectionBase Files { get {return _httpRequestBase.Files; } }
		
		public void SetFiles(HttpFileCollectionBase val)
		{
		}
		
		public override Stream Filter
		{
			set
			{
				_httpRequestBase.Filter=value;
			}
			get
			{
				return _httpRequestBase.Filter;
			}
		}
		
		
		public override NameValueCollection Form { get {return _httpRequestBase.Form; } }
		
		public void SetForm(NameValueCollection val)
		{
		}
		
		
		public override String HttpMethod { get {return _httpRequestBase.HttpMethod; } }
		
		public void SetHttpMethod(String val)
		{
		}
		
		
		public override Stream InputStream { get {return _httpRequestBase.InputStream; } }
		
		public void SetInputStream(Stream val)
		{
		}
		
		
		public override Boolean IsAuthenticated { get {return _httpRequestBase.IsAuthenticated; } }
		
		public void SetIsAuthenticated(Boolean val)
		{
		}
		
		
		public override Boolean IsLocal { get {return _httpRequestBase.IsLocal; } }
		
		public void SetIsLocal(Boolean val)
		{
		}
		
		
		public override Boolean IsSecureConnection { get {return _httpRequestBase.IsSecureConnection; } }
		
		public void SetIsSecureConnection(Boolean val)
		{
		}
		
		
		public override WindowsIdentity LogonUserIdentity { get {return _httpRequestBase.LogonUserIdentity; } }
		
		public void SetLogonUserIdentity(WindowsIdentity val)
		{
		}
		
		
		public override NameValueCollection Params { get {return _httpRequestBase.Params; } }
		
		public void SetParams(NameValueCollection val)
		{
		}
		
		
		public override String Path { get {return _httpRequestBase.Path; } }
		
		public void SetPath(String val)
		{
		}
		
		
		public override String PathInfo { get {return _httpRequestBase.PathInfo; } }
		
		public void SetPathInfo(String val)
		{
		}
		
		
		public override String PhysicalApplicationPath { get {return _httpRequestBase.PhysicalApplicationPath; } }
		
		public void SetPhysicalApplicationPath(String val)
		{
		}
		
		
		public override String PhysicalPath { get {return _httpRequestBase.PhysicalPath; } }
		
		public void SetPhysicalPath(String val)
		{
		}
		
		
		public override String RawUrl { get {return _httpRequestBase.RawUrl; } }
		
		public void SetRawUrl(String val)
		{
		}
		
		
		public override ReadEntityBodyMode ReadEntityBodyMode { get {return _httpRequestBase.ReadEntityBodyMode; } }
		
		public void SetReadEntityBodyMode(ReadEntityBodyMode val)
		{
		}
		
		public override RequestContext RequestContext
		{
			set
			{
				_httpRequestBase.RequestContext=value;
			}
			get
			{
				return _httpRequestBase.RequestContext;
			}
		}
		
		public override String RequestType
		{
			set
			{
				_httpRequestBase.RequestType=value;
			}
			get
			{
				return _httpRequestBase.RequestType;
			}
		}
		
		
		public override NameValueCollection ServerVariables { get {return _httpRequestBase.ServerVariables; } }
		
		public void SetServerVariables(NameValueCollection val)
		{
		}
		
		
		public override CancellationToken TimedOutToken { get {return _httpRequestBase.TimedOutToken; } }
		
		public void SetTimedOutToken(CancellationToken val)
		{
		}
		
		
		public override Int32 TotalBytes { get {return _httpRequestBase.TotalBytes; } }
		
		public void SetTotalBytes(Int32 val)
		{
		}
		
		
		public override UnvalidatedRequestValuesBase Unvalidated { get {return _httpRequestBase.Unvalidated; } }
		
		public void SetUnvalidated(UnvalidatedRequestValuesBase val)
		{
		}
		
		
		public override Uri Url { get {return _httpRequestBase.Url; } }
		
		public void SetUrl(Uri val)
		{
		}
		
		
		public override Uri UrlReferrer { get {return _httpRequestBase.UrlReferrer; } }
		
		public void SetUrlReferrer(Uri val)
		{
		}
		
		
		public override String UserAgent { get {return _httpRequestBase.UserAgent; } }
		
		public void SetUserAgent(String val)
		{
		}
		
		
		public override String[] UserLanguages { get {return _httpRequestBase.UserLanguages; } }
		
		public void SetUserLanguages(String[] val)
		{
		}
		
		
		public override String UserHostAddress { get {return _httpRequestBase.UserHostAddress; } }
		
		public void SetUserHostAddress(String val)
		{
		}
		
		
		public override String UserHostName { get {return _httpRequestBase.UserHostName; } }
		
		public void SetUserHostName(String val)
		{
		}
		
		
		public override NameValueCollection Headers { get {return _httpRequestBase.Headers; } }
		
		public void SetHeaders(NameValueCollection val)
		{
		}
		
		
		public override NameValueCollection QueryString { get {return _httpRequestBase.QueryString; } }
		
		public void SetQueryString(NameValueCollection val)
		{
		}


		public override string this[string key]
		{
			get { return _httpRequestBase[key]; }
		}
	}
}