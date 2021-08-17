
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Axle.Server.Database
{
    public class MongoCRUD
    {
        private IMongoDatabase db;

        public MongoCRUD(string databaseName)
        {
            var client = new MongoClient();
            db = client.GetDatabase(databaseName);
        }

        public MongoCRUD(string databaseName, string uri)
        {
            var client = new MongoClient(uri);
            db = client.GetDatabase(databaseName);
        }

        public void InsertRecord<T>(string collectionName, T record)
        {
            var collection = db.GetCollection<T>(collectionName);
            collection.InsertOne(record);
        }

        public void BatchInsertRecord<T>(string collectionName, T batch)
        {
            var collection = db.GetCollection<T>(collectionName);
            collection.InsertMany((IEnumerable<T>)batch);
        }

        public List<T> ReadCollection<T>(string collectionName)
        {
            var collection = db.GetCollection<T>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T ReadRecordById<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        public void UpsetRecord<T>(string collectionName, Guid id, T record)
        {
            BsonBinaryData binData = new BsonBinaryData(id, GuidRepresentation.Standard);

            var collection = db.GetCollection<T>(collectionName);
            var response = collection.ReplaceOne(
                new BsonDocument("_id", binData),
                record,
                new ReplaceOptions {IsUpsert = true}  
            );
        }

        public void DeleteRecord<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);

            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }

    }
}
