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
using System.Collections.ObjectModel;
using System.Linq;
using ClassWrapper;
using Http.Shared.Controllers;
using Http.Shared.Routing;

namespace Http.Controllers
{
	public class ControllerWrapperDescriptor
	{
		private readonly Dictionary<string, Dictionary<string, ReadOnlyCollection<MethodWrapperDescriptor>>> _methods;
		private readonly ClassWrapperDescriptor _cd;

		public ClassWrapperDescriptor WrapperDescriptor
		{
			get { return _cd; }
		}

		public ControllerWrapperDescriptor(ClassWrapperDescriptor cd)
		{
			_cd = cd;
			_methods = new Dictionary<string, Dictionary<string, ReadOnlyCollection<MethodWrapperDescriptor>>>(StringComparer.OrdinalIgnoreCase);
			InitializeMethodGroups();
		}

		public ControllerWrapperInstance CreateWrapper(IController classInstance)
		{
			return new ControllerWrapperInstance(this, _cd.CreateWrapper(classInstance));
		}

		public IEnumerable<MethodWrapperDescriptor> GetMethodGroup(string action, string verb)
		{
			if (!_methods.ContainsKey(verb) || !_methods[verb].ContainsKey(action))
			{
				if (!_methods.ContainsKey("ALL") || !_methods["ALL"].ContainsKey(action)) yield break;
				verb = "ALL";
			}
			foreach (var method in _methods[verb][action])
			{
				yield return method;
			}
		}

		private void InitializeMethodGroups()
		{
			foreach (var methodName in _cd.Methods)
			{
				foreach (var method in _cd.GetMethodGroup(methodName))
				{
					if (method.IsVoid || method.Visibility != ItemVisibility.Public) continue;
					var verb = "GET";
					var action = methodName;
					var attr = (HttpRequestTypeAttribute)method.Attributes.FirstOrDefault(a => a is HttpRequestTypeAttribute);
					var cho = (ChildActionOnly)method.Attributes.FirstOrDefault(a => a is ChildActionOnly);

					var attrName = (ActionName)method.Attributes.FirstOrDefault(a => a is ActionName);
					if (attr != null)
					{
						verb = attr.Verb;
						if (attrName != null)
						{
							action = attrName.Name;
						}
						else if (!string.IsNullOrWhiteSpace(attr.Action))
						{
							action = attr.Action;
						}
					}
					if (cho != null)
					{
						verb = "ALL";
					}
					SetupMethod(verb, action, method);
				}
			}
		}

		private void SetupMethod(string verb, string action, MethodWrapperDescriptor method)
		{
			if (!_methods.ContainsKey(verb))
			{
				_methods.Add(verb, new Dictionary<string, ReadOnlyCollection<MethodWrapperDescriptor>>(StringComparer.OrdinalIgnoreCase));
			}
			if (!_methods[verb].ContainsKey(action))
			{
				_methods[verb].Add(action, new ReadOnlyCollection<MethodWrapperDescriptor>(new List<MethodWrapperDescriptor>()));
			}
			var methods = new List<MethodWrapperDescriptor>(_methods[verb][action]);
			methods.Add(method);
			_methods[verb][action] = new ReadOnlyCollection<MethodWrapperDescriptor>(methods);
		}
	}
}
