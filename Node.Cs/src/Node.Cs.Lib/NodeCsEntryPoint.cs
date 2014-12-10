using Castle.MicroKernel.Registration;
using Castle.Windsor;
using GenericHelpers;
using Node.Cs.CommandHandlers;
using Node.Cs.Consoles;
using System;
using System.Reflection;

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
				basicModule.PreInitialize();
			}
		}
	}
}
