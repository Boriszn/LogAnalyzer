using System;

namespace LogAnalyzer.Web.Models
{
    public class LogEntryViewModel
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime EventDate { get; set; }
        public string Level { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime VisitDate { get; set; }
    }
}