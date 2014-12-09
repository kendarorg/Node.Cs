using System;
using ConcurrencyHelpers.Containers;

namespace Node.Cs
{
	public interface INodeExecutionContext
	{
		string[] Args { get; }
		Version Version { get; }
		string NodeCsExecutablePath { get; }
		LockFreeItem<string> NodeCsExtraBinDirecotry { get; }
		LockFreeItem<string> CurrentDirectory { get; set; }
	}
}