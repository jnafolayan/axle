using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Axle.Engine.Database;
using Axle.Engine.Database.Models.Index;
using Axle.Engine.Database.Models.Autocomplete;
using MongoDB.Driver;
using static Axle.Engine.Indexer;

namespace Axle.Engine
{
    /// <summary>
    /// Store class
    /// </summary>
    public class Store : MongoCRUD
    {
        private IMongoCollection<DocumentModel> _documents;
        private IMongoCollection<TokenModel> _invertedIndex;
        private IMongoCollection<UnigramModel> _unigrams;
        private IMongoCollection<BigramModel> _bigrams;
        public Store(string databaseName, string connectionURI) : base(databaseName, connectionURI)
        {
            _documents = db.GetCollection<DocumentModel>("documents");
            _invertedIndex = db.GetCollection<TokenModel>("invertedIndex");
            _unigrams = db.GetCollection<UnigramModel>("unigramModel");
            _bigrams = db.GetCollection<BigramModel>("bigramModel");
        }

        /// <summary>
        /// Gets all documents that have not been indexed.
        /// </summary>
        /// <returns>A list of documents</returns>
        public List<DocumentModel> GetAllRedDocuments()
        {
            var filter = Builders<DocumentModel>.Filter.Eq("IsIndexed", false);
            var projection = Builders<DocumentModel>.Projection.Include("SourcePath");

            var result = _documents.Find(filter).Project<DocumentModel>(projection).ToList();

            return result;
        }

        /// <summary>
        /// Adds a new document to the database
        /// </summary>
        /// <param name="sourcePath">The path to the file</param>
        /// <param name="title">The title of the document</param>
        /// <param name="description">The description of the document</param>
        /// <returns></returns>
        public Task AddDocument(string sourcePath, string title, string description)
        {
            var doc = new DocumentModel
            {
                SourcePath = sourcePath,
                Title = title,
                Description = description,
                IsIndexed = false,
            };

            return _documents.InsertOneAsync(doc);
        }

        /// <summary>
        /// Sets a documents as indexed.
        /// </summary>
        /// <param name="document">The document</param>
        /// <returns></returns>
        public Task<UpdateResult> MarkDocumentAsIndexed(DocumentModel document)
        {
            var filter = Builders<DocumentModel>.Filter.Eq("Id", document.Id);
            var update = Builders<DocumentModel>.Update
                .Set("IsIndexed", true)
                .Set("DateIndexed", DateTime.UtcNow);

            return _documents.UpdateOneAsync(filter, update);
        }

        /// <summary>
        /// Inserts new documents into the inverted index. Creates new token entries
        /// if they don't already exist.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <param name="documents">A list of token documents to push</param>
        /// <param name="sourceMap">A mapping from source path to document id</param>
        /// <returns></returns>
        public Task<UpdateResult> UpsertTokenDocuments(string token, List<TokenDocument> documents, Dictionary<string, Guid> sourceMap)
        {
            var modelInstances = documents.ConvertAll<TokenDocumentModel>((doc) => new TokenDocumentModel
            {
                Tf = doc.TFScore,
                DocumentId = sourceMap[doc.SourcePath],
            });
            var filter = Builders<TokenModel>.Filter.Eq("Token", token);
            var update = Builders<TokenModel>.Update.PushEach("ContainingDocuments", modelInstances);

            return _invertedIndex.UpdateOneAsync(filter, update, new UpdateOptions { IsUpsert = true });
        }

        /// <summary>
        /// Gets a token from the database.
        /// </summary>
        /// <param name="tokenString">The token string</param>
        /// <returns>The token</returns>
        public Task<TokenModel> GetToken(string tokenString)
        {
            var filter = Builders<TokenModel>.Filter.Eq("Token", tokenString);
            var task = _invertedIndex.Find(filter).FirstOrDefaultAsync();
            return task;
        }

        /// <summary>
        /// Gets a document from the database
        /// </summary>
        /// <param name="documentId">The document id</param>
        /// <returns>The document</returns>
        public Task<DocumentModel> GetDocument(Guid documentId)
        {
            var filter = Builders<DocumentModel>.Filter.Eq("Id", documentId);
            var document = _documents.Find(filter).FirstOrDefaultAsync();
            return document;
        }

        /// <summary>
        /// Counts the documents that have been indexed.
        /// </summary>
        /// <returns>The number of indexed documents</returns>
        public Task<long> CountIndexedDocuments()
        {
            var filter = Builders<DocumentModel>.Filter.Eq("IsIndexed", true);
            return _documents.CountDocumentsAsync(filter);
        }

        /// <summary>
        /// Inserts new documents into the inverted index. Creates new token entries
        /// if they don't already exist.
        /// </summary>
        /// <param name="token">The token string</param>
        /// <param name="documents">A list of token documents to push</param>
        /// <param name="sourceMap">A mapping from source path to document id</param>
        /// <returns></returns>
        public void InsertOrUpdateUnigramDocuments(List<UnigramModel> documents)
        {
            foreach(UnigramModel document in documents)
            {
                var filter = Builders<UnigramModel>.Filter.Eq("Token", document.Token);
                var update = Builders<UnigramModel>.Update.Inc("Count", document.Count);
                _unigrams.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
            }
        }

        public void InsertOrUpdateBigramDocuments(List<BigramModel> documents)
        {
            foreach(BigramModel document in documents)
            {
                var filter = Builders<BigramModel>.Filter.Eq("Before", document.Before);
                var f2 = Builders<BigramModel>.Filter.Eq("After", document.After);
                var update = Builders<BigramModel>.Update.Inc("Count", document.Count);
                _bigrams.UpdateOne(filter & f2, update, new UpdateOptions { IsUpsert = true });
            }
        }

        public List<BigramModel> GetBigrams(string Before)
        {
            var filter = Builders<BigramModel>.Filter.Eq("Before", Before);
            return _bigrams.Find<BigramModel>(filter).ToList();
        }

        public List<BigramModel> GetTopNBigrams(string Before, int n)
        {
            var pattern = new MongoDB.Bson.BsonRegularExpression(new Regex("^" + Before + "\\w+", RegexOptions.IgnoreCase));
            var filter = Builders<BigramModel>.Filter.Regex("Before", pattern);
            var sort = Builders<BigramModel>.Sort.Descending("Count");
            var result = _bigrams.Find<BigramModel>(filter).Sort(sort).Limit(n).ToList();
            while(result.Count < n)
            {
                result.Add(new BigramModel{
                    Before = Before,
                    After = "<end>",
                    Count = 0
                });
            }
            return result;
        }
         
    }
}