using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Axle.Server.Database.Models.Index
{
    public class DocumentModel
    {
        [BsonId]
        public Guid Id {get; set;}
        public decimal Tf {get; set;}
        public string SourcePath {get; set;}
        public bool IsIndexed {get; set;}
        public DateTime DateIndexed {get; set;}
    }
}