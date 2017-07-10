using Moq;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.AutoMoq;
using Zapp.Config;

namespace Zapp
{
    public class TestBiolerplate<T> where T : class
    {
        protected IFixture fixture;
        protected MoqMockingKernel kernel;

        protected ZappConfig config;

        [SetUp]
        public virtual void Setup()
        {
            fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            kernel = new MoqMockingKernel();

            config = new ZappConfig();

            kernel.GetMock<IConfigStore>()
                .Setup(_ => _.Value)
                .Returns(() => config);

            var configMock = fixture.Freeze<Mock<IConfigStore>>();

            configMock
                .Setup(_ => _.Value)
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
