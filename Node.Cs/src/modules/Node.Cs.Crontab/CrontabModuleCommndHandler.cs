using Node.Cs.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs.Crontab
{
    public interface ICrontabModuleCommndHandler
    {

        void List(Utils.INodeExecutionContext obj);

        void Remove(Utils.INodeExecutionContext arg1, string arg2);

        void Register(Utils.INodeExecutionContext arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7);
    }

    public class CrontabModuleCommndHandler : ICrontabModuleCommndHandler, INodeModuleService
    {
        public void List(Utils.INodeExecutionContext obj)
        {
            throw new NotImplementedException();
        }

        public void Remove(Utils.INodeExecutionContext arg1, string arg2)
        {
            throw new NotImplementedException();
        }

        public void Register(Utils.INodeExecutionContext arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7)
        {
            throw new NotImplementedException();
        }
    }
}
