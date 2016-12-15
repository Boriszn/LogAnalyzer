using System;
using System.Collections.Generic;
using LogAnalyzer.Model.Dal;
using MongoDB.Bson;

namespace LogAnalyzer.Dal
{
    public interface IRepository
    {
        bool IsCollectionExists(string collectionName);
        IEnumerable<LogCollection> GetAllCollections(DateTime from, DateTime to);
        BsonDocument GetLogById(string collectionName, string logId);
        IEnumerable<LogEntry> GetLogsByQuery(string collectionName, string loadToId, string query, DateTime? loadFrom, DateTime? loadTo, int count);
        int GetNumberOfNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo);
        IEnumerable<LogEntry> GetNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo, int count);
        IEnumerable<LogEntry> GetErrors(string collectionName, DateTime? loadFrom, DateTime? loadTo);
    }
}