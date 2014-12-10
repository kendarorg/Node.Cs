using System;
using ConcurrencyHelpers.Containers;
using GenericHelpers;

namespace Node.Cs
{
	public interface INodeExecutionContext
	{
		CommandLineParser Args { get; }
		Version Version { get; }
		string NodeCsExecutablePath { get; }
		LockFreeItem<string> NodeCsExtraBinDirecotry { get; }
		LockFreeItem<string> CurrentDirectory { get; set; }
	}
}