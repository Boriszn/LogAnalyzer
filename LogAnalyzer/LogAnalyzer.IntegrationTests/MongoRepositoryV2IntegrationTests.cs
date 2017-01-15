using System;
using FluentAssertions;
using LogAnalyzer.Dal;
using NUnit.Framework;

namespace LogAnalyzer.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class MongoRepositoryV2IntegrationTests
    {
        private IRepository mongoRepository;

        [SetUp]
        public void Init()
        {
            mongoRepository = new MongoRepositoryV2();
        }

        [Test]
        public void IsCollectionExists_Test()
        {
            //Arrange 
            const string collectionName = "WebPortalLog";

            //Act 
            bool mongoCollectionList = mongoRepository.IsCollectionExists(collectionName);

            //Assert
            mongoCollectionList.Should().BeTrue();
        }

        [Test]
        public void GetAllCollections_Test()
        {
            //Arrange 
            DateTime from = DateTime.Parse("1-11-2016"),
                     to = DateTime.Parse("1-14-2017");

            //Act 
            var mongoCollectionList = mongoRepository.GetAllCollections(from, to);

            //Assert
            mongoCollectionList.Should().NotBeNull();
        }

        [Test]
        public void GetLogById_Test()
        {
            //Arrange 
            const string collectionName = "MobileAppLog";
            const string logId = "585d18280d3e47d08c6a8881";

            //Act 
            var mongoCollectionList = mongoRepository.GetLogById(collectionName, logId);

            //Assert
            mongoCollectionList.Should().NotBeNull();
        }

        [Test]
        public void GetLogsByQuery_Test()
        {
            //Arrange 
            const string collectionName = "MobileAppLog";
            const string query = "Level == \"Error\"";
            const int count = 5;

            //Act 
            var mongoCollectionList = mongoRepository.GetLogsByQuery(collectionName, null, query, null, null, count);

            //Assert
            mongoCollectionList.Should().NotBeNull();
        }

        [Test]
        public void GetNumberOfNewItems_Test()
        {
            Assert.Fail();
        }

        [Test]
        public void GetNewItems_Test()
        {
            Assert.Fail();
        }

        [Test]
        public void GetErrors_Test()
        {
            Assert.Fail();
        }
    }
}
