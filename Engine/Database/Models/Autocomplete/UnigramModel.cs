using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Axle.Engine.Database.Models.Autocomplete
{
    public class UnigramModel
    {
        [BsonId]
        public string Token { get; internal set;}
        public long Count { get; internal set;}
    }
}