using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using LogAnalyzer.Dal;
using LogAnalyzer.Mappers;
using LogAnalyzer.Model.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using Ninject.Extensions.Logging;

namespace LogAnalyzer.Web.Controllers
{
    public class LogApiController : ApiController
    {
        private const int DefaultLogItemsCount = 50;
        private readonly IRepository mongoRepository;
        private readonly ILogger logger;

        public LogApiController(IRepository repository, ILogger logger)
        {
            mongoRepository = repository;
            this.logger = logger;
        }

        // GET api/collections
        [HttpGet]
        [ResponseType(typeof(LogCollectionViewModel))]
        public IHttpActionResult GetAllCollections()
        {
            return Ok(mongoRepository
                .GetAllCollections(DateTime.Now.AddDays(-1), DateTime.Now)
                .Select(MongoMapper.Map));
        }

        // GET api/{collectionName}
        /// <summary>
        /// Gets the log by identifier.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="logId">The log identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof (object))]
        public IHttpActionResult GetLogById(string collectionName, string logId)
        {
            if (!mongoRepository.IsCollectionExists(collectionName))
            {
                return NotFound();
            }

            var mongoREsult = mongoRepository.GetLogById(collectionName, logId);

            if (mongoREsult == null)
            {
                return NotFound();
            }

            return Ok(mongoREsult.ToJson(new JsonWriterSettings
            {
                OutputMode = JsonOutputMode.Strict
            }));
        }

        //GET api/{collectionName}
        /// <summary>
        /// Gets the logs by query.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadToId">The load to identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(LogEntryViewModel))]
        public IHttpActionResult GetLogsByQuery(string collectionName, string loadToId = null, string query = null, DateTime? loadFrom = null, DateTime? loadTo = null)
        {
            if (!mongoRepository.IsCollectionExists(collectionName))
            {
                return NotFound();
            }

            return Ok(mongoRepository
                          .GetLogsByQuery(collectionName, loadToId, query, loadFrom, loadTo, DefaultLogItemsCount)
                          .Select(MongoMapper.Map));
        }

        //GET api/{collectionName}
        /// <summary>
        /// Gets the number of new items.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFromId">The load from identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetNumberOfNewItems(string collectionName, string loadFromId, string query = null, DateTime? loadFrom = null, DateTime? loadTo = null)
        {
            if (!mongoRepository.IsCollectionExists(collectionName))
            {
                return NotFound();
            }

            var count = mongoRepository.GetNumberOfNewItems(collectionName, loadFromId, query, loadFrom, loadTo);
            return Ok(count);
        }

        //GET api/getNewItems/{collectionName}
        /// <summary>
        /// Gets the new items.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFromId">The load from identifier.</param>
        /// <param name="query">The query.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(object))]
        public IHttpActionResult GetNewItems(string collectionName, string loadFromId, string query = null, DateTime? loadFrom = null, DateTime? loadTo = null)
        {
            if (!mongoRepository.IsCollectionExists(collectionName))
            {
                return NotFound();
            }

            return Ok(mongoRepository.GetNewItems(collectionName, loadFromId, query, loadFrom, loadTo, DefaultLogItemsCount)
                                      .Select(MongoMapper.Map));
        }

        //GET api/getErrorsCount/{collectionName}
        /// <summary>
        /// Gets the errors count.
        /// </summary>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="loadFrom">The load from.</param>
        /// <param name="loadTo">The load to.</param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(LogEntryViewModel))]
        public IHttpActionResult GetErrorsCount(string collectionName, DateTime? loadFrom, DateTime? loadTo)
        {
            if (!mongoRepository.IsCollectionExists(collectionName))
            {
                return NotFound();
            }

            return Ok(mongoRepository.GetErrors(collectionName, loadFrom, loadTo)
                .Select(MongoMapper.Map));
        }
    }
}