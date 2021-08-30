using System;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Axle.Engine.Database.Models.Autocomplete
{
    public class BigramModel
    {
        [BsonId]
        public ObjectId Id { get; internal set; }
        public string Before {get; internal set;}
        public string After { get; internal set;}
        public long Count { get; internal set;}
        public decimal Probability { get; internal set; }
        public decimal LogProbability { get; internal set; }
    }
}