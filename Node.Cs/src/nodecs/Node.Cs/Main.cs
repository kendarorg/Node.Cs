// ===========================================================
// Copyright (c) 2014-2015, Enrico Da Ros/kendar.org
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without
// modification, are permitted provided that the following conditions are met:
// 
// * Redistributions of source code must retain the above copyright notice, this
//   list of conditions and the following disclaimer.
// 
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
// DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE
// FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
// SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
// CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE
// OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// ===========================================================


using System;
using System.IO;
using System.Reflection;
using System.Threading;
using CoroutinesLib;
using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Enums;
using CoroutinesLib.Shared.Logging;
using GenericHelpers;
using NodeCs.Parser;
using NodeCs.Shared;

namespace NodeCs
{


	public class Main : MarshalByRefObject, IMain
	{
		private CommandLineParser _clp;
		public string AssemblyDirectory { get; private set; }
		private string _configPath;
		private ICoroutinesManager _coroutinesManager;
		private NodeCsParser _parser;
		private static string _binPath;
		private static string _binFallBackPath;

		public ICoroutinesManager CoroutinesManager
		{
			get { return _coroutinesManager; }
		}

		public void Initialize(string[] args, string helpMessage)
		{
			AssemblyDirectory = AssembliesManager.GetAssemblyDirectory();
			_clp = new CommandLineParser(args, helpMessage ?? string.Empty);
			_configPath = _clp.GetOrDefault("config", Path.Combine(AssemblyDirectory, "config.json"));
			_binPath = _clp.GetOrDefault("tempBin", Path.Combine(AssemblyDirectory, "bin"));
			_binFallBackPath = AssemblyDirectory;


			RunnerFactory.Initialize();
			_coroutinesManager = RunnerFactory.Create();
			_parser = new NodeCsParser(_clp, _configPath, _coroutinesManager);
			NodeRoot.Initialize(this);
		}

		public bool Wait(int ms)
		{
			return WaitHelper.Wait(() => _coroutinesManager.Status.HasFlag(RunningStatus.Stopped), new TimeSpan(0, 0, 0, 0, ms));
		}

		public void Abort()
		{
			_coroutinesManager.Abort();
		}


		static Assembly LoadFromBinPath(object sender, ResolveEventArgs args)
		{
			var name = new AssemblyName(args.Name).Name;
			string assemblyPath = Path.Combine(_binPath, name + ".dll");
			if (!File.Exists(assemblyPath))
			{
				assemblyPath = Path.Combine(_binFallBackPath, name + ".dll");
				if (!File.Exists(assemblyPath))
				{
					Console.WriteLine("Unable to load " + assemblyPath);
					return null;
				}
			}
			Assembly assembly = Assembly.LoadFrom(assemblyPath);
			return assembly;
		}




		static Assembly LoadReflectionFromBinPath(object sender, ResolveEventArgs args)
		{
			string assemblyPath = Path.Combine(_binPath, new AssemblyName(args.Name).Name + ".dll");
			if (!File.Exists(assemblyPath) == false)
			{
				assemblyPath = Path.Combine(_binFallBackPath, new AssemblyName(args.Name).Name + ".dll");
				if (!File.Exists(assemblyPath))
				{
					return null;
				}
			}
			Assembly assembly = Assembly.ReflectionOnlyLoad(assemblyPath);
			return assembly;
		}

		public void Start()
		{
			AppDomain currentDomain = AppDomain.CurrentDomain;
			currentDomain.AssemblyResolve += LoadFromBinPath;
			currentDomain.ReflectionOnlyAssemblyResolve += LoadReflectionFromBinPath;
			_coroutinesManager.Start();
		}

		public void Stop()
		{
			_coroutinesManager.Stop();
		}

		public void StopIncoming()
		{
			_coroutinesManager.AllowIncomingMessage = false;
		}

		public void AllowIncoming()
		{
			_coroutinesManager.AllowIncomingMessage = true;
		}

		public bool Execute(string result)
		{
			return _parser.Execute(result);
		}

		public void RenewLease()
		{

		}

		public void AddThread(Thread thread, string threadName)
		{
			Commands.AddThread(thread, threadName);
		}

		public void RemoveThread(string threadName)
		{
			Commands.RemoveThread(threadName);
		}

		public void RegisterCommand(string name, CommandDefinition definition)
		{
			Commands.RegisterCommand(name, definition);
		}

		public void UnregisterCommand(string name, int paramsCount)
		{
			Commands.UnregisterCommand(name, paramsCount);
		}

		public ILogger Log { get; set; }
	}
}
