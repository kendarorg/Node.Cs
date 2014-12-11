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
