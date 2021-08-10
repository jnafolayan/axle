using System;
using Xunit;
using Axle.Engine.FileParsers;
using Axle.Engine.FileParsers.Exceptions;

namespace Axle.UnitTests.FileParsers
{
    public class FileParserBaseTest
    {
        [Fact]
        public void ShouldNotParseNonExistingFiles()
        {
            var almighty = new AlmightyFileParser();
            Assert.Throws<FileNotFoundException>(() => almighty.ParseLocalFile("testFiles/heh.txt"));
        }

        [Fact]
        public void ShouldParseTxtFiles()
        {
            var almighty = new AlmightyFileParser();
            var textContent = almighty.ParseLocalFile("testFiles/file.txt");

            Assert.NotEmpty(textContent);
        }
    }
}