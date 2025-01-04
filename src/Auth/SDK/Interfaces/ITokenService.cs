using System.Threading.Tasks;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Auth.Interfaces;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Extras.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Nwpie.Foundation.Auth.SDK.Interfaces
{
    public interface ITokenService : ISingleCObject
    {
        Task<IServiceResponse<T>> VerifyToken<T>(string encrypted, TokenKindEnum kind = TokenKindEnum.AccessToken, T compareToCurrent = null)
            where T : class, ITokenDataModel, new();
        /// <summary>
        /// Decode > deserialize to object
        /// </summary>
        /// <param name="encryptedToken"></param>
        /// <returns></returns>
        Task<T> Deserialize<T>(string encrypted);

        /// <summary>
        /// Debug Use
        /// </summary>
        /// <returns></returns>
        Task<T> GenerateAdminTokenModel<T>(
            TokenKindEnum kind = TokenKindEnum.AccessToken,
            TokenLevelEnum level = TokenLevelEnum.ApplicationUser)
            where T : class, ITokenDataModel, new();

        /// <summary>
        /// Encode token model
        /// Model > serialized string > encrypt the string
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<string> Encode(ITokenDataModel model);

        /// <summary>
        /// Decode
        /// encrypted string > serialized string
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> Decode(string encrypted);

        /// <summary>
        /// Parse from header or queryString
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        string GetAuthorizationString(HttpRequest request);

        /// <summary>
        /// Decode and try deserialize
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<T> GetTokenDetail<T>(HttpRequest request)
            where T : class, ITokenDataModel, new();

        /// <summary>
        /// Extend expire time base on current token model
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> RenewToken(string encrypted);
        Task<string> RenewToken(HttpRequest request);

        /// <summary>
        /// Specifal token condition
        /// return:
        ///     null: decode 失敗
        ///     false: decode 成功且 NOT admin token
        ///     true: decode 成功且 is admin token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool?> IsAdmin(string encrypted);
        Task<bool?> IsAdmin(HttpRequest request);

        /// <summary>
        /// Token is issued by CIAM or not
        /// return:
        ///     false: decode 失敗
        ///     true: decode 成功
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool?> IsGeneratedFromAuthServer(string encrypted);
        Task<bool?> IsGeneratedFromAuthServer(HttpRequest request);

        /// <summary>
        /// Token is expired or not
        /// return:
        ///     null: decode 失敗
        ///     false: decode 成功且 expired
        ///     true: decode 成功且 NOT expired
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<bool?> IsExpired(string encryptedToken);
        Task<bool?> IsExpired(HttpRequest request);
        Task<bool?> IsExpired(ITokenDataModel tokenModel);
        Task<bool?> IsClientSourceChanged(ITokenDataModel src, ITokenDataModel current);
        Task<int?> GetMinimumRefershTokenVersion();
        Task IncreseMinimumRefershTokenVersion();

        /// <summary>
        /// Divider: ',' or ';'
        /// </summary>
        string AdminMails { get; set; }

        /// <summary>
        /// Default admin token mapping Account GUID (if enabled and needed)
        /// </summary>
        string AdminAccountId { get; set; }
    }

    public interface ITokenService<T> : ITokenService
        where T : class, ITokenDataModel, new()
    {
        //new T GetTokenDetail(HttpRequest request);
    }
}
