using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Config.Models;
using Nwpie.Foundation.Abstractions.Statics;
using Nwpie.Foundation.Common;

namespace Nwpie.xUnit.App_Start
{
    public static class DefaultAuthUtil
    {
        public static (string old, string @new) OverwriteAuthAESConfig(string newConfig = null)
        {
            string oldConfig = ServiceContext.Configuration?[SysConfigKey.Default_Auth_ConfigKey];
            if (string.IsNullOrEmpty(newConfig))
            {
                newConfig = AES_Gen_PrivateKey();
            }
            ServiceContext.Configuration[SysConfigKey.Default_Auth_ConfigKey] = newConfig;
            return (oldConfig, newConfig);
        }

        private static string AES_Gen_PrivateKey()
        {
            Auth_Option option = new()
            {
                Enabled = true,
                AdminToken = "fake_admin_token",
                AdminTokenEnabled = true,
                CacheMinutes = 5,
                ExpireMinutes = 4320,
                AccessTokenMinutes = 480,
                RefreshTokenMinutes = 14400,
                ApiTokenMinutes = 262800,
                MinimumAccessTokenVersion = 0,
                MinimumRefershTokenVersion = 0,
                MinimumApiTokenVersion = 0,
                JwtAlgorithm = JwtHashAlgorithmEnum.RS256.ToString(),
            };

            using var rsa = RSA.Create(2048);
            var jwtPrivateKey = ExportPrivateKeyToPem(rsa);
            option.JwtPrivateKey = jwtPrivateKey;
            Console.WriteLine("jwtPrivateKey (PEM):");
            Console.WriteLine(jwtPrivateKey);

            var jwtPrivateKeyXml = rsa.ToXmlString(true);
            option.JwtPrivateKeyXml = jwtPrivateKeyXml;
            Console.WriteLine("\njwtPrivateKeyXml (XML):");
            Console.WriteLine(jwtPrivateKeyXml);

            var jwtPublicKeyXml = rsa.ToXmlString(false);
            option.JwtPublicKeyXml = jwtPublicKeyXml;
            Console.WriteLine("\njwtPublicKeyXml (XML):");
            Console.WriteLine(jwtPublicKeyXml);

            var jwtAuthKeyBase64 = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
            option.JwtAuthKeyBase64 = jwtAuthKeyBase64;
            Console.WriteLine("\njwtAuthKeyBase64 (Base64):");
            Console.WriteLine(jwtAuthKeyBase64);

            using var aes = Aes.Create();
            aes.KeySize = 256;
            aes.GenerateKey();
            aes.GenerateIV();

            var authAESKey = Convert.ToBase64String(aes.Key);
            var authAESIV = Convert.ToBase64String(aes.IV);
            option.AuthAESKey = authAESKey;
            option.AuthAESIV = authAESIV;
            Console.WriteLine("\nauthAESKey (Base64):");
            Console.WriteLine(authAESKey);
            Console.WriteLine("\nauthAESIV (Base64):");
            Console.WriteLine(authAESIV);

            return JsonSerializer.Serialize(option);
        }

        private static string ExportPrivateKeyToPem(RSA rsa)
        {
            var privateKey = rsa.ExportRSAPrivateKey();
            var base64PrivateKey = Convert.ToBase64String(privateKey);

            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN RSA PRIVATE KEY-----"); // # gitleaks:ignore
            sb.AppendLine(base64PrivateKey);
            sb.AppendLine("-----END RSA PRIVATE KEY-----"); // # gitleaks:ignore

            return sb.ToString();
        }
    }
}
