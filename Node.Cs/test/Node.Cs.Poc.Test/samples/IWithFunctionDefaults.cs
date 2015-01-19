namespace Node.Cs.Poc.Test.samples
{
	public interface IWithFunctionDefaults
	{
		void VaArgsFunction(string par, params string[] pars);
		void ArrayFunction(string par, string[] pars);
		void NonOptionalFunction(string par, int pars);
		void OptionalFunction(string par, int pars = 1);
		void NonOptionalFunctionString(string par, string pars);
		void OptionalFunctionString(string par, string pars = "optionalValue");
		void NonOptionalFunctionObject(string par, FunctionParamsSource pars);
		void OptionalFunctionObject(string par, FunctionParamsSource pars = null);
	}
}