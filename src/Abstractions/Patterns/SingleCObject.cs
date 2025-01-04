using Nwpie.Foundation.Abstractions.Extras.Interfaces;

namespace Nwpie.Foundation.Abstractions.Patterns
{
    public abstract class SingleCObject<T> :
        LazySingleton<T>,
        ISingleCObject
        where T : LazySingleton<T>, new()
    {
        public SingleCObject()
        {
        }

        public abstract void Dispose();
    }
}
