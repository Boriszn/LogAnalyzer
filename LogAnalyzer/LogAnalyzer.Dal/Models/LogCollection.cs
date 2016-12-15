using System.Collections.Generic;

namespace LogAnalyzer.Dal.Models
{
    public class LogCollection
    {
        public string CollectionName { get; set; }
        public IEnumerable<LogLevelInfo> LastInfo { get; set; }
    }
}
