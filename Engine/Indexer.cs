using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using Axle.Engine.FileParsers;
using Porter2Stemmer;

namespace Axle.Engine
{
    /// <summary>
    /// Indexer class
    /// </summary>
    public class Indexer
    {
        private HashSet<string> stopWords = new HashSet<string>();
        private EnglishPorter2Stemmer stemmer = new EnglishPorter2Stemmer();
        private FileParserFactory parserFactory;

        public Indexer()
        {
        }

        /// <summary>
        /// Two-param constructor for the Indexer class
        /// </summary>
        /// <param name="parserFactory">A reference to the parser factory</param>
        /// <param name="stopWordsFileURL">The path to the stopwords file</param>
        public Indexer(FileParserFactory parserFactory_, string stopWordsFileURL)
        {
            parserFactory = parserFactory_;
            ReadStopWords(stopWordsFileURL);
        }

        public Indexer(FileParserFactory parserFactory_)
        {
            parserFactory = parserFactory_;
        }
        
        /// <summary>
        /// Reads the stopwords file into memory
        /// </summary>
        /// <param name="stopWordsFileURL">The path to the stopwords file</param>
        public void ReadStopWords(string stopWordsFileURL)
        {
            stopWords.Clear();
            foreach (string line in File.ReadAllLines(stopWordsFileURL))
            {
                stopWords.Add(line);
            }
        }
        public Dictionary<string, List<TokenDocument>> BuildIndex(List<string> documentURLs)
        {
            return BuildIndex(documentURLs, (string s) => {});
        }

        /// <summary>
        /// Builds an index from a list of documents
        /// </summary>
        /// <param name="documentURLs">A list of document file paths</param>
        /// <returns>A list of (token, document_token) pairs</returns>
        public Dictionary<string, List<TokenDocument>> BuildIndex(List<string> documentURLs, Action<string> onAdded)
        {
            var tokenMap = new ConcurrentDictionary<string, List<TokenDocument>>();
            int maxTasksPerRun = 50;
            int i = 0;
            Task[] taskArray = new Task[maxTasksPerRun];

            Func<string, List<TokenDocument>> CreateTokenDocumentList = (_) => new List<TokenDocument>();

            Func<object, object> IndexFunc = (object url) =>
            {
                var documentURL = (string)url;
                Dictionary<string, TokenDocument> tfScores = ParseDocument(documentURL);

                foreach (string token in tfScores.Keys)
                {
                    tokenMap.GetOrAdd(token, CreateTokenDocumentList);
                    tfScores[token].SourcePath = documentURL;
                    tokenMap[token].Add(tfScores[token]);
                }

                onAdded(documentURL);

                return null;
            };

            while (i < documentURLs.Count)
            {
                int stop = Math.Min(documentURLs.Count, i + maxTasksPerRun);
                int k = 0;

                int batchSize = stop - i;
                if (batchSize < maxTasksPerRun)
                {
                    taskArray = new Task[batchSize];
                }

                for (; i < stop; i++)
                {
                    taskArray[k++] = Task<object>.Factory.StartNew(IndexFunc, documentURLs[i]);
                }

                Task.WaitAll(taskArray);
            }

            return new Dictionary<string, List<TokenDocument>>(tokenMap);
        }

        /// <summary>
        /// Parses a single document file
        /// </summary>
        /// <param name="documentURL">The path to the document</param>
        /// <returns>(token, document_token) pairs</returns>
        public Dictionary<string, TokenDocument> ParseDocument(string documentURL)
        {
            // fetch the appropriate parser for the document
            string ext = Path.GetExtension(documentURL).Substring(1);
            FileParserBase parser = parserFactory.GetParser(ext);

            // parse the text
            string text = parser.ParseLocalFile(documentURL).ToLower();
            int termsCount;

            Dictionary<string, int> wordFreqs = CountWordFrequencies(text, out termsCount);
            Dictionary<string, TokenDocument> tfScores = CalculateTFScores(wordFreqs, termsCount);

            return tfScores;
        }

        /// <summary>
        /// Counts the number of occurences of each word in a text
        /// </summary>
        /// <param name="text">The body of text</param>
        /// <param name="termsCount">
        /// Pointer to a count variable that stores the total number of words in the
        /// text. 
        /// </param>
        /// <returns>token/count pairs</returns>
        public Dictionary<string, int> CountWordFrequencies(string text, out int termsCount)
        {
            var wordFreqs = new Dictionary<string, int>();
            List<string> terms = PreprocessText(text);

            foreach (string term in terms)
            {
                int count = 0;
                wordFreqs.TryGetValue(term, out count);
                wordFreqs[term] = count + 1;
            }

            termsCount = terms.Count;
            return wordFreqs;
        }

        /// <summary>
        /// Calculate the term frequency scores for each token
        /// </summary>
        /// <param name="wordFreqs">A frequency dictionary</param>
        /// <param name="termsCount">The total number of words</param>
        /// <returns>token/document_token pairs</returns>
        public Dictionary<string, TokenDocument> CalculateTFScores(Dictionary<string, int> wordFreqs, int termsCount)
        {
            var tokens = new Dictionary<string, TokenDocument>();

            foreach (string token in wordFreqs.Keys)
            {
                // TODO: handle first entries
                tokens[token] = new TokenDocument
                {
                    Count = wordFreqs[token],
                    TFScore = (decimal)wordFreqs[token] / termsCount
                };
            }

            return tokens;
        }

        /// <summary>
        /// Preprocesses text into valid terms
        /// </summary>
        /// <param name="text">text content</param>
        /// <returns>a list of valid terms</returns>
        public List<string> PreprocessText(string text)
        {
            string trimmed = text.Trim();
            // remove punctuation
            trimmed = RemovePunctuation(trimmed);
            // remove special characters
            trimmed = RemoveSpecialCharacters(trimmed);
            
            var terms = new List<string>(Regex.Split(trimmed, @"\s"));
            // remove stop words
            // terms = RemoveStopWords(terms);
            // ignore empty strings
            terms = RemoveEmptyStrings(terms);
            // stem words
            terms = StemWords(terms);
            return terms;
        }

        /// <summary>
        /// Stems words
        /// </summary>
        /// <param name="words">A list of words</param>
        /// <returns>A list of resulting words</returns>
        public List<string> StemWords(List<string> words)
        {
            return words.ConvertAll<string>((string word) => stemmer.Stem(word).Value);
        }

        /// <summary>
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

        /// <summary>
        /// Removes stop words
        /// </summary>
        /// <param name="words">A list of words</param>
        /// <returns>A list of filtered words</returns>
        public List<string> RemoveStopWords(List<string> words)
        {
            return words.FindAll((string word) => !stopWords.Contains(word));
        }

        /// <summary>
        /// Contains information about a token
        /// </summary>
        public class TokenDocument
        {
            public decimal TFScore { get; set; }
            public double Count { get; set; }
            public string SourcePath { get; set; }
        }
    }
}