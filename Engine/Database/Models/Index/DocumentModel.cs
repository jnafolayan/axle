using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Axle.Engine.Database.Models.Index
{
    public class DocumentModel
    {
        [BsonId]
        public Guid Id {get; internal set;}
        public decimal Tf {get; internal set;}
        public string SourcePath {get; internal set;}
        public bool IsIndexed {get; internal set;}
        public DateTime DateIndexed {get; internal set;}
    }
}