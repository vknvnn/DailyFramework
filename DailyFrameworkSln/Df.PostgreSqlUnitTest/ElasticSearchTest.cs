using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Df.PostgreSqlUnitTest
{
    /// <summary>
    /// The Fielddata is true that it's using to search
    /// </summary>
    [ElasticsearchType(IdProperty = "id", Name ="person")]
    public class Person
    {
        public long Id { get; set; }
        
        [Text(Name="first_name", Analyzer = "analyzer_startswith", SearchAnalyzer = "analyzer_startswith", Fielddata = true)]
        public string FirstName { get; set; }
        [Text(Name = "last_name", Analyzer = "analyzer_startswith", SearchAnalyzer = "analyzer_startswith", Fielddata = true)]
        public string LastName { get; set; }
    }

    public class PersonDataResultEs : DataResultEs<Person>{}
    public class DataResultEs<T> where T: class
    {
        public long Count { get; set; }
        public List<T> Data { get; set; }
    }

    public static class ElasticSearchExtention
    {
        public static List<T> GetList<T>(this ISearchResponse<T> response) where T: class
        {
            var result = new List<T>();
            foreach (var item in response.Hits)
            {
                result.Add(item.Source);
            }
            return result;
        }
        public static TDataResult GetDataResult<TDataResult, T>(this ISearchResponse<T> response) 
            where TDataResult : DataResultEs<T>, new()
            where T : class
        {
            var result = new TDataResult {
                Count = response.Hits.Count,
                Data = response.GetList()
            };            
            return result;
        }        
    }
    [TestClass]
    public class ElasticSearchTest
    {
        [TestMethod]
        public void ElasticSearch_CheckConnection()
        {
            var esNode = new Uri("http://localhost:9200/");
            var esConfig = new ConnectionSettings(esNode).DefaultIndex("person");
            var esClient = new ElasticClient(esConfig);
            //var settings = new IndexSettings { NumberOfReplicas = 1, NumberOfShards = 2 };

            //var indexConfig = new IndexState
            //{
            //    Settings = settings
            //};
            var indexName = new IndexName();
            indexName.Name = "person";
            var responseDelete = esClient.DeleteIndex(indexName);
            if (!esClient.IndexExists("person").Exists)
            {
                // Create a customize Startswith
                Func<CreateIndexDescriptor, ICreateIndexRequest> config = r => r.Settings(s => s
                            .Analysis(al => al.Analyzers(a => a.Custom("analyzer_startswith", c => c.Tokenizer("keyword").Filters("lowercase")))))
                            .Mappings(m => m.Map<Person>(t => t.AutoMap()));


                var responseCreate = esClient.CreateIndex(indexName, config);



                //esClient.Map<Person>(m =>
                //{
                //    var putMappingDescriptor = m.Index(Indices.Index("person")).AutoMap();
                //    return putMappingDescriptor;
                //});
            }

            var person = new Person
            {
                Id = 1,
                FirstName = "Võ Kế",
                LastName = "Nghiệp"
            };
            var repInsertDoc = esClient.Index(person);

            person = new Person
            {
                Id = 2,
                FirstName = "Võ Trọng",
                LastName = "Nghĩa"
            };
            repInsertDoc = esClient.Index(person);

            person = new Person
            {
                Id = 3,
                FirstName = "Võ Trắc",
                LastName = "Nghị"
            };
            repInsertDoc = esClient.Index(person);

            person = new Person
            {
                Id = 4,
                FirstName = "Võ Nguyên",
                LastName = "Khang"
            };
            repInsertDoc = esClient.Index(person);
            //esClient.DeleteIndex("people");
            Assert.AreEqual(repInsertDoc.Id, person.Id.ToString());


        }

        [TestMethod]
        public void ElasticSearch_QueryStartSwith()
        {
            var esNode = new Uri("http://localhost:9200/");
            var esConfig = new ConnectionSettings(esNode).DefaultIndex("person");
            var esClient = new ElasticClient(esConfig);
            var query = esClient.Search<Person>(search =>
                        search.Query(qu =>
                            qu.Bool(b => b.Should(
                                    should => should.MatchPhrasePrefix(
                                        mpp => mpp.Field(f=>f.FirstName).Query("Võ T").MaxExpansions(5)
                                    ),
                                    should => should.MatchPhrasePrefix(
                                        mpp => mpp.Field(f => f.LastName).Query("Võ T").MaxExpansions(5)
                                    )
                                )                                
                            )
                        ).Sort(sort => sort.Field("last_name", SortOrder.Ascending))
                        .Sort(sort => sort.Field("first_name", SortOrder.Ascending))
                        .From(0)
                        .Take(10)
                    );
            var dataResult = query.GetDataResult<PersonDataResultEs, Person>();
            Assert.AreEqual(dataResult.Count, 2);
        }
    }
}
