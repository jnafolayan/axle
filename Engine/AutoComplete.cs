using System.Collections.Generic;
using System;
using System.Text.RegularExpressions;
using Axle.Engine.Database.Models.Autocomplete;
using Axle.Engine.Database;
using MongoDB.Driver;
// Typedef for Dictionary<string, Dictionary<string, BigramModel>
using BigramType = System.Collections.Generic.Dictionary<string, System.Collections.Generic.Dictionary<string, Axle.Engine.Database.Models.Autocomplete.BigramModel>>;
// Typedef for Dictionary<string, Dictionary<string, UnigramModel>
using UnigramType = System.Collections.Generic.Dictionary<string, Axle.Engine.Database.Models.Autocomplete.UnigramModel>;

namespace Axle.Engine
{
    /// <summary>
    /// Autocomplete class
    /// </summary>
    public class AutoComplete
    {
        private Store _store;
        public AutoComplete()
        {
        }
        public AutoComplete(Store store)
        {
            _store = store;
        }
        public AutoCompleteIndex Index(string text)
        {
            var unigramDict = new UnigramType();
            var bigramDict = new BigramType();

            // Build unigram and bigram dictionary from tokens
            List<string> sentences = GetSentences(text);
            foreach(string sentence in sentences)
            {
                List<string> tokens = GetTokens(sentence);
                for(int i = 1; i < tokens.Count; i++){
                    string before = tokens[i-1], after = tokens[i];
                    if (i == 1)
                        updateUnigram(tokens[0], unigramDict);

                    updateUnigram(tokens[i], unigramDict); 
                    updateBigram(before, after, bigramDict);
                }
            }

            // Disgusting!!
            var bigramList = new List<BigramModel>();
            foreach (string key in bigramDict.Keys)
            {
                foreach (BigramModel bm in bigramDict[key].Values)
                {
                    bigramList.Add(bm);
                }                
            }

            return new AutoCompleteIndex{
                Bigram = bigramList,
                Unigram = new List<UnigramModel>(unigramDict.Values)
            };

        }

        public void saveUnigram(List<UnigramModel> unigram)
        {
            _store.InsertOrUpdateUnigramDocuments(unigram);
        }

        public UnigramType updateUnigram(string token, UnigramType unigram)
        {
            if(!unigram.ContainsKey(token))
            {
                unigram.Add(token, new UnigramModel{
                    Token = token,
                    Count = 1
                });
                return unigram;
            }
            unigram[token].Count++;
            return unigram;
        }

        public BigramType updateBigram(string before, string after, BigramType bigram)
        {

            if(!bigram.ContainsKey(before))
            {
                bigram.Add(before, new Dictionary<string, BigramModel>());
            }
            if(!bigram[before].ContainsKey(after))
            {
                bigram[before].Add(after, new BigramModel{
                    Before = before,
                    After = after,
                    Count = 1,
                    Probability = 0,
                    LogProbability = 0
                });
                return bigram;
            }
            // Both exists, increment count
            bigram[before][after].Count++;
            return bigram;
        }

        public List<string> GetSentences(string text) 
        {
            List<string> sentences = new List<string>(text.Split("."));
            sentences = sentences.ConvertAll<string>((sentence) => {
                return sentence.Trim();
            });
            return sentences;
        }

        public List<string> GetTokens(string sentence)
        {
            // Splits a sentence into tokens
            // removes all punctuations and empty strings
            // Bounds sentence with <start> and <end> symbol
            string cleanSentence = RemovePunctuation(sentence);
            cleanSentence = RemoveSpecialCharacters(cleanSentence);

            List<string> terms = new List<string>(Regex.Split(cleanSentence, @"\s"));

            terms = RemoveEmptyStrings(terms);
            terms.Insert(0, "<start>");
            terms.Add("<end>");

            return terms;
        }

        // <summary>
        /// Removes special characters
        /// </summary>
        /// <param name="text">Body of text</param>
        /// <returns>A list of filtered words</returns>
        public string RemoveSpecialCharacters(string text)
        {
            return Regex.Replace(text, @"[^0-9a-zA-Z\s]+", "");
        }

        /// <summary>
        /// Removes empty strings
        /// </summary>
        /// <param name="words">A list of words</param>
        /// <returns>A list of filtered words</returns>
        public List<string> RemoveEmptyStrings(List<string> words)
        {
            return words.FindAll((string word) => word != "");
        }

        /// <summary>
        /// Removes punctuations
        /// </summary>
        /// <param name="text">Body of text</param>
        /// <returns>A list of filtered words</returns>
        public string RemovePunctuation(string text)
        {
            return Regex.Replace(text, @"\p{P}", "");
        }
    }

    public class AutoCompleteIndex {
        public List<BigramModel> Bigram { get; set; }
        public List<UnigramModel> Unigram { get; set; }
    }
}
