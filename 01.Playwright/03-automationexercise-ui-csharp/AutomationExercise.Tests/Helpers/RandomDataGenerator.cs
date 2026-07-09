using System;

namespace AutomationExercise.Tests.Helpers
{
    public static class RandomDataGenerator
    {
        private static readonly Random _random = new Random();

        public static string GenerateEmail(string prefix = "qa_user")
        {
            return $"{prefix}_{DateTime.Now.Ticks}_{_random.Next(1000, 9999)}@example.com";
        }

        public static string GenerateName(string prefix = "User")
        {
            return $"{prefix}_{_random.Next(1000, 9999)}";
        }
    }
}
