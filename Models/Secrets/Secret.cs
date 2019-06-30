using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Models.Secrets
{
    public class Secret
    {
        [BsonId]
        [BsonRepresentation(BsonType.String)]
        public Guid Id { get; set; }
        
        [BsonElement("ImageUrl")]
        [BsonRepresentation(BsonType.String)]
        public string ImageUrl { get; set; }
        
        [BsonElement("Description")]
        [BsonRepresentation(BsonType.String)]
        public string Description { get; set; }
    }
}