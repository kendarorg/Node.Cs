using System.ComponentModel;
using System.Threading;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using Moq;
using System;
using System.Collections;
using System.Diagnostics;
using Component = Castle.MicroKernel.Registration.Component;

namespace Node.Cs.Test
{
	public class LazyComponentAutoMocker : ILazyComponentLoader
	{
		private readonly WindsorContainer _container;

		public LazyComponentAutoMocker(WindsorContainer container)
		{
			_container = container;
		}

		public IRegistration Load(string key, Type service, IDictionary arguments)
		{
			var mockInterfaceType = typeof (Mock<>).MakeGenericType(service);
			var constructor = mockInterfaceType.GetConstructor(new Type[] {});
			Debug.Assert(constructor != null, "LazyComponentAutoMocker::Load, constructor != null");
			var mockedObject = constructor.Invoke(new object[] {});
			var mock = (Mock)mockedObject;
			var realObject = mock.Object;

			_container.Register(Component.For(mockInterfaceType).Instance(mockedObject));
			return Component.For(service).Instance(realObject);
		}
	}
}
