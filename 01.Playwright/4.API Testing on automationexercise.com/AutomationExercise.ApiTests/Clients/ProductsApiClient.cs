using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Models.Requests;

namespace AutomationExercise.ApiTests.Clients
{
    public class ProductsApiClient : ApiClient
    {
        public ProductsApiClient(IAPIRequestContext request) : base(request)
        {
        }

        public async Task<IAPIResponse> GetAllProductsAsync()
        {
            return await ExecuteGetAsync(ApiEndpoints.ProductsList);
        }

        public async Task<IAPIResponse> PostToProductsListAsync()
        {
            return await ExecutePostAsync(ApiEndpoints.ProductsList);
        }

        public async Task<IAPIResponse> SearchProductAsync(string? productName)
        {
            var requestModel = new SearchProductRequest { SearchProduct = productName };
            return await ExecutePostAsync(ApiEndpoints.SearchProduct, requestModel.ToFormData());
        }

        public async Task<IAPIResponse> SearchProductWithoutParamAsync()
        {
            return await ExecutePostAsync(ApiEndpoints.SearchProduct);
        }
    }
}
