namespace Nwpie.Foundation.Abstractions.Auth.Enums
{
    /// <summary>
    /// @see EMSecurityLevel
    /// </summary>
    public enum AccessLevelEnum
    {
        Anonymous = 0,
        Any = 1,
        Specific = 2,

        Admin = 99,
        Deny = 1000,
    }
}
