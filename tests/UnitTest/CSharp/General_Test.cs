using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Notification.Enums;
using Nwpie.Foundation.Abstractions.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceStack;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.CSharp
{
    public class General_Test
    {
        public General_Test(ITestOutputHelper output)
        {
            m_Output = output;
        }

        [Fact]
        public void Version_Test()
        {
            var version = "1.2.3";
            var versionDto = new Version(version);
            Assert.Equal(1, versionDto.Major);
            Assert.Equal(2, versionDto.Minor);
            Assert.Equal(3, versionDto.Build);
        }

        [Fact]
        public void Enum_Test()
        {
            {
                var extensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(NotifyChannelEnum), NotifyChannelEnum.Email.ToString() }
                };

                var notifyChannel = extensionMap?[nameof(NotifyChannelEnum)];
                Assert.True(notifyChannel.HasValue());
                var found = Enum.TryParse(typeof(NotifyChannelEnum), notifyChannel, true, out var e);
                Assert.True(found);
                Assert.Equal(NotifyChannelEnum.Email, e);

                e = Enum<NotifyChannelEnum>.TryParse(notifyChannel, NotifyChannelEnum.UnSet);
                Assert.Equal(NotifyChannelEnum.Email, e);
            }

            {
                var extensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(NotifyChannelEnum), NotifyChannelEnum.Email.ToString().ToLower() }
                };

                var notifyChannel = extensionMap?[nameof(NotifyChannelEnum)];
                Assert.True(notifyChannel.HasValue());
                var found = Enum.TryParse(typeof(NotifyChannelEnum), notifyChannel, true, out var e);
                Assert.True(found);
                Assert.Equal(NotifyChannelEnum.Email, e);

                e = Enum<NotifyChannelEnum>.TryParse(notifyChannel, NotifyChannelEnum.UnSet);
                Assert.Equal(NotifyChannelEnum.Email, e);
            }

            {
                var extensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(NotifyChannelEnum), NotifyChannelEnum.UnSet.ToString() }
                };

                var notifyChannel = extensionMap?[nameof(NotifyChannelEnum)];
                Assert.True(notifyChannel.HasValue());
                var found = Enum.TryParse(typeof(NotifyChannelEnum), notifyChannel, true, out var e);
                Assert.True(found);
                Assert.Equal(NotifyChannelEnum.UnSet, e);

                e = Enum<NotifyChannelEnum>.TryParse(notifyChannel, NotifyChannelEnum.UnSet);
                Assert.Equal(NotifyChannelEnum.UnSet, e);
            }

            {
                var extensionMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { nameof(NotifyChannelEnum), NotifyChannelEnum.UnSet.ToString().ToLower() }
                };

                var notifyChannel = extensionMap?[nameof(NotifyChannelEnum)];
                Assert.True(notifyChannel.HasValue());
                var found = Enum.TryParse(typeof(NotifyChannelEnum), notifyChannel, true, out var e);
                Assert.True(found);
                Assert.Equal(NotifyChannelEnum.UnSet, e);

                e = Enum<NotifyChannelEnum>.TryParse(notifyChannel, NotifyChannelEnum.UnSet);
                Assert.Equal(NotifyChannelEnum.UnSet, e);
            }

            {
                Dictionary<string, string> extensionMap = null;
                var notifyChannel = extensionMap?[nameof(NotifyChannelEnum)];
                Assert.False(notifyChannel.HasValue());
                var found = Enum.TryParse(typeof(NotifyChannelEnum), notifyChannel, true, out var e);
                Assert.False(found);
                Assert.Null(e);

                e = Enum<NotifyChannelEnum>.TryParse(notifyChannel, NotifyChannelEnum.UnSet);
                Assert.Equal(NotifyChannelEnum.UnSet, e);
            }
        }


        public class TestStruct
        {
            [JsonExtensionData]
            public Dictionary<string, JToken> errors { get; set; }
        }


        [Fact]
        public void Dictionary_Test()
        {
            {
                var errorJson = "{\"errors\":{\"line_items[0].variant_id\":[\"not found\"]}}";
                var typedMsg = JsonConvert.DeserializeObject<TestStruct>(errorJson);
                Assert.Single(typedMsg?.errors);

                var root = typedMsg.errors.First();
                Assert.Equal("errors", root.Key);

                var child = root.Value.ToObject<TestStruct>();
                var childRoot = child.errors.First();
                Assert.Equal("line_items[0].variant_id", childRoot.Key);

                var msgList = childRoot.Value.ToObject<List<string>>();
                Assert.Single(msgList);
                Assert.Equal("not found", msgList.First());
            }

            {
                var collection = new NameValueCollection(StringComparer.OrdinalIgnoreCase)
                {
                    { "key1", "value1" }
                };

                Assert.NotNull(collection?["key1"]);
                Assert.Null(collection?["key2"]);
            }

            {
                var collection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "key1", "value1" }
                };

                Assert.NotNull(collection?["key1"]);
                var found = collection.TryGetValue("key2", out var e);
                Assert.False(found);
                Assert.Null(e);
            }
        }

        [Fact]
        public void File_Test()
        {
            // Windows style
            {
                var file = @"C:\temp\abc.jpg";
                var name = Path.GetFileName(file);
                Assert.Equal("abc.jpg", name);
            }

            // Unix style
            {
                var file = @"/temp/abc.jpg";
                var name = Path.GetFileName(file);
                Assert.Equal("abc.jpg", name);
            }
        }

        [Fact]
        public void Casting_Test()
        {
            var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"a", "a1" }
            };

            var dynamicKV = JsonConvert.DeserializeObject<ExpandoObject>(
                JsonConvert.SerializeObject(dict)
            ) as dynamic;
            Assert.NotNull(dynamicKV);
            Assert.NotNull(dynamicKV.a);
            Assert.Equal(dict.First().Value, dynamicKV.a);
        }

        [Fact]
        public void Syntax_Test()
        {
            TestModel a = null;
            Assert.True(true);
            Assert.False(false);
            Assert.NotNull(new object());
            Assert.NotNull(nameof(TestModel.Items.NullList));
            Assert.Null(null);
            Assert.Equal(1, 1);
            Assert.NotEqual(1, 2);
            Assert.Empty(new List<int>());
            Assert.NotEmpty(new List<int>() { 1, 2 });
            Assert.Same(a, a);
            Assert.NotSame(new object(), new object());
            Assert.Contains(new List<int>() { 1, 2, 3 }, o => 3 == o);
            Assert.DoesNotContain(new List<int>() { 1, 2, 3 }, o => 4 == o);
            Assert.Null(a?.Items.NullList.Count());
        }

        [Fact]
        public void Time_Test()
        {
            {
                var now = DateTime.Now;
                var ts = UnixTime.FromLocal(now);
                var backToNow = UnixTime.ToUtcDateTime(ts);
                Assert.Equal(now.ToUniversalTime().Date, backToNow.Date);
                Assert.Equal(now.ToUniversalTime().ToString("s"), backToNow.ToString("s"));
            }

            {
                var now = DateTime.UtcNow;
                var ts = UnixTime.FromUtc(now);
                var backToNow = UnixTime.ToUtcDateTime(ts);
                Assert.Equal(now.Date, backToNow.Date);
                Assert.Equal(now.ToString("s"), backToNow.ToString("s"));
            }
        }

        [Fact]
        public void Url_Test()
        {
            var absoluteUrl = "https://api-dev.kevinw.net/es/HealthCheck/HlckEchoRequest?a=b";
            var rawUrl = "/HealthCheck/HlckEchoRequest";
            var fromPathInfo = absoluteUrl.InferBaseUrl(rawUrl);
            Assert.Equal("https://api-dev.kevinw.net/es", fromPathInfo);

            var uri = new Uri(absoluteUrl);
            Assert.Equal("api-dev.kevinw.net", uri.Host);
            Assert.Equal("api-dev.kevinw.net", absoluteUrl.ExtractHostUrl());
            Assert.Equal("api-dev.kevinw.net/es", absoluteUrl.ExtractHostUrl("es"));

            Assert.Equal("/es/HealthCheck/HlckEchoRequest?a=b", uri.PathAndQuery);
            Assert.Equal("/es/HealthCheck/HlckEchoRequest",
                uri.PathAndQuery.IndexOf('?') > 0
                ? uri.PathAndQuery.Substring(0, uri.PathAndQuery.IndexOf('?'))
                : uri.PathAndQuery
            );
            Assert.Equal("/es/HealthCheck/HlckEchoRequest", uri.PathAndQuery.GetPathWithoutQuery());
            Assert.Equal("/HealthCheck/HlckEchoRequest", uri.PathAndQuery.GetPathWithoutQuery("es"));
        }

        [Fact]
        public async Task Parallel_Test()
        {
            var ids = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            var tasks = new List<Task>();
            m_Output.WriteLine($"Start");
            foreach (var id in ids)
            {
                var myId = id;
                tasks.Add(Task.Run(async () =>
                {
                    await Task.CompletedTask;
                    m_Output.WriteLine($"{myId}");
                }));
            }

            await Task.WhenAll(tasks);
            m_Output.WriteLine($"End");
        }

        private class TestModel
        {
            public TestModelItem Items { get; set; }
        }

        private class TestModelItem
        {
            public List<string> NullList { get; set; }
        }

        private readonly ITestOutputHelper m_Output;
    }
}
