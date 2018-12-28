using System;
using System.Threading.Tasks;
using Moq;
using Maze.Server.BusinessDataAccess.Accounts;
using Maze.Server.BusinessLogic.Authentication;
using Maze.Server.Connection.Authentication;
using Maze.Server.Data.EfClasses;
using Xunit;
using Xunit.Abstractions;

namespace Maze.Server.BusinessLogic.Tests.Authentication
{
    public class AuthenticateAdministrationActionTest : BusinessLogicTestBase<AuthenticateAdministrationAction>
    {
        public AuthenticateAdministrationActionTest(ITestOutputHelper output) : base(output)
        {
        }

        protected AuthenticateAdministrationAction CreateBusinessLogic(Action<Mock<IAccountDbAccess>> setupDbAccess)
        {
            var mock = new Mock<IAccountDbAccess>();
            setupDbAccess(mock);
            return new AuthenticateAdministrationAction(mock.Object, Logger);
        }

        [Fact]
        public async Task TestCorrectPassword()
        {
            var action =
                CreateBusinessLogic(mock =>
                    mock.Setup(x => x.FindAccountByUsername("Nora")).ReturnsAsync(new Account
                    {
                        IsEnabled = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("2018")
                    }));

            var result = await action.BizActionAsync(new LoginInfo {Username = "Nora", Password = "2018"});
            Assert.NotNull(result);
            Assert.False(action.HasErrors);
        }

        [Fact]
        public async Task TestInvalidPassword()
        {
            var action =
                CreateBusinessLogic(mock =>
                    mock.Setup(x => x.FindAccountByUsername("Nora")).ReturnsAsync(new Account
                    {
                        IsEnabled = true,
                        Password = BCrypt.Net.BCrypt.HashPassword("2018")
                    }));

            var result = await action.BizActionAsync(new LoginInfo {Username = "Nora", Password = "test"});
            Assert.Null(result);
            Assert.True(action.HasErrors);
        }

        [Fact]
        public async Task TestDisabledAccount()
        {
            var action =
                CreateBusinessLogic(mock =>
                    mock.Setup(x => x.FindAccountByUsername("Nora")).ReturnsAsync(new Account {IsEnabled = false}));

            var result = await action.BizActionAsync(new LoginInfo {Username = "Nora", Password = "test"});
            Assert.Null(result);
            Assert.True(action.HasErrors);
        }

        [Fact]
        public async Task TestInvalidUsername()
        {
            var action =
                CreateBusinessLogic(mock =>
                    mock.Setup(x => x.FindAccountByUsername("Nora")).ReturnsAsync(default(Account)));

            var result = await action.BizActionAsync(new LoginInfo {Username = "Nora", Password = "test"});
            Assert.Null(result);
            Assert.True(action.HasErrors);
        }

        [Fact]
        public async Task TestNullValues()
        {
            var action = CreateBusinessLogic(mock => { });
            var result = await action.BizActionAsync(new LoginInfo());
            Assert.Null(result);
            Assert.True(action.HasErrors);
        }
    }
}