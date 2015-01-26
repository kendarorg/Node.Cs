using Kendar.TestUtils;
using Node.Cs.Utils;

namespace Node.Cs.Test
{
	public static class NodeCopyUtils
	{
		public static void CopyDllOnTarget(string externalDllName, INodeExecutionContext context)
		{
			CopyUtils.CopyDllOnTarget(externalDllName, context.CurrentDirectory.Data, new[] { "test", "mockProjects", externalDllName });
		}
	}
}