﻿using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using LogAnalyzer.Dal;
using LogAnalyzer.Model.Vm;
using LogAnalyzer.Mappers;
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

        //GET api/environmentInfo
        [HttpGet]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetEnvironmentName()
        {
            return Ok(string.Format("{0} v{1}", ConfigurationManager.AppSettings["Environment"].ToUpper(), 
                ConfigurationManager.AppSettings["Version"].ToUpper()));
        }

        //GET api/getErrorsCount/{collectionName}
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