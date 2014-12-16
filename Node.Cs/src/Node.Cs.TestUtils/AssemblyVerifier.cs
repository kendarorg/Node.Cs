using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Node.Cs.Test
{
	public class AssemblyVerifier : IDisposable
	{
		private CustomDomain _cd;
		private readonly AppDomain _newAppDomain;

		public AssemblyVerifier()
		{
			_newAppDomain = AppDomain.CreateDomain(
				Guid.NewGuid().ToString("N"),
			AppDomain.CurrentDomain.Evidence,
										new AppDomainSetup()
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
			var path = an.Name.Split(',').Skip(1).First().Trim('/').Trim('\\').Trim()+".dll";
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
				foreach (var type in _cd.GetTypes())
				{
					yield return type;
				}
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


		public void Dispose()
		{
			AppDomain.Unload(_newAppDomain);
		}
	}
}
