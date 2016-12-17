using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using LogAnalyzer.Dal;
using LogAnalyzer.Model.Dal;
using MongoDB.Bson;
using Moq;
using NUnit.Framework;

namespace LogAnalyzer.UnitTests
{
    [TestFixture]
    public class MongoRepositoryTests
    {
        private Mock<IRepository> mockContainer;

        [SetUp]
        public void Init()
        {
            mockContainer = new Mock<IRepository>();
        }

        [Test]
        public void IsCollectionExists_Fail_Test()
        {
            //Moq-Up
            mockContainer
                .Setup(r => r.IsCollectionExists("AppCollection"))
                .Returns(false);

            //Arrange 
            IRepository repository = mockContainer.Object;
            
            //Act
            bool isCollectionExist = repository.IsCollectionExists("AppCollection");

            //Assert
            isCollectionExist.Should().Be(false);
        }

        [Test]
        public void IsCollectionExists_Success_Test()
        {
            //Moq-Up
            mockContainer
                .Setup(r => r.IsCollectionExists("AppCollection1"))
                .Returns(true);

            //Arrange 
            IRepository repository = mockContainer.Object;

            //Act
            bool isCollectionExist = repository.IsCollectionExists("AppCollection1");

            //Assert
            isCollectionExist.Should().Be(true);
        }

        [Test]
        public void GetAllCollections_Success_Test()
        {
            //Arrange 
            DateTime dateTimeFrom  = DateTime.Parse("01.11.2016 12:00");
            DateTime dateTimeTo = DateTime.Parse("01.12.2016 14:00");

            var logCollectionList = new List<LogCollection>
                {
                    new LogCollection
                        {
                            CollectionName = "ApplicationOne",
                            LastInfo = new List<LogLevelInfo>
                                {
                                    new LogLevelInfo
                                        {
                                            Count = 5,
                                            Level = "Error"
                                        },
                                    new LogLevelInfo
                                        {
                                            Count = 1,
                                            Level = "Info"
                                        }
                                }
                        },
                    new LogCollection
                        {
                            CollectionName = "ApplicationTwo",
                            LastInfo = new List<LogLevelInfo>
                                {
                                    new LogLevelInfo
                                        {
                                            Count = 1,
                                            Level = "Error"
                                        }
                                }
                        }
                };

            mockContainer
                .Setup(r => r.GetAllCollections(dateTimeFrom, dateTimeTo))
                .Returns(logCollectionList);

            //Act
            IEnumerable<LogCollection> logCollection = mockContainer
                .Object
                .GetAllCollections(dateTimeFrom, dateTimeTo);

            //Assert
            logCollection
                .Should()
                .Contain(r => r.CollectionName == "ApplicationOne")
                .And
                .Contain(r=>r.LastInfo != null)
                .And
                .Contain(r => r.CollectionName == "ApplicationTwo")
                .And
                .Contain(r => r.LastInfo != null);
        }

        [Test]
        public void GetAllCollections_Fail_Test()
        {
            //Arrange 
            DateTime dateTimeFrom = DateTime.Parse("01.11.2016 12:00");
            DateTime dateTimeTo = DateTime.Parse("01.12.2016 14:00");

            var logCollectionList = new List<LogCollection>
                {
                    new LogCollection
                        {
                            CollectionName = "AppOne",
                            LastInfo = new List<LogLevelInfo>
                                {
                                    new LogLevelInfo
                                        {
                                            Count = 5,
                                            Level = "Error"
                                        }
                                }
                        }
                };
            
            mockContainer
                .Setup(r => r.GetAllCollections(dateTimeFrom, dateTimeTo))
                .Returns(logCollectionList);

            //Act
            var logCollection = mockContainer
                .Object
                .GetAllCollections(dateTimeFrom, dateTimeTo);

            //Assert
            logCollection
                .Should()
                .NotContain(r => r.CollectionName == "ApplicationOne")
                .And
                .NotContain(r => r.LastInfo.ToList()[0].Count == 1);
        }

        [Test]
        public void GetLogById_Success_Test()
        {
            //Arrange
            var logEntryBsonDocument = new LogEntry
                {
                    Message = "test message",
                    Level = "Error"
                }.ToBsonDocument();

            mockContainer
               .Setup(r => r.GetLogById("ApplicationOne", "584d92e5da1363088db1c1ed"))
               .Returns(logEntryBsonDocument);

            //Act
            IRepository repository = mockContainer.Object;
            BsonDocument logCollection = repository.GetLogById("ApplicationOne", "584d92e5da1363088db1c1ed");

            //Assert
            logCollection
                .Should()
                .NotBeNull();
        }

        [Test]
        public void GetLogsByQuery_Success_Test()
        {
            //Arrange
            mockContainer
                .Setup(r => r.GetLogsByQuery("ApplicationOne", "584d92e5da1363088db1c1ed", "Level == \"Error\"",
                                             DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"), 10))
                .Returns(new List<LogEntry>
                    {
                        new LogEntry
                            {
                                Message = "test message",
                                Level = "Error"
                            }
                    });

            //Act
            var logsByQuery = mockContainer
                .Object
                .GetLogsByQuery("ApplicationOne", "584d92e5da1363088db1c1ed", "Level == \"Error\"",
                                DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"), 10);

            //Assert
            logsByQuery
                .Should()
                .NotBeNull()
                .And
                .Contain(r => r.Message == "test message");
        }

        [Test]
        public void GetNumberOfNewItems_Success_Test()
        {
            //Arrange
            mockContainer
                .Setup(r => r.GetNumberOfNewItems("ApplicationOne", "584d92e5da1363088db1c1ed", "Level == \"Error\"", 
                    DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00")))
                .Returns(2);

            //Act
            var logsByQuery = mockContainer
                .Object
                .GetNumberOfNewItems("ApplicationOne", "584d92e5da1363088db1c1ed", "Level == \"Error\"",
                                DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"));

            //Assert
            logsByQuery.Should().Be(2);
        }

        [Test]
        public void GetNewItems_Success_Test()
        {
            //Arrange
            mockContainer
                .Setup(r => r.GetNewItems("ApplicationTwo", "584d92e5da1363088db1c1ed", "Level == \"Error\"",
                    DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"), 1))
                .Returns(new List<LogEntry>
                    {
                        new LogEntry
                            {
                                Message = "test message 2",
                                Level = "Info"
                            }
                    });

            //Act
            var logsByQuery = mockContainer
                .Object
                .GetNewItems("ApplicationTwo", "584d92e5da1363088db1c1ed", "Level == \"Error\"",
                                DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"), 1);

            //Assert
            logsByQuery.Should().NotBeNull().And.Contain(l => l.Level == "Info");
        }

        [Test]
        public void GetErrors_Success_Test()
        {
            //Arrange
            mockContainer
                .Setup(r => r.GetErrors("ApplicationOne", DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00")))
                .Returns(new List<LogEntry>
                    {
                        new LogEntry
                            {
                                Message = "test message 2",
                                Level = "Info"
                            },
                       new LogEntry
                            {
                                Message = "Error message",
                                Level = "Error"
                            }
                    });

            //Act
            var logsByQuery = mockContainer
                .Object
                .GetErrors("ApplicationOne", DateTime.Parse("01.11.2016 12:00"), DateTime.Parse("01.12.2016 14:00"));

            //Assert
            logsByQuery.Should().NotBeNull().And.Contain(l=>l.Level == "Error").And.HaveCount(2);
        }
    }
}
