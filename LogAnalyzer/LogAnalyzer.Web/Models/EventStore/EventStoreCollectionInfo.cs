using System;
using System.Collections.Generic;

namespace LogAnalyzer.Web.Models.EventStore
{
    public class EventStoreCollectionInfo
    {
        public string CollectionName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public IEnumerable<EventStoreCollectionInfoEntry> ConsumerToolsTotalInfo { get; set; }
    }
}