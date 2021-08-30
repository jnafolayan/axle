using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Axle.Engine.Database.Models.Autocomplete
{
    public class UnigramModel
    {
        [BsonId]
        public int Token { get; internal set;}
        public int Count { get; internal set;}
    }
}