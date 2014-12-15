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



using System;
using System.Collections.Generic;
using System.Linq;
using Node.Cs.Consoles;
using System.IO;
using System.Reflection;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using SharpTemplate.Compilers;

namespace Node.Cs.CommandHandlers
{
	internal class RunnableDefinition
	{
		public DateTime Timestamp { get; set; }
		public Type Type { get; set; }
	}

	public class BasicNodeCommands : IBasicNodeCommands
	{
		private readonly INodeConsole _console;
		private readonly IWindsorContainer _container;
		private readonly Dictionary<string, RunnableDefinition> _runnables =
new Dictionary<string, RunnableDefinition>(StringComparer.InvariantCultureIgnoreCase);

		public BasicNodeCommands(INodeConsole console, IWindsorContainer container)
		{
			_console = console;
			_container = container;
		}

		/// <summary>
		/// Runs a .cs file
		/// </summary>
		/// <param name="context"></param>
		/// <param name="path"></param>
		public void Run(INodeExecutionContext context, string path, string function = null)
		{
			if (function == null)
			{
				function = "Execute";
			}
			path = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
			var absolutePath = Path.Combine(context.CurrentDirectory.Data.Trim(Path.DirectorySeparatorChar), path.Trim(Path.DirectorySeparatorChar));
			if (!File.Exists(absolutePath))
			{
				throw new FileNotFoundException("File not found.", path);
			}
			if (Path.GetExtension(absolutePath).ToLowerInvariant() == ".cs")
			{
				RunAndBuild(absolutePath, context, function);
			}
			else if (Path.GetExtension(absolutePath).ToLowerInvariant() == ".ncs")
			{
				throw new NotImplementedException();
			}
			else
			{
				throw new NotSupportedException();
			}
		}

		public void Echo(INodeExecutionContext context, string message)
		{
			_console.WriteLine(message);
		}

		public void Exit(INodeExecutionContext context, int errorCode)
		{
			Environment.Exit(errorCode);
		}

		private void RunAndBuild(string path, INodeExecutionContext context, string function)
		{
			var loadedAssembly = string.Empty;
			var dllName = "dll_" + NodeUtils.CalculateMD5Hash(path.ToLowerInvariant());
			var className = "class_"+dllName;
			var namespaceName = "ns_" + dllName;
			try
			{
				Type type;
				if (_runnables.ContainsKey(path))
				{
					var def = _runnables[path];
					if (def.Timestamp < File.GetLastWriteTime(path))
					{
						_runnables.Remove(path);
					}
				}
				if (!_runnables.ContainsKey(path))
				{
					var source = "namespace " + namespaceName + " {\r\n" + File.ReadAllText(path) + "\r\n}";

					var sc = new SourceCompiler(dllName, context.TempPath)
					{
						UseAppdomain = true
					};
					sc.AddFile(namespaceName, className, source);
					sc.LoadCurrentAssemblies();
					loadedAssembly = sc.Compile(0);
					if (sc.HasErrors)
					{
						var errs = new HashSet<string>();
						var compilationErrors = "Error compiling " + path;
						foreach (var errorList in sc.Errors)
						{
							var singleErrorList = errorList;
							foreach (var error in singleErrorList)
							{
								if (!errs.Contains(error))
								{
									compilationErrors += "\r\n" + error;
									errs.Add(error);
								}
							}
						}
						throw new Exception(compilationErrors.Replace("\t", " "));
					}
					var content = File.ReadAllBytes(loadedAssembly);
					var compileSimpleObjectAsm = Assembly.Load(content);
					type =
					compileSimpleObjectAsm.GetTypes()
					.FirstOrDefault(a => a.GetMethods().Any(m => String.Equals(m.Name, function, StringComparison.InvariantCultureIgnoreCase)));
					_runnables.Add(path, new RunnableDefinition
					{
						Type = type,
						Timestamp = File.GetLastWriteTime(path)
					});
					_container.Register(Component.For(type).LifestyleTransient());
				}
				else
				{
					var def = _runnables[path];
					type = def.Type;
				}

				var instance = _container.Resolve(type);
				var methodInfo = instance.GetType().GetMethods().First(m => String.Equals(m.Name, function, StringComparison.InvariantCultureIgnoreCase));
				methodInfo.Invoke(instance, new object[] { });
			}
			finally
			{
				if (File.Exists(loadedAssembly))
				{
					File.Delete(loadedAssembly);
				}
			}
		}
	}
}
