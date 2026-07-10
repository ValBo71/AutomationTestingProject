using System.Text.Json.Serialization;

namespace AutomationExercise.RestSharp.ApiTests.Models.Responses
{
    public class ApiMessageResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;
    }
}
