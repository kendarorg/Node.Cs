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

using Commons.Ioc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace Commons.CastleWindsor.Test
{
#if FUFFA
	[TestClass]
	public class CastleIocTest
	{
		[TestMethod]
		public void ItShouldBePossibleToInitializeTheWoleThing()
		{
			var ioc = CastleIoc.Instance;
			ioc.Register(
				Component.Create<IBaseTransient>()
						.With<BaseTransient>()
						.WithLifestyle(LifeStyle.Transient),
				Component.Create<IBaseSingleton>()
						.With<BaseSingleton>()
						.WithLifestyle(),
				Class.Create().FromAllAssembly()
						.BasedOn<IBaseAllAssemblies>()
						.WithName("AllAssemblies").WithLifestyle(LifeStyle.Transient),
				Component.Create<IUsingAll>()
						.With<UsingAll>()
						.WithLifestyle()
				);

			var base1 = ioc.Resolve<IBaseTransient>();
			var base2 = ioc.Resolve<IBaseTransient>();
			Assert.AreNotEqual((object) base1.Id, base2.Id);
			ioc.Release(base1);
			ioc.Release(base2);


			var singleton1 = ioc.Resolve<IBaseSingleton>();
			var singleton2 = ioc.Resolve<IBaseSingleton>();
			Assert.AreEqual((object) singleton1.Id, singleton2.Id);


			var all = ioc.Resolve<IUsingAll>();
			Assert.AreEqual(2,all.AllBase.Count());

		}
	}
#endif
}
