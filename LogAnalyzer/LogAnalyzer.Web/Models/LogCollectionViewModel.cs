﻿using System.Collections.Generic;

namespace LogAnalyzer.Web.Models
{
    public class LogCollectionViewModel
    {
        public string CollectionName { get; set; }
        public IEnumerable<LogLevelInfoViewModel> LastInfo { get; set; }
    }
}
