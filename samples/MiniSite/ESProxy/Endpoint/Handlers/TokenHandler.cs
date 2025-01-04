using System;
using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Models;
using Nwpie.Foundation.Abstractions.Config.Interfaces;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Extensions;
using Nwpie.Foundation.Abstractions.Timers;
using Nwpie.Foundation.Auth.SDK.Interfaces;
using Nwpie.Foundation.Auth.SDK.Utilities;
using Nwpie.Foundation.Common;
using Nwpie.Foundation.Common.Extras;
using Microsoft.AspNetCore.Http;

namespace Nwpie.MiniSite.ES.Endpoint.Handlers
{
    public class TokenHandler : TimerHandlerBase
    {
        public TokenHandler()
            : this(DefaultRefreshAppInterval)
        {
        }

        public TokenHandler(int interval)
            : base(interval)
        {
        }

        public override void Initialization()
        {
            m_AuthOption = ComponentMgr.Instance.TryResolve<IConfigOptions<Auth_Option>>();
            m_TokenService = ComponentMgr.Instance.TryResolve<IJwtAuthService>();
        }

        public override void Start()
        {
            OnExecuting(null); // Manual run
            base.Start();
        }

        public override void OnExecuting(object state)
        {
            if (string.IsNullOrWhiteSpace(m_AuthOption?.Value?.AdminToken) ||
                string.IsNullOrWhiteSpace(ServiceContext.AuthServiceUrl))
            {
                return;
            }
        }

        public async Task<bool> IsValidToken(HttpRequest request)
        {
            var token = TokenUtils.GetTokenFromHeaderOrQuery(request);
            if (token.HasValue() && null != m_TokenService)
            {
                var decoded = await m_TokenService.VerifyToken<TokenDataModel>(token,
                    TokenKindEnum.AccessToken
                );
                if (true != decoded?.IsSuccess)
                {
                    throw new UnauthorizedAccessException(decoded.ErrMsg ?? StatusCodeEnum.InvalidAccessToken.ToString());
                }

                return true;
            }

            return false;
        }

        public const int DefaultRefreshAppInterval = 300000;

        protected IConfigOptions<Auth_Option> m_AuthOption;
        protected ITokenService m_TokenService; // Jwt
    }
}
