using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutomationExercise.RestSharp.ApiTests.Models.Responses
{
    public class BrandsListResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("brands")]
        public List<BrandItem> Brands { get; set; } = new();
    }

    public class BrandItem
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("brand")]
        public string BrandName { get; set; } = string.Empty;
    }
}
