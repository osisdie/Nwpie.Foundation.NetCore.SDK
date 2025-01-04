using Nwpie.xUnit.Models;
using ServiceStack;
using Xunit;

namespace Nwpie.xUnit.Foundation.ServiceNode
{
    public class Mapper_Test
    {
        [Fact]
        public void NullJson_Test()
        {
            CamelEntity camelEntity = null;

            {
                var json = camelEntity?.ToJson();
                Assert.Null(json);
            }

            {
                var json = string.Empty;
                var camelDeserialized = json.FromJson<CamelEntity>();
                Assert.Null(camelDeserialized);
            }

            {
                string json = null;
                var camelDeserialized = json?.FromJson<CamelEntity>();
                Assert.Null(camelDeserialized);
            }
        }

        [Fact]
        public void Json_Test()
        {
            var camelEntity = new CamelEntity
            {
                id = 1,
                columnChar = "2",
                columnInt = 3
            };

            var pascalEntity = new PascalEntity
            {
                Id = 1,
                ColumnChar = "2",
                ColumnInt = 3
            };

            {
                var json = camelEntity.ToJson();
                var camelDeserialized = json.FromJson<CamelEntity>();
                Assert.Equal(camelEntity.id, camelDeserialized.id);
                Assert.Equal(camelEntity.columnChar, camelDeserialized.columnChar);
                Assert.Equal(camelEntity.columnInt, camelDeserialized.columnInt);

                var pascalDeserialized = json.FromJson<PascalEntity>();
                Assert.Equal(pascalEntity.Id, pascalDeserialized.Id);
                Assert.Equal(pascalEntity.ColumnChar, pascalDeserialized.ColumnChar);
                Assert.Equal(pascalEntity.ColumnInt, pascalDeserialized.ColumnInt);
            }

            {
                var json = pascalEntity.ToJson();
                var pascalDeserialized = json.FromJson<PascalEntity>();
                Assert.Equal(pascalEntity.Id, pascalDeserialized.Id);
                Assert.Equal(pascalEntity.ColumnChar, pascalDeserialized.ColumnChar);
                Assert.Equal(pascalEntity.ColumnInt, pascalDeserialized.ColumnInt);

                var camelDeserialized = json.FromJson<CamelEntity>();
                Assert.Equal(camelEntity.id, camelDeserialized.id);
                Assert.Equal(camelEntity.columnChar, camelDeserialized.columnChar);
                Assert.Equal(camelEntity.columnInt, camelDeserialized.columnInt);
            }
        }
    }
}
