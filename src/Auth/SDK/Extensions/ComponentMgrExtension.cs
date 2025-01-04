using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Common.Extras;

namespace Nwpie.Foundation.Auth.SDK.Extensions
{
    public static class ComponentMgrExtension
    {
        public static ITokenService GetDefaultTokenService(this ComponentMgr component) =>
            component.TryResolve<ITokenService>();

        public static IJwtAuthService GetDefaultJwtTokenService(this ComponentMgr component) =>
            component.TryResolve<IJwtAuthService>();

        public static IAesAuthService GetDefaultAesTokenService(this ComponentMgr component) =>
            component.TryResolve<IAesAuthService>();
    }
}
