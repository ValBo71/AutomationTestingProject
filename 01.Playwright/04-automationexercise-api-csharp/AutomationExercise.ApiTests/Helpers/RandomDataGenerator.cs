using System;

namespace AutomationExercise.ApiTests.Helpers
{
    public static class RandomDataGenerator
    {
        private static readonly Random Rand = new();

        /// <summary>
        /// The timestamp keeps addresses readable; the random suffix is what makes them unique when two
        /// runs (CI jobs, or someone else on the same public site) register in the same millisecond.
        /// </summary>
        public static string GenerateUniqueEmail()
        {
            return $"testuser+api_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N")[..8]}@example.com";
        }

        public static string GenerateRandomString(string prefix, int length = 8)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[Rand.Next(chars.Length)];
            }
            return $"{prefix}_{new string(buffer)}";
        }

        public static string GenerateRandomNumberString(int length = 10)
        {
            const string chars = "0123456789";
            var buffer = new char[length];
            for (int i = 0; i < length; i++)
            {
                buffer[i] = chars[Rand.Next(chars.Length)];
            }
            return new string(buffer);
        }
    }
}
