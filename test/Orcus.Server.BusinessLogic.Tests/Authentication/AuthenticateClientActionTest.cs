using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NuGet.Versioning;
using Orcus.Server.BusinessDataAccess.Clients;
using Orcus.Server.BusinessLogic.Authentication;
using Orcus.Server.Connection;
using Orcus.Server.Connection.Authentication.Client;
using Orcus.Server.Data.EfClasses;
using Xunit;
using Xunit.Abstractions;

namespace Orcus.Server.BusinessLogic.Tests.Authentication
{
    public class AuthenticateClientActionTest : BusinessLogicTestBase<AuthenticateClientAction>
    {
        public AuthenticateClientActionTest(ITestOutputHelper output) : base(output)
        {
        }

        protected AuthenticateClientAction CreateBusinessLogic(Action<Mock<IClientDbAccess>> setupDbAccess)
        {
            var mock = new Mock<IClientDbAccess>();
            setupDbAccess(mock);
            return new AuthenticateClientAction(mock.Object, Logger);
        }

        protected ClientAuthenticationContext DefaultAuthenticationContext = new ClientAuthenticationContext
        {
            Dto = new ClientAuthenticationDto
            {
                Username = "Nora",
                OperatingSystem = "Windows 10",
                SystemLanguage = "de",
                ClientPath = "C:\\hello.world",
                HardwareId = new Hash(SHA256.Create().ComputeHash(Encoding.ASCII.GetBytes("N.O.R.A"))).ToString(),
                MacAddress = new byte[] {10, 11, 12, 13, 14, 67},
                ClientVersion = new NuGetVersion(1, 2, 3)
            },
            IpAddress = IPAddress.Parse("192.168.178.89")
        };

        [Fact]
        public async Task TestExistingHardwareId()
        {
            var action = CreateBusinessLogic(mock =>
                mock.Setup(x => x.FindClientByHardwareId(DefaultAuthenticationContext.Dto.HardwareId))
                    .ReturnsAsync(new Client {ClientSessions = new List<ClientSession>()}));

            var result = await action.BizActionAsync(DefaultAuthenticationContext);
            Assert.NotNull(result);
            Assert.False(action.HasErrors);
            Assert.Single(result.ClientSessions);
        }
    }
}