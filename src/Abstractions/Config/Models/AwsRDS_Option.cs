namespace Nwpie.Foundation.Abstractions.Config.Models
{
    public class AwsRDS_Option : CloudProfile_Option
    {
        public string Name { get; set; } // RDS_DB_NAME
        public string Username { get; set; } // RDS_USERNAME
        public string Password { get; set; } // RDS_PASSWORD
        public string Hostname { get; set; } // RDS_HOSTNAME
        public int? Port { get; set; } // RDS_PORT
        public int? ConnectionTimeout { get; set; }
        public string ConnectionString { get; set; }
    }
}
