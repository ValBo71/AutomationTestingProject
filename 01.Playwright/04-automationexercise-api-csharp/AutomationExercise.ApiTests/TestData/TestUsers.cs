using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.Models.Requests;

namespace AutomationExercise.ApiTests.TestData
{
    public static class TestUsers
    {
        public static readonly string DefaultEmail = System.Environment.GetEnvironmentVariable("AUTOMATION_USER_EMAIL") ?? "testuser_api@example.com";
        public static readonly string DefaultPassword = System.Environment.GetEnvironmentVariable("AUTOMATION_USER_PASSWORD") ?? "placeholder_password";

        public static CreateUserRequest GenerateRegisterUserRequest()
        {
            var uniqueEmail = RandomDataGenerator.GenerateUniqueEmail();
            var name = RandomDataGenerator.GenerateRandomString("User");
            return new CreateUserRequest
            {
                Name = name,
                Email = uniqueEmail,
                Password = "SecretPassword123",
                Title = "Mr",
                BirthDate = "15",
                BirthMonth = "May",
                BirthYear = "1990",
                FirstName = RandomDataGenerator.GenerateRandomString("First"),
                LastName = RandomDataGenerator.GenerateRandomString("Last"),
                Company = RandomDataGenerator.GenerateRandomString("Company"),
                Address1 = "123 Main Street",
                Address2 = "Apt 4B",
                Country = "Canada",
                Zipcode = "M4B 1B3",
                State = "Ontario",
                City = "Toronto",
                MobileNumber = RandomDataGenerator.GenerateRandomNumberString(10)
            };
        }
    }
}
