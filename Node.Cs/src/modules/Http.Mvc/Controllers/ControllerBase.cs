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
using System.Dynamic;
using System.Web;
using GenericHelpers;
using Http.Shared.Contexts;
using Http.Shared.Controllers;

namespace HttpMvc.Controllers
{
	public abstract class ControllerBase : MainControllerBase
	{
		protected IResponse NotFound(string url = null)
		{
			return new NotFoundResponse(url);
		}

		public HttpSessionStateBase Session
		{
			get
			{
				return HttpContext.Session;
			}
		}

		private dynamic _dynamicViewDataDictionary;

		private Dictionary<string, object> _viewData;


		protected IResponse Redirect(string url)
		{
			return new RedirectResponse(url);
		}

		public dynamic ViewBag
		{
			get
			{
				if (_dynamicViewDataDictionary == null)
				{
					_dynamicViewDataDictionary = new ExpandoObject();
				}
				return _dynamicViewDataDictionary;
			}
			set { _dynamicViewDataDictionary = value; }
		}

		public Dictionary<string, object> ViewData
		{
			get
			{
				if (_viewData == null)
				{
					_viewData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
				}
				return _viewData;
			}
		}

		public IResponse View(object model)
		{
			var action = HttpContext.RouteParams["action"].ToString();
			return View(action, model);
		}

		public IResponse View(string view, object model)
		{
			return new ViewResponse(view, model, ViewBag);
		}

		public IResponse View()
		{
			return View(new object());
		}

		protected IResponse PartialView(object model)
		{
			var action = (string)HttpContext.RouteParams["action"];
			if (model is string)
			{
				action = model.ToString();
				model = null;
			}
			return PartialView(action, model);
		}

		protected IResponse PartialView(string action, object model)
		{
			return new PartialViewResponse(action, model, ViewBag);
		}

		protected IResponse RedirectToAction(string action, string controller = null)
		{
			return RedirectToAction(action, null, controller);
		}

		protected IResponse RedirectToAction(string action, dynamic value, string controller = null)
		{
			Dictionary<string, object> pars;
			if (value != null)
			{
				pars = ReflectionUtils.ObjectToDictionary(value);
			}
			else
			{
				pars = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			}

			controller = controller ?? (string)HttpContext.RouteParams["controller"];
			if (!pars.ContainsKey("controller"))
			{
				pars.Add("controller", controller);
			}
			if (!pars.ContainsKey("action"))
			{
				pars.Add("action", action);
			}
			//var redirectUrl = GlobalVars.RoutingService.ResolveFromParams(pars);
			//return Redirect(redirectUrl);
			throw new NotImplementedException();
		}
	}
}
