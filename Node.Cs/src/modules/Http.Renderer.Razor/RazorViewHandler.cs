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
using Http.Contexts;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using HttpMvc;
using HttpMvc.Controllers;
using NodeCs.Shared;
using CoroutinesLib.Shared;

namespace Http.Renderer.Razor
{
	public class RazorViewHandler:IResponseHandler
	{
		private readonly HttpModule _httpModule;
		private MvcModule _mvcModule;

		public RazorViewHandler()
		{
			_mvcModule = NodeRoot.GetModule("http.mvc") as MvcModule;
			_httpModule = ServiceLocator.Locator.Resolve<HttpModule>();
		}
		public IEnumerable<ICoroutineResult> Handle(IHttpContext context, IResponse response, object viewBag)
		{
			var viewResponse = (ViewResponse)response;
			var view = viewResponse.View ?? context.RouteParams["action"].ToString();
			
			if (view.StartsWith("~"))
			{
				view = view.TrimStart('~');
			}
			else
			{
				var viewsRoot = _mvcModule.GetParameter<string>("views").TrimStart('~').TrimStart('/');
				viewsRoot = viewsRoot.Replace("{controller}", context.RouteParams["controller"].ToString());
				viewsRoot = viewsRoot.Replace("{action}", view.Trim('/'));
				if (context.RouteParams.ContainsKey("area"))
				{
					viewsRoot = viewsRoot.Replace("{area}", context.RouteParams["area"].ToString());
				}
				view = "/" + viewsRoot.Trim('/');
				view = view.TrimStart('~');
			}

			if (!view.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase))
			{
				view += ".cshtml";
			}
			var wrappedContext = new WrappedHttpContext(context);
			var wrappedRequest = (IHttpRequest)wrappedContext.Request;
			wrappedRequest.SetInputStream(context.Request.InputStream);
			wrappedRequest.SetUrl(new Uri(view,UriKind.RelativeOrAbsolute));
			var wrappedResponse = (IHttpResponse)wrappedContext.Response;
			wrappedResponse.SetOutputStream(context.Response.OutputStream);
			yield return _httpModule.ExecuteRequestInternal(wrappedContext, viewResponse.Model, new ModelStateDictionary(), viewBag);
		}

		public bool CanHandle(IResponse response)
		{
			var viewResponse = response as ViewResponse;
			if (viewResponse == null) return false;
			if (string.IsNullOrWhiteSpace(viewResponse.View)) return false;
			return true;
		}
	}
}