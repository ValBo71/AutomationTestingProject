using System;

namespace ApiTests.Helpers
{
    public static class RandomDataGenerator
    {
        private static readonly Random Rand = new();

        /// <summary>
        /// The timestamp keeps the addresses readable and sortable; the random suffix is what makes
        /// them unique. On its own a millisecond timestamp collides as soon as two runs - two CI jobs,
        /// or a colleague against the same public sandbox - register in the same millisecond.
        /// Twelve hex characters of a GUID, not all 32: the full GUID pushes the local part past the
        /// 64-character limit and the API refuses the address as invalid.
        /// </summary>
        public static string GenerateUniqueEmail()
        {
            return $"testuser+expand_{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid().ToString("N")[..12]}@example.com";
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
