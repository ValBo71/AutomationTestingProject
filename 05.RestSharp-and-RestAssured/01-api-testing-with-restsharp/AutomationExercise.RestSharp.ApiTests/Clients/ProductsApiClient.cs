using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationExercise.RestSharp.ApiTests.Constants;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Clients
{
    public class ProductsApiClient
    {
        private readonly ApiClient _apiClient;

        public ProductsApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<RestResponse> GetAllProductsAsync()
        {
            return await _apiClient.GetAsync(ApiEndpoints.ProductsList);
        }

        public async Task<RestResponse> PostToProductsListAsync()
        {
            return await _apiClient.PostFormAsync(ApiEndpoints.ProductsList);
        }

        public async Task<RestResponse> SearchProductAsync(string productName)
        {
            var formParams = new Dictionary<string, string>
            {
                { "search_product", productName }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.SearchProduct, formParams);
        }

        public async Task<RestResponse> SearchProductWithoutParameterAsync()
        {
            return await _apiClient.PostFormAsync(ApiEndpoints.SearchProduct);
        }
    }
}
