using System;
using System.Threading;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Configuration.SDK.Extensions;
using Nwpie.Foundation.DataAccess.Database;
using Nwpie.Foundation.Logging.ElasticSearch;
using Nwpie.Foundation.Measurement.SDK;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Console.ReadLine();

            await Task.CompletedTask;
        }

        static void InitSDK()
        {
            ServiceContext.Initialize();
            IConfigurationBuilder config = new ConfigurationBuilder();
            ServiceContext.Configuration = config.SdkConfiguration().Build();
        }

        static async Task ExecuteDB()
        {
            try
            {
                var cmd = new CommandExecutor("Unittest:Db:Version");
                var result = await cmd.ExecuteScalarAsync<string>();
                Console.WriteLine($"checked db version is : {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception : {ex}");
            }
        }

        static void ExecuteLog()
        {
            LogMgr.LoggerFactory.AddProvider(new ElasticSearchProvider());
            var logger = LogMgr.CreateLogger(typeof(Program));

            logger.LogInformation($"this is current time info: {DateTime.UtcNow}");
            logger.LogWarning($"中文测试");
            logger.LogError($"test error. {new Exception("this is exception.")}");
        }

        static void ExecuteMeasurement()
        {
            MeasurementHelper.WritePoint(MeasurementName, DefaultKey, g_Random.Next(1, 100), DefaultTagOpID, Opid);

            for (int i = 0; i < 300; ++i)
            {
                MeasurementHelper.WritePoint(MeasurementName, DefaultKey, g_Random.Next(1, 100), DefaultTagOpID, Opid);

                Thread.Sleep(1000 * 10);
            }

        }

        const string MeasurementName = "_console_test";
        const string DefaultKey = "defaultkey";
        const string DefaultTagOpID = "opid";
        const string Opid = "Nwpie.Foundation.ConsoleApp1";

        static readonly Random g_Random = new();
    }
}
