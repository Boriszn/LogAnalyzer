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
    public class MongoRepositoryV2 : IRepository
    {
        private const string SystemCollection = "system.indexes";
        private const string LevelKey = "Level";
        private const string CountKey = "Count";
        private const string IdKey = "_id";

        private readonly IMongoDatabase database;
        private readonly ConfigurationProvider configurationProvider;

        public MongoRepositoryV2()
        {
            configurationProvider = new ConfigurationProvider();
             database = new MongoClient(configurationProvider.ConnectionString).GetDatabase(configurationProvider.Database);
        }

        public bool IsCollectionExists(string collectionName)
        {
            return database.ListCollectionsAsync(
                new ListCollectionsOptions { Filter =  new BsonDocument("name", collectionName) })
                .Result.AnyAsync().Result;
        }

        public IEnumerable<LogCollection> GetAllCollections(DateTime from, DateTime to)
        {
            var result = new List<LogCollection>();
           
            var allCollections = database.ListCollectionsAsync().Result.ToListAsync().Result;

            foreach (var collectionItem in allCollections.Where(r => r != SystemCollection))
            {
                string collectionName = collectionItem["name"].ToString();
                IMongoCollection<LogEntry> mongoCollection = database.GetCollection<LogEntry>(collectionName);

                result.Add( new LogCollection
                    {
                        CollectionName = collectionName,
                        LastInfo = mongoCollection
                            .Find(_ => true)
                            .ToList()
                            .GroupBy(r => r.Level).Select(group => new LogLevelInfo
                                {
                                    Level = @group.Key,
                                    Count = @group.Count()
                                })
                    });
            }

            return result;
        }

        public BsonDocument GetLogById(string collectionName, string logId)
        {
            var collection = database.GetCollection<BsonDocument>(collectionName);

            return collection
                .Find(Builders<BsonDocument>
                .Filter
                .Eq("_id", new ObjectId(logId))).FirstOrDefault();
        }

        public IEnumerable<LogEntry> GetLogsByQuery(string collectionName, string loadToId, string query, DateTime? loadFrom, DateTime? loadTo, int count)
        {
            IQueryable<LogEntry> collection = database
                .GetCollection<LogEntry>(collectionName)
                .AsQueryable().AsQueryable();

            collection = ApplyConditions(collection, query, loadFrom, loadTo);

            if (loadToId != null)
            {
                collection = collection.Where(r => r.BsonObjectId < 
                    new BsonObjectId(
                        new ObjectId(loadToId)));
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
            IMongoCollection<LogEntry> mongoCollection = database.GetCollection<LogEntry>(collectionName);

            return mongoCollection.Find(_ => _.Level.Contains("Error")).ToList();
        }

        #region Private functions

        private IQueryable<LogEntry> ApplyConditions(IQueryable<LogEntry> collection, string query, DateTime? loadFrom, DateTime? loadTo)
        {
            if (loadFrom != null)
            {
                var utcDateFrom = loadFrom.Value;

                //Notice: new ObjectId() func converts datetime to UTC by default
                //From Bson Id;
                collection = collection.Where(r => r.BsonObjectId >=
                    new BsonObjectId(
                        new ObjectId(utcDateFrom, 0, 0, 0)));
            }

            if (loadTo != null)
            {
                var utcDateTo = loadTo.Value/*.AddDays(1)*/;

                //Notice: new ObjectId() func converts datetime to UTC by default
                collection = collection.Where(r => r.BsonObjectId <= 
                    new BsonObjectId(
                        new ObjectId(utcDateTo, 0, 0, 0)));
            }

            if (query != null)
            {
                collection = collection.Where(query);
            }

            return collection;
        }

        private IQueryable<LogEntry> GetNewItemsCollection(string collectionName, string loadFromId, string query, DateTime? loadFrom, DateTime? loadTo)
        {
            var collection = database.GetCollection<LogEntry>(collectionName).AsQueryable();

            var fromBsonId = new BsonObjectId(new ObjectId(loadFromId));
            collection = collection.Where(r => r.BsonObjectId > fromBsonId);

            return ApplyConditions(collection, query, loadFrom, loadTo);
        }

        #endregion
    }
}
