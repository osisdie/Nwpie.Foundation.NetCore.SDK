namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IExecutor
    {
        int Execute();
        void AddToContext(IDataContext context);
    }
}
