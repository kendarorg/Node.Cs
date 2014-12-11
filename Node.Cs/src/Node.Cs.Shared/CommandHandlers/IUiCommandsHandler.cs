// ===========================================================
// Copyright (C) 2014-2015 Kendar.org
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation 
// files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, 
// modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software 
// is furnished to do so, subject to the following conditions:
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES 
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS 
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF 
// OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// ===========================================================



namespace Node.Cs.CommandHandlers
{
	public interface IUiCommandsHandler
	{
		/// <summary>
		/// Register a command
		/// </summary>
		/// <param name="cd"></param>
		void RegisterCommand(CommandDescriptor cd);

		/// <summary>
		/// Run a command passing all parameters
		/// </summary>
		/// <param name="command"></param>
		void Run(string command);

		/// <summary>
		/// Show the help for the given command/subcommand couple
		/// </summary>
		/// <param name="context"></param>
		/// <param name="command"></param>
		/// <param name="subcommand"></param>
		void Help(NodeExecutionContext context, string command = null, string subcommand = null);
	}
}
