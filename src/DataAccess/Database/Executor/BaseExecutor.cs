using System.Data;

namespace Nwpie.Foundation.DataAccess.Database
{
    internal class BaseExecutor : IExecutor
    {
        public BaseExecutor(string tableAlias, OperationEnum operation)
        {
            CommandBuilder = new EntityCommandBuilder(tableAlias, operation);
        }

        public int Execute()
        {
            var command = CommandBuilder.Build();
            command.Parameters.Add("@returnValue", null, DbType.Int32, ParameterDirection.ReturnValue);

            _ = command.ExecuteNonQueryAsync();
            var returnValue = command.Parameters.Get<int>("@returnValue");

            return returnValue;
        }

        public void AddToContext(IDataContext context)
        {
            var command = CommandBuilder.Build();
            context.AddCommand(command);
        }

        protected EntityCommandBuilder CommandBuilder { get; set; }
    }
}
