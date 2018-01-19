using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Elasticsearch.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nest;

namespace Df.PostgreSqlUnitTest
{
    /// <summary>
    /// The Fielddata is true that it's using to search
    /// </summary>
    [ElasticsearchType(IdProperty = "Id", Name ="person")]
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
        private readonly IElasticClient _esClient;
        private const string IndexName = "nghiepvo";
        public ElasticSearchTest()
        {
            var esNode = new Uri("http://localhost:9200/");
            var esConfig = new ConnectionSettings(esNode).DefaultIndex(IndexName);
            _esClient = new ElasticClient(esConfig);
        }
        [TestMethod]
        public void ElasticSearch_CheckConnection()
        {
            
            var responseDelete = _esClient.DeleteIndex(IndexName);
            if (!_esClient.IndexExists(IndexName).Exists)
            {
                // Create a customize Startswith
                ICreateIndexRequest Config(CreateIndexDescriptor r) => r.Settings(s => s.NumberOfShards(1)
                        .NumberOfReplicas(5)
                        .Analysis(al => al.Analyzers(a => a.Custom("analyzer_startswith", c => c.Tokenizer("keyword").Filters("lowercase")))))
                    .Mappings(m => m.Map<Person>(t => t.AutoMap()));

                var responseCreate = _esClient.CreateIndex(IndexName, Config);
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
            var repInsertDoc = _esClient.Index(person, s => s.Index<Person>());

            person = new Person
            {
                Id = 2,
                FirstName = "Võ Trọng",
                LastName = "Nghĩa"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());

            person = new Person
            {
                Id = 3,
                FirstName = "Võ Trắc",
                LastName = "Nghị"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());

            person = new Person
            {
                Id = 4,
                FirstName = "Võ Nguyên",
                LastName = "Khang"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());

            person = new Person
            {
                Id = 5,
                FirstName = "Delete",
                LastName = "Delete"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());
            var repDeleteDoc =  _esClient.Delete<Person>(5);
            person = new Person
            {
                Id = 6,
                FirstName = "Update",
                LastName = "Update"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());
            person = new Person
            {
                Id = 6,
                FirstName = "Update 1",
                LastName = "Update 1"
            };
            repInsertDoc = _esClient.Index(person, s => s.Index<Person>());
            var repGetDoc = _esClient.Get<Person>(6);

            //esClient.DeleteIndex("people");
            //Assert.AreEqual(repInsertDoc.Result, Result.Updated);
            var repUpdateDoc = _esClient.Update<Person, Person>(new Person { Id = 6, FirstName = "Update 1"},
                p => p.Doc(new Person { FirstName = "Update 2"}));
            Assert.AreEqual(repUpdateDoc.Version, 3);
        }

        [TestMethod]
        public void ElasticSearch_QueryStartSwith()
        {
            var query = _esClient.Search<Person>(search =>
                        search.Query(qu =>
                            qu.Bool(b => b.Should(
                                    should => should.MatchPhrasePrefix(
                                        mpp => mpp.Field(f=>f.FirstName).Query("Võ T").MaxExpansions(5)
                                    )
                                    , should => should.MatchPhrasePrefix(
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

        [TestMethod]
        public void ElasticSearch_GetRawQuery()
        {
            var esNode = new Uri("http://localhost:9200/");
            var esConfig = new ConnectionSettings(esNode).DefaultIndex("person");
            var esClient = new ElasticClient(esConfig);
            SearchRequest<dynamic> searchRequest = new SearchRequest<dynamic>
            {
                
                Query = new RawQuery
                {
                    Raw = "{\"bool\":{\"should\":[{\"match_phrase_prefix\":{\"first_name\":{\"query\":\"Võ T\",\"max_expansions\":5}}},{\"match_phrase_prefix\":{\"last_name\":{\"query\":\"Võ T\",\"max_expansions\":5}}}]}}",

                },
                Sort = new List<ISort>()
                {
                    new SortField {Field = "first_name", Order = SortOrder.Ascending},
                    new SortField {Field = "last_name", Order = SortOrder.Ascending},
                }
            };
            Func<SearchDescriptor<Person>, ISearchRequest> search = s =>
                        s.Query(qu =>
                            qu.Bool(b => b.Should(
                                    should => should.MatchPhrasePrefix(
                                        mpp => mpp.Field(f => f.FirstName).Query("Võ T").MaxExpansions(5)
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
                        ;
            
            var query = esClient.Search<dynamic>(searchRequest);

            //var dataResult = query.GetDataResult<PersonDataResultEs, Person>();
            //Assert.AreEqual(dataResult.Count, 2);
        }
    }
}
