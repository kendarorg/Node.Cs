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


using System.Linq;
using System.Web;
using System;
using System.Web.WebSockets;
using System.Threading.Tasks;
using System.Globalization;
using System.Web.SessionState;
using System.Web.Configuration;
using System.Web.Caching;
using System.Collections;
using System.Web.Instrumentation;
using System.Web.Profile;
using System.Security.Principal;
using System.Collections.Generic;
using Http.Shared.Contexts;

namespace Http.Contexts
{
	public class SimpleHttpContext : HttpContextBase, IHttpContext
	{
		public SimpleHttpContext()
		{
			RouteParams = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		}

		public IHttpContext RootContext
		{
			get
			{
				if (Parent == null) return this;
				return Parent.RootContext;
			}
		}

		public IHttpContext Parent
		{
			get { return null; }
		}

		public void ForceRootDir(string rootDir)
		{
			RootDir = rootDir;
		}
		public string RootDir { get; private set; }

		public void ForceHeader(string key, string value)
		{
			var nameValueCollection = _request.Headers;
			if (!nameValueCollection.AllKeys.ToArray().Contains(key))
			{
				nameValueCollection.Add(key, value);
			}
			else
			{
				nameValueCollection.Set(key, value);
			}
		}

		public Dictionary<string, object> RouteParams { get; set; }

		public override ISubscriptionToken AddOnRequestCompleted(Action<HttpContextBase> callback)
		{
			return null;
		}

		public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc)
		{
		}

		public override void AcceptWebSocketRequest(Func<AspNetWebSocketContext, Task> userFunc, AspNetWebSocketOptions options)
		{
		}

		public override void AddError(Exception errorInfo)
		{
		}

		public override void ClearError()
		{
		}

		public override ISubscriptionToken DisposeOnPipelineCompleted(IDisposable target)
		{
			return null;
		}

		public override Object GetGlobalResourceObject(String classKey, String resourceKey)
		{
			return null;
		}

		public override Object GetGlobalResourceObject(String classKey, String resourceKey, CultureInfo culture)
		{
			return null;
		}

		public override Object GetLocalResourceObject(String virtualPath, String resourceKey)
		{
			return null;
		}

		public override Object GetLocalResourceObject(String virtualPath, String resourceKey, CultureInfo culture)
		{
			return null;
		}

		public override Object GetSection(String sectionName)
		{
			return null;
		}

		public override void RemapHandler(IHttpHandler handler)
		{
		}

		public override void RewritePath(String path)
		{
		}

		public override void RewritePath(String path, Boolean rebaseClientPath)
		{
		}

		public override void RewritePath(String filePath, String pathInfo, String queryString)
		{
		}

		public override void RewritePath(String filePath, String pathInfo, String queryString, Boolean setClientFilePath)
		{
		}

		public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
		{
		}

		public override Object GetService(Type serviceType)
		{
			return null;
		}
		private Exception[] _allErrors = { };

		public override Exception[] AllErrors { get { return _allErrors; } }

		public void SetAllErrors(Exception[] val)
		{
			_allErrors = val;
		}

		public override Boolean AllowAsyncDuringSyncStages { get; set; }

		private HttpApplicationStateBase _application;

		public override HttpApplicationStateBase Application { get { return _application; } }

		public void SetApplication(HttpApplicationStateBase val)
		{
			_application = val;
		}

		public override HttpApplication ApplicationInstance { get; set; }

		public override AsyncPreloadModeFlags AsyncPreloadMode { get; set; }

		private Cache _cache = new Cache();

		public override Cache Cache { get { return _cache; } }

		public void SetCache(Cache val)
		{
			_cache = val;
		}

		private IHttpHandler _currentHandler;

		public override IHttpHandler CurrentHandler { get { return _currentHandler; } }

		public void SetCurrentHandler(IHttpHandler val)
		{
			_currentHandler = val;
		}

		private RequestNotification _currentNotification;

		public override RequestNotification CurrentNotification { get { return _currentNotification; } }

		public void SetCurrentNotification(RequestNotification val)
		{
			_currentNotification = val;
		}

		private Exception _error = new Exception();

		public override Exception Error { get { return _error; } }

		public void SetError(Exception val)
		{
			_error = val;
		}

		public override IHttpHandler Handler { get; set; }

		private Boolean _isCustomErrorEnabled;

