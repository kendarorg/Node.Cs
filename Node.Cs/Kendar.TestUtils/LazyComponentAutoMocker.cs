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


using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using Moq;
using System;
using System.Collections;
using System.Diagnostics;
using Component = Castle.MicroKernel.Registration.Component;

namespace Kendar.TestUtils
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
			var mockInterfaceType = typeof(Mock<>).MakeGenericType(service);
			var constructor = mockInterfaceType.GetConstructor(new Type[] { });
			Debug.Assert(constructor != null, "LazyComponentAutoMocker::Load, constructor != null");
			var mockedObject = constructor.Invoke(new object[] { });
			var mock = (Mock)mockedObject;
			var realObject = mock.Object;

			_container.Register(Component.For(mockInterfaceType).Instance(mockedObject));
			return Component.For(service).Instance(realObject);
		}
	}
}
