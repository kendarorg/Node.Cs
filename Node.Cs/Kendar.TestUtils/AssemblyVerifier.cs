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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Kendar.TestUtils
{
	[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Used only for testing")]
	public class AssemblyVerifier : IDisposable
	{
		private CustomDomain _cd;
		private readonly AppDomain _newAppDomain;

		public AssemblyVerifier()
		{
			_newAppDomain = AppDomain.CreateDomain(
				Guid.NewGuid().ToString("N"),
			AppDomain.CurrentDomain.Evidence,
										new AppDomainSetup
										{
											ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
											LoaderOptimization = LoaderOptimization.MultiDomainHost
										});

			_newAppDomain.AssemblyResolve += OnResolve;
		}

		private static Assembly OnResolve(object sender, ResolveEventArgs args)
		{
			var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

			foreach (var assembly in loadedAssemblies)
			{
				if (assembly.FullName == args.Name)
				{
					return assembly;
				}
			}
			var an = new AssemblyName(args.Name);
			var path = an.Name.Split(',').Skip(1).First().Trim('/').Trim('\\').Trim() + ".dll";
			// ReSharper disable once LoopCanBeConvertedToQuery
			// ReSharper disable once ForCanBeConvertedToForeach
			for (int index = 0; index < _sp.Count; index++)
			{
				var dir = _sp[index];
				var testPath = Path.Combine(dir, path);
				if (File.Exists(testPath))
				{
					return Assembly.LoadFrom(testPath);
				}
			}
			return null;
		}
		private static readonly List<string> _sp = new List<string>();

		public void AddSearchPath(string path)
		{
			_sp.Add(path);
		}

		public AssemblyName Name
		{
			get { return new AssemblyName(_cd.GetName()); }
		}

		public IEnumerable<string> Types
		{
			get
			{
				return _cd.GetTypes();
			}
		}

		public bool DllLoaded
		{
			get { return _cd.IsDllLoaded(); }
		}

		public void LoadDll(byte[] bytes)
		{
			// ReSharper disable once AssignNullToNotNullAttribute
			_cd = (CustomDomain)_newAppDomain.CreateInstanceAndUnwrap(
				"Node.Cs.TestUtils",
				typeof(CustomDomain).FullName
				);

			_cd.LoadDll(bytes);
		}
		public void LoadDll(string path)
		{
			_cd.LoadDll(path);
		}

		public string FrameworkVersion
		{
			get { return _cd.GetFrameworkVersion(); }
		}


		[SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Justification = "Used only for testing")]
		public void Dispose()
		{
			AppDomain.Unload(_newAppDomain);
		}
	}
}
