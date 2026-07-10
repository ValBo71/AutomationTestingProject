using System.Threading.Tasks;
using AutomationExercise.RestSharp.ApiTests.Constants;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Clients
{
    public class BrandsApiClient
    {
        private readonly ApiClient _apiClient;

        public BrandsApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<RestResponse> GetAllBrandsAsync()
        {
            return await _apiClient.GetAsync(ApiEndpoints.BrandsList);
        }

        public async Task<RestResponse> PutToBrandsListAsync()
        {
            return await _apiClient.PutFormAsync(ApiEndpoints.BrandsList);
        }
    }
}
