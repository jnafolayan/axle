using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Axle.Engine.Database.Models.Index;
using Axle.Engine.Database.Models.Autocomplete;
using Axle.Engine.FileParsers;
using Axle.Engine;
using Microsoft.Extensions.Logging;

namespace Axle.Engine
{
    /// <summary>
    /// The SearchEngine class
    /// </summary>
    public class SearchEngine
    {
        private ILogger<SearchEngine> _logger;
        private Indexer _indexer;
        private FileParserFactory _parserFactory;
        private Store _store;
        private AutoComplete _autoCompleter;
        public SearchEngine(SearchEngineConfig config, ILogger<SearchEngine> logger)
        {
            _logger = logger;
            _parserFactory = new FileParserFactory();
            _autoCompleter = new AutoComplete(_store);
            _indexer = new Indexer(_parserFactory, _autoCompleter, config.StopWordsFilePath);
            _store = new Store(config.DatabaseName, config.DatabaseConnectionURI);

            ConfigureParserFactory();
        }

        /// <summary>
        /// Registers parsers for all recognized file extensions
        /// </summary>
        private void ConfigureParserFactory()
        {
            _parserFactory.RegisterParser("txt", new TxtFileParser());
            _parserFactory.RegisterParser("html", new HTMLParser());
            _parserFactory.RegisterParser("xml", new XMLFileParser());
            _parserFactory.RegisterParser("pdf", new PdfFileParser());
            _parserFactory.RegisterParser("xlsx", new SpreadsheetsParser());
            _parserFactory.RegisterParser("docx", new WordDocumentParser());
            _parserFactory.RegisterParser("pptx", new PptxFileParser());
        }

        /// <summary>
        /// Executes a query against the index.
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>A list of search results</returns>
        public async Task<List<SearchResultItem>> ExecuteQuery(string query)
        {
            var result = new List<SearchResultItem>();

            // strip query of unnecessary terms
            List<string> terms = _indexer.PreprocessText(query);
            if (terms.Count == 0) return result;

            // fetch all relevant tokens
            var tokens = Utils.RunTasks<string, TokenModel>(terms, 2, (term) => _store.GetToken(term));

            long totalDocuments = await _store.CountIndexedDocuments();

            // accumulate scores for relevant documents
            var scores = new Dictionary<Guid, decimal>();
            foreach (var tokenObject in tokens)
            {
                if (tokenObject is null) continue;

                var idf = (decimal)(1 + Math.Log((double)totalDocuments / (tokenObject.ContainingDocuments.Count + 1)));
                decimal tf;
                foreach (var document in tokenObject.ContainingDocuments)
                {
                    tf = scores.GetValueOrDefault(document.DocumentId, 0);
                    scores[document.DocumentId] = tf + document.Tf * idf;
                }
            }

            // abort here if we couldn't find any documents
            if (scores.Count == 0) return result;

            // sort the documents by scores
            var relevantDocuments = new List<Guid>(scores.Keys);
            relevantDocuments.Sort((a, b) =>
            {
                return scores[a] > scores[b] ? -1 : scores[b] > scores[a] ? 1 : 0;
            });

            // return only the top 10 results
            relevantDocuments = relevantDocuments.GetRange(0, Math.Min(50, relevantDocuments.Count));

            // fetch all relevant documents from the database
            var documents = Utils.RunTasks<Guid, DocumentModel>(relevantDocuments, 50, (guid) => _store.GetDocument(guid));

            foreach (var document in documents)
            {
                if (document is null) continue;

                result.Add(new SearchResultItem
                {
                    Link = document.SourcePath,
                    Title = document.Title,
                    Description = document.Description
                });
            }

            return result;
        }

        /// <summary>
        /// Executes autocomplete query agains index
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>A list of search results</returns>
        public List<string> AutoComplete(string query)
        {
            // Get all document that have the last element in the query
            // as their Before field
            // Continuously add terms to those results until <end> is reached or the
            // required depth has been reached. 

            // Need a function that can run a query down

            string[] queryTokens = Regex.Split(query, @"\s");
            string lastToken = queryTokens[queryTokens.Length - 1];
            List<string> suggestions = new List<string>();

            List<BigramModel> bigrams = _store.GetTopNBigrams(lastToken, 5);
            foreach (BigramModel bigram in bigrams)
            {
                queryTokens[queryTokens.Length - 1] = bigram.Before;
                query = String.Join(" ", queryTokens);

                if (bigram.After == "<end>")
                    suggestions.Add(query);
                else
                    suggestions.Add(completeQuery(bigram.After, query + " " + bigram.After, 6));
            }


            return suggestions;
        }

        private string completeQuery(string lastToken, string query, int depth)
        {
            for (int i = 0; i < depth; i++)
            {
                var nextToken = _store.GetTopNBigrams(lastToken, 1)[0].After;
                lastToken = nextToken;
                if (nextToken != "<end>")
                    query += " " + nextToken;
                else
                    return query;
            }
            return query;
        }


        /// <summary>
        /// Checks if the engine can parse a document of the specified type
        /// </summary>
        /// <param name="type">The document type (extension)</param>
        /// <returns>A boolean</returns>
        public bool CanParseDocumentType(string type)
        {
            return _parserFactory.GetParser(type) != null;
        }

        /// <summary>
        /// Indexes all "red" documents. Red documents are documents that have not
        /// been indexed.
        /// </summary>
        public void IndexAllDocuments()
        {
            var watch = new Stopwatch();

            // fetch all new documents
            List<DocumentModel> documents = _store.GetAllRedDocuments();
            var documentURLs = documents.ConvertAll<string>(doc => doc.SourcePath);

            // build small index from these documents
            watch.Start();
            var buildIndexResult = _indexer.BuildIndex(documentURLs);
            watch.Stop();
            _logger.LogDebug($"Built new index in {watch.ElapsedMilliseconds}ms ({documents.Count} documents).");

            var index = buildIndexResult.TokenMap;
            var bigramList = buildIndexResult.Bigrams;
            var unigramList = buildIndexResult.Unigrams;


            // generate a map from file path to document guid
            var sourcePathToId = new Dictionary<string, Guid>();
            for (int k = 0; k < documents.Count; k++)
            {
                sourcePathToId[documentURLs[k]] = documents[k].Id;
            }

            // merge new small index with main index
            watch.Start();
            var indexKeysList = new List<string>(index.Keys);
            Utils.RunTasks<string>(indexKeysList, 50, (key) => _store.UpsertTokenDocuments(key, index[key], sourcePathToId));
            watch.Stop();
            _logger.LogDebug($"Updated the index in {watch.ElapsedMilliseconds}ms.");

            // mark new documents as indexed
            Utils.RunTasks<DocumentModel>(documents, 50, (document) => _store.MarkDocumentAsIndexed(document));

            // Add to autocomplete index
            _store.InsertOrUpdateUnigramDocuments(unigramList);
            _store.InsertOrUpdateBigramDocuments(bigramList);
        }

        /// <summary>
        /// Adds a documents to the database
        /// </summary>
        /// <param name="filePath">The path to the document</param>
        /// <param name="title">The title of the document</param>
        /// <param name="description">The description of the document</param>
        /// <returns></returns>
        public async Task AddDocument(string filePath, string title = "", string description = "")
        {
            await _store.AddDocument(filePath, title, description);
        }
    }

    /// <summary>
    /// Config for the search engine. It is injected by .NET
    /// </summary>
    public class SearchEngineConfig
    {
        public string StopWordsFilePath { get; set; }
        public string DocumentsDirectory { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseConnectionURI { get; set; }
    }
}
