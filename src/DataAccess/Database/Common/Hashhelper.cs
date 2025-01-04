using System.Linq;

namespace Nwpie.Foundation.DataAccess.Database
{
    public class Hashhelper
    {
        /// <summary>
        ///  計算32位MD5碼（大小寫）：Hash_MD5_32
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_MD5_32(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為MD5碼
            var sHash = "";

            try
            {
                using (var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = md5CSP.ComputeHash(bytValue);
                    md5CSP.Clear();
                }

                string sTemp;
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///   計算16位MD5碼（大小寫）：Hash_MD5_16
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_MD5_16(string word, bool toUpper = true)
        {
            try
            {
                var sHash = Hash_MD5_32(word).Substring(8, 16);
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///  計算32位2重MD5碼（大小寫）：Hash_2_MD5_32
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_2_MD5_32(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為MD5碼
            var sHash = "";

            try
            {
                string sTemp;
                using (var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = md5CSP.ComputeHash(bytValue);

                    for (var counter = 0; counter < bytHash.Count(); counter++)
                    {
                        long i = bytHash[counter] / 16;
                        if (i > 9)
                        {
                            sTemp = ((char)(i - 10 + 0x41)).ToString();
                        }
                        else
                        {
                            sTemp = ((char)(i + 0x30)).ToString();
                        }

                        i = bytHash[counter] % 16;
                        if (i > 9)
                        {
                            sTemp += ((char)(i - 10 + 0x41)).ToString();
                        }
                        else
                        {
                            sTemp += ((char)(i + 0x30)).ToString();
                        }
                        sHash += sTemp;
                    }

                    bytValue = System.Text.Encoding.UTF8.GetBytes(sHash);
                    bytHash = md5CSP.ComputeHash(bytValue);
                    md5CSP.Clear();
                }

                sHash = "";
                //根據 計算得到的Hash碼翻譯為MD5碼
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///  計算16位2重MD5碼（大小寫）：Hash_2_MD5_16
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_2_MD5_16(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為MD5碼
            var sHash = "";

            try
            {
                string sTemp;
                using (var md5CSP = new System.Security.Cryptography.MD5CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = md5CSP.ComputeHash(bytValue);

                    for (var counter = 0; counter < bytHash.Count(); counter++)
                    {
                        long i = bytHash[counter] / 16;
                        if (i > 9)
                        {
                            sTemp = ((char)(i - 10 + 0x41)).ToString();
                        }
                        else
                        {
                            sTemp = ((char)(i + 0x30)).ToString();
                        }

                        i = bytHash[counter] % 16;
                        if (i > 9)
                        {
                            sTemp += ((char)(i - 10 + 0x41)).ToString();
                        }
                        else
                        {
                            sTemp += ((char)(i + 0x30)).ToString();
                        }
                        sHash += sTemp;
                    }

                    sHash = sHash.Substring(8, 16);
                    bytValue = System.Text.Encoding.UTF8.GetBytes(sHash);
                    bytHash = md5CSP.ComputeHash(bytValue);
                    md5CSP.Clear();
                }

                sHash = "";
                //根據 計算得到的Hash碼翻譯為MD5碼
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                sHash = sHash.Substring(8, 16);
                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///  計算SHA-1碼（大小寫）：Hash_SHA_1
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_SHA_1(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為SHA-1碼
            var sHash = "";

            try
            {
                using (var sha1CSP = new System.Security.Cryptography.SHA1CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = sha1CSP.ComputeHash(bytValue);
                    sha1CSP.Clear();
                }

                string sTemp;
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///  計算SHA-256碼（大小寫）：Hash_SHA_256
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_SHA_256(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為SHA-1碼
            var sHash = "";

            try
            {
                using (var sha256CSP = new System.Security.Cryptography.SHA256CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = sha256CSP.ComputeHash(bytValue);
                    sha256CSP.Clear();
                }

                string sTemp;
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///  計算SHA-384碼（大小寫）：Hash_SHA_384
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_SHA_384(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為SHA-1碼
            var sHash = "";
            try
            {
                using (var sha384CSP = new System.Security.Cryptography.SHA384CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = sha384CSP.ComputeHash(bytValue);
                    sha384CSP.Clear();
                }

                string sTemp;
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }

        /// <summary>
        ///   計算SHA-512碼（大小寫）：Hash_SHA_512
        /// </summary>
        /// <param name="word">字符串</param>
        /// <param name="toUpper">返回 Hash 值格式 true：英文大寫，false：英文小寫</param>
        /// <returns></returns>
        public static string Hash_SHA_512(string word, bool toUpper = true)
        {
            byte[] bytHash;
            //根據 計算得到的Hash碼翻譯為SHA-1碼
            var sHash = "";

            try
            {
                using (var sha512CSP = new System.Security.Cryptography.SHA512CryptoServiceProvider())
                {
                    var bytValue = System.Text.Encoding.UTF8.GetBytes(word);
                    bytHash = sha512CSP.ComputeHash(bytValue);
                    sha512CSP.Clear();
                }

                string sTemp;
                for (var counter = 0; counter < bytHash.Count(); counter++)
                {
                    long i = bytHash[counter] / 16;
                    if (i > 9)
                    {
                        sTemp = ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp = ((char)(i + 0x30)).ToString();
                    }

                    i = bytHash[counter] % 16;
                    if (i > 9)
                    {
                        sTemp += ((char)(i - 10 + 0x41)).ToString();
                    }
                    else
                    {
                        sTemp += ((char)(i + 0x30)).ToString();
                    }
                    sHash += sTemp;
                }

                //根據大小寫規則决定返回的字符串
                return toUpper ? sHash : sHash.ToLower();
            }
            catch { throw; }
        }
    }
}
