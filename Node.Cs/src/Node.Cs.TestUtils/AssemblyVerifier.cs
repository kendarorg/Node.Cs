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


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Node.Cs.Test
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
