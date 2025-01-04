using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Models;
using InfluxData.Net.Common.Enums;
using InfluxData.Net.InfluxDb;
using InfluxData.Net.InfluxDb.Models;
using InfluxData.Net.InfluxDb.Models.Responses;
using Microsoft.Extensions.Logging;

namespace Nwpie.Foundation.Measurement.Core
{
    public class InfluxDBManager : CObject
    {
        public InfluxDBManager(string uri, string username, string password)
            : base()
        {
            m_DBClient = new InfluxDbClient(uri, username, password, InfluxDbVersion.Latest);
        }

        public async Task WriteAsync(string dbName, IList<Point> points)
        {
            try
            {
                await m_DBClient.Client.WriteAsync(points, dbName: dbName);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }
        }

        public async Task<IEnumerable<Serie>> QueryAsync(string dbName, string query)
        {
            IEnumerable<Serie> response = new List<Serie>();

            try
            {
                response = await m_DBClient.Client.QueryAsync(query, dbName: dbName);
            }
            catch (Exception ex)
            {
                Logger.LogWarning(ex.ToString());
            }

            return response;
        }

        protected InfluxDbClient m_DBClient;
    }
}
