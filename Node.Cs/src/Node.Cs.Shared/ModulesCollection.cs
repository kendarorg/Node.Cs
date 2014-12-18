// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================


using Node.Cs.Exceptions;
using System;
using System.Collections.Concurrent;

namespace Node.Cs
{
	public class ModulesCollection : IModulesCollection
	{
		private readonly ConcurrentDictionary<string, INodeModule> _modules =
			new ConcurrentDictionary<string, INodeModule>(StringComparer.OrdinalIgnoreCase);

		private readonly ConcurrentDictionary<Type, INodeModule> _modulesTypes =
			new ConcurrentDictionary<Type, INodeModule>();

		public void Register(params INodeModule[] modules)
		{
			foreach (var module in modules)
			{
				Register(module);
			}
		}

		private void Register(INodeModule module)
		{
			if (_modules.ContainsKey(module.Name))
			{
				var present = _modules[module.Name];
				if (present.Version != module.Version)
				{
					throw new DuplicateModuleException(
						"Module '{0}' is present in 2 versions '{1}' and '{2}'.",
						module.Name, module.Version, present.Version);
				}
				return;
			}
			_modulesTypes.TryAdd(module.GetType(), module);
			_modules.TryAdd(module.Name, module);
		}

		public T GetModule<T>(string name = null) where T : class, INodeModule
		{
			if (string.IsNullOrWhiteSpace(name) && _modulesTypes.ContainsKey(typeof(T)))
			{
				return (T)_modulesTypes[typeof(T)];
			}
			if (!string.IsNullOrWhiteSpace(name) && _modules.ContainsKey(name))
			{
				var result = _modules[name] as T;
				if (result == null)
				{
					throw new ModuleTypeException("Module '{0}' is not of type '{1}'.", name, typeof (T));
				}
				return result;
			}
			throw new MissingModuleException(typeof(T).ToString());
		}

		public INodeModule GetModule(string name)
		{
			if (!_modules.ContainsKey(name))
			{
				throw new MissingModuleException(name);
			}
			return _modules[name];
		}
	}
}