namespace AutomationExercise.Tests.TestData
{
    public class TestUser
    {
        public string Name { get; set; } = "QA Tester";
        public string Email { get; set; } = "qa_tester@example.com";
        public string Password { get; set; } = "Password123!";
        public string FirstName { get; set; } = "John";
        public string LastName { get; set; } = "Doe";
        public string Company { get; set; } = "QA Org";
        public string Address { get; set; } = "123 Test St";
        public string Country { get; set; } = "United States";
        public string State { get; set; } = "California";
        public string City { get; set; } = "Los Angeles";
        public string Zipcode { get; set; } = "90001";
        public string MobileNumber { get; set; } = "1234567890";
    }

    public static class TestUsers
    {
        public static TestUser GetDefaultUser() => new TestUser();
    }
}
