﻿using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using LogAnalyzer.Model.Dal;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Configuration;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace LogAnalyzer.Dal
{
    [Obsolete("Used old mongo driver")]
    public class MongoRepository : IRepository
    {
        private const string SystemCollection = "system.indexes";
        private const string LevelKey = "Level";
        private const string CountKey = "Count";
        private const string IdKey = "_id";

        private readonly MongoDatabase _database;
        //private readonly IMongoDatabase database;

        public MongoRepository()
        {
            string connectionString = ConfigurationManager.AppSettings["DomainsLogging_MongoDB"];
            var client = new MongoClient(connectionString);
            
            var server = client.GetServer();
            _database = server.GetDatabase(/*new MongoUrl(connectionString).DatabaseName*/"LogDb");

            //database = client.GetDatabase(new MongoUrl(connectionString).DatabaseName);
        }

        public bool IsCollectionExists(string collectionName)
        {
            return _database.CollectionExists(collectionName);
        }

        public IEnumerable<LogCollection> GetAllCollections(DateTime from, DateTime to)
        {
            var result = new List<LogCollection>();
            var allCollections = _database.GetCollectionNames();
            
            //new mongo driver
            //var item = database.ListCollectionsAsync().Result.ToListAsync().Result;

            //Notice: new ObjectId() func converts datetime to UTC by default
            var fromBsonId = new BsonObjectId(new ObjectId(from, 0, 0, 0));
            var toBsonId = new BsonObjectId(new ObjectId(to, 0, 0, 0));

            foreach (var collectionName in allCollections.Where(r => r != SystemCollection))
            {
                var logCollection = new LogCollection
                {
                    CollectionName = collectionName
                };

                MongoCollection<LogEntry> collection = _database.GetCollection<LogEntry>(collectionName);

                IEnumerable<BsonDocument> group = collection.Group(BuildBsonQuery(fromBsonId, toBsonId));

                logCollection.LastInfo = GetLogLevels(group.ToList());

                result.Add(logCollection);
            }

            return result;
        }

        public BsonDocument GetLogById(string collectionName, string logId)
        {
            var query = Query.EQ(IdKey, new BsonObjectId(new ObjectId(logId)));
            var collection = _database.GetCollection(collectionName);

            return collection.Find(query).FirstOrDefault();
        }

        public IEnumerable<LogEntry> GetLogsByQuery(string collectionName, string loadToId, string query, DateTime? loadFrom, DateTime? loadTo, int count)
        {
            var collection = _database.GetCollection<LogEntry>(collectionName).AsQueryable();
            collection = ApplyConditions(collection, query, loadFrom, loadTo);

            if (loadToId != null)
            {
                var id = new BsonObjectId(new ObjectId(loadToId));
                collection = collection.Where(r => r.BsonObjectId < id);
            }

            return collection.OrderByDescending(r => r.BsonObjectId).Take(count).ToList();
        }

        public int GetNumberOfNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo)
        {
            IQueryable<LogEntry> collection = GetNewItemsCollection(collectionName, loadFromId, query, loadFrom, loadTo);

            return collection.Count();
        }

        public IEnumerable<LogEntry> GetNewItems(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo, int count)
        {
            return GetNewItemsCollection(collectionName, loadFromId, query, loadFrom, loadTo)
                .OrderByDescending(r => r.BsonObjectId).Take(count).ToList();
        }

        public IEnumerable<LogEntry> GetErrors(string collectionName, DateTime? loadFrom, DateTime? loadTo)
        {
            //TODO: temp solution, should be used Agregation framework
            IQueryable<LogEntry> collection = _database.GetCollection<LogEntry>(collectionName).AsQueryable();

           /* var command = new CommandDocument
            {
                {"aggregate", collectionName},
                {
                    "pipeline",
                    new BsonArray
                    {
                        new BsonDocument
                        {
                            {
                                "$match",
                                new BsonDocument
                                {
                                    { "Level", "Error" }
                                }
                            }
                        }
                    }
                }

            };
            
            var result = _database.RunCommand(command);
            var document = result.Response["result"].AsBsonArray.Select(v => v.AsBsonDocument);*/

            collection = ApplyConditions(collection, "Level == \"Error\"", loadFrom, loadTo); 

            return collection.ToList();
        }

        #region Private functions

        private IQueryable<LogEntry> ApplyConditions(IQueryable<LogEntry> collection, string query, DateTime? loadFrom, DateTime? loadTo)
        {
            if (loadFrom != null)
            {
                var utcDateFrom = loadFrom.Value;
                //Notice: new ObjectId() func converts datetime to UTC by default
                var obid = new ObjectId(utcDateFrom, 0, 0, 0);
                var fromBSonId = new BsonObjectId(obid);
                collection = collection.Where(r => r.BsonObjectId >= fromBSonId);
            }

            if (loadTo != null)
            {
                var utcDateTo = loadTo.Value/*.AddDays(1)*/;
                //Notice: new ObjectId() func converts datetime to UTC by default
                var obId = new ObjectId(utcDateTo, 0, 0, 0);
                var toBsonId = new BsonObjectId(obId);
                collection = collection.Where(r => r.BsonObjectId <= toBsonId);
            }

            if (query != null)
            {
                collection = collection.Where(query);
            }

            return collection;
        }

        private IQueryable<LogEntry> GetNewItemsCollection(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo)
        {
            var collection = _database.GetCollection<LogEntry>(collectionName).AsQueryable();

            var fromBsonId = new BsonObjectId(new ObjectId(loadFromId));
            collection = collection.Where(r => r.BsonObjectId > fromBsonId);

            return ApplyConditions(collection, query, loadFrom, loadTo);
        }

        private static IEnumerable<LogLevelInfo> GetLogLevels(IEnumerable<BsonDocument> infos)
        {
            var logLevels = (infos.Select(
                info => new { info, levelValue = info.Elements.Single(r => r.Name == LevelKey).Value })
                                  .Select(
                                      @t =>
                                      new { @t, countValue = @t.info.Elements.Single(r => r.Name == CountKey).Value })
                                  .Where(
                                      @t =>
                                      @t.@t.levelValue != null && @t.@t.levelValue != BsonNull.Value &&
                                      @t.countValue != null && @t.countValue != BsonNull.Value)
                                  .Select(@t => new LogLevelInfo
                                  {
                                      Level = @t.@t.levelValue.AsString,
                                      Count = (int)@t.countValue.AsDouble
                                  })).ToList();
            return logLevels;
        }

        private GroupArgs BuildBsonQuery(BsonObjectId fromBsonId, BsonObjectId toBsonId)
        {
            var query = Query<LogEntry>.Where(r =>
                                              r.BsonObjectId >= fromBsonId &&
                                              r.BsonObjectId <= toBsonId);

            var groupArgs = new GroupArgs
            {
                Initial = new BsonDocument(new BsonElement(CountKey, new BsonInt32(0))),
                KeyFields = new GroupByBuilder(new[] { LevelKey }),
                ReduceFunction = new BsonJavaScript(
                    string.Format("function ( curr, result ) {{ result.{0}++; }}", CountKey)),
                Query = query
            };

            return groupArgs;
        }

        #endregion
    }
}
