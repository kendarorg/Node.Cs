using ConcurrencyHelpers.Containers;
using System;
using GenericHelpers;

namespace Node.Cs
{
	public class NodeExecutionContext : INodeExecutionContext
	{
		public NodeExecutionContext(
			CommandLineParser args,
			Version version,
			string nodeCsExecutablePath,
			string nodeCsExtraBinDirecotry,
			string currentDirectory)
		{
			Args = args;
			Version = version;
			NodeCsExecutablePath = nodeCsExecutablePath;
			NodeCsExtraBinDirecotry = new LockFreeItem<string>(nodeCsExtraBinDirecotry);
			CurrentDirectory = new LockFreeItem<string>(currentDirectory);
		}
		public CommandLineParser Args { get; private set; }
		public Version Version { get; private set; }
		public string NodeCsExecutablePath { get; private set; }
		public LockFreeItem<string> NodeCsExtraBinDirecotry { get; private set; }
		public LockFreeItem<string> CurrentDirectory { get; set; }
	}
}
