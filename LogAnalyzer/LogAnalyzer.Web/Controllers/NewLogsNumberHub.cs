using System;
using LogAnalyzer.Web.Models;
using Microsoft.AspNet.SignalR;

namespace LogAnalyzer.Web.Controllers
{
    /// <summary>
    /// Intended for communicationg with SignalR Client uses in new-logs-number-service.js
    /// </summary>
    public class NewLogsNumberHub : Hub
    {
        private readonly NewLogNumberBroadcaster _newLogNumberBroadcaster;

        public NewLogsNumberHub(): this(NewLogNumberBroadcaster.Instance) 
        { }

        public NewLogsNumberHub(NewLogNumberBroadcaster newLogNumberBroadcaster)
        {
            _newLogNumberBroadcaster = newLogNumberBroadcaster;
        }

        public void UpdateNumberOfNewItems(string collectionName, string loadFromId, string query = null,
            DateTime? loadFrom = null, DateTime? loadTo = null)
        {
            _newLogNumberBroadcaster.UpdateModel(new NumberOfNewLogItemsViewModel
            {
                CollectionName = collectionName,
                LoadFromId = loadFromId,
                Query = query,
                LoadFrom = loadFrom,
                LoadTo = loadTo
            });
        }
    }
}