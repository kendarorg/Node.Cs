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
using GenericHelpers;
using Http.Shared.Contexts;
using Http.Shared.Routing;

namespace HttpMvc.Controllers
{
	public class UrlHelper
	{
		private readonly IHttpContext _context;
		private readonly IRoutingHandler _routingHandler;

		public UrlHelper(IHttpContext context, IRoutingHandler routingHandler)
		{
			_context = context;
			_routingHandler = routingHandler;
		}

		public string RealRootAddress
		{
			get
			{
#if AAA
				var dataPath = NodeCsSettings.Settings.Listener.RootDir ?? string.Empty;
				dataPath = dataPath.Trim('/');
				if (dataPath.Length > 0)
				{
					dataPath = "/" + dataPath;
				}
#else
				if(new Random().Next()>=int.MinValue)
				throw new NotImplementedException();
#endif
				var dataPath = "";
				var url = _context.Request.Url;
				var port = url.Port.ToString();
				if (port != "80")
				{
					port = ":" + port;
				}
				else
				{
					port = "";
				}

				var realUrl = string.Format("{0}://{1}{2}/{3}",
					url.Scheme,
					url.Host,
					port,
					dataPath
					);
				return realUrl.TrimEnd('/');
			}
		}

		public string Content(string content)
		{
			//~/Scripts/jquery.validate.min.js
			if (content == "~") return "/";
			if (IsLocalUrl(content))
			{
				return RealRootAddress + "/" + content.Trim('~').TrimStart('/');
			}
			return content;
		}

		public string MergeUrl(string toMerge)
		{
			if (toMerge.StartsWith("~"))
			{
				toMerge = toMerge.Substring(1);
			}
			if (toMerge.StartsWith("/"))
			{
				toMerge = toMerge.TrimStart('/');
				return RealRootAddress + "/" + toMerge;
			}
			return toMerge;
		}

		public bool IsLocalUrl(string returnUrl)
		{
			if (returnUrl.StartsWith("~/") || returnUrl.StartsWith("/"))
			{
				return true;
			}
			return false;
		}

		public string Action(string action, dynamic routeValues)
		{
			var controller = _context.RouteParams["controller"].ToString();
			return Action(action, controller, routeValues);
		}

		public string Action(string action, string controller, dynamic routeValues)
		{
			var result = ReflectionUtils.ObjectToDictionary(routeValues);
			if (!result.ContainsKey("controller"))
			{
				result["controller"] = controller;
			}
			if (result.ContainsKey("action"))
			{
				result["action"] = action;
			}
			else
			{
				result.Add("action", action);
			}

			var link = _routingHandler.ResolveFromParams(result);
			return link;
		}

		public string Action(string action, string controller = null)
		{
			if (controller == null)
			{
				throw new NotImplementedException();
			}
			var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
			             {
				             {"controller", controller},
				             {"action", action}
			             };
			var link = _routingHandler.ResolveFromParams(result);
			return link;
		}
	}
}
