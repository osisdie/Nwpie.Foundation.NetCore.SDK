using Nwpie.Foundation.Abstractions.Logging;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Logging.Log4net;
using Microsoft.Extensions.Logging;

namespace Nwpie.xUnit.App_Start
{
    public static class DefaultLogUtil
    {
        public static void InitialLog4netProvider(bool overwriteFactory = true)
        {
            if (overwriteFactory)
            {
                LogMgr.LoggerFactory = new LoggerFactory();
            }

            var logFilePath = LoggingUtils.SDK_Log4netFilePath_DependsOnEnv(ConfigConst.DefaultConfigFolder);
            LogMgr.LoggerFactory.AddProvider(new Log4netProvider(logFilePath));
        }
    }
}
