using System;
using System.IO;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Nwpie.Foundation.Common.Utilities
{
    public static class CryptoUtils
    {
        #region base64
        public static byte[] TryDecodeBase64(string encoded)
        {
            if (string.IsNullOrWhiteSpace(encoded) ||
                0 != encoded.Length % 4 ||
                encoded.Contains(" ") ||
                encoded.Contains("\t") ||
                encoded.Contains("\r") ||
                encoded.Contains("\n"))
            {
                return null;
            }

            try
            {
                return Convert.FromBase64String(encoded);
            }
            catch { }

            return null;
        }
        #endregion

        #region sha1, md5
        public static byte[] GetSha1Bytes(string raw, Encoding encoding = null)
        {
            using (HashAlgorithm algorithm = SHA1.Create())
            {
                return algorithm.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(raw));
            }
        }

        public static string ComputeSha1Hash(string raw, Encoding encoding = null)
        {
            var sb = new StringBuilder();
            foreach (var b in GetSha1Bytes(raw, encoding))
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }

        /// <summary>
        /// Compute hash for string encoded as UTF8
        /// </summary>
        /// <param name="s">String to be hashed</param>
        /// <returns>40-character hex string</returns>
        public static string GetSha1String(string s, Encoding encoding = null)
        {
            var bytes = (encoding ?? Encoding.UTF8).GetBytes(s);
            byte[] hashBytes;
            using (var sha1 = SHA1.Create())
            {
                hashBytes = sha1.ComputeHash(bytes);
            }

            return DecodeFromHex(hashBytes);
        }


        public static string GetMD5String(string s, Encoding encoding = null)
        {
            // Use input string to calculate MD5 hash
            using (var md5 = MD5.Create())
            {
                var inputBytes = (encoding ?? Encoding.UTF8).GetBytes(s);
                var hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                var sb = new StringBuilder();
                for (var i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }

                return sb.ToString().ToLower();
            }
        }

        /// <summary>
        /// Convert an array of bytes to a string of hex digits
        /// </summary>
        /// <param name="encoded">array of bytes</param>
        /// <returns>String of hex digits</returns>
        public static string DecodeFromHex(byte[] encoded)
        {
            var sb = new StringBuilder();
            foreach (var b in encoded)
            {
                var hex = b.ToString("x2");
                sb.Append(hex);
            }

            return sb.ToString().ToLower();
        }
        #endregion

        #region sha256
        public static string GetSha256String(string rawData, Encoding encoding = null)
        {
            // Create a SHA256
            using (var sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                var bytes = sha256Hash.ComputeHash((encoding ?? Encoding.UTF8).GetBytes(rawData));

                // Convert byte array to a string
                var builder = new StringBuilder();
                for (var i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }

                return builder.ToString();
            }
        }
        #endregion

        #region Rijndael Encryption

        /// <summary>
        /// Encrypt the given text and give the byte array back as a BASE64 string
        /// </summary>
        /// <param name="text" />The text to encrypt
        /// <param name="aesSalt" />The aes salt
        /// <returns>The encrypted text</returns>
        public static string EncodeToAES(string text, string aesKey, string aesSalt)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            if (string.IsNullOrWhiteSpace(aesSalt))
            {
                throw new ArgumentNullException(nameof(aesSalt));
            }

            ICryptoTransform encryptor;
            using (var aesAlg = NewRijndaelManaged(aesKey, aesSalt))
            {
                encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
            }

            var msEncrypt = new MemoryStream();
            using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
            using (var swEncrypt = new StreamWriter(csEncrypt))
            {
                swEncrypt.Write(text);
            }

            return Convert.ToBase64String(msEncrypt.ToArray());
        }
        #endregion

        #region Rijndael Dycryption
        /// <summary>
        /// Checks if a string is base64 encoded
        /// </summary>
        /// <param name="encoded" />The base64 encoded string
        /// <returns>Base64 encoded stringt</returns>
        public static bool IsBase64String(string encoded)
        {
            encoded = (encoded ?? "").Trim();
            return 0 == encoded.Length % 4 && Regex.IsMatch(encoded, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);
        }

        /// <summary>
        /// Decrypts the given text
        /// </summary>
        /// <param name="encoded" />The encrypted BASE64 text
        /// <param name="aseSalt" />The pasword salt
        /// <returns>The decrypted text</returns>
        public static string DecodeFromAES(string encoded, string aesKey, string aseSalt)
        {
            if (string.IsNullOrWhiteSpace(encoded))
            {
                throw new ArgumentNullException(nameof(encoded));
            }

            if (string.IsNullOrWhiteSpace(aseSalt))
            {
                throw new ArgumentNullException(nameof(aseSalt));
            }

            if (false == IsBase64String(encoded))
            {
                throw new Exception("The cipherText input parameter is not base64 encoded. ");
            }

            string text;
            ICryptoTransform decryptor;
            using (var aesAlg = NewRijndaelManaged(aesKey, aseSalt))
            {
                decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
            }

            var cipher = Convert.FromBase64String(encoded);
            using (var msDecrypt = new MemoryStream(cipher))
            {
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (var srDecrypt = new StreamReader(csDecrypt))
                    {
                        text = srDecrypt.ReadToEnd();
                    }
                }
            }

            return text;
        }
        #endregion

        /// <summary>
        /// Create a new RijndaelManaged class and initialize it
        /// </summary>
        /// <param name="aesSalt" />The text salt
        /// <returns></returns>
        public static RijndaelManaged NewRijndaelManaged(string aesKey, string aesSalt)
        {
            if (string.IsNullOrWhiteSpace(aesSalt))
            {
                throw new ArgumentNullException(nameof(aesSalt));
            }

            var saltBytes = Encoding.ASCII.GetBytes(aesSalt);
            RijndaelManaged aesAlg;
            using (var key = new Rfc2898DeriveBytes(aesKey, saltBytes))
            {
                aesAlg = new RijndaelManaged();
                aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
                aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);
            }

            return aesAlg;
        }

        public static void FromXmlString(this RSACryptoServiceProvider rsa, string xmlString)
        {
            var parameters = new RSAParameters();
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            if (xmlDoc.DocumentElement.Name.Equals("RSAKeyValue"))
            {
                foreach (XmlNode node in xmlDoc.DocumentElement.ChildNodes)
                {
                    switch (node.Name)
                    {
                        case "Modulus": parameters.Modulus = Convert.FromBase64String(node.InnerText); break;
                        case "Exponent": parameters.Exponent = Convert.FromBase64String(node.InnerText); break;
                        case "P": parameters.P = Convert.FromBase64String(node.InnerText); break;
                        case "Q": parameters.Q = Convert.FromBase64String(node.InnerText); break;
                        case "DP": parameters.DP = Convert.FromBase64String(node.InnerText); break;
                        case "DQ": parameters.DQ = Convert.FromBase64String(node.InnerText); break;
                        case "InverseQ": parameters.InverseQ = Convert.FromBase64String(node.InnerText); break;
                        case "D": parameters.D = Convert.FromBase64String(node.InnerText); break;
                    }
                }
            }
            else
            {
                throw new Exception("Invalid XML RSA key. ");
            }

            rsa.ImportParameters(parameters);
        }

        public static string ToXmlString(this RSACryptoServiceProvider rsa)
        {
            var parameters = rsa.ExportParameters(true);

            return string.Format("<RSAKeyValue><Modulus>{0}</Modulus><Exponent>{1}</Exponent><P>{2}</P><Q>{3}</Q><DP>{4}</DP><DQ>{5}</DQ><InverseQ>{6}</InverseQ><D>{7}</D></RSAKeyValue>",
                Convert.ToBase64String(parameters.Modulus),
                Convert.ToBase64String(parameters.Exponent),
                Convert.ToBase64String(parameters.P),
                Convert.ToBase64String(parameters.Q),
                Convert.ToBase64String(parameters.DP),
                Convert.ToBase64String(parameters.DQ),
                Convert.ToBase64String(parameters.InverseQ),
                Convert.ToBase64String(parameters.D));
        }

        public static string GetHashNumberString(string s, int len = 10)
        {
            var bytes = Encoding.UTF8.GetBytes(s);
            string iValue;

            if (len <= 10)
            {
                iValue = BitConverter.ToInt32(bytes, 0).ToString().Replace('-', '1');
            }
            else
            {
                iValue = new BigInteger(bytes).ToString().Replace('-', '1');
            }

            return iValue.PadLeft(len, '0').Substring(Math.Max(0, iValue.Length - len));
        }
    }
}
