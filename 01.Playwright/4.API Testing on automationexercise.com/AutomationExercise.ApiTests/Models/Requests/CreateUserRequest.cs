using System.Collections.Generic;

namespace AutomationExercise.ApiTests.Models.Requests
{
    public class CreateUserRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
        public string BirthMonth { get; set; } = string.Empty;
        public string BirthYear { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string Address2 { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Zipcode { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;

        public Dictionary<string, object> ToFormData()
        {
            return new Dictionary<string, object>
            {
                { "name", Name },
                { "email", Email },
                { "password", Password },
                { "title", Title },
                { "birth_date", BirthDate },
                { "birth_month", BirthMonth },
                { "birth_year", BirthYear },
                { "firstname", FirstName },
                { "lastname", LastName },
                { "company", Company },
                { "address1", Address1 },
                { "address2", Address2 },
                { "country", Country },
                { "zipcode", Zipcode },
                { "state", State },
                { "city", City },
                { "mobile_number", MobileNumber }
            };
        }
    }
}
