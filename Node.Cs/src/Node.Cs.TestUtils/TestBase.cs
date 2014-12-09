using Castle.MicroKernel.Registration;
using Castle.Windsor;

namespace Node.Cs.Test
{
	public abstract class TestBase
	{
		public WindsorContainer Container { get; private set; }

		protected TestBase()
		{
			Container = new WindsorContainer();
			Container.Register(Component.For<LazyComponentAutoMocker>());
		}

		protected T MockOf<T>() where T : class
		{
			return Container.Resolve<T>();
		}

		protected T Create<T>() where T : class
		{
			Container.Register(Component.For<T>());
			return Container.Resolve<T>();
		}
	}
}