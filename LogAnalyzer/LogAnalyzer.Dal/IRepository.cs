using System;
using System.Collections.Generic;
using LogAnalyzer.Model.Dal;
using MongoDB.Bson;

namespace LogAnalyzer.Dal
{
    public interface IRepository
    {
        /// <summary>
        /// Determines whether [is collection exists] [the specified collection name].
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <returns>
        ///   <c>true</c> if [is collection exists] [the specified collection name]; otherwise, <c>false</c>.
        /// </returns>
        bool IsCollectionExists(string collectionName);

        /// <summary>
        /// Gets all collections.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="to">To.</param>
        /// <returns></returns>
        IEnumerable<LogCollection> GetAllCollections(DateTime from, DateTime to);

        /// <summary>
        /// Gets the log by identifier.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="logId">The log identifier.</param>
        /// <returns></returns>
        BsonDocument GetLogById(string collectionName, string logId);

        /// <summary>
        /// Gets the logs by query.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadToId">The load to identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IEnumerable<LogEntry> GetLogsByQuery(string collectionName, string loadToId, string query, DateTime? loadFrom, DateTime? loadTo, int count);

        /// <summary>
        /// Gets the number of new items.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFromId">The load from identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        int GetNumberOfNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo);

        /// <summary>
        /// Gets the new items.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFromId">The load from identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IEnumerable<LogEntry> GetNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo, int count);

        /// <summary>
        /// Gets the errors.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        IEnumerable<LogEntry> GetErrors(string collectionName, DateTime? loadFrom, DateTime? loadTo);
    }
}