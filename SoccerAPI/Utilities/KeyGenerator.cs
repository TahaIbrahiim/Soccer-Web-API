using System;
using System.Security.Cryptography;

namespace SoccerAPI.Utilities
{
    public static class KeyGenerator
    {
        public static string GenerateKey()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                var key = new byte[32]; // 32 bytes = 256 bits
                rng.GetBytes(key);
                return Convert.ToBase64String(key);
            }
        }
    }
}
