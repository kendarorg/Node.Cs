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
using System.Security.Principal;
using System.Text;
using Http.Shared;
using Http.Shared.Contexts;
using Http.Shared.Controllers;
using Http.Shared.Routing;
using NodeCs.Shared;

namespace HttpMvc.Controllers
{
	public abstract class MainControllerBase : IController
	{
		public ModelStateDictionary ModelState { get; set; }

		
		public IHttpContext HttpContext { get; set; }

		public IResponse JsonResponse(object obj, string contentType = null, Encoding encoding = null)
		{
			var res = new JsonResponse();
			res.Initialize(obj, contentType, encoding);
			return res;
		}

		public IResponse TextResponse(string obj, string contentType = null, Encoding encoding = null)
		{
			var res = new StringResponse();
			res.Initialize(obj, contentType, encoding);
			return res;
		}

		public IResponse XmlResponse(object obj, string contentType = null, Encoding encoding = null)
		{
			var res = new XmlResponse();
			res.Initialize(obj, contentType, encoding);
			return res;
		}

		public IResponse ByteResponse(byte[] obj, string contentType = null, Encoding encoding = null)
		{
			var res = new ByteResponse();
			res.Initialize(obj, contentType, encoding);
			return res;
		}

		public UrlHelper Url { get; set; }

		public IConversionService ConversionService { get; set; }

		public IPrincipal User
		{
			get
			{
				return HttpContext.User;
			}
		}

		protected bool TryUpdateModel(Object model)
		{
			var requestContentType = HttpContext.Request.ContentType;
			if (!ConversionService.HasConverter(requestContentType)) return false;
			var convertedModel = ConversionService.Convert(model.GetType(), requestContentType, HttpContext.Request);
			var old = ValidationService.GetWrapper(model);
			var converted = ValidationService.GetWrapper(convertedModel);

			foreach (var prop in converted.Properties)
			{
				var newProp = converted.GetObject(prop);
				if (newProp != null)
				{
					old.Set(prop, newProp);
				}
			}

			if (!ValidationService.CanValidate(model)) return false;
			var result = ValidationService.ValidateModel(model);
			foreach (var item in result)
			{
				ModelState.AddModelError(item.Property,item.Message);
			}
			return ModelState.IsValid;
		}
	}
}