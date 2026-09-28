using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.Models.Requests;

namespace AutomationExercise.ApiTests.TestData
{
    /// <summary>
    /// There is no shared "default" account. An earlier version kept one permanent account on the
    /// public site - registered under a real name, with its password in the repo, never deleted, and
    /// readable by anyone through getUserDetailByEmail. Every test that needs an account now
    /// registers one of these with generated data and deletes it when it is done.
    /// </summary>
    public static class TestUsers
    {
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
