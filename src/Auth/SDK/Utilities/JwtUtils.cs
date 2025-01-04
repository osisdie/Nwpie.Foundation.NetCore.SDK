using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Nwpie.Foundation.Abstractions.Auth.Enums;
using Nwpie.Foundation.Abstractions.Contracts;
using Nwpie.Foundation.Abstractions.Contracts.Extensions;
using Nwpie.Foundation.Abstractions.Contracts.Interfaces;
using Nwpie.Foundation.Abstractions.Enums;
using Nwpie.Foundation.Abstractions.Serializers.Interfaces;
using Nwpie.Foundation.Common.Serializers;
using Newtonsoft.Json.Linq;

namespace Nwpie.Foundation.Auth.SDK.Utilities
{
    public static class JwtUtils
    {
        static JwtUtils()
        {
            HashAlgorithms = new Dictionary<JwtHashAlgorithmEnum, Func<byte[], byte[], byte[]>>
            {
                { JwtHashAlgorithmEnum.RS256, (key, value) => {
                    using (var sha = new HMACSHA256(key))
                    {
                        return sha.ComputeHash(value);
                    }
                }},
                { JwtHashAlgorithmEnum.HS384, (key, value) => {
                    using (var sha = new HMACSHA384(key))
                    {
                        return sha.ComputeHash(value);
                    }
                }},
                { JwtHashAlgorithmEnum.HS512, (key, value) => {
                    using (var sha = new HMACSHA512(key))
                    {
                        return sha.ComputeHash(value);
                    }
                }}
            };

            m_Serializer = new DefaultSerializer();
        }

        public static string Encode(object payload, string key, JwtHashAlgorithmEnum algorithm) =>
            Encode(payload, Encoding.UTF8.GetBytes(key), algorithm);

        public static string Encode(object payload, byte[] keyBytes, JwtHashAlgorithmEnum algorithm)
        {
            var segments = new List<string>();
            var header = new { alg = algorithm.ToString(), typ = "JWT" };
            var headerBytes = Encoding.UTF8.GetBytes(m_Serializer.Serialize(header));
            var payloadBytes = Encoding.UTF8.GetBytes(m_Serializer.Serialize(payload));
            //byte[] payloadBytes = Encoding.UTF8.GetBytes(@"{"iss":"761326798069-r5mljlln1rd4lrbhg75efgigp36m78j5@developer.gserviceaccount.com","scope":"https://www.googleapis.com/auth/prediction","aud":"https://accounts.google.com/o/oauth2/token","exp":1328554385,"iat":1328550785}");

            segments.Add(Base64UrlEncode(headerBytes));
            segments.Add(Base64UrlEncode(payloadBytes));

            var stringToSign = string.Join(".", segments.ToArray());
            var bytesToSign = Encoding.UTF8.GetBytes(stringToSign);
            var signature = HashAlgorithms[algorithm](keyBytes, bytesToSign);
            segments.Add(Base64UrlEncode(signature));

            return string.Join(".", segments.ToArray());
        }

        public static IServiceResponse<T> Decode<T>(string token, string key, bool verify)
        {
            try
            {
                var parts = token.Split('.');
                var header = parts[0];
                var payload = parts[1];
                var crypto = Base64UrlDecode(parts[2]);

                var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
                var headerData = JObject.Parse(headerJson);
                var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));
                var payloadData = JObject.Parse(payloadJson);

                if (verify)
                {
                    var bytesToSign = Encoding.UTF8.GetBytes(string.Concat(header, ".", payload));
                    var keyBytes = Encoding.UTF8.GetBytes(key);
                    var algorithm = (string)headerData["alg"];

                    var signature = HashAlgorithms[GetHashAlgorithm(algorithm)](keyBytes, bytesToSign);
                    var decodedCrypto = Convert.ToBase64String(crypto);
                    var decodedSignature = Convert.ToBase64String(signature);

                    if (decodedCrypto != decodedSignature)
                    {
                        throw new ApplicationException($"Invalid signature. Expected {decodedCrypto} got {decodedSignature}. ");
                    }
                }

                if (typeof(T) == typeof(string))
                {
                    return new ServiceResponse<T>(true)
                        .Content((T)Convert.ChangeType(payloadData.ToString(), typeof(string)));
                }

                return new ServiceResponse<T>(true)
                    .Content(payloadData.ToObject<T>());
            }
            catch (Exception ex)
            {
                return new ServiceResponse<T>()
                    .Error(StatusCodeEnum.Exception, ex);
            }
        }

        public static JwtHashAlgorithmEnum GetHashAlgorithm(string algorithm)
        {
            switch (algorithm)
            {
                case "RS256":
                    return JwtHashAlgorithmEnum.RS256;
                case "HS384":
                    return JwtHashAlgorithmEnum.HS384;
                case "HS512":
                    return JwtHashAlgorithmEnum.HS512;
                default:
                    throw new Exception("Algorithm not supported. ");
            }
        }

        // from JWT spec
        public static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input);
            output = output.Split('=')[0]; // Remove any trailing '='s
            output = output.Replace('+', '-'); // 62nd char of encoding
            output = output.Replace('/', '_'); // 63rd char of encoding

            return output;
        }

        // from JWT spec
        public static byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+'); // 62nd char of encoding
            output = output.Replace('_', '/'); // 63rd char of encoding
            switch (output.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: output += "=="; break; // Two pad chars
                case 3: output += "="; break; // One pad char
                default:
                    throw new Exception("Illegal base64url string. ");
            }

            var converted = Convert.FromBase64String(output); // Standard base64 decoder
            return converted;
        }

        public static readonly Dictionary<JwtHashAlgorithmEnum, Func<byte[], byte[], byte[]>> HashAlgorithms;
        public static readonly JwtHashAlgorithmEnum DefaultJwtAlgorithm = JwtHashAlgorithmEnum.RS256;
        static readonly ISerializer m_Serializer;
    }
}
