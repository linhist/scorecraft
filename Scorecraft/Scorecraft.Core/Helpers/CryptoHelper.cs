using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Scorecraft.Helpers
{
    public static class CryptoHelper
    {
        private static string _hashChars = "9oiuy3tr4ewq5axdfg6hjk7lmnb2vc1xzs80";
        private static string _cryptoKey = "W5fdgeFr63eF9dHj";
        private static string _oldCode = "";

        static CryptoHelper()
        { }

        public static int RandomNumber(int max = int.MaxValue, int min = 0)
        {
            if (min == max) return min;

            Random rand = new Random();
            if (min > max) return rand.Next(max, min);
            return rand.Next(min, max);
        }

        public static string RandomString(int length = 1, string hashChars = null)
        {
            if (length < 1) return "";

            hashChars = hashChars?.Trim();
            char[] hash = string.IsNullOrEmpty(hashChars) ? _hashChars.ToCharArray() : hashChars.ToCharArray();

            int idx;
            Random rand = new Random();
            StringBuilder sb = new StringBuilder();

            while (sb.Length < length)
            {
                idx = rand.Next(0, hash.Length);
                sb.Append(hash[idx]);
            }

            return sb.ToString();
        }

        public static string RandomCode(int length = 9)
        {
            Guid guid = Guid.NewGuid();
            IEnumerable<char> code = guid.ToString().Replace("-", "").ToCharArray();

            int idx = RandomNumber(code.Count() - length - 1);
            if (idx % 2 == 0)
            {
                code = code.Reverse().ToArray();
            }
            code = code.Skip(idx).Take(length);
            StringBuilder sb = new StringBuilder();
            sb.Append(code);

            string result = sb.ToString();
            if (result != _oldCode)
            {
                _oldCode = result;
                return result;
            }

            return RandomCode();
        }

        public static string Base64Encode(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
        }

        public static string Base64Decode(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(value));
            }
            catch { }

            return "";
        }

        public static bool TryBase64Decode(string value, out string content)
        {
            content = "";
            if (string.IsNullOrEmpty(value)) return false;

            try
            {
                content = Encoding.UTF8.GetString(Convert.FromBase64String(value));
                return true;
            }
            catch { }

            return false;
        }

        public static string MD5Hash(string content)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] res = md5.ComputeHash(Encoding.Unicode.GetBytes(content));
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < res.Length; i++)
                {
                    sb.Append(res[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string Encrypt(string content)
        {
            if (string.IsNullOrEmpty(content)) return "";

            using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
            {
                byte[] desKey = hash.ComputeHash(Encoding.UTF8.GetBytes(_cryptoKey));

                using (TripleDESCryptoServiceProvider desAlg = new TripleDESCryptoServiceProvider())
                {
                    desAlg.Key = desKey;
                    desAlg.Mode = CipherMode.ECB;
                    desAlg.Padding = PaddingMode.PKCS7;

                    byte[] val = Encoding.UTF8.GetBytes(content);
                    using (ICryptoTransform cryptor = desAlg.CreateEncryptor())
                    {
                        return Convert.ToBase64String(cryptor.TransformFinalBlock(val, 0, val.Length));
                    }
                }
            }
        }

        public static string Decrypt(string value)
        {
            if (string.IsNullOrEmpty(value)) return "";

            using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
            {
                byte[] desKey = hash.ComputeHash(Encoding.UTF8.GetBytes(_cryptoKey));

                using (TripleDESCryptoServiceProvider desAlg = new TripleDESCryptoServiceProvider())
                {
                    desAlg.Key = desKey;
                    desAlg.Mode = CipherMode.ECB;
                    desAlg.Padding = PaddingMode.PKCS7;

                    try
                    {
                        byte[] val = Convert.FromBase64String(value);
                        using (ICryptoTransform cryptor = desAlg.CreateDecryptor())
                        {
                            return Encoding.UTF8.GetString(cryptor.TransformFinalBlock(val, 0, val.Length));
                        }
                    }
                    catch 
                    { }
                }
            }

            return "";
        }

        public static bool TryDecrypt(string value, out string content)
        {
            content = "";
            if (string.IsNullOrEmpty(value)) return false;

            using (MD5CryptoServiceProvider hash = new MD5CryptoServiceProvider())
            {
                byte[] desKey = hash.ComputeHash(Encoding.UTF8.GetBytes(_cryptoKey));

                using (TripleDESCryptoServiceProvider desAlg = new TripleDESCryptoServiceProvider())
                {
                    desAlg.Key = desKey;
                    desAlg.Mode = CipherMode.ECB;
                    desAlg.Padding = PaddingMode.PKCS7;

                    try
                    {
                        byte[] val = Convert.FromBase64String(value);
                        using (ICryptoTransform cryptor = desAlg.CreateDecryptor())
                        {
                            content = Encoding.UTF8.GetString(cryptor.TransformFinalBlock(val, 0, val.Length));
                        }
                        return true;
                    }
                    catch
                    { }
                }
            }

            return false;
        }
    }
}
