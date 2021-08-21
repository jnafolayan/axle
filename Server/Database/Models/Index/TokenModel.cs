using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Axle.Server.Database.Models.Index
{
    public class TokenModel
    {
        [BsonId]
        public Guid Id {get; set;}
        public string Token {get; set;}
        public decimal Idf {get; set;}
        public List<DocumentModel> ContainingDocuments = new List<DocumentModel>();
    }
}