using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Config;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Measurement.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Nwpie.Foundation.Common.Utilities;
using Nwpie.Foundation.Measurement.Core;
using Nwpie.Foundation.Measurement.Core.Interfaces;
using Nwpie.Foundation.Measurement.SDK;
using Microsoft.Extensions.Configuration;
using Xunit;
using Xunit.Abstractions;

namespace Nwpie.xUnit.Foundation.Measurement
{
    public class Measurement_APM_Test : TestBase
    {
        public Measurement_APM_Test(ITestOutputHelper output) : base(output) { }

        [Fact(Skip = "Won't test metrics service")]
        public void InsertQueue_ViaClient_Test()
        {
            // Part 1
            var opid = MethodBase.GetCurrentMethod()?.Name;
            m_MetricClient.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);

            for (var i = 0; i < 300; ++i)
            {
                m_MetricClient.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);

                Thread.Sleep(1000 * 10);
            }
        }

        [Fact(Skip = "Won't test metrics service")]
        public void InsertQueue_ViaHelper_Test()
        {
            // Part 1
            var opid = MethodBase.GetCurrentMethod()?.Name;
            MeasurementHelper.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);

            for (var i = 0; i < 300; ++i)
            {
                MeasurementHelper.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);
                Thread.Sleep(1000 * 10);
            }
        }

        [Fact(Skip = "Won't test metrics service")]
        public void InsertQueue_ConsoleMode_Test()
        {
            Console.WriteLine("Hello World!");

            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json");

            var opid = Guid.NewGuid().ToString();
            MeasurementHelper.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);

            for (var i = 0; i < 300; ++i)
            {
                MeasurementHelper.WritePoint(MeasurementName, DefaultKey, Utility.Randomizer.Next(1, 100), DefaultTagOpID, opid);
                Thread.Sleep(1000 * 10);
            }

            Console.ReadLine();
        }

        [Fact(Skip = "Won't test metrics service")]
        public void Insert_ViaServer_Test()
        {
            // Part 1
            var opid = MethodBase.GetCurrentMethod()?.Name;
            var record = new List<MetricPoint>() {
                new MetricPoint()
                {
                    Name = MeasurementName,
                     Fields = new List<Field>()
                     {
                         new Field()
                         {
                             Name = DefaultKey,
                             Value = Utility.Randomizer.Next(1, 100)
                         }
                     },
                     Tags = new List<Tag>()
                     {
                         new Tag()
                         {
                             Name = DefaultTagOpID,
                             Value = opid
                         }
                     }
                }
            };

            Write(record);

            for (var i = 0; i < 300; ++i)
            {
                record.First().Fields.First().Value = Utility.Randomizer.Next(1, 100);
                Write(record);

                Thread.Sleep(1000 * 10);
            }
        }

        private void Write(List<MetricPoint> points)
        {
            if (true == ServiceContext.Config.APM?.InfluxDefaultDB.HasValue())
            {
                m_DBServer.Write(ServiceContext.Config.APM?.InfluxDefaultDB, points);
                return;
            }

            m_DBServer.Write(points);
        }

        public override Task<bool> IsReady()
        {
            var host = ServiceContext.Config.APM?.InfluxHostUri;
            Assert.NotNull(host);

            var database = ServiceContext.Config.APM?.InfluxDefaultDB;
            //Assert.NotNull(database); // Use InfluxDBEngine.DefaultDB

            var username = ServiceContext.Config.APM?.InfluxDbUser;
            Assert.NotNull(username);
            var password = ServiceContext.Config.APM?.InfluxDbPwd;
            Assert.NotNull(password);

            m_MetricClient = ComponentMgr.Instance.TryResolve<IMeasurement>();
            Assert.NotNull(m_MetricClient);

            m_DBServer = new InfluxDBEngine(
                new ConfigOptions<InfluxDB_Option>(
                    new InfluxDB_Option()
                    {
                        Host = host,
                        Database = database,
                        Username = username,
                        Password = password
                    }
                )
            );

            return base.IsReady();
        }

        private const string MeasurementName = "_unit_test";
        private const string DefaultKey = "defaultkey";
        private const string DefaultTagOpID = "opid";

        protected IMeasurement m_MetricClient;
        protected IInfluxDBEngine m_DBServer;
    }
}
