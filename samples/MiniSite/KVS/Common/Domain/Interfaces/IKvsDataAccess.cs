namespace Nwpie.MiniSite.KVS.Common.Domain.Interfaces
{
    public interface IKvsDataAccess
    {
        MySql.Data.MySqlClient.MySqlConnection GetMySqlConnection();
    }
}
