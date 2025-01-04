using System;
using System.Data;
using System.Data.Common;

namespace Nwpie.Foundation.DataAccess.Database.Common.Interfaces
{
    public interface IDataCommand
    {
        IDataCommand SetDbProviderFactory(DbProviderFactory Dbfactory);
        IDataCommand SetConectString(string connectString);
        IDataCommand SetTimeOut(int timeout);
        IDataCommand SetCommand(string cmd);
        IDataCommand SetCommandText(string commandTest);
        IDataCommand SetCommandType(CommandType type);
        IDataCommand SetTransaction(DbTransaction trans);
        IDataCommand AddParameter(DbParameter parameter);
        IDataCommand AddParameter<TParameter>(string name, TParameter value);
        IDataCommand AddParameterOut<TParameter>(string name, Action<TParameter> callback);
        IDataCommand AddParameterOut<TParameter>(string name, TParameter value, Action<TParameter> callback);
        IDataCommand ReturnValue<TParameter>(Action<TParameter> callback);
        DbParameter CreateParameter();
    }
}
