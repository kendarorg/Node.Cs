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


using System.Diagnostics;
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
							Component.For<INugetVersionVerifier>()
											.ImplementedBy<NugetVersionVerifier>()
											.LifestyleSingleton()
											.OnlyNewServices());

			_container.Register(
							Component.For<INugetArchiveList>()
											.ImplementedBy<NugetArchiveList>()
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
							Component.For<IModulesCollection>()
											.ImplementedBy<ModulesCollection>()
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
											Classes.FromAssemblyInThisApplication()
											.BasedOn<INodeModule>()
											.WithServiceBase()
											.LifestyleSingleton()
											.AllowMultipleMatches()
							);

			try
			{
				_container.Register(Component.For<IWindsorContainer>().Instance(_container));
			}
			catch (Exception)
			{
				Debug.WriteLine("Duplicate Castle Registration.");
			}
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
