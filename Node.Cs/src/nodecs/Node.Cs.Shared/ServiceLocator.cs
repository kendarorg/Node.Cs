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
	public interface IServiceLocator
	{
		T Resolve<T>(bool nullIfNotFound = false);
		object Resolve(Type t,bool nullIfNotFound = false);
		void Release(object ob);
		void Register(Type t);
		void Register<T>(Func<Type,object> resolver, bool isSingleton = true);
		void Register<T>(object instance);
        void SetChildLocator(IServiceLocator childLocator);
	}

	public class ServiceLocator : IServiceLocator
	{
		private readonly static IServiceLocator _locator = new ServiceLocator();
		public static IServiceLocator Locator
		{
			get { return _locator; }
		} 
		class ServiceInstance
		{
			public bool IsSingleton;
			public object Instance;
			public Func<Type,object> Create;
		}
		private readonly Dictionary<Type, ServiceInstance> _services;
		private readonly Dictionary<Type, ServiceInstance> _resolved;

		public IServiceLocator ChildLocator { get; private set; }

        public void SetChildLocator(IServiceLocator childLocator)
        {
            if (ChildLocator != null)
            {
                return;
            }
            ChildLocator = childLocator;
        }

		private ServiceLocator()
		{
			_services = new Dictionary<Type, ServiceInstance>();
			_resolved = new Dictionary<Type, ServiceInstance>();
			Register<ILogger>(NullLogger.Create());
		}

		public void Register<T>(Func<Type,object> resolver = null,bool isSingleton=true)
		{
			if (resolver == null)
			{
				resolver = Activator.CreateInstance;
			}
			var instance = resolver(typeof(T));
			_services[typeof (T)] = new ServiceInstance
			{
				Create = resolver,
				IsSingleton = isSingleton,
				Instance = isSingleton ? instance : null
			};
			_resolved[instance.GetType()] = _services[typeof (T)];
			if (!isSingleton)
			{
				var disposable = instance as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		public void Register(Type t)
		{
			if (ChildLocator != null)
			{
				ChildLocator.Register(t);
			}
			else
			{
				Func<Type,object> resolver = Activator.CreateInstance;

				_services[t] = new ServiceInstance
				{
					Create = resolver,
					IsSingleton = false,
					Instance = null
				};
				_resolved[t] = _services[t];
			}
		}

		public void Register<T>(object instance)
		{
			if (ChildLocator != null)
			{
				ChildLocator.Register<T>(instance);
			}
			else
			{
				_services[typeof (T)] = new ServiceInstance
				{
					Create = null,
					IsSingleton = true,
					Instance = instance
				};
				_resolved[instance.GetType()] = _services[typeof (T)];
			}
		}

		public T Resolve<T>(bool nullIfNotFound = false)
		{
			if (_services.ContainsKey(typeof (T)))
			{
				var desc = _services[typeof (T)];
				if (desc.Instance!=null) return (T) desc.Instance;
				desc.Instance = desc.Create(typeof(T));
				return (T)desc.Instance;
			}
			
			if (ChildLocator != null)
			{
				var result = ChildLocator.Resolve<T>();
				if(result != null) return  result;
			}
			if (typeof(T).IsAbstract || typeof(T).IsInterface || nullIfNotFound)
			{
				return default(T);
			}
			if (IsSystem(typeof (T))) return default(T);
			return Activator.CreateInstance<T>();
		}

		public object Resolve(Type t,bool nullIfNotFound = false)
		{
			if (_services.ContainsKey(t))
			{
				var desc = _services[t];
				if (desc.Instance != null) return desc.Instance;
				desc.Instance = desc.Create(t);
				return desc.Instance;
			}

			if (ChildLocator != null)
			{
				var result = ChildLocator.Resolve(t);
				if (result != null) return result;
			}

			if (t.IsAbstract || t.IsInterface || nullIfNotFound)
			{
				return null;
			}
			if (IsSystem(t)) return null;
			return Activator.CreateInstance(t);
		}

		private bool IsSystem(Type type)
		{
			var ns = type.Namespace ?? "";
			return type.IsPrimitive ||
						 ns.StartsWith("System.") ||
						 ns == "System." ||
			       type.Module.ScopeName == "CommonLanguageRuntimeLibrary";
		}


		public void Release(object ob)
		{
			if (ChildLocator != null)
			{
				ChildLocator.Release(ob);
			}
			if (_resolved.ContainsKey(ob.GetType()))
			{
				var desc = _resolved[ob.GetType()];
				if (desc.IsSingleton) return;
				var disposable = desc.Instance as IDisposable;
				if (disposable != null)
				{
					disposable.Dispose();
				}
			}
		}
	}
}
