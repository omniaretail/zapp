using NUnit.Framework;
using System;
using System.IO;

namespace Zapp.Pack
{
    [TestFixture]
    public class ZipPackageServiceTests
    {
        private ZipPackageService sut;

        [SetUp]
        public void Setup()
        {
            sut = new ZipPackageService();
        }
    }
}
