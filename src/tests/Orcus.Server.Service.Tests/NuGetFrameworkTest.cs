using System;
using NuGet.Frameworks;
using Xunit;

namespace Orcus.Server.Service.Tests
{
    public class NuGetFrameworkTest
    {
        [Fact]
        public void TestCreateOrcusFramework()
        {
            var x = new NuGetFramework("Orcus.Server", new Version(1, 0));
        }
    }
}