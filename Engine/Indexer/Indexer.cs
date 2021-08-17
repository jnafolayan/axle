using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using Axle.Engine.FileParsers;

namespace Axle.Engine.Indexer
{
    /// <summary>
    /// Indexer class
    /// </summary>
    public class Indexer
    {
        private HashSet<string> stopWords = new HashSet<string>();
        private FileParserFactory parserFactory_;

        public Indexer()
        {
        }

        /// <summary>
        /// Two-param constructor for the Indexer class
        /// </summary>
        /// <param name="parserFactory">A reference to the parser factory</param>
        /// <param name="stopWordsFileURL">The path to the stopwords file</param>
        public Indexer(FileParserFactory parserFactory, string stopWordsFileURL)
        {
            parserFactory_ = parserFactory;
            ReadStopWords(stopWordsFileURL);
        }

        public Indexer(FileParserFactory parserFactory)
        {
            parserFactory_ = parserFactory;
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

        /// <summary>
        /// Builds an index from a list of documents
        /// </summary>
        /// <param name="documentURLs">A list of document file paths</param>
        /// <returns>A list of (token, document_token) pairs</returns>
        public KeyValuePair<string, List<DocumentToken>>[] BuildIndex(string[] documentURLs)
        {
            ConcurrentDictionary<string, List<DocumentToken>> tokenMap = new ConcurrentDictionary<string, List<DocumentToken>>();
            int maxTasksPerRun = 50;
            int i = 0;
            Task[] taskArray = new Task[maxTasksPerRun];

            Func<string, List<DocumentToken>> CreateDocumentTokenList = (_) => new List<DocumentToken>();

            Func<object, object> IndexFunc = (object url) =>
            {
                string documentURL = (string)url;
                Dictionary<string, DocumentToken> tfScores = ParseDocument(documentURL);

                foreach (string token in tfScores.Keys)
                {
                    tokenMap.GetOrAdd(token, CreateDocumentTokenList);
                    tfScores[token].DocumentURL = documentURL;
                    tokenMap[token].Add(tfScores[token]);
                }

                return null;
            };

            while (i < documentURLs.Length)
            {
                int stop = Math.Min(documentURLs.Length, i + maxTasksPerRun);
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

            return tokenMap.ToArray();
        }

        /// <summary>
        /// Parses a single document file
        /// </summary>
        /// <param name="documentURL">The path to the document</param>
        /// <returns>(token, document_token) pairs</returns>
        public Dictionary<string, DocumentToken> ParseDocument(string documentURL)
        {
            // fetch the appropriate parser for the document
            string ext = Path.GetExtension(documentURL).Substring(1);
            FileParserBase parser = parserFactory_.GetParser(ext);

            // parse the text
            string text = parser.ParseLocalFile(documentURL).ToLower();
            int termsCount;

            Dictionary<string, int> wordFreqs = CountWordFrequencies(text, out termsCount);
            Dictionary<string, DocumentToken> tfScores = CalculateTFScores(wordFreqs, termsCount);

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
            Dictionary<string, int> wordFreqs = new Dictionary<string, int>();
            // remove punctuation
            List<string> terms = new List<string>(RemovePunctuation(text.Trim()).Split(" "));

            // remove stop words
            terms = RemoveStopWords(terms);
            // ignore empty strings
            terms = RemoveEmptyStrings(terms);

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
        public Dictionary<string, DocumentToken> CalculateTFScores(Dictionary<string, int> wordFreqs, int termsCount)
        {
            Dictionary<string, DocumentToken> tokens = new Dictionary<string, DocumentToken>();

            foreach (string token in wordFreqs.Keys)
            {
                // TODO: handle first entries
                tokens[token] = new DocumentToken
                {
                    Count = wordFreqs[token],
                    TFScore = (double)wordFreqs[token] / termsCount
                };
            }

            return tokens;
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
        /// <param name="words">A list of words</param>
        /// <returns>A list of filtered words</returns>
        public string RemovePunctuation(string text)
        {
            return Regex.Replace(text, @"[^\w\s]", "");
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
        public class DocumentToken
        {
            public double TFScore { get; set; }
            public double Count { get; set; }
            public string DocumentURL { get; set; }
        }
    }
}