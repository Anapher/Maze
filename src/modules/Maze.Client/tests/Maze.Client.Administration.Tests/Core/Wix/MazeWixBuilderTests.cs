using System.Linq;
using Maze.Client.Administration.Core;
using Maze.Server.Connection.Clients;
using Xunit;

namespace Maze.Client.Administration.Tests.Core.Wix
{
    public class MazeWixBuilderTests
    {
        [Fact]
        public void TestGetLoadOnStartupPackages()
        {
            var configs = new[]
            {
                @"{
  ""Modules"": {
    ""LocalPath"": ""%appdata%/Maze/modules"",
    ""TempPath"": ""%appdata%/Maze/tmp"",
    ""LoadOnStartup"": {
      ""Tasks.Infrastructure"": ""1.4.56"",
      ""Tasks.Common"": ""1.4.56""
    }
  },
  ""Connection"": {
    ""ServerUris"": {
      ""MainServer"": ""http://localhost:54941/""
    },
    ""ReconnectDelay"": 5000
  }
}",
                @"{
  ""Modules"": {
    ""LocalPath"": ""%appdata%/Maze/modules"",
    ""TempPath"": ""%appdata%/Maze/tmp""
  },
  ""Connection"": {
    ""ServerUris"": {
      ""MainServer"": ""http://localhost:54941/""
    },
    ""ReconnectDelay"": 5000
  }
}",
                @"{
  ""Modules"": {
    ""LocalPath"": ""%appdata%/Maze/modules"",
    ""TempPath"": ""%appdata%/Maze/tmp"",
    ""LoadOnStartup"": {
      ""Tasks.Infrastructure"": ""1.0"",
      ""RemoteDesktop"": ""1.0"",
      ""TaskManager"": ""1.2""
    }
  },
  ""Connection"": {
    ""ServerUris"": {
      ""MainServer"": ""http://localhost:54941/""
    },
    ""ReconnectDelay"": 5000
  }
}"
            };

            var dtos = configs.Select(x => new ClientConfigurationDto {Content = x}).ToArray();
            var loadOnStartupPackages = MazeWixBuilder.GetLoadOnStartupPackages(dtos);
            Assert.Collection(loadOnStartupPackages, x => Assert.Equal("Tasks.Infrastructure", x), x => Assert.Equal("Tasks.Common", x),
                x => Assert.Equal("RemoteDesktop", x), x => Assert.Equal("TaskManager", x));
        }
    }
}