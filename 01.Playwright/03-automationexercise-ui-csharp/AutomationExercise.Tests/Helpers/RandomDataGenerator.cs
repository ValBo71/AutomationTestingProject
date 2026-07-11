using System;

namespace AutomationExercise.Tests.Helpers
{
    public static class RandomDataGenerator
    {
        public static string GenerateEmail(string prefix = "qa_user")
        {
            var unique = Guid.NewGuid().ToString("N").Substring(0, 10);
            return $"{prefix}_{unique}_{Random.Shared.Next(1000, 9999)}@example.com";
        }

        public static string GenerateName(string prefix = "User")
        {
            var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{prefix}_{unique}_{Random.Shared.Next(1000, 9999)}";
        }
    }
}
