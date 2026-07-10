using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutomationExercise.RestSharp.ApiTests.Models.Responses
{
    public class ProductsListResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("products")]
        public List<Product> Products { get; set; } = new();
    }

    public class Product
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public string Price { get; set; } = string.Empty;

        [JsonPropertyName("brand")]
        public string Brand { get; set; } = string.Empty;

        [JsonPropertyName("category")]
        public ProductCategory Category { get; set; } = new();
    }

    public class ProductCategory
    {
        [JsonPropertyName("usertype")]
        public UserType Usertype { get; set; } = new();

        [JsonPropertyName("category")]
        public string CategoryName { get; set; } = string.Empty;
    }

    public class UserType
    {
        [JsonPropertyName("usertype")]
        public string Name { get; set; } = string.Empty;
    }
}
