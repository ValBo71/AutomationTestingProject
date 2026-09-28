using System;

namespace AutomationExercise.Tests.Helpers
{
    public static class RandomDataGenerator
    {
        /// <summary>
        /// Every generated address is handed to AccountCleanup, so TearDown can delete the account if the
        /// test registered one and did not get as far as deleting it.
        /// </summary>
        public static string GenerateEmail(string prefix = "qa_user")
        {
            var unique = Guid.NewGuid().ToString("N").Substring(0, 10);
            var email = $"{prefix}_{unique}_{Random.Shared.Next(1000, 9999)}@example.com";
            Infrastructure.AccountCleanup.Track(email);
            return email;
        }

        public static string GenerateName(string prefix = "User")
        {
            var unique = Guid.NewGuid().ToString("N").Substring(0, 8);
            return $"{prefix}_{unique}_{Random.Shared.Next(1000, 9999)}";
        }
    }
}
