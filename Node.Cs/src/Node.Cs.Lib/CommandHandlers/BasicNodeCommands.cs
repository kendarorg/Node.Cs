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
using Node.Cs.Consoles;
using System.IO;
using System.Reflection;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using SharpTemplate.Compilers;

namespace Node.Cs.CommandHandlers
{
    public class BasicNodeCommands : IBasicNodeCommands
    {
        private readonly INodeConsole _console;
        private IWindsorContainer _container;

        public BasicNodeCommands(INodeConsole console,IWindsorContainer container)
        {
            _console = console;
            _container = container;
        }

        /// <summary>
        /// Runs a .cs file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="path"></param>
        public void Run(INodeExecutionContext context, string path)
        {
            path = path.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            var absolutePath = Path.Combine(context.CurrentDirectory.Data.Trim(Path.DirectorySeparatorChar), path.Trim(Path.DirectorySeparatorChar));
            if (!File.Exists(absolutePath))
            {
                throw new FileNotFoundException("File not found.", path);
            }
            if (Path.GetExtension(absolutePath).ToLowerInvariant() == ".cs")
            {
                RunAndBuild(absolutePath, context);
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

        private void RunAndBuild(string absolutePath, INodeExecutionContext context)
        {
            var source = File.ReadAllText(absolutePath);
            var md5 = NodeUtils.CalculateMD5Hash(absolutePath);
            var dllName = "MD5-" + md5 + ".dll";
            var path = context.NodeCsExtraBinDirectory.Data;

            var sc = new SourceCompiler(dllName, path);

            var ns = "Node.Cs.ScriptFiles";
            var cl =  "MD5-"+md5;
            var fqn = ns+"."+cl;

            sc.AddFile(ns,cl, source);
            sc.LoadCurrentAssemblies();
            var loadedAssembly = sc.Compile();

            var content = File.ReadAllBytes(loadedAssembly);
            var compileSimpleObjectAsm = Assembly.Load(content);
            var realType = compileSimpleObjectAsm.GetType(fqn,true,true);
            var result = _container.Resolve(realType) as IScript;
            result.Execute();
        }
    }
}
