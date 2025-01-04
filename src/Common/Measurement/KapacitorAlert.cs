using System.Collections.Generic;

namespace Nwpie.Foundation.Common.Measurement
{
    public class KapacitorAlert
    {
        public string ID { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }
        public string Time { get; set; }
        public long Duration { get; set; }
        public string Level { get; set; }
        public KapacitorAlertData Data { get; set; }
    }

    public class KapacitorAlertData
    {
        public List<KapacitorAlertDataSeries> Series { get; set; }
    }

    public class KapacitorAlertDataSeries
    {
        public string Name { get; set; }
        public List<string> Columns { get; set; }
        public List<List<string>> Values { get; set; }
    }
}
