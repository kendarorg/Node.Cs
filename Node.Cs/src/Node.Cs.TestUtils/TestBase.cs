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
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using Moq;
using Node.Cs.Utils;

namespace Node.Cs.Test
{
	public abstract class TestBase<T, TI> : TestBase
		where T : class, TI
		where TI : class
	{
		protected TI Target { get; private set; }
		protected virtual void SetupTarget(T target = default(T))
		{
			if (target == default(T))
			{
				Container.Register(
					Component.For<TI>().ImplementedBy<T>().LifestyleSingleton());
				Target = Container.Resolve<TI>();
			}
			else
			{
				Target = target;
			}
		}
	}

	public abstract class TestBase<T> : TestBase where T : class
	{
		protected T Target { get; private set; }
		protected virtual void SetupTarget(T target = default(T))
		{
			if (target == default(T))
			{
				Container.Register(
					Component.For<T>().ImplementedBy<T>().LifestyleSingleton());
				Target = Container.Resolve<T>();
			}
			else
			{
				Target = target;
			}
		}
	}

	public abstract class TestBase
	{
		public WindsorContainer Container { get; private set; }

		public virtual void TestInitialize()
		{
			Container = new WindsorContainer();
			Container.Register(Component.For<IWindsorContainer>()
				.Instance(Container));
			Container.Register(Component.For<ILazyComponentLoader>()
				.Instance(new LazyComponentAutoMocker(Container)));
		}



		protected void CopyDllOnTarget(string externalDllName, INodeExecutionContext context)
		{
#if DEBUG
			const string dest = "Debug";
#else
			const string dest = "Release";
#endif
			var srcDll = Path.Combine(PathResolver.GetSolutionRoot(), "test", "mockProjects", externalDllName, "bin", dest,
				externalDllName + ".dll");
			srcDll = PathResolver.FindByPath(srcDll, Assembly.GetCallingAssembly());
			if (string.IsNullOrWhiteSpace(srcDll) || !File.Exists(srcDll))
			{

				srcDll = PathResolver.FindByPath(Path.Combine(PathResolver.GetAssemblyPath(Assembly.GetCallingAssembly()), externalDllName + ".dll"), Assembly.GetCallingAssembly());
			}
			var destDll = Path.Combine(context.CurrentDirectory.Data, externalDllName + ".dll");
			if (srcDll == destDll)
			{
				return;
			}

			if (File.Exists(destDll))
			{
				File.Delete(destDll);
			}
			File.Copy(srcDll, destDll);
		}

		protected void InitializeMock<T>() where T : class
		{
			var t = MockOf<T>();
			Debug.Assert(t != null);
		}

		protected Mock<T> MockOf<T>() where T : class
		{
			var t = Container.Resolve<T>();
			Debug.Assert(t != null);
			return Container.Resolve<Mock<T>>();
		}

		protected T Object<T>() where T : class
		{
			Container.Register(Component.For<T>());
			return Container.Resolve<T>();
		}

		[DebuggerStepThrough]
		[DebuggerNonUserCode]
		protected void RunSeries<TA>(Action<TA> action, params TA[] pars)
		{
			TA par1 = default(TA);
			try
			{
				foreach (var par in pars)
				{
					TestInitialize();
					par1 = par;
					action.Invoke(par1);
				}
			}
			catch (Exception ex)
			{
				AddMessage(ex, string.Format("\r\nExecuting with parameter '{0}'.", par1));
				throw;
			}
		}

		protected Tuple<TA, TB> Tuple<TA, TB>(TA item1, TB item2)
		{
			return new Tuple<TA, TB>(item1, item2);
		}

		[DebuggerStepThrough]
		[DebuggerNonUserCode]
		protected void RunSeries<TA, TB>(Action<TA, TB> action, params Tuple<TA, TB>[] pars)
		{
			TA par1 = default(TA);
			TB par2 = default(TB);
			try
			{
				foreach (var par in pars)
				{
					TestInitialize();
					par1 = par.Item1;
					par2 = par.Item2;
					action.Invoke(par1, par2);
				}
			}
			catch (Exception ex)
			{
				AddMessage(ex, string.Format("\r\nExecuting with parameters '{0}', '{1}'.", par1, par2));
				throw;
			}
		}

		private void AddMessage(Exception exception, string message)
		{
			var fi = exception.GetType().GetField("_message", BindingFlags.NonPublic | BindingFlags.Instance);
			if (fi == null)
			{
				throw new MissingFieldException(exception.GetType().ToString(), "_message");
			}
			var value = fi.GetValue(exception) as string;
			// ReSharper disable once ConvertIfStatementToNullCoalescingExpression
			if (value == null)
			{
				value = string.Empty;
			}
			fi.SetValue(exception, message + value);
		}


		protected Tuple<TA, TB, TC> Tuple<TA, TB, TC>(TA item1, TB item2, TC item3)
		{
			return new Tuple<TA, TB, TC>(item1, item2, item3);
		}

		[DebuggerStepThrough]
		[DebuggerNonUserCode]
		protected void RunSeries<TA, TB, TC>(Action<TA, TB, TC> action, params Tuple<TA, TB, TC>[] pars)
		{
			TA par1 = default(TA);
			TB par2 = default(TB);
			TC par3 = default(TC);
			try
			{
				foreach (var par in pars)
				{
					TestInitialize();
					par1 = par.Item1;
					par2 = par.Item2;
					par3 = par.Item3;
					action.Invoke(par1, par2, par3);
				}
			}
			catch (Exception ex)
			{
				AddMessage(ex, string.Format("\r\nExecuting with parameters '{0}', '{1}', '{2}'.", par1, par2, par3));
				throw;
			}
		}
	}
}