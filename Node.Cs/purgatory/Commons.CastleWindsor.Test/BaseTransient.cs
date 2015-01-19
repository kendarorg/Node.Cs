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

namespace Commons.CastleWindsor.Test
{
	public class BaseTransient : IBaseTransient
	{
		public BaseTransient()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
	}
	public interface IBaseTransient
	{
		Guid Id { get; set; }
	}
	public interface IBaseSingleton
	{
		Guid Id { get; set; }
	}

	public class BaseSingleton : IBaseSingleton
	{
		public BaseSingleton()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id { get; set; }
	}

	public class AllAssemblies01 : IBaseAllAssemblies
	{

	}
	public class AllAssemblies02 : IBaseAllAssemblies
	{

	}
	public interface IBaseAllAssemblies
	{

	}

	public interface IUsingAll
	{
		IBaseAllAssemblies[] AllBase { get; set; }
	}

	public class UsingAll
	{
		public IBaseAllAssemblies[] AllBase { get; set; }

		public UsingAll(IBaseAllAssemblies[] allBase)
		{
			AllBase = allBase;
		}
	}
}