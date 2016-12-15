using Nuts.InterDom.Queue;
using System;
using System.Collections.Generic;

namespace LogAnalyzer.Web.Models.EventStore
{
    public class MongoEventStoreMessage
    {
        public string id { get; set; }
        public string Type { get; set; }
        public Tool From;
        public string ProducerToolName;
        public DateTime CreateOn;
        public List<Dispatch> DispatchHistory;
    }
}