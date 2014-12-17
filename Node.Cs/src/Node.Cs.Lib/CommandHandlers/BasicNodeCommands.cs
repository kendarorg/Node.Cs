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
using System.Linq;
using Castle.Windsor.Diagnostics.Extensions;
using Node.Cs.Consoles;
using System.IO;
using System.Reflection;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using Node.Cs.Exceptions;
using SharpTemplate.Compilers;
using Ionic.Zip;

namespace Node.Cs.CommandHandlers
{
	public class BasicNodeCommands : IBasicNodeCommands
	{
		private readonly INodeConsole _console;
		private readonly IWindsorContainer _container;
		private INugetPackagesDownloader _nugetDownloader;
		private IAssemblySeeker _asmSeeker;
		private readonly IModulesCollection _modulesCollection;

		private readonly Dictionary<string, RunnableDefinition> _runnables =
				new Dictionary<string, RunnableDefinition>(StringComparer.InvariantCultureIgnoreCase);

		public BasicNodeCommands(
			INodeConsole console,
			IWindsorContainer container,
			INugetPackagesDownloader nugetDownloader,
			IAssemblySeeker asmSeeker,
			IModulesCollection modulesCollection)
		{
			_console = console;
			_container = container;
			_nugetDownloader = nugetDownloader;
			_asmSeeker = asmSeeker;
			_modulesCollection = modulesCollection;
		}

		/// <summary>
		/// Runs a .cs file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		public void Run(INodeExecutionContext context, string path, string function = null)
		{
			if (function == null)
			{
				function = "Execute";
			}
			path = path.ToPath();
			var absolutePath = Path.Combine(context.CurrentDirectory.Data.Trim(Path.DirectorySeparatorChar), path.Trim(Path.DirectorySeparatorChar));
			if (!File.Exists(absolutePath))
			{
				throw new FileNotFoundException("File not found.", path);
			}
			var extension = Path.GetExtension(absolutePath);
			if (extension.ToLowerInvariant() == ".cs")
			{
				RunAndBuild(absolutePath, context, function);
			}
			else if (extension.ToLowerInvariant() == ".ncs")
			{
				RunNcs(absolutePath, context);
			}
			else
			{
				throw new NotSupportedException(string.Format("Extension '{0}' is not supported.", extension));
			}
		}

		public void Echo(INodeExecutionContext context, string message)
		{
			_console.WriteLine(message);
		}

		public void Exit(INodeExecutionContext context, int errorCode)
		{
			Environment.Exit(errorCode);
		}

		public void LoadDll(INodeExecutionContext context, string dllPath)
		{
			var foundedPath = _asmSeeker.FindAssembly(dllPath);
			if (foundedPath != null)
			{
				var content = File.ReadAllBytes(foundedPath);
				InitializeNewDlls(() => Assembly.Load(content));
			}
			else
			{
				throw new DllNotFoundException(dllPath);
			}
		}

		#region Private functions

		private void RunAndBuild(string path, INodeExecutionContext context, string function)
		{
			var loadedAssembly = string.Empty;
			var dllName = "dll_" + Guid.NewGuid().ToString("N");
			var className = "class_" + dllName;
			var namespaceName = "ns_" + dllName;
			try
			{
				Type type;
				if (_runnables.ContainsKey(path))
				{
					var def = _runnables[path];
					if (def.Timestamp < File.GetLastWriteTime(path))
					{
						_runnables.Remove(path);
					}
				}
				if (!_runnables.ContainsKey(path))
				{
					var originalSource = File.ReadAllText(path);
					var source = "namespace " + namespaceName + " {" + originalSource + "}";

					var sc = SetupSourceCompiler(context, dllName, namespaceName, className, source);
					loadedAssembly = sc.Compile(0);
					if (sc.HasErrors)
					{
						ThrowCompilationErrors(path, sc, originalSource);
					}
					var content = File.ReadAllBytes(loadedAssembly);
					var compileSimpleObjectAsm = Assembly.Load(content);
					type =
					compileSimpleObjectAsm.GetTypes()
							.FirstOrDefault(a =>
									a.GetMethods().
									Any(m => String.Equals(m.Name, function, StringComparison.InvariantCultureIgnoreCase)));

					ThrowIfFindsErrors(type, function, namespaceName, originalSource);

					_runnables.Add(path, new RunnableDefinition
					{
						Type = type,
						Timestamp = File.GetLastWriteTime(path)
					});
					_container.Register(Component.For(type).LifestyleTransient());
				}
				else
				{
					var def = _runnables[path];
					type = def.Type;
				}

				var instance = _container.Resolve(type);
				var methodInfo = instance.GetType().GetMethods().First(m => String.Equals(m.Name, function, StringComparison.InvariantCultureIgnoreCase));
				methodInfo.Invoke(instance, new object[] { });
			}
			finally
			{
				if (File.Exists(loadedAssembly))
				{
					File.Delete(loadedAssembly);
				}
			}
		}

