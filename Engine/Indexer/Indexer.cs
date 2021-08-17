using System.IO;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using System;
using System.Threading.Tasks;
using Axle.Engine.FileParsers;

namespace Axle.Engine.Indexer
{
    public class Indexer
    {
        private HashSet<string> stopWords = new HashSet<string>();
        private FileParserFactory parserFactory_;

        public Indexer()
        {
        }

        public Indexer(FileParserFactory parserFactory, string stopWordsFileURL)
        {
            parserFactory_ = parserFactory;
            ReadStopWords(stopWordsFileURL);
        }

        public Indexer(FileParserFactory parserFactory)
        {
            parserFactory_ = parserFactory;
        }

        public void ReadStopWords(string stopWordsFileURL)
        {
            stopWords.Clear();
            foreach (string line in File.ReadAllLines(stopWordsFileURL))
            {
                stopWords.Add(line);
            }
        }

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

        public Dictionary<string, DocumentToken> ParseDocument(string documentURL)
        {
            // fetch the appropriate parser for the document
            string ext = Path.GetExtension(documentURL).Substring(1);
            FileParserBase parser = parserFactory_.GetParser(ext);

            // parse the text
            string text = parser.ParseLocalFile(documentURL);
            int termsCount;

            Dictionary<string, int> wordFreqs = CountWordFrequencies(text, out termsCount);
            Dictionary<string, DocumentToken> tfScores = CalculateTFScores(wordFreqs, termsCount);

            return tfScores;
        }

        public Dictionary<string, int> CountWordFrequencies(string text, out int termsCount)
        {
            Dictionary<string, int> wordFreqs = new Dictionary<string, int>();
            string[] terms = RemovePunctuation(text).Split(" ");
            int numberOfTerms = 0;

            foreach (string term in terms)
            {
                // ignore empty strings
                if (term == " ") continue;
                int count = 0;
                wordFreqs.TryGetValue(term, out count);
                wordFreqs[term] = count + 1;
                numberOfTerms++;
            }

            termsCount = numberOfTerms;
            return wordFreqs;
        }

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

        public string RemovePunctuation(string text)
        {
            return Regex.Replace(text, @"[^\w\s]", "");
        }

        public class DocumentToken
        {
            public double TFScore { get; set; }
            public double Count { get; set; }
            public string DocumentURL { get; set; }
        }
    }
}