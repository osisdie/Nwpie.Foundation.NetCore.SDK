namespace Nwpie.Foundation.DataAccess.Database
{
    public interface IKeyedObject<T>
    {
        T Key { get; }
    }

    public interface IKeyedObject : IKeyedObject<string> { }
}
