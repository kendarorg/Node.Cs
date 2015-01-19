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


using CoroutinesLib.Shared;
using CoroutinesLib.Shared.Logging;
using GenericHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NodeCs.Shared
{
	public static class NodeRoot
	{
		public static IMain Main { get; private set; }
		public static ILogger Log { get; private set; }


		public static string Pad(object data, int pad, string padVal = " ")
		{
			var str = data == null ? "" : data.ToString();
			if (str.Length > pad)
			{
				str = str.Substring(0, pad - 3) + "...";
			}
			else
			{
				while (str.Length < pad)
				{
					str += padVal;
				}
			}
			return str;
		}

		public static string Lpad(object data, int pad, string padVal = " ")
		{
			var str = data == null ? "" : data.ToString();
			if (str.Length > pad)
			{
				str = str.Substring(0, pad - 3) + "...";
			}
			else
			{
				while (str.Length < pad)
				{
					str = padVal + str;
				}
			}
			return str;
		}


		public static void CClean()
		{
			Console.Clear();
		}

		public static void WriteLine(object data = null)
		{
			var what = data == null ? "" : data.ToString();
			Console.WriteLine("NC>" + what);
		}

		public static void Write(object data = null)
		{
			var what = data == null ? "" : data.ToString();
			Console.Write("NC>" + what);
		}

		public static void CWriteLine(object data = null)
		{
			var what = data == null ? "" : data.ToString();
			Console.WriteLine(what);
		}

		public static void CWrite(object data = null)
		{
			var what = data == null ? "" : data.ToString();
			Console.Write(what);
		}

		private static readonly ConcurrentDictionary<string, INodeModule> _modules =
			new ConcurrentDictionary<string, INodeModule>(StringComparer.OrdinalIgnoreCase);

		public static INodeModule GetModule(string moduleId)
		{
			if (!_modules.ContainsKey(moduleId))
			{
				return null;
			}
			return _modules[moduleId];
		}

		public static INodeModule GetNamedModule(string name, string moduleId)
		{
			if (!_modules.ContainsKey(name + "@" + moduleId))
			{
				if (!_modules.ContainsKey(moduleId))
				{
					return null;
				}
				return _modules[moduleId];
			}
			return _modules[name + "@" + moduleId];
		}

		public static void StartModules()
		{
			var initialized = new HashSet<string>();
			var modules = new List<KeyValuePair<string, INodeModule>>(_modules);
			foreach (var module in modules)
			{
				InitializeModule(module, initialized);
			}
		}

		private static void InitializeModule(KeyValuePair<string, INodeModule> module, HashSet<string> initialized)
		{
			var def = FindDefinition(module.Key);
			var deps = new List<ModuleDependency>(def.Dependencies);
			foreach (var dep in deps)
			{
				var subModule = _modules[dep.Name];
				InitializeModule(new KeyValuePair<string, INodeModule>(dep.Name, subModule), initialized);
			}
			if (initialized.Contains(module.Key))
			{
				return;
			}
			initialized.Add(module.Key);
			NodeRoot.CWriteLine(string.Format("Initializing Module: {0}.", module.Key));
			module.Value.Initialize();

			var runnable = module.Value as IRunnableModule;
			if (runnable != null)
			{
				if (!runnable.IsRunning)
				{
					runnable.Start();
				}
			}
		}

		private static readonly Dictionary<string, ModuleDefinition> _moduleDefinitions =
			new Dictionary<string, ModuleDefinition>(StringComparer.OrdinalIgnoreCase);

		private static LogContainer _logContainer;

		private static ModuleDefinition FindDefinition(string moduleId)
		{
			var atIndex = moduleId.IndexOf("@", StringComparison.OrdinalIgnoreCase);
			if (atIndex > 0)
			{
				moduleId = moduleId.Substring(atIndex + 1);
			}
			if (_moduleDefinitions.ContainsKey(moduleId))
			{
				return _moduleDefinitions[moduleId];
			}
			var reflectionOnly = AssembliesManager.LoadReflectionAssemblyFrom(Main.AssemblyDirectory,
				moduleId + ".dll", false);
			if (reflectionOnly != null)
			{
				var moduleDescriptor = ResourceContentLoader.LoadText(moduleId + ".json", reflectionOnly, false);
				if (moduleDescriptor != null)
				{
					var result = JsonConvert.DeserializeObject<ModuleDefinition>(moduleDescriptor);
					_moduleDefinitions[moduleId] = result;
					if (!string.IsNullOrWhiteSpace(result.ReplaceItem))
					{
						_moduleDefinitions[result.ReplaceItem] = result;
					}
					return result;
				}
			}
			throw new NotImplementedException("Not implemented automatic module download!\r\nMissing module '" + moduleId + "'");
		}

		public static INodeModule LoadNamedModule(string moduleName, string moduleId, string moduleVersion = null)
		{
			ModuleDefinition moduleDefinition = FindDefinition(moduleId);

			foreach (var dep in moduleDefinition.Dependencies)
			{
				LoadNamedModule(moduleName, dep.Name, dep.Version);
			}
			var moduleIndex = moduleName == null ? moduleId : moduleName + "@" + moduleId;
			if (moduleDefinition.Singleton)
			{
				moduleIndex = moduleId;
			}
			var anotherOne = GetModule(moduleIndex);
			if (anotherOne != null)
			{
				return anotherOne;
			}

			if (AssembliesManager.LoadAssemblyFrom(Main.AssemblyDirectory,
				moduleId + ".dll", false))
			{
				var moduleType = AssembliesManager.LoadType(moduleDefinition.Class);
				if (moduleType != null)
				{
					var result = (INodeModule)ServiceLocator.Locator.Resolve(moduleType);
					NodeRoot.CWriteLine(string.Format("Loaded Module: {0} Version: {1}.", Pad(moduleId, 20), moduleVersion ?? "X.X.X.X"));
					_modules[moduleIndex] = result;
					return result;
				}
			}
			throw new NotImplementedException("Not implemented automatic module download!");
		}

		public static IEnumerable<ModuleInstance> Modules
		{
			get
			{
				foreach (var module in _modules)
				{
					yield return new ModuleInstance(module.Key, module.Value);
				}
			}
		}

		public static INodeModule LoadModule(string moduleId, string moduleVersion = null)
		{
			return LoadNamedModule(null, moduleId, moduleVersion);
		}

		public static void Initialize(IMain main)
		{
			Main = main;
			_logContainer = new LogContainer();
			ServiceLocator.Locator.Register<ILogger>(_logContainer);
			ServiceLocator.Locator.Register<IMain>(Main);
			ServiceLocator.Locator.Register<ICoroutinesManager>(Main.CoroutinesManager);
			_logContainer.SetLoggingLevel(_loggingLevel);
			Log = _logContainer;
			Main.Log = _logContainer;
			Main.CoroutinesManager.Log = _logContainer;
		}


		public static void RegisterLogger(ILogger logger)
		{
			_logContainer.RegisterLogger(logger);
		}
		public static void UnregisterLogger(ILogger logger)
		{
			_logContainer.UnregisterLogger(logger);
		}

		public static void LoadDll(string dll)
		{
			AssembliesManager.LoadAssemblyFrom(Main.AssemblyDirectory, dll + ".dll");
		}
		private static LoggerLevel _loggingLevel;
		public static void SetLoggingLevel(string level)
		{
			_loggingLevel = (LoggerLevel)Enum.Parse(typeof(LoggerLevel), level, true);
			_logContainer.SetLoggingLevel(_loggingLevel);
		}

		public static void SetLogFile(string logFilePath)
		{
			_logContainer.SetLogFile(logFilePath);
		}
	}
}
