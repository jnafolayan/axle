using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Axle.Engine.Database.Models.Index;
using Axle.Engine.FileParsers;
using static Axle.Engine.Indexer;

namespace Axle.Engine
{
    /// <summary>
    /// The SearchEngine class
    /// </summary>
    public class SearchEngine
    {
        private Indexer indexer;
        private FileParserFactory parserFactory;
        private Store store;
        public SearchEngine(SearchEngineConfig config)
        {
            parserFactory = new FileParserFactory();
            indexer = new Indexer(parserFactory, config.StopWordsFilePath);
            store = new Store(config.DatabaseName, config.DatabaseConnectionURI);

            ConfigureParserFactory();
        }

        /// <summary>
        /// Registers parsers for all recognized file extensions
        /// </summary>
        private void ConfigureParserFactory()
        {
            parserFactory.RegisterParser("txt", new TxtFileParser());
            parserFactory.RegisterParser("html", new HTMLParser());
            parserFactory.RegisterParser("xml", new XMLFileParser());
            parserFactory.RegisterParser("pdf", new PdfFileParser());
            parserFactory.RegisterParser("xlsx", new SpreadsheetsParser());
            parserFactory.RegisterParser("docx", new WordDocumentParser());
            parserFactory.RegisterParser("pptx", new PptxFileParser());
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
            List<string> terms = indexer.PreprocessText(query);
            if (terms.Count == 0) return result;

            Console.WriteLine(String.Join(" ", terms));

            var tasks = new Task<TokenModel>[terms.Count];
            for (int i = 0; i < terms.Count; i++)
            {
                tasks[i] = store.GetToken(terms[i]);
            }

            Task.WaitAll(tasks);

            long totalDocuments = await store.CountIndexedDocuments();

            // accumulate scores for relevant documents
            var scores = new Dictionary<Guid, decimal>();
            foreach (var task in tasks)
            {
                TokenModel tokenObject = task.Result;
                if (tokenObject is null) continue;

                var idf = (decimal)(1 + Math.Log(totalDocuments / tokenObject.ContainingDocuments.Count));

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
            relevantDocuments = relevantDocuments.GetRange(0, Math.Min(10, relevantDocuments.Count));

            // fetch all relevant documents from the database
            var documentsTasks = new Task<DocumentModel>[relevantDocuments.Count];
            for (int i = 0; i < relevantDocuments.Count; i++)
            {
                documentsTasks[i] = store.GetDocument(relevantDocuments[i]);
            }

            Task.WaitAll(documentsTasks);

            foreach (var task in documentsTasks)
            {
                if (task.Result is null) continue;
                result.Add(new SearchResultItem
                {
                    Link = task.Result.SourcePath,
                    Title = task.Result.Title,
                    Description = task.Result.Description
                });
            }

            return result;
        }

        /// <summary>
        /// Checks if the engine can parse a document of the specified type
        /// </summary>
        /// <param name="type">The document type (extension)</param>
        /// <returns>A boolean</returns>
        public bool CanParseDocumentType(string type)
        {
            return parserFactory.GetParser(type) != null;
        }

        /// <summary>
        /// Indexes all "red" documents. Red documents are documents that have not
        /// been indexed.
        /// </summary>
        public void IndexAllDocuments()
        {
            var watch = new Stopwatch();

            List<DocumentModel> documents = store.GetAllRedDocuments();
            var documentURLs = documents.ConvertAll<string>(doc => doc.SourcePath);

            watch.Start();
            var index = indexer.BuildIndex(documentURLs);
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to build index having {documents.Count} documents.");

            var sourcePathToId = new Dictionary<string, Guid>();
            for (int k = 0; k < documents.Count; k++)
            {
                sourcePathToId[documentURLs[k]] = documents[k].Id;
            }

            var tasks = new Task[index.Count];
            var errors = new List<string>();
            int i = 0;

            watch.Start();
            foreach (var token in index)
            {
                // TODO: handle error
                tasks[i++] = store.UpsertTokenDocuments(token.Key, token.Value, sourcePathToId);
            }
            Task.WaitAll(tasks);
            watch.Stop();
            Console.WriteLine($"Took {watch.ElapsedMilliseconds}ms to update the index.");

            i = 0;
            tasks = new Task[documents.Count];
            foreach (var document in documents)
            {
                // TODO: handle error
                tasks[i++] = store.SetDocumentAsIndexed(document);
            }
            Task.WaitAll(tasks);
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
            await store.AddDocument(filePath, title, description);
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
