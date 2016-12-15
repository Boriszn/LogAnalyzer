using System.Linq;
using LogAnalyzer.Model.Dal;
using LogAnalyzer.Model.Vm;

namespace LogAnalyzer.Mappers
{
    public static class MongoMapper
    {
        public static LogCollectionViewModel Map(LogCollection mongoLogCollection)
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

        public static LogEntryViewModel Map(LogEntry mongoLogEntry)
        {
            if (mongoLogEntry == null)
            {
                return null;
            }

            return new LogEntryViewModel
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
    }
}