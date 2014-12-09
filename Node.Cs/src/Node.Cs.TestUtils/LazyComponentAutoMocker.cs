using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Moq;
using System;
using System.Collections;
using System.Diagnostics;

namespace Node.Cs.Test
{
	public class LazyComponentAutoMocker : ILazyComponentLoader
	{
		public IRegistration Load(string key, Type service, IDictionary arguments)
		{
			var constructor = typeof (Mock<>).MakeGenericType(service).GetConstructor(new Type[] {});
			Debug.Assert(constructor != null, "LazyComponentAutoMocker::Load, constructor != null");
			return Component.For(service).Instance(constructor.Invoke(new object[]{}));
		}
	}
}
