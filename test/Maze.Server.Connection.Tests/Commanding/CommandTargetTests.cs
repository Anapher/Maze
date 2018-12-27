using Orcus.Server.Connection.Commanding;
using Xunit;

namespace Orcus.Server.Connection.Tests.Commanding
{
    public class CommandTargetTests
    {
        public static TheoryData<string, CommandTargetCollection> MultipleClientsTestData =
            new TheoryData<string, CommandTargetCollection>
            {
                {
                    "C1,C45,C675,C23",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1),
                        new CommandTarget(CommandTargetType.Client, 45),
                        new CommandTarget(CommandTargetType.Client, 675),
                        new CommandTarget(CommandTargetType.Client, 23)
                    }
                },
                {
                    "C1,C2",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1),
                        new CommandTarget(CommandTargetType.Client, 2),
                    }
                }
            };

        public static TheoryData<string, CommandTargetCollection> ClientRangeTestData =
            new TheoryData<string, CommandTargetCollection>
            {
                {
                    "C1-8,C45-50",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1, 8),
                        new CommandTarget(CommandTargetType.Client, 45, 50),
                    }
                },
                {
                    "C1-100,C9-1000",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1, 100),
                        new CommandTarget(CommandTargetType.Client, 9, 1000),
                    }
                }
            };

        public static TheoryData<string, CommandTargetCollection> MixedTestData =
            new TheoryData<string, CommandTargetCollection>
            {
                {
                    "C1-8,G5,C100-115,G8-10",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1, 8),
                        new CommandTarget(CommandTargetType.Group, 5),
                        new CommandTarget(CommandTargetType.Client, 100, 115),
                        new CommandTarget(CommandTargetType.Group, 8, 10),
                    }
                },
                {
                    "C1,C4,C6,G7",
                    new CommandTargetCollection
                    {
                        new CommandTarget(CommandTargetType.Client, 1),
                        new CommandTarget(CommandTargetType.Client, 4),
                        new CommandTarget(CommandTargetType.Client, 6),
                        new CommandTarget(CommandTargetType.Group, 7),
                    }
                }
            };

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("Server")]
        public void TestParseServer(string value)
        {
            var target = CommandTargetCollection.Parse(value);
            Assert.Empty(target);
            Assert.True(target.TargetsServer);
        }

        [Theory]
        [InlineData("C23", 23)]
        [InlineData("C1", 1)]
        [InlineData("C435345345", 435345345)]
        public void TestParseSingleClientTarget(string value, int expectedClientId)
        {
            var target = CommandTargetCollection.Parse(value);
            Assert.True(target.IsSingleClient(out var clientId));
            Assert.Equal(expectedClientId, clientId);
        }

        [Theory]
        [MemberData(nameof(MultipleClientsTestData))]
        public void TestParseMultipleClients(string value, CommandTargetCollection expectedTarget)
        {
            var target = CommandTargetCollection.Parse(value);
            CompareTargetCollections(target, expectedTarget);
        }

        [Theory]
        [MemberData(nameof(ClientRangeTestData))]
        public void TestParseClientRanges(string value, CommandTargetCollection expectedTarget)
        {
            var target = CommandTargetCollection.Parse(value);
            CompareTargetCollections(target, expectedTarget);
        }

        [Theory]
        [MemberData(nameof(MixedTestData))]
        public void TestParseMixedData(string value, CommandTargetCollection expectedTarget)
        {
            var target = CommandTargetCollection.Parse(value);
            CompareTargetCollections(target, expectedTarget);
        }

        [Theory]
        [InlineData("C1-8,C9,C10-15,C17", "C1-15,C17")]
        [InlineData("C1", "C1")]
        [InlineData("C9,C5", "C5,C9")]
        [InlineData("C9-12", "C9-12")]
        [InlineData("C6-9,C7-20,C21-40", "C6-40")]
        [InlineData("G9,C12,G6-10,C13,C14-20,G11,G14", "C12-20,G6-11,G14")]
        public void TestSimplifyCollection(string origin, string expectedResult)
        {
            var collection = CommandTargetCollection.Parse(origin);
            Assert.Equal(expectedResult, collection.ToString());
        }

        private void CompareTargetCollections(CommandTargetCollection actual, CommandTargetCollection expected)
        {
            Assert.Equal(expected.TargetsServer, actual.TargetsServer);
            Assert.Equal(expected.Count, actual.Count);

            for (var i = 0; i < expected.Count; i++)
            {
                var commandTarget = expected[i];
                var actualCommandTarget = actual[i];
                Assert.Equal(commandTarget.Type, actualCommandTarget.Type);
                Assert.Equal(commandTarget.From, actualCommandTarget.From);
                Assert.Equal(commandTarget.To, actualCommandTarget.To);
            }
        }
    }
}