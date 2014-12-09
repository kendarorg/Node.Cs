using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Node.Cs.Consoles;
using System;
using System.Reflection;

namespace Node.Cs
{
	public class NodeCsEntryPoint : INodeCsEntryPoint
	{
		private readonly WindsorContainer _container;
		private readonly NodeExecutionContext _executionContext;

		public NodeCsEntryPoint(string[] args, WindsorContainer container)
		{
			var asm = Assembly.GetCallingAssembly();
			var uri = new UriBuilder(asm.CodeBase);
			_container = container;
			_executionContext = new NodeExecutionContext(
				args,
				Assembly.GetExecutingAssembly().GetName().Version,
				uri.Path, null,
				Environment.CurrentDirectory
				);
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

		private static void RunAsService()
		{
			throw new NotImplementedException("Cannot run Node.Cs as a service");
		}

		private void RunAsInteractive()
		{
			var commandsHandler = _container.Resolve<IUiCommandsHandler>();
			var console = _container.Resolve<INodeConsole>();
			while (true)
			{
				var command = console.ReadLine();
				if (!string.IsNullOrWhiteSpace(command))
				{
					if (command.ToLowerInvariant().Trim() == "exit")
					{
						return;
					}
					commandsHandler.Run(command);
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
			var nodeConsole = new BasicNodeConsole();
			var commandHandler = new UiCommandsHandler();

			_container.Register(
				Component.For<INodeConsole>()
					.Instance(nodeConsole)
					.LifestyleSingleton()
					.OnlyNewServices());

			_container.Register(
				Component.For<INodeCsEntryPoint>()
					.Instance(this)
					.LifestyleSingleton());

			_container.Register(
				Component.For<IUiCommandsHandler>()
					.Instance(commandHandler)
					.LifestyleSingleton()
					.OnlyNewServices());

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
				basicModule.PreInitialize();
			}
		}
	}
}
