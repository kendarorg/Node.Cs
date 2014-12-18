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


using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using GenericHelpers;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using System;
using System.Reflection;
using Node.Cs.Modules;
using Node.Cs.Nugets;
using Node.Cs.Utils;

namespace Node.Cs
{
	public class NodeCsEntryPoint : INodeCsEntryPoint
	{
		public const string COMMAND_ERROR_FORMAT = "Error executing command '{0}'. Error was: '{1}'";

		private readonly WindsorContainer _container;
		private readonly NodeExecutionContext _executionContext;

		public NodeCsEntryPoint(CommandLineParser args, WindsorContainer container)
		{
			var asm = Assembly.GetCallingAssembly();
			var uri = new UriBuilder(asm.CodeBase);
			var binDir = Path.Combine(Environment.CurrentDirectory, "bin");
			var tmpDir = Path.Combine(Environment.CurrentDirectory, "tmp");
			var packagesDir = Path.Combine(Environment.CurrentDirectory, "packages");
			_container = container;
			_container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel));

			var tar = (TargetFrameworkAttribute)Assembly.GetCallingAssembly()
					.GetCustomAttributes(typeof(TargetFrameworkAttribute)).First();
			var las = tar.FrameworkName.LastIndexOf("v", StringComparison.Ordinal);
			var name = tar.FrameworkName.Substring(las + 1).Replace(".", "");

			_executionContext = new NodeExecutionContext(
							args,
							Assembly.GetExecutingAssembly().GetName().Version,
							uri.Path, binDir,
							Environment.CurrentDirectory,
							tmpDir,
														packagesDir,
							"net" + name);
			AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
		}

		public void Run(bool asService = false)
		{
			Initialize();

			if (asService)
			{
				RunAsService();
			}
			else
			{
				RunAsInteractive();
			}
		}

		protected virtual bool ShouldContinueRunning()
		{
			return true;
		}

		private static void RunAsService()
		{
			throw new NotImplementedException("Cannot run Node.Cs as a service");
		}

		private void RunAsInteractive()
		{
			var commandsHandler = _container.Resolve<IUiCommandsHandler>();
			var console = _container.Resolve<INodeConsole>();
			while (ShouldContinueRunning())
			{
				var command = console.ReadLine();
				if (!string.IsNullOrWhiteSpace(command))
				{
					try
					{
						commandsHandler.Run(command);
					}
					catch (Exception ex)
					{
						console.WriteLine(COMMAND_ERROR_FORMAT, command, ex.Message);
					}
				}
			}
		}

		private void Initialize()
		{
			_container.Register(
							Component.For<INodeExecutionContext>()
											.Instance(_executionContext)
											.LifestyleSingleton());

			SetupMainDependencies();
			PreInitializeBasicModules();
		}

		private void SetupMainDependencies()
		{
			_container.Register(
							Component.For<IBasicNodeCommands>()
											.ImplementedBy<BasicNodeCommands>()
											.LifestyleSingleton()
											.OnlyNewServices());


			_container.Register(
											Component.For<IAssemblySeeker>()
																			.ImplementedBy<AssemblySeeker>()
																			.LifestyleSingleton()
																			.OnlyNewServices());

			_container.Register(
							Component.For<INugetPackagesDownloader>()
											.ImplementedBy<NugetPackagesDownloader>()
											.LifestyleSingleton()
											.OnlyNewServices());


			_container.Register(
							Component.For<ICommandParser>()
											.ImplementedBy<BasicCommandParser>()
											.LifestyleSingleton()
											.OnlyNewServices());

			_container.Register(
							Component.For<INodeConsole>()
											.ImplementedBy<BasicNodeConsole>()
											.LifestyleSingleton()
											.OnlyNewServices());

			_container.Register(
							Component.For<IUiCommandsHandler>()
											.ImplementedBy<UiCommandsHandler>()
											.LifestyleSingleton()
											.OnlyNewServices());

			_container.Register(
							Component.For<INodeCsEntryPoint>()
											.Instance(this)
											.LifestyleSingleton());

			_container.Register(
							Classes.FromThisAssembly()
											.BasedOn<INodeModule>()
											.LifestyleSingleton()
							);
		}

		private void PreInitializeBasicModules()
		{
			foreach (var basicModule in _container.ResolveAll<INodeModule>())
			{
				basicModule.Initialize();
			}
		}

		private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
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

			var seeker = _container.Resolve<IAssemblySeeker>();

			var foundedPath = seeker.FindAssembly(path);
			if (foundedPath != null)
			{
				return Assembly.LoadFrom(foundedPath);
			}

			var console = _container.Resolve<INodeConsole>();
			console.WriteLine("Dll '{0}' not found.", path);
			return null;
		}
	}
}
