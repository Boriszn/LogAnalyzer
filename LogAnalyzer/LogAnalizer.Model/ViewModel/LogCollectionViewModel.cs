using System.Collections.Generic;

namespace LogAnalyzer.Model.ViewModel
{
    public class LogCollectionViewModel
    {
        public string CollectionName { get; set; }
        public IEnumerable<LogLevelInfoViewModel> LastInfo { get; set; }
    }
}
