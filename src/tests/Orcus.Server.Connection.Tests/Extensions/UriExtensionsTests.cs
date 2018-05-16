using System;
using System.Collections.Generic;
using Orcus.Server.Connection.Extensions;
using Xunit;

namespace Orcus.Server.Connection.Tests.Extensions
{
    public class UriExtensionsTests
    {
        public static IEnumerable<object[]> Data =>
            new List<object[]>
            {
                new object[]
                {
                    new Uri("/", UriKind.Relative), "nora", "beautiful", new Uri("/?nora=beautiful", UriKind.Relative)
                },
                new object[]
                {
                    new Uri("/?weather=nice&day=fine", UriKind.Relative), "nora", "beautiful",
                    new Uri("/?weather=nice&day=fine&nora=beautiful", UriKind.Relative)
                },
                new object[]
                {
                    new Uri("/#fragment", UriKind.Relative), "nora", "beautiful",
                    new Uri("/?nora=beautiful#fragment", UriKind.Relative)
                },
                new object[]
                {
                    new Uri("hello/world/", UriKind.Relative), "nora", "beautiful",
                    new Uri("hello/world/?nora=beautiful", UriKind.Relative)
                },
                new object[]
                {
                    new Uri("hello/world/?weather=nice&day=fine", UriKind.Relative), "nora", "beautiful",
                    new Uri("hello/world/?weather=nice&day=fine&nora=beautiful", UriKind.Relative)
                },
                new object[]
                {
                    new Uri("hello/world/#fragment", UriKind.Relative), "nora", "beautiful",
                    new Uri("hello/world/?nora=beautiful#fragment", UriKind.Relative)
                },

                new object[]
                {
                    new Uri("https://www.codeelements.net"), "nora", "beautiful",
                    new Uri("https://www.codeelements.net?nora=beautiful")
                },
                new object[]
                {
                    new Uri("http://www.codeelements.net"), "nora", "beautiful",
                    new Uri("http://www.codeelements.net?nora=beautiful")
                },
                new object[]
                {
                    new Uri("https://www.codeelements.net?weather=nice"), "nora", "beautiful",
                    new Uri("https://www.codeelements.net?weather=nice&nora=beautiful")
                },
                new object[]
                {
                    new Uri("https://www.codeelements.net:123?weather=nice"), "nora", "beautiful",
                    new Uri("https://www.codeelements.net:123?weather=nice&nora=beautiful")
                }
            };

        [Theory]
        [MemberData(nameof(Data))]
        public void TestAddQuery(Uri uri, string paramName, string paramValue, Uri result)
        {
            var actual = uri.AddQueryParameters(paramName, paramValue);
            Assert.Equal(result, actual);
        }
    }
}