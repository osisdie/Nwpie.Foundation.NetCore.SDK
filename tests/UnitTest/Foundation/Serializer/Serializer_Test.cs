using System;
using Nwpie.Foundation.Abstractions.Notification.Contracts;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Serializers;
using Nwpie.Foundation.Common.Serializers.Resolvers;
using Nwpie.xUnit.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Serializer
{
    public class Serializer_Test : TestBase
    {
        [Fact]
        public void DefaultDeserialize_Test()
        {
            var message = "World";
            var o = Serializer.Deserialize<NotifySend_RequestModel>(message, ignoreException: true);
            Assert.Null(o);
        }

        public Serializer_Test(ITestOutputHelper output) : base(output)
        {
            m_ExpectedModel = new PascalEntity()
            {
                ColumnChar = $"{m_RandomInt}-●㊣？",
                ColumnInt = m_RandomInt,
                ColumnDecimal = 1.1M,
                ColumnBool = m_RandomInt % 2 == 1,
                ColumnDate = DateTime.UtcNow,
                ColumnDatetime = m_Now
            };
        }


        [Fact]
        public void Null_Deserialize_Test()
        {
            var nil = "null";
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);

            PascalEntity o;
            o = defaultSerializer.Deserialize<PascalEntity>(nil);
            Assert.Null(o);
            o = defaultSerializer.Deserialize<PascalEntity>(string.Empty);
            Assert.Null(o);

            var s = defaultSerializer.Serialize(o);
            Assert.Equal("null", s);
        }

        [Fact]
        public void Deserialize_FromPascalEntityToModel_Test()
        {
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var json = defaultSerializer.Serialize(m_ExpectedModel);
            serializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            };

            var pascalIsBack = serializer.Deserialize<PascalEntity>(json);
            Assert.Equal(m_ExpectedModel.ColumnChar, pascalIsBack.ColumnChar);
        }

        [Fact]
        public void Deserialize_FromCamelEntityToModel_Test()
        {
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var entity = new CamelEntity()
            {
                columnChar = $"{m_RandomInt}-●㊣？",
                columnInt = m_RandomInt,
                columnDecimal = 1.1M,
                columnBool = m_RandomInt % 2 == 1,
                columnDate = DateTime.Today,
                columnDatetime = m_Now
            };

            var json = defaultSerializer.Serialize(entity);
            serializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            var pascalIsBack = serializer.Deserialize<PascalEntity>(json);
            Assert.Equal(m_ExpectedModel.ColumnChar, pascalIsBack.ColumnChar);
        }

        [Fact]
        public void Deserialize_FromSnakeEntityToModel_Test()
        {
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var entity = new UnderscoreEntity()
            {
                column_char = $"{m_RandomInt}-●㊣？",
                column_int = m_RandomInt,
                column_decimal = 1.1M,
                column_bool = m_RandomInt % 2 == 1,
                column_date = DateTime.Today,
                column_datetime = m_Now
            };
            var json = defaultSerializer.Serialize(entity);
            serializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
            };

            var pascalIsBack = serializer.Deserialize<PascalEntity>(json);
            Assert.Equal(m_ExpectedModel.ColumnChar, pascalIsBack.ColumnChar);
        }

        [Fact]
        public void Deserialize_FromUpperCaseEntityToModel_Test()
        {
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var underscore = new UpperCaseEntity()
            {
                COLUMNCHAR = $"{m_RandomInt}-●㊣？",
                COLUMNINT = m_RandomInt,
                COLUMNDECIMAL = 1.1M,
                COLUMNBOOL = m_RandomInt % 2 == 1,
                COLUMNDATE = DateTime.Today,
                COLUMNDATETIME = m_Now
            };
            var json = defaultSerializer.Serialize(underscore);
            serializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
            };

            var underscoreIsBack = serializer.Deserialize<PascalEntity>(json);
            Assert.Equal(m_ExpectedModel.ColumnChar, underscoreIsBack.ColumnChar);
        }

        [Fact]
        public void Deserialize_FromUpperCaseSnakeEntityToModel_Test()
        {
            var defaultSerializer = ComponentMgr.Instance.GetDefaultSerializer(isUseDI: false);
            var serializer = ComponentMgr.Instance
                .TryResolve<IEntitySerializer>()
                as DefaultEntitySerializer;
            Assert.NotNull(serializer);

            var underscore = new UpperCaseSnakeEntity()
            {
                COLUMN_CHAR = $"{m_RandomInt}-●㊣？",
                COLUMN_INT = m_RandomInt,
                COLUMN_DECIMAL = 1.1M,
                COLUMN_BOOL = m_RandomInt % 2 == 1,
                COLUMN_DATE = DateTime.Today,
                COLUMN_DATETIME = m_Now
            };
            var json = defaultSerializer.Serialize(underscore);
            serializer.Settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new UnderscorePropertyNamesContractResolver(),
            };

            var underscoreIsBack = serializer.Deserialize<PascalEntity>(json);
            Assert.Equal(m_ExpectedModel.ColumnChar, underscoreIsBack.ColumnChar);
        }

        protected PascalEntity m_ExpectedModel;
        protected DateTime? m_Now = DateTime.UtcNow;
        protected int m_RandomInt = new Random().Next(1, 100);
    }
}
