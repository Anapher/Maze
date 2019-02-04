using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Maze.Client.Administration.Core;
using Maze.Client.Administration.Core.Wix;
using Xunit;

namespace Maze.Client.Administration.Tests.Core
{
    public class WixComponentGeneratorTests
    {
        private static string Generate(WixComponentGenerator generator, IEnumerable<WixFile> files)
        {
            var stringWriter = new StringWriter();
            using (var xmlWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings {Indent = true}))
            {
                generator.Write(files, xmlWriter);
            }

            return stringWriter.ToString();
        }

        private static void CompareDocuments(string source, string expectedPattern)
        {
            var regexPattern = Regex.Escape(expectedPattern);
            regexPattern = regexPattern.Replace("RANDOM", ".*?");

            //https://stackoverflow.com/questions/11040707/c-sharp-regex-for-guid
            regexPattern = regexPattern.Replace("GUID", "[{(]?[0-9A-F]{8}[-]?(?:[0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?");

            Assert.Matches("^" + regexPattern + "$", source);
        }

        [Fact]
        public void TestGenerateComponent()
        {
            var files = new List<WixFile>
            {
                new WixFile("C:\\Anapher.Wpf.Toolkit.Metro.dll", null),
                new WixFile("C:\\Anapher.Wpf.Toolkit.dll", null),
                new WixFile("C:\\CodeElements.Async.dll", null),
                new WixFile("C:\\Test\\Programs\\CodeElements.Async.dll", null),
                new WixFile("C:\\Test\\Programs\\CodeElements.Suite.LicenseSystem.dll", "suite"),
                new WixFile("C:\\Test\\CodeElements.Suite.LicenseSystem.pdb", "suite"),
                new WixFile("C:\\Test\\CodeElements.Suite.Resources.dll", "suite"),
                new WixFile("C:\\CodeElements.Suite.SyntaxHighlightBox.dll", "suite\\res"),
                new WixFile("C:\\odeElements.Suite.SyntaxHighlightBox.dll", "suite\\res")
            };

            var generator = new WixComponentGenerator("CodeElementsFiles", "MainDirectory");
            var result = Generate(generator, files);

            const string expected = @"<?xml version=""1.0"" encoding=""utf-16""?>
<Wix xmlns=""http://schemas.microsoft.com/wix/2006/wi"">
  <Fragment>
    <DirectoryRef Id=""MainDirectory"">
      <Component Id=""cmpRANDOM"" Guid=""GUID"">
        <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\Anapher.Wpf.Toolkit.Metro.dll"" />
      </Component>
      <Component Id=""cmpRANDOM"" Guid=""GUID"">
        <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\Anapher.Wpf.Toolkit.dll"" />
      </Component>
      <Component Id=""cmpRANDOM"" Guid=""GUID"">
        <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\CodeElements.Async.dll"" />
      </Component>
      <Component Id=""cmpRANDOM"" Guid=""GUID"">
        <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\Test\Programs\CodeElements.Async.dll"" />
      </Component>
      <Directory Id=""dirRANDOM"" Name=""suite"">
        <Component Id=""cmpRANDOM"" Guid=""GUID"">
          <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\Test\Programs\CodeElements.Suite.LicenseSystem.dll"" />
        </Component>
        <Directory Id=""dirRANDOM"" Name=""res"">
          <Component Id=""cmpRANDOM"" Guid=""GUID"">
            <File Id=""fiRANDOM"" KeyPath=""yes"" Source=""C:\CodeElements.Suite.SyntaxHighlightBox.dll"" />
          </Component>
        </Directory>
      </Directory>
    </DirectoryRef>
  </Fragment>
  <Fragment>
    <ComponentGroup Id=""CodeElementsFiles"">
      <ComponentRef Id=""cmpRANDOM"" />
      <ComponentRef Id=""cmpRANDOM"" />
      <ComponentRef Id=""cmpRANDOM"" />
      <ComponentRef Id=""cmpRANDOM"" />
      <ComponentRef Id=""cmpRANDOM"" />
      <ComponentRef Id=""cmpRANDOM"" />
    </ComponentGroup>
  </Fragment>
</Wix>";
            CompareDocuments(result, expected);
        }
    }
}