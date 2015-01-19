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


namespace Commons.Ioc
{
	public class Class : IClass, IFromAssembly, IIocLifestyle, IIocWhere
	{
		public static IFromAssembly Create()
		{
			return new Class();
		}

		private Class()
		{
			LifeStyle = LifeStyle.Singleton;
			FromAssembly = FromAssembly.All;
		}

		public Type BasedOn { get; private set; }
		public Func<Type, bool> Where { get; private set; }
		public FromAssembly FromAssembly { get; private set; }
		public LifeStyle LifeStyle { get; private set; }

		public IIocWhere FromCurrentAssembly()
		{
			FromAssembly = FromAssembly.Current;
			return this;
		}

		public IIocWhere FromAllAssembly()
		{
			FromAssembly = FromAssembly.All;
			return this;
		}

		public IIocWhere FromEntryAssembly()
		{
			FromAssembly = FromAssembly.Entry;
			return this;
		}

		public IFinalRegistration WithLifestyle(LifeStyle lifeStyle = LifeStyle.Singleton)
		{
			LifeStyle = lifeStyle;
			return this;
		}

		IIocWhere IIocWhere.Where(Func<Type, bool> func)
		{
			Where = func;
			return this;
		}

		IIocWhere IIocWhere.BasedOn<T>()
		{
			BasedOn = typeof (T);
			return this;
		}

		public IIocWhere WithName(string name, StringComparison comparison = StringComparison.InvariantCulture)
		{
			Where = (a) =>
			{
				return a.Name.IndexOf(name, comparison) >= 0;
			};
			return this;
		}
	}
}