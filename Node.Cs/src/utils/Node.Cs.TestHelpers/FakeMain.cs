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
using NodeCs.Shared;
using System.Threading;

namespace Node.Cs.TestHelpers
{
	public class FakeMain : IMain
	{
		public FakeMain(string assemblyDirectory, ICoroutinesManager coroutinesManager)
		{
			AssemblyDirectory = assemblyDirectory;
			CoroutinesManager = coroutinesManager;
		}
		public string AssemblyDirectory { get; private set; }
		public void Initialize(string[] args, string helpMessage)
		{

		}

		public bool Wait(int ms)
		{
			return false;
		}

		public void Abort()
		{

		}

		public void Start()
		{

		}

		public void Stop()
		{

		}

		public void StopIncoming()
		{

		}

		public void AllowIncoming()
		{

		}

		public bool Execute(string result)
		{
			return true;
		}

		public void RenewLease()
		{

		}

		public void AddThread(Thread thread, string threadName)
		{

		}

		public void RemoveThread(string threadName)
		{

		}

		public void RegisterCommand(string name, CommandDefinition definition)
		{

		}

		public void UnregisterCommand(string name, int paramsCount)
		{

		}

		public ICoroutinesManager CoroutinesManager { get; private set; }
		public ILogger Log { get; set; }
	}
}