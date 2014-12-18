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
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Node.Cs.Consoles;
using Node.Cs.Exceptions;
using Node.Cs.Modules;
using Node.Cs.Nugets;
using Node.Cs.Utils;
using SharpTemplate.Compilers;

namespace Node.Cs.CommandHandlers
{
	public class BasicNodeCommands : IBasicNodeCommands
	{
		private readonly IAssemblySeeker _asmSeeker;
		private readonly INodeConsole _console;
		private readonly IWindsorContainer _container;
		private readonly IModulesCollection _modulesCollection;
		private readonly INugetPackagesDownloader _nugetDownloader;

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
		///   Runs a .cs file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		/// <param name="function"></param>
		public void Run(INodeExecutionContext context, string path, string function = null)
		{
			if (function == null)
			{
				function = "Execute";
			}
			path = path.ToPath();
			string absolutePath = Path.Combine(context.CurrentDirectory.Data.Trim(Path.DirectorySeparatorChar),
				path.Trim(Path.DirectorySeparatorChar));
			if (!File.Exists(absolutePath))
			{
				throw new FileNotFoundException("File not found.", path);
			}
			string extension = Path.GetExtension(absolutePath);
			if (extension.ToLowerInvariant() == ".cs")
			{
				RunAndBuild(absolutePath, context, function);
			}
			else if (extension.ToLowerInvariant() == ".ncs")
			{
				RunNcs(absolutePath);
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

		[ExcludeFromCodeCoverage]
		public void Exit(INodeExecutionContext context, int errorCode)
		{
			Environment.Exit(errorCode);
		}

		public void LoadDll(INodeExecutionContext context, string dllPath)
		{
			string foundedPath = _asmSeeker.FindAssembly(dllPath);
			if (foundedPath != null)
			{
				byte[] content = File.ReadAllBytes(foundedPath);
				InitializeNewDlls(() => Assembly.Load(content));
			}
			else
			{
				throw new DllNotFoundException(dllPath);
			}
		}

		public void LoadNuget(INodeExecutionContext context, string packageName, string version = null,
			bool allowPreRelease = false)
		{
			var dlls = new List<string>();
			foreach (
				NugetDll dll in _nugetDownloader.DownloadPackage(context.ImageRuntimeVersion, packageName, version, allowPreRelease)
				)
			{
				string path = Path.Combine(context.NodeCsPackagesDirectory, dll.Name);
				File.WriteAllBytes(path, dll.Data);
				dlls.Add(path);
			}
			InitializeNewDlls(() =>
			{
				foreach (string dllPath in dlls)
				{
					byte[] content = File.ReadAllBytes(dllPath);
					Assembly.Load(content);
				}
			});
		}

		#region Private functions

		private void RunAndBuild(string path, INodeExecutionContext context, string function)
		{
			string loadedAssembly = string.Empty;
			string dllName = "dll_" + Guid.NewGuid().ToString("N");
			string className = "class_" + dllName;
			string namespaceName = "ns_" + dllName;
			try
			{
				Type type;
				if (_runnables.ContainsKey(path))
				{
					RunnableDefinition def = _runnables[path];
					if (def.Timestamp < File.GetLastWriteTime(path))
					{
						_runnables.Remove(path);
					}
				}
				if (!_runnables.ContainsKey(path))
				{
					string originalSource = File.ReadAllText(path);
					string source = "namespace " + namespaceName + " {" + originalSource + "}";

					SourceCompiler sc = SetupSourceCompiler(context, dllName, namespaceName, className, source);
					loadedAssembly = sc.Compile(0);
					if (sc.HasErrors)
					{
						ThrowCompilationErrors(path, sc, originalSource);
					}
					byte[] content = File.ReadAllBytes(loadedAssembly);
					Assembly compileSimpleObjectAsm = Assembly.Load(content);
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
					RunnableDefinition def = _runnables[path];
					type = def.Type;
				}

				object instance = _container.Resolve(type);
				MethodInfo methodInfo =
					instance.GetType()
						.GetMethods()
						.First(m => String.Equals(m.Name, function, StringComparison.InvariantCultureIgnoreCase));
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

		// ReSharper disable UnusedParameter.Local
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
		// ReSharper restore UnusedParameter.Local

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
			string compilationErrors = "Error compiling " + path;
			foreach (var errorList in sc.Errors)
			{
				List<string> singleErrorList = errorList;
				foreach (string error in singleErrorList)
				{
					if (!errs.Contains(error))
					{
						// ReSharper disable once AssignNullToNotNullAttribute
						int where = error.IndexOf(".cs\t", StringComparison.InvariantCultureIgnoreCase);
						if (@where > 0)
						{
							// ReSharper disable once PossibleNullReferenceException
							string newError = error.Substring(@where + ".cs\t".Length).TrimStart();
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

		private void RunNcs(string absolutePath)
		{
			var cmdHandler = _container.Resolve<IUiCommandsHandler>();
			string[] src = File.ReadAllLines(absolutePath);
			foreach (string line in src)
			{
				cmdHandler.Run(line);
			}
		}

		#endregion

		private void InitializeNewDlls(Action action)
		{
			Assembly[] beforeDlls = AppDomain.CurrentDomain.GetAssemblies();
			action();
			Assembly[] afterDlls = AppDomain.CurrentDomain.GetAssemblies();
			for (int i = (beforeDlls.Length); i < afterDlls.Length; i++)
			{
				Assembly dllToElaborate = afterDlls[i];
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