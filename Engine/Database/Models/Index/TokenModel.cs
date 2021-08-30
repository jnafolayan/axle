using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Axle.Engine.Database.Models.Index
{
    public class TokenModel
    {
        [BsonId]
        public string Token {get; set;}
        public decimal Idf {get; set;}
        public List<TokenDocumentModel> ContainingDocuments = new List<TokenDocumentModel>();
    }

    public class TokenDocumentModel 
    {
        public decimal Tf { get; set; }
        public Guid DocumentId { get; set; }
    }
}