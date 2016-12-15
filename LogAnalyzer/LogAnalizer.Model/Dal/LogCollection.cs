using System.Collections.Generic;

namespace LogAnalyzer.Model.Dal
{
    public class LogCollection
    {
        public string CollectionName { get; set; }
        public IEnumerable<LogLevelInfo> LastInfo { get; set; }
    }
}
