using log4net;
using Moq;
using NUnit.Framework;
using Zapp.Fuse;

namespace Zapp.Schedule
{
    [TestFixture]
    public class ScheduleServiceTests : TestBiolerplate<ScheduleService>
    {
        [Test]
        public void Schedule_WhenGlobalExtractionNotCompletedAndFinishesNow_LogsInfo()
        {
            kernel.GetMock<IFusionService>()
                .Setup(m => m.TryExtract())
                .Returns(true);

            var sut = GetSystemUnderTest();

            sut.Schedule(new[] { "test" });

            kernel.GetMock<ILog>()
                .Verify(m => m.Info(It.IsAny<string>()), Times.Exactly(2));
        }

        [Test]
        public void Schedule_WhenGlobalExtractionNotCompletedAndDoesNotFinishes_LogsFatal()
        {
            kernel.GetMock<IFusionService>()
                .Setup(m => m.TryExtract())
                .Returns(false);

            var sut = GetSystemUnderTest();

            sut.Schedule(new[] { "test" });

            kernel.GetMock<ILog>()
                .Verify(m => m.Info(It.IsAny<string>()), Times.Exactly(1));

            kernel.GetMock<ILog>()
                .Verify(m => m.Fatal(It.IsAny<string>()), Times.Exactly(1));
        }
    }
}
