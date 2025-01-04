using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Models;

namespace Nwpie.Foundation.Measurement.SDK.Interfaces
{
    public interface IMeasurementHost : ISingleCObject
    {
        void InitialClient();
        void PerformWrite(MetricPoint point);
        void ProcessPoints(object sender, List<MetricPoint> points);
        Task PushPointsToServer(List<MetricPoint> points);

        string DefaultTopic { get; set; }
        string DefaultDBName { get; set; }
        bool MeasurementEnabled { get; set; }
        string QueueName { get; set; }

    }
}
