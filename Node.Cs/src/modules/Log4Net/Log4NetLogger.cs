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


using CoroutinesLib.Shared.Logging;
using log4net;
using System;
using System.Reflection;

namespace NodeLog4Net
{
	public class Log4NetLog : BaseLogger
	{
		private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		protected override void LogException(Exception exception, string format, object[] pars, LoggerLevel level)
		{
			var toWrite = string.Format(format, pars);
			switch (level)
			{
				case (LoggerLevel.Fatal):
					_log.Fatal(toWrite, exception);
					break;
				case (LoggerLevel.Error):
					_log.Error(toWrite, exception);
					break;
				case (LoggerLevel.Warning):
					_log.Warn(toWrite, exception);
					break;
				case (LoggerLevel.Info):
					_log.Info(toWrite, exception);
					break;
				case (LoggerLevel.Debug):
					_log.Debug(toWrite, exception);
					break;
			}
		}

		protected override void WriteLine(string toWrite, LoggerLevel level)
		{
			
		}

		protected override void LogFormat(string format, object[] pars, LoggerLevel level)
		{
			var toWrite = string.Format(format, pars);
			switch (level)
			{
				case (LoggerLevel.Fatal):
					_log.Fatal(toWrite);
					break;
				case (LoggerLevel.Error):
					_log.Error(toWrite);
					break;
				case (LoggerLevel.Warning):
					_log.Warn(toWrite);
					break;
				case (LoggerLevel.Info):
					_log.Info(toWrite);
					break;
				case (LoggerLevel.Debug):
					_log.Debug(toWrite);
					break;
			}
		}
	}
}
