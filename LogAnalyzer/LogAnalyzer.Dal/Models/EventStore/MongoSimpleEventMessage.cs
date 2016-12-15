using MongoDB.Bson;
using Nuts.InterDom.Queue;
using System;
using System.Collections.Generic;

namespace LogAnalyzer.Dal.Models.EventStore
{
    public class MongoSimpleEventMessage
    {
        public ObjectId _id { get; set; }
        public string Type { get; set; }
        public Tool From;
        public string ProducerToolName;
        public BsonDocument Body;
        public DateTime CreateOn;
        public List<Dispatch> DispatchHistory;
    }
}
