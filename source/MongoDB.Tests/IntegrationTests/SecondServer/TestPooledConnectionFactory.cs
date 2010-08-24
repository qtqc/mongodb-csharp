using System;
using System.Diagnostics;
using System.Threading;
using MongoDB.Connections;
using NUnit.Framework;

namespace MongoDB.IntegrationTests.SecondServer
{
    [Ignore("Run manually since it needs a second server")]
    [TestFixture]
    public class TestPooledConnectionFactory
    {
        [TestFixtureSetUp]
        public void SetUp()
        {
            ConnectionFactoryFactory.Shutdown();
        }

        [Test]
        public void TestServerCirculationWorks()
        {
            var builder = new MongoConnectionStringBuilder();
            builder.AddServer("localhost", 27017);
            builder.AddServer("localhost", 27018);
            using(var pool = new PooledConnectionFactory(builder.ToString()))
            {
                var connection1 = pool.Open();
                var connection2 = pool.Open();
                var connection3 = pool.Open();
                var connection4 = pool.Open();
                var connection5 = pool.Open();
                Assert.AreEqual(27017, connection1.EndPoint.Port);
                Assert.AreEqual(27018, connection2.EndPoint.Port);
                Assert.AreEqual(27017, connection3.EndPoint.Port);
                Assert.AreEqual(27018, connection4.EndPoint.Port);
                Assert.AreEqual(27017, connection5.EndPoint.Port);
            }
        }

        [Test]
        public void Foo()
        {
            while(true)
            {
                using(var mongo = new Mongo("Servers=localhost:27017,localhost:27018,localhost:27019"))
                {
                    try
                    {
                        mongo.Connect();

                        var database = mongo.GetDatabase("test");

                        var collection = database.GetCollection("test");

                        collection.Insert(new Document("foo", 1), true);
                    }
                    catch(Exception exception)
                    {
                        Debug.WriteLine(exception);
                    }
                }

                Thread.Sleep(500);
            }
        }
    }
}