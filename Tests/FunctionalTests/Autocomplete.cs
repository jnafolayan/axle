using System.Collections.Generic;
using System;
using Xunit;
using Axle.Engine;
using Axle.Engine.Database.Models.Autocomplete;

namespace Axle.FunctionalTests.AutoCompletion
{
    public class AutoCompleteTests
    {
        [Fact]
        public void ShouldExtractSentences()
        {
            AutoComplete autoCompleter = new AutoComplete();

            string text = "sentence 1. sentence2.";
            List<string> result = autoCompleter.GetSentences(text);
            Assert.Equal("sentence 1", result[0]);
            Assert.Equal("sentence2", result[1]);
        }

        [Fact]
        public void ShouldExtractTokens()
        {
            AutoComplete autoCompleter = new AutoComplete();

            string sentence = "this is my [ sentence";
            List<string> tokens = autoCompleter.GetTokens(sentence);
            Assert.Equal(6, tokens.Count);
            Assert.Equal("<start>", tokens[0]);
            Assert.Equal("<end>", tokens[tokens.Count - 1]);
        }

        [Fact]
        public void ShouldUpdateUnigram()
        {
            AutoComplete autoCompleter = new AutoComplete();
            var unigram = new Dictionary<string, UnigramModel>();
            autoCompleter.updateUnigram("a", unigram);
            autoCompleter.updateUnigram("b", unigram);
            autoCompleter.updateUnigram("a", unigram);
            Assert.Equal(2, unigram["a"].Count);
            Assert.Equal(1, unigram["b"].Count);
        }

        [Fact]
        public void ShouldUpdateBigram()
        {
            AutoComplete autoCompleter = new AutoComplete();
            var bigram = new Dictionary<string, Dictionary<string, BigramModel>>();
            autoCompleter.updateBigram("a", "b", bigram);
            autoCompleter.updateBigram("b", "c", bigram);
            autoCompleter.updateBigram("a", "b", bigram);
            Assert.Equal(2, bigram["a"]["b"].Count);
            Assert.Equal(1, bigram["b"]["c"].Count);
        }
    }
}