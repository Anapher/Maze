using System;
using System.IO.Abstractions.TestingHelpers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Tasks.Infrastructure.Client.Library;
using Tasks.Infrastructure.Client.Options;
using Tasks.Infrastructure.Client.Storage;
using Tasks.Infrastructure.Core;
using Xunit;

namespace Tasks.Infrastructure.Client.Tests
{
    public class TaskSessionManagerTests
    {
        [Fact]
        public async Task TestOpenNonExistingSession()
        {
            var fileSystem = new MockFileSystem();
            var options = new TasksOptions {SessionsDirectory = "C:\\test"};

            var taskId = Guid.Parse("CB49D689-95FD-4A09-A78A-3A4397E9425E");

            var sessionManager = new DatabaseTaskStorage(fileSystem, null, new OptionsWrapper<TasksOptions>(options));
            var session = await sessionManager.OpenSession(SessionKey.Create("test"), new OrcusTask {Id = taskId}, "Test123");

            Assert.Empty(fileSystem.AllFiles);
            Assert.NotNull(session);
        }

        //[Fact]
        //public async Task TestCreateExecution()
        //{
        //    var fileSystem = new MockFileSystem();
        //    var options = new TasksOptions {SessionsDirectory = "C:\\test"};

        //    var taskId = Guid.Parse("CB49D689-95FD-4A09-A78A-3A4397E9425E");
        //    var sessionKey = SessionKey.Create("test");

        //    var mock = new Mock<IRequestTransmitter>();
        //    mock.Setup(x => x.Transmit(It.IsAny<HttpRequestMessage>())).ReturnsAsync(true);

        //    var sessionManager = new DatabaseTaskStorage(fileSystem, mock.Object, new OptionsWrapper<TasksOptions>(options));
        //    await sessionManager.CreateExecution(new OrcusTask {Id = taskId},
        //        new TaskSession {Description = "test description", TaskSessionId = sessionKey.Hash},
        //        new TaskExecution {Result = Convert.ToBase64String(Encoding.UTF8.GetBytes("Test"))});

        //    var path = Assert.Single(fileSystem.AllFiles);
        //    Assert.Equal($"C:\\test\\{taskId:N}", path);

        //    mock.Verify(x => x.Transmit(It.IsAny<HttpRequestMessage>()), Times.Once);;
        //}
    }
}