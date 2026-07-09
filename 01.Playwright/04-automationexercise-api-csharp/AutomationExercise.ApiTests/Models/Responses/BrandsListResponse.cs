using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutomationExercise.ApiTests.Models.Responses
{
    public class BrandsListResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("brands")]
        public List<BrandModel> Brands { get; set; } = new();
    }

    public class BrandModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;
    }
}
