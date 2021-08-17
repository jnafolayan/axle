
using System;
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
    }
}
