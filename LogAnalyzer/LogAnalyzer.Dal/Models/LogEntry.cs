using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LogAnalyzer.Dal.Models
{
    [BsonIgnoreExtraElements]
    public class LogEntry
    {
        [BsonElement("_id")]
        public BsonObjectId BsonObjectId { get; set; }

        public string Message { get; set; }
        
        public string Level { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime VisitDate { get; set; }
    }
}