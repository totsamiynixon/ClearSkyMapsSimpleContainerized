using System;
using System.Security.Cryptography;

namespace Web.Helpers
{
    public static class CryptoHelper
    {
        public static string GenerateApiKey()
        {
            var key = new byte[32];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(key);
            return Convert.ToBase64String(key);
        }
    }
}
