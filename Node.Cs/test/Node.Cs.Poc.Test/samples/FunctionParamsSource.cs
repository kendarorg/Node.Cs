using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Node.Cs.Poc.Test.samples
{
    public class FunctionParamsSource
    {
        public void VaArgsFunction(string par, params string[] pars)
        {

        }

        public void ArrayFunction(string par, string[] pars)
        {

        }


        public void NonOptionalFunction(string par, int pars)
        {

        }

        public void OptionalFunction(string par, int pars = 1)
        {

        }

        public void NonOptionalFunctionString(string par, string pars)
        {

        }

        public void OptionalFunctionString(string par, string pars = "optionalValue")
        {

        }


        public void NonOptionalFunctionObject(string par, FunctionParamsSource pars)
        {

        }

        public void OptionalFunctionObject(string par, FunctionParamsSource pars = null)
        {

        }
    }
}
