using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Nwpie.Foundation.Abstractions.Measurement.Models;
using Nwpie.Foundation.Common.Measurement;

namespace Nwpie.Foundation.Measurement.Core.Interfaces
{
    public interface IInfluxDBEngine : ISingleCObject
    {
        void Write(List<MetricPoint> points);
        void Write(string dbName, List<MetricPoint> points);

        Task<List<MetricSerie>> QueryAsync(string query);
        Task<List<MetricSerie>> QueryAsync(string dbName, string query);

        bool IsSilent { get; set; }
    }
}