		private void ThrowIfFindsErrors(Type type, string function, string namespaceName, string originalSource)
		{
			if (type == null)
			{
				throw new MissingFunctionException("Missing function '{0}'.", function);
			}

			if (type.Namespace != namespaceName)
			{
				throw new CompilationException("Must not set the namespace for scripts!!.", originalSource);
			}
		}

		private static SourceCompiler SetupSourceCompiler(INodeExecutionContext context, string dllName, string namespaceName,
				string className, string source)
		{
			var sc = new SourceCompiler(dllName, context.TempPath)
			{
				UseAppdomain = true
			};
			sc.AddFile(namespaceName, className, source);
			sc.LoadCurrentAssemblies();
			return sc;
		}

		private static void ThrowCompilationErrors(string path, SourceCompiler sc, string originalSource)
		{
			var errs = new HashSet<string>();
			var compilationErrors = "Error compiling " + path;
			foreach (var errorList in sc.Errors)
			{
				var singleErrorList = errorList;
				foreach (var error in singleErrorList)
				{
					if (!errs.Contains(error))
					{
						// ReSharper disable once AssignNullToNotNullAttribute
						var where = error.IndexOf(".cs\t", StringComparison.InvariantCultureIgnoreCase);
						if (@where > 0)
						{
							// ReSharper disable once PossibleNullReferenceException
							var newError = error.Substring(@where + ".cs\t".Length).TrimStart();
							compilationErrors += "\r\n" + newError;
						}
						else
						{
							compilationErrors += "\r\n" + error;
						}

						errs.Add(error);
					}
				}
			}
			throw new CompilationException(compilationErrors, originalSource);
		}

		private void RunNcs(string absolutePath, INodeExecutionContext context)
		{
			var cmdHandler = _container.Resolve<IUiCommandsHandler>();
			var src = File.ReadAllLines(absolutePath);
			foreach (var line in src)
			{
				cmdHandler.Run(line);
			}
		}
		#endregion

		public void LoadNuget(INodeExecutionContext context, string packageName, string version = null, bool allowPreRelease = false)
		{
			var dlls = new List<string>();
			foreach (var dll in _nugetDownloader.DownloadPackage(context.ImageRuntimeVersion, packageName, version, allowPreRelease))
			{
				var path = Path.Combine(context.NodeCsPackagesDirectory, dll.Name);
				File.WriteAllBytes(path, dll.Data);
				dlls.Add(path);
			}
			InitializeNewDlls(() =>
			{
				foreach (var dllPath in dlls)
				{
					var content = File.ReadAllBytes(dllPath);
					Assembly.Load(content);
				}
			});
		}

		private void InitializeNewDlls(Action action)
		{
			var beforeDlls = AppDomain.CurrentDomain.GetAssemblies();
			action();
			var afterDlls = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = (beforeDlls.Length); i < afterDlls.Length; i++)
			{
				var dllToElaborate = afterDlls[i];
				InitializeModulesInDll(dllToElaborate);
			}
		}

		private void InitializeModulesInDll(Assembly dllToElaborate)
		{
			_container.Register(Classes.FromAssembly(dllToElaborate)
				.BasedOn<INodeModule>()
				.WithServiceAllInterfaces()
				.LifestyleSingleton());

			_modulesCollection.Register(_container.ResolveAll<INodeModule>());
		}
	}
}
