using System;
using System.IO;
using Xunit;
using Axle.Engine.FileParsers;

namespace Axle.UnitTests.FileParsers
{
    public class FileParsersTests
    {
        private string GetResourcePath(string fileName)
        {
            var cwd = Directory.GetParent(Environment.CurrentDirectory);
            string parent = Path.Combine(cwd.Parent.Parent.FullName, "Tests/resources");
            return Path.Combine(parent, fileName);
        }
    }

    public class FileParserFactoryTests
    {
        [Fact]
        public void ShouldRegisterFileParsers()
        {
            // TODO:
            // var ext = "txt";
            // var fpFactory = new FileParserFactory();
            // fpFactory.RegisterParser(ext, new FileParserBase());
            
            // Assert.IsType<AlmightyFileParser>(fpFactory.GetParser(ext));
        }
    }
}