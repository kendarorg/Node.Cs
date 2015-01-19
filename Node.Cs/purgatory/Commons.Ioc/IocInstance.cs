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
using System.Reflection;

namespace Commons.Ioc
{
	public abstract class IocInstance : IIoc
	{
		private static IIoc _instance;

		public static IIoc GetInstance()
		{
			return _instance;
		}

		protected static void SetInstance(IIoc value)
		{
			if (_instance != null)
				{
					throw new ContainerException("Ioc Container already initialized");
				}
				_instance = value;
		}

		public void Register(params IFinalRegistration[] registrations)
		{
			var currentAssembly = Assembly.GetCallingAssembly();
			foreach (var item in registrations)
			{
				var component = item as IComponent;
				var classe = item as IClass;
				try
				{
					if(component!=null)
						Register(component, currentAssembly);
					else if (classe != null)
						Register(classe, currentAssembly);
				}
				catch (ContainerException)
				{
					throw;
				}
				catch (Exception exception)
				{
					throw new ContainerException("Exception registering " + component.Interface, exception);
				}
			}
		}

		public T Resolve<T>()
		{
			return (T)Resolve(typeof(T));
		}
		public IEnumerable<T> ResolveAll<T>()
		{
			foreach (var item in ResolveAll(typeof(T)))
			{
				yield return (T)item;
			};
		}

		public abstract object Resolve(Type t);
		public abstract IEnumerable<object> ResolveAll(Type t);
		public abstract void Release(object t);

		public abstract object ContainerInstance { get; }
		protected abstract void Register(IComponent component, Assembly currentAssembly);
		protected abstract void Register(IClass component, Assembly currentAssembly);
	}
}