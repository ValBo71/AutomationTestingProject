using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutomationExercise.ApiTests.Models.Responses
{
    public class ProductsListResponse
    {
        [JsonPropertyName("responseCode")]
        public int ResponseCode { get; set; }

        [JsonPropertyName("products")]
        public List<ProductModel> Products { get; set; } = new();
    }

    public class ProductModel
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
        public ProductCategoryModel Category { get; set; } = new();
    }

    public class ProductCategoryModel
    {
        [JsonPropertyName("usertype")]
        public ProductUserTypeModel Usertype { get; set; } = new();

        [JsonPropertyName("category")]
        public string Category { get; set; } = string.Empty;
    }

    public class ProductUserTypeModel
    {
        [JsonPropertyName("usertype")]
        public string Usertype { get; set; } = string.Empty;
    }
}
