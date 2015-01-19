using Node.Cs.CommandHandlers;
using Node.Cs.Modules;
using Node.Cs.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs.Crontab
{
    public class CrontabModule:INodeModule
    {

        private const string NAME = "Node.Cs.CrontabModule";
        private static readonly Version _version = new Version("2.0.0.0");

        private IUiCommandsHandler _commandsHandler;
        ICrontabModuleCommndHandler _crontabHandler;

        public CrontabModule(IUiCommandsHandler commandsHandler,ICrontabModuleCommndHandler crontabHandler)
        {
            _commandsHandler = commandsHandler;
            _crontabHandler = crontabHandler;
        }
        public string Name
        {
            get { return NAME; }
        }

        public Version Version
        {
            get { return _version; }
        }

        public void Initialize()
        {
            _commandsHandler.RegisterCommand(
                new CommandDescriptor(
                    "crontab register", new Action<INodeExecutionContext, string, string, string, string, string, string>(_crontabHandler.Register), "crontab register [crontab expression] [.cs(::Function)|.ncs|Class::Function] (p1) (p2) (p3) (p4)"));
            
            _commandsHandler.RegisterCommand(
                new CommandDescriptor(
                    "crontab remove", new Action<INodeExecutionContext, string>(_crontabHandler.Remove), "crontab remove [crontab expression|id from list]"));

            _commandsHandler.RegisterCommand(
                new CommandDescriptor(
                    "crontab list", new Action<INodeExecutionContext>(_crontabHandler.List), "crontab list"));
        }
    }
}
