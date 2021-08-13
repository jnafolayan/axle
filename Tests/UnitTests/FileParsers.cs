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
        
        [Fact]
        public void ShouldNotParseNonExistingFiles()
        {
            var almighty = new AlmightyFileParser();
            Assert.Throws<FileNotFoundException>(() => almighty.ParseLocalFile("heh.txt"));
        }

        [Fact]
        public void ShouldParseTxtFiles()
        {
            var almighty = new AlmightyFileParser();
            var textContent = almighty.ParseLocalFile(GetResourcePath("file.txt"));
            Assert.NotEmpty(textContent);
        }
    }

    public class FileParserFactoryTests
    {
        [Fact]
        public void ShouldRegisterFileParsers()
        {
            var ext = "txt";
            var fpFactory = new FileParserFactory();
            fpFactory.RegisterParser(ext, new AlmightyFileParser());
            
            Assert.IsType<AlmightyFileParser>(fpFactory.GetParser(ext));
        }
    }
}