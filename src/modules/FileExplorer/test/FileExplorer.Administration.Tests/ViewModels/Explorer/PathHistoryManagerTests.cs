using FileExplorer.Administration.Models;
using FileExplorer.Administration.ViewModels.Explorer;
using FileExplorer.Administration.ViewModels.Explorer.Base;
using FileExplorer.Administration.ViewModels.Explorer.Helpers;
using Moq;
using Xunit;

namespace FileExplorer.Administration.Tests.ViewModels.Explorer
{
    public class PathHistoryManagerTests
    {
        public PathHistoryManagerTests()
        {
            var mock = new Mock<IFileSystem>();
            mock.Setup(x => x.ComparePaths(It.IsAny<string>(), It.IsAny<string>()))
                .Returns<string, string>((s, s1) => s.Equals(s1));

            _pathHistoryManager = new PathHistoryManager(mock.Object);
        }

        private readonly PathHistoryManager _pathHistoryManager;

        [Fact]
        public void TestGoBack()
        {
            _pathHistoryManager.Navigate("C:\\");
            _pathHistoryManager.Navigate("C:\\hello");
            var newPath = _pathHistoryManager.GoBack();

            Assert.Equal("C:\\", newPath);
            Assert.False(_pathHistoryManager.CanGoBack);
            Assert.True(_pathHistoryManager.CanGoForward);
        }

        [Fact]
        public void TestGoBackGoForward()
        {
            _pathHistoryManager.Navigate("C:\\");
            _pathHistoryManager.Navigate("C:\\hello");
            _pathHistoryManager.Navigate("C:\\hello\\hey");
            _pathHistoryManager.GoBack();
            _pathHistoryManager.Navigate("C:\\hello\\heyy");
            _pathHistoryManager.Navigate("C:\\hello\\heyy\\welcome");
            _pathHistoryManager.GoBack();
            _pathHistoryManager.GoBack();
            _pathHistoryManager.GoBack();

            Assert.Equal("C:\\", _pathHistoryManager.CurrentPath);
        }

        [Fact]
        public void TestGoForward()
        {
            _pathHistoryManager.Navigate("C:\\");
            _pathHistoryManager.Navigate("C:\\hello");

            var newPath = _pathHistoryManager.GoBack();
            Assert.Equal("C:\\", newPath);

            newPath = _pathHistoryManager.GoForward();
            Assert.Equal("C:\\hello", newPath);

            Assert.True(_pathHistoryManager.CanGoBack);
            Assert.False(_pathHistoryManager.CanGoForward);
        }

        [Fact]
        public void TestMoveOnInitialization()
        {
            Assert.False(_pathHistoryManager.CanGoBack);
            Assert.False(_pathHistoryManager.CanGoForward);
        }

        [Fact]
        public void TestNavigateToPath()
        {
            _pathHistoryManager.Navigate("C:\\");

            Assert.False(_pathHistoryManager.CanGoBack);
            Assert.False(_pathHistoryManager.CanGoForward);
            Assert.Equal("C:\\", _pathHistoryManager.CurrentPath);
        }

        [Fact]
        public void TestNavigateToTwoPaths()
        {
            _pathHistoryManager.Navigate("C:\\");

            Assert.False(_pathHistoryManager.CanGoBack);
            Assert.False(_pathHistoryManager.CanGoForward);
            Assert.Equal("C:\\", _pathHistoryManager.CurrentPath);

            _pathHistoryManager.Navigate("D:\\");
            Assert.True(_pathHistoryManager.CanGoBack);
            Assert.False(_pathHistoryManager.CanGoForward);
            Assert.Equal("D:\\", _pathHistoryManager.CurrentPath);
        }
    }
}