using System.Collections.Generic;
using System.Linq;
using LogAnalyzer.Dal.Models.EventStore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Configuration;
using MongoDB.Driver.Builders;
using Nuts.InterDom.Queue;

namespace LogAnalyzer.Dal
{
    public class MongoEventsRepository
    {
        private readonly MongoDatabase _database;
        private const string SystemCollection = "system.indexes";
        private readonly string ConnectionString = ConfigurationManager.AppSettings["MongoConnection"];
        private readonly string DatabaseName = ConfigurationManager.AppSettings["EventStore_MongoDataBase"];
        private const string CountKey = "Count";
        private const string ConsumerToolNameKey = "ConsumerToolName";
        private const string ConsumerIsSuccessKey = "IsSuccess";
        private const string QNameFormat = "{0}:{1}";

        public MongoEventsRepository()
        {
            var client = new MongoClient(ConnectionString);
            var server = client.GetServer();
            _database = server.GetDatabase(DatabaseName);
        }

        public bool IsCollectionExists(string collectionName)
        {
            return _database.CollectionExists(collectionName);
        }

        public IEnumerable<EventStoreCollectionInfo> GetAllCollections(DateTime from, DateTime to)
        {

            var allCollections = _database.GetCollectionNames();
            var result = new List<EventStoreCollectionInfo>();

            var fromBson = new BsonDateTime(from);
            var toBson = new BsonDateTime(to);

            foreach (var collectionName in allCollections.Where(r => r != SystemCollection))
            {
                var collection = _database.GetCollection<MongoSimpleEventMessage>(collectionName);

                var match = new BsonDocument
                {
                    { "$match",
                        new BsonDocument
                        {
                            {"CreateOn", new BsonDocument
                                {
                                    {"$gte", fromBson},
                                    {"$lte", toBson}
                                }
                            },
                        }
                    }
                };

                var group = new BsonDocument 
                { 
                    { "$group", 
                        new BsonDocument 
                            { 
                                { "_id", new BsonDocument 
                                             { 
                                                 {ConsumerToolNameKey,"$DispatchHistory.ConsumerToolName"}, 
                                                 {ConsumerIsSuccessKey,"$DispatchHistory.IsSuccess" }, 
                                             } 
                                }, 
                                { 
                                    CountKey, new BsonDocument 
                                                 { 
                                                     { 
                                                         "$sum", 1 
                                                     } 
                                                 } 
                                } 
                            } 
                  } 
                };

                var project = new BsonDocument 
                { 
                    { 
                        "$project", 
                        new BsonDocument 
                            { 
                                {"_id", 0}, 
                                {ConsumerToolNameKey,"$_id.ConsumerToolName"}, 
                                {ConsumerIsSuccessKey, "$_id.IsSuccess"}, 
                                {CountKey, 1}, 
                            } 
                    } 
                };



                var resultsTotal = collection.Aggregate(new AggregateArgs
                {
                    Pipeline = new BsonDocument[] { match, group, project }
                });

                var collectionInfo = new EventStoreCollectionInfo()
                {
                    CollectionName = collectionName,
                    FromDate = from,
                    ToDate = to,
                    ConsumerToolsTotalInfo = FillEventStoreCollectionInfo(resultsTotal),
                };


                result.Add(collectionInfo);
            }

            return result;
        }

        public IEnumerable<MongoSimpleEventMessage> GetCollection(string collectionName, DateTime from, DateTime to)
        {

            var collection = _database.GetCollection<MongoSimpleEventMessage>(collectionName);

            var query = Query<MongoSimpleEventMessage>.Where(r =>
                             r.CreateOn >= from &&
                             r.CreateOn <= to);

            return collection.Find(query).SetLimit(50).ToList();            

        }

        public List<MongoSimpleEventMessage> Dispatch(string collectionName, string eventId, string consumers, DateTime? from, DateTime? to)
        {
            //build consumerQueueNames
            var consumerQueueNames = string.Format(QNameFormat, collectionName, consumers);
            var collection = _database.GetCollection<MongoSimpleEventMessage>(collectionName);

            var items = new List<MongoSimpleEventMessage>();

            if (!string.IsNullOrEmpty(eventId))
            {
                var bid = MongoDB.Bson.ObjectId.Parse(eventId);
                var query = Query<MongoSimpleEventMessage>.Where(r =>
                    r._id == bid);
                items = collection.Find(query).ToList();
            }
            else
            {
                if (!from.HasValue)
                    from = DateTime.Now.AddDays(-1);
                if (!to.HasValue)
                    to = DateTime.Now;

                var query = Query<MongoSimpleEventMessage>.Where(r =>
                             r.CreateOn >= from &&
                             r.CreateOn <= to);

                items = collection.Find(query).ToList();                
            }


            var rez = new List<MongoSimpleEventMessage>();

            Type messageType = Type.GetType(MessageForCollection.Instance.GetQueueType(collectionName));
            var dispatcher = Activator.CreateInstance(messageType, consumerQueueNames) as IDispatchAgain;
            foreach (var item in items)
            {
                dispatcher.DispatchAgain(item._id.ToString(), consumerQueueNames);
                rez.Add(GetEventById(collectionName, item._id.ToString()));
            }

            return rez;

        }

        public MongoSimpleEventMessage GetEventById(string collectionName, string eventId)
        {
            var collection = _database.GetCollection<MongoSimpleEventMessage>(collectionName);
            var bid = MongoDB.Bson.ObjectId.Parse(eventId);
            var query = Query<MongoSimpleEventMessage>.Where(r =>
                r._id == bid);
            var items = collection.Find(query).ToList();
            return items.FirstOrDefault();

        }

        private IEnumerable<EventStoreCollectionInfoEntry> FillEventStoreCollectionInfo( IEnumerable<MongoDB.Bson.BsonDocument> resultsTotal)
        {
            var outCollection = new List<EventStoreCollectionInfoEntry>();
            foreach (var resultTotal in resultsTotal)
            {
                var ConsumerToolNameValues = resultTotal.Elements.Single(r => r.Name == ConsumerToolNameKey).Value.AsBsonArray;
                var ConsumerIsSuccessValues = resultTotal.Elements.Single(r => r.Name == ConsumerIsSuccessKey).Value.AsBsonArray;
                var CountValue = resultTotal.Elements.Single(r => r.Name == CountKey).Value.ToInt32();

                for (int i = 0; i < ConsumerToolNameValues.Count; i++)
                {


                    EventStoreCollectionInfoEntry ConsumerToolsTotalInfo;
                    if (outCollection.Any(c => c.ToolName == ConsumerToolNameValues[i].ToString()))
                    {
                        ConsumerToolsTotalInfo = outCollection.FirstOrDefault(c => c.ToolName == ConsumerToolNameValues[i].ToString());
                    }
                    else
                    {
                        ConsumerToolsTotalInfo = new EventStoreCollectionInfoEntry
                        {
                            ToolName = ConsumerToolNameValues[i].ToString()
                        };
                        outCollection.Add(ConsumerToolsTotalInfo);
                    }

                    ConsumerToolsTotalInfo.TotalCount += CountValue;

                    if (ConsumerIsSuccessValues[i].ToString().Contains("true"))
                    {
                        ConsumerToolsTotalInfo.SuccessCount += CountValue;
                    }
                    else if (ConsumerIsSuccessValues[i].ToString().Contains("false"))
                    {
                        ConsumerToolsTotalInfo.NotSuccessCount += CountValue;
                    }
                    else
                    {
                        ConsumerToolsTotalInfo.NotProcessedCount += CountValue;
                    }
                }
            }

            return outCollection;
        }

    }
}
