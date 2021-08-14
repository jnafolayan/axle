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
        public void ShouldParseXLSXFile()
        {
            var filePath = GetResourcePath("file.xlsx");
            var xlsxParser = new SpreadsheetsParser();
            var contents = xlsxParser.ParseLocalFile(filePath);
            
            Assert.NotEmpty(contents);
        }

        [Fact]
        public void ShouldParseHTMLFile()
        {
            var filePath = GetResourcePath("file.html");
            var htmlParser = new HTMLParser();
            var contents = htmlParser.ParseLocalFile(filePath);
            
            Assert.NotEmpty(contents);
        }

        [Fact]
        public void ShouldExtractHTMLPageInfo()
        {
            var filePath = GetResourcePath("file.html");
            var htmlParser = new HTMLParser();
            var pageInfo = htmlParser.ExtractHTMLFileInfo(filePath);
             
            Assert.Equal("Test File", pageInfo.Title);
            Assert.Empty(pageInfo.Description);
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