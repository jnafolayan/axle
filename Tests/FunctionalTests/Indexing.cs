using System;
using System.IO;
using Xunit;
using Axle.Engine.Indexer;
using Axle.Engine.FileParsers;

namespace Axle.FunctionalTests.Indexing

{
    public class IndexerTests
    {
        private string GetResourcePath(string fileName)
        {
            var cwd = Directory.GetParent(Environment.CurrentDirectory);
            string parent = Path.Combine(cwd.Parent.Parent.FullName, "Tests/resources");
            return Path.Combine(parent, fileName);
        }

        [Fact]
        public void ShouldRemovePunctuation()
        {
            Indexer indexer = new Indexer();
            string text = "hello, world! hi; hee.";
            string result = indexer.RemovePunctuation(text);
            Assert.Equal("hello world hi hee", result);
        }

        [Fact]
        public void ShouldCountWordFreqs()
        {
            Indexer indexer = new Indexer();
            string text = "hello, hello, world! hi; hee.";
            int wordCount;
            var wordFreqs = indexer.CountWordFrequencies(text, out wordCount);

            Assert.Equal(5, wordCount);
            Assert.Equal(4, wordFreqs.Count);
            Assert.Equal(2, wordFreqs["hello"]);
            Assert.Equal(1, wordFreqs["world"]);
        }

        [Fact]
        public void ShouldCalculateTFScores()
        {
            Indexer indexer = new Indexer();
            string text = "hello, hello, world! hi; hee.";
            int wordCount;
            var wordFreqs = indexer.CountWordFrequencies(text, out wordCount);
            var tfScores = indexer.CalculateTFScores(wordFreqs, wordCount);

            Assert.Equal(2 / 5.0, tfScores["hello"].TFScore);
            Assert.Equal(1 / 5.0, tfScores["world"].TFScore);
        }

        [Fact]
        public void ShouldBuildIndex()
        {
            var fpFactory = new FileParserFactory();
            fpFactory.RegisterParser("html", new HTMLParser());
            fpFactory.RegisterParser("xlsx", new SpreadsheetsParser());
            fpFactory.RegisterParser("docx", new WordDocumentParser());

            Indexer indexer = new Indexer(fpFactory, GetResourcePath("../../Engine/resources/stopwords.txt"));
            string[] documents = new string[]{
                GetResourcePath("file.docx"),
                GetResourcePath("file.html"),
                GetResourcePath("file.xlsx")
            };
            
            var index = indexer.BuildIndex(documents);
            Assert.NotEmpty(index);
            Assert.NotEmpty(index[0].Value);
            Assert.NotEqual(0, index[0].Value[0].TFScore);
            // foreach (var entry in index)
            // {
            //     var s = "";
            //     entry.Value.ForEach((t) => {
            //         if (s.Length > 0) s += ", ";
            //         s += "( " + Path.GetFileName(t.DocumentURL) + ", score: " + t.TFScore + " )"; 
            //     });
            //     Console.WriteLine("{0,25}: {1}", entry.Key, s);
            // }
        }
    }
}