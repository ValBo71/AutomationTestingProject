using System.Text.Json.Serialization;

namespace AutomationExercise.ApiTests.Models.Responses
{
    public class UserAccountResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("user")]
        public UserDetailsModel User { get; set; } = new();
    }

    public class UserDetailsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("last_name")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("birth_day")]
        public string BirthDay { get; set; } = string.Empty;

        [JsonPropertyName("birth_month")]
        public string BirthMonth { get; set; } = string.Empty;

        [JsonPropertyName("birth_year")]
        public string BirthYear { get; set; } = string.Empty;

        [JsonPropertyName("company")]
        public string Company { get; set; } = string.Empty;

        [JsonPropertyName("address1")]
        public string Address1 { get; set; } = string.Empty;

        [JsonPropertyName("address2")]
        public string Address2 { get; set; } = string.Empty;

        [JsonPropertyName("country")]
        public string Country { get; set; } = string.Empty;

        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;

        [JsonPropertyName("city")]
        public string City { get; set; } = string.Empty;

        [JsonPropertyName("zipcode")]
        public string Zipcode { get; set; } = string.Empty;

        [JsonPropertyName("mobile_number")]
        public string MobileNumber { get; set; } = string.Empty;
    }
}
