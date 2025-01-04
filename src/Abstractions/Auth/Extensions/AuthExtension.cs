using System;

namespace Nwpie.Foundation.Abstractions.Auth.Extensions
{
    public static class AuthExtension
    {
        public static bool IsAttachBasic(this string token, string keyword = "Basic") => IsAttachPrefix(token, keyword);
        public static bool IsAttachBearer(this string token, string keyword = "Bearer") => IsAttachPrefix(token, keyword);
        public static bool IsAttachPAT(this string token, string keyword = "PAT") => IsAttachPrefix(token, keyword);

        public static string AttachBasic(this string token, string keyword = "Basic") => AttachPrefix(token, keyword);
        public static string AttachBearer(this string token, string keyword = "Bearer") => AttachPrefix(token, keyword);
        public static string AttachPAT(this string token, string keyword = "PAT") => AttachPrefix(token, keyword);

        public static string TrimBasic(this string token, string keyword = "Basic") => TrimPrefix(token, keyword);
        public static string TrimBearer(this string token, string keyword = "Bearer") => TrimPrefix(token, keyword);
        public static string TrimPAT(this string token, string keyword = "PAT") => TrimPrefix(token, keyword);

        public static bool IsAttachPrefix(string src, string leading)
        {
            return true == src?.StartsWith($"{leading} ", StringComparison.OrdinalIgnoreCase);
        }

        public static string AttachPrefix(string src, string leading)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return src;
            }

            if (false == IsAttachPrefix(src, leading))
            {
                src = $"{leading} " + src;
            }

            return src;
        }

        public static string TrimPrefix(string src, string trimer)
        {
            if (string.IsNullOrWhiteSpace(src))
            {
                return src;
            }

            if (true == src?.StartsWith($"{trimer} ", StringComparison.OrdinalIgnoreCase))
            {
                src = src.Replace($"{trimer} ", string.Empty).Trim();
            }
            return src;
        }
    }
}
