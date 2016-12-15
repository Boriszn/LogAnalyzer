using System.Linq;
using LogAnalyzer.Web.Models;
using LogAnalyzer.Web.Models.EventStore;
using EventStoreCollectionInfo = LogAnalyzer.Dal.Models.EventStore.EventStoreCollectionInfo;
using EventStoreCollectionInfoEntry = LogAnalyzer.Dal.Models.EventStore.EventStoreCollectionInfoEntry;

namespace LogAnalyzer.Web.Mappers
{
    internal static class MongoMapper
    {
        internal static LogCollectionViewModel Map(Dal.Models.LogCollection mongoLogCollection)
        {
            if (mongoLogCollection == null)
            {
                return null;
            }

            return new LogCollectionViewModel
            {
                CollectionName = mongoLogCollection.CollectionName,
                LastInfo = mongoLogCollection.LastInfo.Select(r => new LogLevelInfoViewModel
                {
                    Level = r.Level,
                    Count = r.Count
                })
            };
        }

        internal static Models.LogEntryViewModel Map(LogAnalyzer.Dal.Models.LogEntry mongoLogEntry)
        {
            if (mongoLogEntry == null)
            {
                return null;
            }

            return new Models.LogEntryViewModel
            {
                Id = mongoLogEntry.BsonObjectId.ToString(),
                Level = mongoLogEntry.Level,
                Message = mongoLogEntry.Message,
                EventDate = mongoLogEntry.BsonObjectId.Value.CreationTime.ToLocalTime(),
                Name = mongoLogEntry.Name,
                Email = mongoLogEntry.Email,
                VisitDate = mongoLogEntry.VisitDate
            };
        }

        internal static MongoEventStoreMessage Map(Dal.Models.EventStore.MongoSimpleEventMessage EventMessage)
        {
            if (EventMessage == null)
                return null;

            return new MongoEventStoreMessage
            {
                CreateOn = EventMessage.CreateOn,
                DispatchHistory = EventMessage.DispatchHistory,
                From = EventMessage.From,
                id = EventMessage._id.ToString(),
                ProducerToolName = EventMessage.ProducerToolName,
                Type = EventMessage.Type
            };

        }

        internal static Models.EventStore.EventStoreCollectionInfoEntry Map(EventStoreCollectionInfoEntry EventMessageInfoEntry)
        {
            return new Models.EventStore.EventStoreCollectionInfoEntry
            {
                NotProcessedCount = EventMessageInfoEntry.NotProcessedCount,
                NotSuccessCount = EventMessageInfoEntry.NotSuccessCount,
                SuccessCount = EventMessageInfoEntry.SuccessCount,
                ToolName = EventMessageInfoEntry.ToolName,
                TotalCount = EventMessageInfoEntry.TotalCount
            };
        }

        internal static Models.EventStore.EventStoreCollectionInfo Map(EventStoreCollectionInfo EventMessageInfo)
        {
            return new Models.EventStore.EventStoreCollectionInfo
            {
                CollectionName = EventMessageInfo.CollectionName,
                ConsumerToolsTotalInfo = EventMessageInfo.ConsumerToolsTotalInfo.Select(Map),
                FromDate = EventMessageInfo.FromDate,
                ToDate = EventMessageInfo.ToDate
            };
        }

    }
}