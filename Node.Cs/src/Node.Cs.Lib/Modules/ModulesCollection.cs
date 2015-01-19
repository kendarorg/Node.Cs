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
using Node.Cs.Exceptions;

namespace Node.Cs.Modules
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