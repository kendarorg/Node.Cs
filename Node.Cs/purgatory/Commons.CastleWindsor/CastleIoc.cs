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
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Commons.Ioc;
using Component = Castle.MicroKernel.Registration.Component;

namespace Commons.CastleWindsor
{
	public class CastleIoc : IocInstance
	{
		private readonly WindsorContainer _containerInstance;

		static CastleIoc()
		{
			SetInstance(new CastleIoc());
		}

		public static CastleIoc Instance { get { return (CastleIoc)GetInstance(); } }

		private CastleIoc()
		{
			_containerInstance = new WindsorContainer();
			_containerInstance.Kernel.Resolver.AddSubResolver(new ListResolver(_containerInstance.Kernel));
			_containerInstance.Kernel.Resolver.AddSubResolver(new ArrayResolver(_containerInstance.Kernel));
		}

		public override object Resolve(Type t)
		{
			return _containerInstance.Resolve(t);
		}

		public override IEnumerable<object> ResolveAll(Type t)
		{
			throw new Exception();
		}

		public override void Release(object t)
		{
			_containerInstance.Release(t);
		}

		public override object ContainerInstance
		{
			get { return _containerInstance; }
		}

		protected override void Register(IComponent component, Assembly currentAssembly)
		{
			var componentFor = Component.For(component.Interface);

			var withCreation = InitializeCreationPolicy(component, componentFor);
			var result = InitializeLifeStyle(component, withCreation);
			_containerInstance.Register(result);
		}

		protected override void Register(IClass component, Assembly currentAssembly)
		{
			var classes = InitializeAssemblies(component, currentAssembly);
			var basedOn = InitializeSelection(component, classes);
			var result = InitializeLifeStyle(component, basedOn);
			_containerInstance.Register(result);
		}

		private BasedOnDescriptor InitializeSelection(IClass classs, FromAssemblyDescriptor componentFor)
		{
			if (classs.Where != null)
			{
				return componentFor.Where(a=>classs.Where(a));
			}
			if (classs.BasedOn != null)
			{
				return componentFor.BasedOn(classs.BasedOn).WithServiceAllInterfaces();
			}
			throw new ContainerException("Implementation not allowed for component");
		}

		private FromAssemblyDescriptor InitializeAssemblies(IClass component,Assembly currentAssembly)
		{
			switch (component.FromAssembly)
			{
				case (FromAssembly.All):
					return Classes.FromAssemblyInThisApplication();
				case (FromAssembly.Current):
					return Classes.FromAssembly(currentAssembly);
				case(FromAssembly.Entry):
					return Classes.FromAssembly(Assembly.GetEntryAssembly());
				default:
					throw new ContainerException("Not supported type for assembly source "+component.FromAssembly);
			}
		}

		private BasedOnDescriptor InitializeLifeStyle(IClass component, BasedOnDescriptor componentFor)
		{
			switch (component.LifeStyle)
			{
				case (LifeStyle.Singleton):
					return componentFor.LifestyleSingleton();
				case (LifeStyle.Transient):
					return componentFor.LifestyleTransient();
				case (LifeStyle.WebRequest):
					return componentFor.LifestylePerWebRequest();
				case (LifeStyle.Pooled):
					return componentFor.LifestylePooled();
				default:
					throw new ContainerException("LifeStyle not allowed " + component.LifeStyle);
			}
		}

		private static ComponentRegistration<object> InitializeLifeStyle(IComponent component, ComponentRegistration<object> componentFor)
		{
			switch (component.LifeStyle)
			{
				case (LifeStyle.Singleton):
					return componentFor.LifestyleSingleton();
				case (LifeStyle.Transient):
					return componentFor.LifestyleTransient();
				case (LifeStyle.WebRequest):
					return componentFor.LifestylePerWebRequest();
				case (LifeStyle.Pooled):
					return componentFor.LifestylePooled();
				default:
					throw new ContainerException("LifeStyle not allowed " + component.LifeStyle);
			}
		}

		private ComponentRegistration<object> InitializeCreationPolicy(IComponent component, ComponentRegistration componentFor)
		{
			if (component.Instance != null)
			{
				return componentFor.Instance(component.Instance);
			}
			if (component.Implementation != null)
			{
				return componentFor.ImplementedBy(component.Implementation);
			}
			if (component.Factory != null)
			{
				return componentFor.UsingFactoryMethod(a => component.Factory(this));
			}
			throw new ContainerException("Implementation not allowed for interface " + component.Interface);
		}
	}
}
