using System;

namespace LogAnalyzer.Web.Models
{
    public class NumberOfNewLogItemsViewModel
    {
        public string CollectionName { get; set; }
        public string LoadFromId { get; set; }
        public string Query { get; set; }
        public DateTime? LoadFrom { get; set; }
        public DateTime? LoadTo { get; set; }
    }
}