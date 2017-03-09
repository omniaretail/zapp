using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Zapp.Config;

namespace Zapp
{
    public class TestBiolerplate<T> where T : class
    {
        protected MoqMockingKernel kernel;

        protected ZappConfig config;

        [SetUp]
        public virtual void Setup()
        {
            kernel = new MoqMockingKernel();

            config = new ZappConfig();

            kernel.GetMock<IConfigStore>()
                .Setup(m => m.Value)
                .Returns(() => config);
        }

        [TearDown]
        public virtual void Teardown()
        {
            kernel?.Dispose();
            kernel = null;
        }

        protected T sut => kernel.Get<T>();

        protected T GetSystemUnderTest() => kernel.Get<T>();
    }
}
