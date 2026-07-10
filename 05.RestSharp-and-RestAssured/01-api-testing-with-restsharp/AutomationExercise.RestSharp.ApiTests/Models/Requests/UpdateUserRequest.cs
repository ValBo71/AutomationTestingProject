namespace AutomationExercise.RestSharp.ApiTests.Models.Requests
{
    public class UpdateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Title { get; set; } = "Mr";
        public string BirthDate { get; set; } = "10";
        public string BirthMonth { get; set; } = "5";
        public string BirthYear { get; set; } = "1990";
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Country { get; set; } = "Canada";
        public string Zipcode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
    }
}
