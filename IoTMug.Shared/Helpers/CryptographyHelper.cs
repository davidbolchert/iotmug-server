using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using System;

namespace IoTMug.Shared.Helpers
{
    public static class CryptographyHelper
    {
        private const string _PEPPER = "k82SkLkyb?4zv+a^";

        public static byte[] GetHash(string @string)
        {
            using var algorithm = MD5.Create();
            return algorithm.ComputeHash(Encoding.UTF8.GetBytes(@string));
        }

        public static string GetHashString(string @string)
        {
            var stringBuilder = new StringBuilder();
            var hashedString = GetHash(@string + _PEPPER);
            hashedString.ToList().ForEach(b => stringBuilder.Append(b.ToString("X2")));
            return stringBuilder.ToString();
        }
    }
}
