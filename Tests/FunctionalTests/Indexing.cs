using System;
using System.IO;
using Xunit;
using Axle.Engine;
using Axle.Engine.FileParsers;
using System.Collections.Generic;

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
        public void ShouldRemoveStopWords()
        {
            Indexer indexer = new Indexer(new FileParserFactory(), GetResourcePath("../../Engine/resources/stopwords.txt"));
            string text = "i want to eat";
            List<string> result = indexer.RemoveStopWords(new List<string>(text.Split(" ")));
            Assert.Equal("eat", String.Join(" ", result));
        }

        [Fact]
        public void ShouldStemWords()
        {
            Indexer indexer = new Indexer(new FileParserFactory(), GetResourcePath("../../Engine/resources/stopwords.txt"));

            var result1 = indexer.StemWords(new List<string> { "friendly" });
            var result2 = indexer.StemWords(new List<string> { "fishing" });
            var result3 = indexer.StemWords(new List<string> { "rolling" });
            Assert.Equal("friend", result1[0]);
            Assert.Equal("fish", result2[0]);
            Assert.Equal("roll", result3[0]);
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

            Assert.Equal((decimal)2 / 5, tfScores["hello"].TFScore);
            Assert.Equal((decimal)1 / 5, tfScores["world"].TFScore);
        }

        /*
        [Fact]
        public void ShouldBuildIndex()
        {
            var fpFactory = new FileParserFactory();
            fpFactory.RegisterParser("html", new HTMLParser());
            fpFactory.RegisterParser("xlsx", new SpreadsheetsParser());
            fpFactory.RegisterParser("docx", new WordDocumentParser());

            Indexer indexer = new Indexer(fpFactory, GetResourcePath("../../Engine/resources/stopwords.txt"));
            var documents = new List<string>{
                GetResourcePath("file.docx"),
                GetResourcePath("file.html"),
                GetResourcePath("file.xlsx")
            };

            var index = indexer.BuildIndex(documents);
            Assert.NotEmpty(index);
            Assert.NotEmpty(index["lorem"]);
            Assert.NotEqual(0, index["lorem"][0].TFScore);
        }
        */
    }
}