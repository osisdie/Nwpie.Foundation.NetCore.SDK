namespace Nwpie.Foundation.Abstractions.Extensions
{
    public static class BoolExtension
    {
        public static int ToInt(this bool src) => true == src ? 1 : 0;
    }
}
