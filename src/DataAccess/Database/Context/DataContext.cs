using System.Collections.Generic;
using System.Threading.Tasks;
using System.Transactions;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class DataContext : IDataContext
    {
        public void AddCommand(ICommand command)
        {
            m_Commands.Add(command);
        }

        public async Task CommitAsync()
        {
            using (var transactionScope = new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                foreach (var current in m_Commands)
                {
                    await current.ExecuteNonQueryAsync().ConfigureAwait(false);
                }

                transactionScope.Complete();
            }

            m_Commands.Clear();
        }

        protected readonly List<ICommand> m_Commands = new List<ICommand>();
    }
}
