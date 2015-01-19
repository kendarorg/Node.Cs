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
using CoroutinesLib.Shared.Logging;

namespace NodeCs.Shared
{
	public abstract class NodeModuleBase : INodeModule,ILoggable
	{
		protected NodeModuleBase()
		{
			Log = ServiceLocator.Locator.Resolve<ILogger>();
		}
		private readonly Dictionary<string, object> _parameters = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
		public abstract void Initialize();
		public void PreInitialize()
		{
			
		}

		public void PostInitialize()
		{
			
		}

		public virtual void SetParameter(string name, object value)
		{
			_parameters[name] = value;
		}

		public virtual void RemoveParameter(string name)
		{
			if (_parameters.ContainsKey(name))
			{
				_parameters.Remove(name);
			}
		}

		public T GetParameter<T>(string name)
		{
			if (!_parameters.ContainsKey(name))
			{
				return default(T);
			}
			return (T)_parameters[name];
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~NodeModuleBase()
		{
			Dispose(false);
		}

		protected abstract void Dispose(bool disposing);

		public ILogger Log { get; set; }
	}
}
