﻿namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class InfluxDB_Option : OptionBase
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string CharSet { get; set; }
        public string Database { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int ConnectionLifetime { get; set; }
        public int ConnectionTimeout { get; set; }
        public int MaxPoolSize { get; set; }
        public int MinPoolSize { get; set; }
        public bool ConvertZeroDatetime { get; set; }
        public bool AllowZeroDatetime { get; set; }
        public string ConnectionString { get; set; }
    }
}
