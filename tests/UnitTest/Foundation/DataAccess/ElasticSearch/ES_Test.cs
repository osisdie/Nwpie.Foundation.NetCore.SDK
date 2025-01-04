using System;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.DataAccess;
using Nwpie.Foundation.Abstractions.Mappers.Interfaces;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Serializers.Resolvers;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.MiniSite.ES.Contract.Entities;
using Nwpie.MiniSite.ES.Contract.Models;
using Elasticsearch.Net;
using Nest;
using Newtonsoft.Json;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.DataAccess.ElasticSearch
{
    public class ES_Test : TestBase
    {
        public ES_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task EntityMapper_Test()
        {
            var cmd = new CommandExecutor("Unittest:ds1:allItemVersion");
            cmd.SetParameterValue("offset", 0);
            cmd.SetParameterValue("fetch", 1);

            var result = await cmd.ExecuteEntityAsync<Ds1SourceItem_Entity>();
            Assert.NotNull(result);
            Assert.NotNull(result.ns_data);

            var esData = m_Mapper.ConvertTo<Ds1SourceItem>(result);
            Assert.NotNull(esData.itemGuid);
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task DS1_MigrationAsync()
        {
            var offset = 0;
            var max = 20000;
            while (offset < max)
            {
                offset += PAGE_SIZE;
                await ESProcess(offset, PAGE_SIZE);
            };
        }

        async Task ESProcess(int offset, int pageSize)
        {
            var cmd = new CommandExecutor("Unittest:ds1:allItemVersion");
            cmd.SetParameterValue("offset", offset);
            cmd.SetParameterValue("fetch", pageSize);

            try
            {
                var result = await cmd.ExecuteEntityListAsync<Ds1SourceItem_Entity>();
                Assert.NotEmpty(result);

                foreach (var item in result)
                {
                    Ds1SourceItemJsonData jsonItemData = null;
                    if (false == string.IsNullOrWhiteSpace(item.ns_data))
                    {
                        jsonItemData = Serializer
                            .Deserialize<Ds1SourceItemJsonData>(item.ns_data);
                    }

                    var esData = m_Mapper.ConvertTo<Ds1SourceItem>(item);
                    Assert.NotNull(esData.itemGuid);
                    esData.version = esData.version ?? ConfigConst.LatestVersion;

                    var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                        .Index(INDEX)
                        .Size(10)
                        .Query(q =>
                            q.Term(t => t
                                .Field(SysFieldKey.UniqueId)
                                .Value($"{esData.itemGuid}:{esData.version}")
                            )
                        )); //Search based on _id
                    Assert.True(queryResponse.IsValid);

                    if (true != (queryResponse.Hits?.Count() > 0))
                    {
                        var indexResponse = await m_EsClient.IndexAsync(esData, i => i
                            .Index(INDEX)
                            .Id($"{esData.itemGuid}:{esData.version}")
                        );
                        //.Refresh());
                        Assert.True(indexResponse.IsValid);
                    }
                }
            }
            catch (Exception ex)
            {
                Assert.NotNull(ex.Message);
            }
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task SearchEntity_ReturnNotNullEntity()
        {
            var origItem = new Ds1SourceItem()
            {
                itemGuid = "186591_636986273807561747",
                version = ConfigConst.LatestVersion,
                nsItemId = "8327TL-L",
                modifyAt = DateTime.Parse("2019-08-28 16:28:26"),
                createAt = DateTime.Parse("2019-08-28 16:28:26")
            };

            // by _id
            {
                var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                    .Index(INDEX)
                    .From(0)
                    .Size(1)
                    .Query(q => q
                        .Term(t => t
                            .Field(SysFieldKey.UniqueId)
                            .Value($"{origItem.itemGuid}:{origItem.version}")
                        )
                    ));
                Assert.True(queryResponse.IsValid);
                Assert.True(queryResponse.Hits?.Count() > 0);
            }

            // by prefix nsItemId
            {
                var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                    .Index(INDEX)
                    .From(0)
                    .Size(10)
                    .Query(q => q
                        .MatchPhrasePrefix(mq => mq
                            .Field(t => t.nsItemId)
                            .Query(origItem.nsItemId.Split('-').First())
                        )
                    ));
                Assert.True(queryResponse.IsValid);
                Assert.True(queryResponse.Hits?.Count() > 0);
            }

            // by nsItemId
            {
                var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                    .Index(INDEX)
                    .From(0)
                    .Size(10)
                    .Query(q => q
                        .Term(t => t.nsItemId, origItem.nsItemId) || q
                        .Match(mq => mq
                            .Field(f => f.nsItemId)
                            .Query(origItem.nsItemId)
                        )
                    ));
                Assert.True(queryResponse.IsValid);
                Assert.True(queryResponse.Hits?.Count() > 0);
            }

            // by nsInternalId
            {
                var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                    .Index(INDEX)
                    .From(0)
                    .Size(10)
                    .Query(q => q
                        .Term(t => t
                            .nsInternalId, origItem.nsInternalId) ||
                            q.Match(mq => mq
                                .Field(f => f.nsInternalId)
                                .Query(origItem.nsInternalId)
                            )
                    ));
                Assert.True(queryResponse.IsValid);
                Assert.True(queryResponse.Hits?.Count() > 0);
            }
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task InsertEntity_ReturnNotNullEntity()
        {
            var origItem = new Ds1SourceItem()
            {
                itemGuid = "186591_636986273807561747",
                version = ConfigConst.LatestVersion,
                nsItemId = "8327TL-L",
                modifyAt = DateTime.Parse("2019-08-28 16:28:26"),
                createAt = DateTime.Parse("2019-08-28 16:28:26")
            };

            var indexResponse = await m_EsClient.IndexAsync(origItem, i => i
                .Index(INDEX)
                .Id($"{origItem.itemGuid}:{origItem.version}")
            );
            //.Refresh());

            Assert.True(indexResponse.IsValid);
            var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                .Index(INDEX)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t
                        .Field(SysFieldKey.UniqueId)
                        .Value($"{origItem.itemGuid}:{origItem.version}")
                    )
                )); //Search based on _id
            Assert.True(queryResponse.IsValid);
            Assert.True(queryResponse.Hits?.Count() > 0);

            foreach (var hit in queryResponse.Hits)
            {
                _ = hit.Id.ToString();
            }
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task UpdateEntity_ReturnNotNullEntity()
        {
            var origItem = new Ds1SourceItem()
            {
                itemGuid = "186591_636986273807561747",
                version = ConfigConst.LatestVersion,
                nsItemId = "8327TL-L",
                modifyAt = DateTime.Parse("2019-08-28 16:28:26"),
                createAt = DateTime.Parse("2019-08-28 16:28:26")
            };

            var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                .Index(INDEX)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t
                        .Field(SysFieldKey.UniqueId)
                        .Value($"{origItem.itemGuid}:{origItem.version}")
                    )
                ));
            Assert.True(queryResponse.IsValid);
            Assert.True(queryResponse.Hits?.Count() > 0);

            foreach (var hit in queryResponse.Hits)
            {
                var id = hit.Id.ToString();
                var item = hit.Source.ConvertTo<Ds1SourceItem>();
                item.modifyAt = DateTime.UtcNow;

                if (false == item.createAt.HasValue)
                {
                    item.createAt = DateTime.UtcNow;
                }

                var updateResponse = await m_EsClient.UpdateAsync<Ds1SourceItem, Ds1SourceItem>(id, d => d
                    .Index(INDEX)
                    .Doc(item)
                );
                Assert.True(updateResponse?.IsValid);
            }
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task DeleteEntity_ReturnNotNullEntity()
        {
            var origItem = new Ds1SourceItem()
            {
                itemGuid = "186591_636986273807561747",
                version = ConfigConst.LatestVersion,
                nsItemId = "8327TL-L",
                modifyAt = DateTime.Parse("2019-08-28 16:28:26"),
                createAt = DateTime.Parse("2019-08-28 16:28:26")
            };

            var queryResponse = await m_EsClient.SearchAsync<Ds1SourceItem>(s => s
                .Index(INDEX)
                .From(0)
                .Size(1)
                .Query(q => q
                    .Term(t => t
                        .Field(SysFieldKey.UniqueId)
                        .Value($"{origItem.itemGuid}:{origItem.version}")
                    )
                )); //Search based on _id

            Assert.True(queryResponse.IsValid);
            Assert.True(queryResponse.Hits?.Count() > 0);

            var delResponse = await m_EsClient.DeleteAsync<Ds1SourceItem>(queryResponse.Hits.First().Id, d =>
                d.Index(INDEX)
            );

            Assert.True(delResponse.IsValid);
        }

        [Fact(Skip = "Won't test elastic search service")]
        public async Task CreateIndex_ReturnSuccessResponse()
        {
            var existResult = await m_EsClient.Indices.ExistsAsync(INDEX);
            if (false == existResult.Exists)
            {
                var createIndexResponse = await m_EsClient.Indices.CreateAsync(INDEX, c => c
                    .Settings(s => s
                        .NumberOfShards(2)
                        .NumberOfReplicas(1)
                    )
                    .Map<Ds1SourceItem>(x => x
                        .AutoMap()
                    ));
                Assert.NotNull(createIndexResponse);
                Assert.True(createIndexResponse.IsValid);
            }
        }

        public override Task<bool> IsReady()
        {
            m_EntitySerializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            m_EntitySerializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
            };

            m_Mapper = ComponentMgr.Instance.TryResolve<IMapperMgr>();
            Assert.NotNull(m_Mapper);

            try
            {
                m_EsClient = CreateEsClient();
                return Task.FromResult(true);
            }
            catch (Exception ex) { Assert.Null(ex); }

            return base.IsReady();
        }

        ElasticClient CreateEsClient()
        {
            ConnectionSettings connectionSettings;
            ElasticClient elasticClient;
            StaticConnectionPool connectionPool;

            //Connection string for Elasticsearch
            /*connectionSettings = new ConnectionSettings(new Uri("http://localhost Jump :9200/")); //local PC
            elasticClient = new ElasticClient(connectionSettings);*/

            //Multiple node for fail over (cluster addresses)
            var nodes = new Uri[]
            {
                new Uri("https://api-dev.kevinw.net/es/")
            };

            connectionPool = new StaticConnectionPool(nodes);
            connectionSettings = new ConnectionSettings(connectionPool)
                .DefaultIndex(INDEX)
                .DisableDirectStreaming();
            //.DefaultIndex(_indexName)
            elasticClient = new ElasticClient(connectionSettings);
            //elasticClient.Indices.Delete(_defaultIndex);
            //elasticClient.Indices.Create(_defaultIndex);
            //var existsResponse = elasticClient.Indices.Exists(_defaultIndex);
            //Console.WriteLine($"{_defaultIndex} index exists: {existsResponse.Exists}");

            return elasticClient;
        }

        private const int PAGE_SIZE = 100;
        private const string INDEX = "ds1_item";

        protected DefaultEntitySerializer m_EntitySerializer;
        protected IMapperMgr m_Mapper;
        protected ElasticClient m_EsClient;
    }
}
