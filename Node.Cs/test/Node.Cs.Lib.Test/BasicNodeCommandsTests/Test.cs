using Node.Cs.Consoles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs.BasicNodeCommandsTests
{
    public class Test : IScript
    {
        private INodeConsole _console;
        public Test(INodeConsole console, INodeExecutionContext context)
        {
            _console = console;
        }
        public void Execute()
        {
            _console.WriteLine("Executing Test.cs");
        }
    }
}