		public override Boolean IsCustomErrorEnabled { get { return _isCustomErrorEnabled; } }

		public void SetIsCustomErrorEnabled(Boolean val)
		{
			_isCustomErrorEnabled = val;
		}

		private Boolean _isDebuggingEnabled;

		public override Boolean IsDebuggingEnabled { get { return _isDebuggingEnabled; } }

		public void SetIsDebuggingEnabled(Boolean val)
		{
			_isDebuggingEnabled = val;
		}

		private Boolean _isPostNotification;

		public override Boolean IsPostNotification { get { return _isPostNotification; } }

		public void SetIsPostNotification(Boolean val)
		{
			_isPostNotification = val;
		}

		private Boolean _isWebSocketRequest;

		public override Boolean IsWebSocketRequest { get { return _isWebSocketRequest; } }

		public void SetIsWebSocketRequest(Boolean val)
		{
			_isWebSocketRequest = val;
		}

		private Boolean _isWebSocketRequestUpgrading;

		public override Boolean IsWebSocketRequestUpgrading { get { return _isWebSocketRequestUpgrading; } }

		public void SetIsWebSocketRequestUpgrading(Boolean val)
		{
			_isWebSocketRequestUpgrading = val;
		}

		private IDictionary _items = new Dictionary<string, object>();

		public override IDictionary Items { get { return _items; } }

		public void SetItems(IDictionary val)
		{
			_items = val;
		}

		private PageInstrumentationService _pageInstrumentation = new PageInstrumentationService();

		public override PageInstrumentationService PageInstrumentation { get { return _pageInstrumentation; } }

		public void SetPageInstrumentation(PageInstrumentationService val)
		{
			_pageInstrumentation = val;
		}

		private IHttpHandler _previousHandler;

		public override IHttpHandler PreviousHandler { get { return _previousHandler; } }

		public void SetPreviousHandler(IHttpHandler val)
		{
			_previousHandler = val;
		}

		private ProfileBase _profile = new ProfileBase();

		public override ProfileBase Profile { get { return _profile; } }

		public void SetProfile(ProfileBase val)
		{
			_profile = val;
		}

		private HttpRequestBase _request;

		public override HttpRequestBase Request { get { return _request; } }

		public void SetRequest(HttpRequestBase val)
		{
			_request = val;
			if (_response != null)
			{
				_response.ContentEncoding = _request.ContentEncoding;
			}
		}

		private HttpResponseBase _response;

		public override HttpResponseBase Response { get { return _response; } }

		public void SetResponse(HttpResponseBase val)
		{
			_response = val;
			if (_request != null)
			{
				_response.ContentEncoding = _request.ContentEncoding;
			}
		}

		private HttpServerUtilityBase _server;

		public override HttpServerUtilityBase Server { get { return _server; } }

		public void SetServer(HttpServerUtilityBase val)
		{
			_server = val;
		}

		private HttpSessionStateBase _session = new SimpleHttpSessionState();

		public override HttpSessionStateBase Session { get { return _session; } }

		public void SetSession(HttpSessionStateBase val)
		{
			_session = val;
		}

		public override Boolean SkipAuthorization { get; set; }

		private DateTime _timestamp;

		public override DateTime Timestamp { get { return _timestamp; } }

		public void SetTimestamp(DateTime val)
		{
			_timestamp = val;
		}

		public override Boolean ThreadAbortOnTimeout { get; set; }

		private TraceContext _trace;

		public override TraceContext Trace { get { return _trace; } }

		public void SetTrace(TraceContext val)
		{
			_trace = val;
		}

		public override IPrincipal User { get; set; }

		private String _webSocketNegotiatedProtocol = "";

		public override String WebSocketNegotiatedProtocol { get { return _webSocketNegotiatedProtocol; } }

		public void SetWebSocketNegotiatedProtocol(String val)
		{
			_webSocketNegotiatedProtocol = val;
		}

		private IList<String> _webSocketRequestedProtocols = new List<string>();

		public override IList<String> WebSocketRequestedProtocols { get { return _webSocketRequestedProtocols; } }

		public void SetWebSocketRequestedProtocols(IList<String> val)
		{
			_webSocketRequestedProtocols = val;
		}

		public Task InitializeWebSocket()
		{
			throw new NotImplementedException();
		}
	}
}