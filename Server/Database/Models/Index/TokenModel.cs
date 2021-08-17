using System;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Axle.Server.Database.Models.Index
{
    public class TokenModel
    {
        [BsonId]
        public Guid Id {get; set;}
        string Token {get; set;}
        decimal Idf {get; set;}
        List<DocumentModel> ContainingDocuments = new List<DocumentModel>();
    }
}