using System.IO;

namespace Nwpie.Foundation.Logging.NLog
{
    public class NLogProvider : Microsoft.Extensions.Logging.ILoggerProvider
    {
        public NLogProvider(string logFilePath)
        {
            ProviderFile = new FileInfo(logFilePath);
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name)
        {
            return new NLogAdapter(name, ProviderFile);
        }

        public void Dispose() { }

        public FileInfo ProviderFile { get; private set; }
    }
}
