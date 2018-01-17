using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Df.PostgreSqlUnitTest
{
    [ElasticsearchType(IdProperty ="Id", Name = "Person")]
    public class Person
    {
        
        public long Id { get; set; }
        [Text(Name = "FirstName", Similarity = "BM25")]
        public string FirstName { get; set; }
        [Text(Name = "LastName", Similarity = "BM25")]
        public string LastName { get; set; }
    }

    [TestClass]
    public class ElasticSearchTest
    {
        [TestMethod]
        public void CheckConnection()
        {
            var esNode = new Uri("http://localhost:9200/");
            var esConfig = new ConnectionSettings(esNode);
            var esClient = new ElasticClient(esConfig);
            var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };
            var indexConfig = new IndexState
            {
                Settings = settings
            };
            if (!esClient.IndexExists("Person").Exists)
            {
                esClient.CreateIndex("Person", c => c
                    .InitializeUsing(indexConfig)
                    .Mappings(m => m.Map<Person>(mp => mp.AutoMap()))
                );
            }
            esConfig.DefaultIndex("Person");
            var person = new Person
            {
                Id = 1,
                FirstName = "kimchy",
                LastName = "kimchy 2"
            };
            esClient.Index(person);
            //esClient.DeleteIndex("people");
            Assert.AreEqual(true, true);

        }
    }
}
