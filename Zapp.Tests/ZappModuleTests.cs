using Ninject;
using NUnit.Framework;
using System.Linq;
using Zapp.Fuse;
using Zapp.Schedule;

namespace Zapp
{
    [TestFixture]
    public class ZappModuleTests
    {
        private readonly string[] pluginTypes = new[]
        {
            nameof(IFusionFilter),
            nameof(IFusionInterceptor),
            nameof(IFusionProcessDrainer),
            nameof(IFusionProcessInterceptor),
        };

        [Test]
        public void Interfaces_WhenImplemented_MustHaveBinding()
        {
            var testCases = typeof(ZappModule).Assembly
                .GetTypes()
                .Where(t => t.IsInterface && t.IsPublic)
                .Where(t => !pluginTypes.Contains(t.Name))
                .ToList();

            var kernel =  new StandardKernel(new ZappModule());

            foreach (var @case in testCases)
            {
                var bindings = kernel.GetBindings(@case);

                Assert.That(bindings, Is.Not.Empty, $"Interface {@case.Name} does not have a proper binding for module {nameof(ZappModule)}.");
            }
        }
    }
}
