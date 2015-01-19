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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using ClassWrapper;
using NodeCs.Shared.Attributes;

namespace NodeCs.Shared
{
	public static class ValidationService
	{
		public static ConcurrentDictionary<Type, ClassWrapperDescriptor> _classWrappers =
			new ConcurrentDictionary<Type, ClassWrapperDescriptor>();

		public static void RegisterModelType(Type type)
		{
			if ((type.Namespace != null && type.Namespace.StartsWith("System")) || type.IsValueType) return;
			if (_classWrappers.ContainsKey(type)) return;
			if (typeof(NameValueCollection).IsAssignableFrom(type)) return;
			var cld = new ClassWrapperDescriptor(type);
			cld.Load();
			_classWrappers.TryAdd(type, cld);

			foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				RegisterModelType(property.PropertyType);
			}
		}

		public static bool CanValidate(object model)
		{
			if (model == null) return false;
			var type = model.GetType();
			return _classWrappers.ContainsKey(type);
		}

		public static ClassWrapper.ClassWrapper GetWrapper(object model)
		{
			var type = model.GetType();
			if (!_classWrappers.ContainsKey(type))
			{
				if ((type.Namespace!=null && type.Namespace.StartsWith("System")) || type.IsValueType) return null;
				var clw = new ClassWrapperDescriptor(type);
				clw.Load();
				_classWrappers.AddOrUpdate(type, clw, (a, b) => b);
			}
			return _classWrappers[type].CreateWrapper(model);
		}


		public static ClassWrapperDescriptor GetWrapperDescriptor(object model)
		{
			var type = model.GetType();
			if (model.GetType() == typeof(Type))
			{
				type = (Type)model;
			}
			if (!_classWrappers.ContainsKey(type)) return null;
			return _classWrappers[type];
		}

		/*
		public static object InvokeGetProperty(object model, string property)
		{
			var type = model.GetType();
			var cld = _classWrappers[type];
			return cld.(property);
		}*/

		public static IEnumerable<ValidationResult> ValidateModel(object model)
		{
			var result = new List<ValidationResult>();
			if (model == null) return result;
			var type = model.GetType();
			var cld = _classWrappers[type];
			var cw = cld.CreateWrapper(model);
			foreach (var propName in cld.Properties)
			{
				var prop = cld.GetProperty(propName);
				if (prop == null || prop.GetterVisibility != ItemVisibility.Public || prop.SetterVisibility != ItemVisibility.Public) continue;
				foreach (var attr in prop.Attributes)
				{
					var validationAttr = attr as IValidationAttribute;
					if (validationAttr == null) continue;
					var compareAttr = attr as ICompareAttribute;
					if (compareAttr != null)
					{
						var toCompareValue = cw.GetObject(compareAttr.WithField);
						if (!compareAttr.IsValid(cw.GetObject(propName), toCompareValue))
						{
							result.Add(new ValidationResult(propName, validationAttr.ErrorMessage));
							break;
						}
					}
					else if (!validationAttr.IsValid(cw.GetObject(propName), prop.PropertyType))
					{
						result.Add(new ValidationResult(propName, validationAttr.ErrorMessage));
						break;
					}
				}
			}
			return result;
		}
	}
}