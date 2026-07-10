using System;

namespace AutomationExercise.RestSharp.ApiTests.Helpers
{
    public static class RandomDataGenerator
    {
        private static readonly Random Random = new();

        public static string GenerateUniqueEmail()
        {
            return $"vbogdanov+api_{DateTime.UtcNow:yyyyMMddHHmmssfff}@abv.bg";
        }

        public static string GenerateName()
        {
            return $"User_{Random.Next(1000, 9999)}";
        }

        public static string GeneratePassword()
        {
            return $"Pass_{Random.Next(100000, 999999)}";
        }

        public static string GenerateFirstName()
        {
            string[] names = { "John", "Jane", "Alice", "Bob", "Charlie", "David", "Emma", "Fiona" };
            return names[Random.Next(names.Length)];
        }

        public static string GenerateLastName()
        {
            string[] names = { "Smith", "Doe", "Johnson", "Brown", "Taylor", "Miller", "Wilson", "Davis" };
            return names[Random.Next(names.Length)];
        }

        public static string GenerateCompany()
        {
            return $"Company_{Random.Next(100, 999)} LLC";
        }

        public static string GenerateAddress()
        {
            return $"{Random.Next(10, 999)} Main St, Apt {Random.Next(1, 50)}";
        }

        public static string GenerateZipcode()
        {
            return Random.Next(10000, 99999).ToString();
        }

        public static string GenerateState()
        {
            string[] states = { "Ontario", "Quebec", "California", "Texas", "New York" };
            return states[Random.Next(states.Length)];
        }

        public static string GenerateCity()
        {
            string[] cities = { "Toronto", "Montreal", "Los Angeles", "Houston", "New York" };
            return cities[Random.Next(cities.Length)];
        }

        public static string GenerateMobileNumber()
        {
            return $"+1{Random.Next(100, 999)}{Random.Next(100, 999)}{Random.Next(1000, 9999)}";
        }
    }
}
