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
using System.Diagnostics;
using CoroutinesLib.Shared.Logging;

namespace ConsoleLoggerModule
{

	public class ConsoleLogger : BaseLogger
	{
		private readonly bool _console;
		private readonly bool _debug;
		private readonly bool _trace;
		private ConsoleColor _color;

		public ConsoleLogger(LoggerLevel level, bool console = true, bool trace = false, bool debug = false) :
			base(level)
		{
			_console = console;
			_debug = debug;
			_trace = trace;
			_color = Console.BackgroundColor;
		}

		protected override void WriteLine(string toWrite, LoggerLevel level)
		{
			switch (level)
			{
				case (LoggerLevel.Fatal):
				case (LoggerLevel.Error):
					Console.BackgroundColor = ConsoleColor.Red;
					break;
				case (LoggerLevel.Warning):
					Console.BackgroundColor = ConsoleColor.Yellow;
					break;
				//case (LoggerLevel.Info):
				//case (LoggerLevel.Debug):
				default:
					Console.BackgroundColor = _color;
					break;
			}

			if (_console) Console.WriteLine(toWrite);
			if (_trace) Trace.WriteLine(toWrite);
#if DEBUG
			if (_debug) System.Diagnostics.Debug.WriteLine(toWrite);
#endif
		}
	}
}
