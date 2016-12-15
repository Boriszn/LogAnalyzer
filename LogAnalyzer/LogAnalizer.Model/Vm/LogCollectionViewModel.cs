using System.Collections.Generic;

namespace LogAnalyzer.Model.Vm
{
    public class LogCollectionViewModel
    {
        public string CollectionName { get; set; }
        public IEnumerable<LogLevelInfoViewModel> LastInfo { get; set; }
    }
}
