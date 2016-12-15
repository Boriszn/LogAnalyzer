namespace LogAnalyzer.Web.Models.EventStore
{
    public class EventStoreCollectionInfoEntry
    {
        public string ToolName { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int NotSuccessCount { get; set; }
        public int NotProcessedCount { get; set; }
    }
}