using System.Collections.Generic;

namespace AutomationExercise.ApiTests.Models.Requests
{
    public class SearchProductRequest
    {
        public string? SearchProduct { get; set; }

        public Dictionary<string, object> ToFormData()
        {
            var data = new Dictionary<string, object>();
            if (SearchProduct != null) data.Add("search_product", SearchProduct);
            return data;
        }
    }
}
