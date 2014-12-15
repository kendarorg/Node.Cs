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
using System.Diagnostics;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.Windsor;
using Moq;

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
			var value = fi.GetValue(exception) as string;
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