
using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Axle.Engine.Database
{
    public class MongoCRUD
    {
        protected IMongoDatabase db;

        /// <summary>
        /// constructor for local MongoDB instance. Served on localhost:27017
        /// </summary>
        /// <param name="databaseName">Name of the database</param>
        public MongoCRUD(string databaseName)
        {
            var client = new MongoClient();
            db = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// constructor for remote database client with via a URI 
        /// </summary>
        /// <param name="databaseName">name of database</param>
        /// <param name="uri">database URI</param>
        public MongoCRUD(string databaseName, string uri)
        {
            var client = new MongoClient(uri);
            db = client.GetDatabase(databaseName);
        }

        /// <summary>
        /// Inserts a record created by a model into database
        /// </summary>
        /// <param name="collectionName">Name of collection(table) to insert record into</param>
        /// <param name="record">record object; contains record fields and values</param>
        /// <typeparam name="T">model type</typeparam>
        public void InsertRecord<T>(string collectionName, T record)
        {
            var collection = db.GetCollection<T>(collectionName);
            collection.InsertOne(record);
        }

        // public void BatchInsertRecords<T>(string collectionName, T batch)
        // {
        //     var collection = db.GetCollection<T>(collectionName);
        //     collection.InsertMany(batch)
        // }

        /// <summary>
        /// Add new collection to database
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        public void CreateCollection(string collectionName)
        {
            db.CreateCollection(collectionName);
        }

        /// <summary>
        /// Gets existing collection from database
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        /// <typeparam name="T">specifies model type</typeparam>
        /// <returns></returns>
        public List<T> ReadCollection<T>(string collectionName)
        {
            var collection = db.GetCollection<T>(collectionName);

            return collection.Find(new BsonDocument()).ToList();
        }

        /// <summary>
        /// deletes an existing collection
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        public void DeleteCollection(string collectionName)
        {
            db.DropCollection(collectionName);
        }

        /// <summary>
        /// gets a record from a collection using the record id
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        /// <param name="id">record id to be searched for</param>
        /// <typeparam name="T">model type of the record searched for</typeparam>
        /// <returns></returns>
        public T ReadRecordById<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq("Id", id);

            return collection.Find(filter).First();
        }

        /// <summary>
        /// Updates an existing record identified by id.
        /// Inserts the record if record does not exist.
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        /// <param name="id">record id</param>
        /// <param name="record">record object to be updated/inserted</param>
        /// <typeparam name="T">record model</typeparam>
        public void UpsertRecord<T>(string collectionName, Guid id, T record)
        {
            BsonBinaryData binData = new BsonBinaryData(id, GuidRepresentation.Standard);

            var collection = db.GetCollection<T>(collectionName);
            var response = collection.ReplaceOne(
                new BsonDocument("_id", binData),
                record,
                new ReplaceOptions {IsUpsert = true}  
            );
        }

        /// <summary>
        /// remove a record using its id
        /// </summary>
        /// <param name="collectionName">name of collection</param>
        /// <param name="id">id of record to be deleted</param>
        /// <typeparam name="T">model type of record</typeparam>
        public void DeleteRecord<T>(string collectionName, Guid id)
        {
            var collection = db.GetCollection<T>(collectionName);

            var filter = Builders<T>.Filter.Eq("Id", id);
            collection.DeleteOne(filter);
        }

    }
}
