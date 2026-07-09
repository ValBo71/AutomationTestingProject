using Microsoft.Playwright;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Constants;

namespace AutomationExercise.ApiTests.Clients
{
    public class BrandsApiClient : ApiClient
    {
        public BrandsApiClient(IAPIRequestContext request) : base(request)
        {
        }

        public async Task<IAPIResponse> GetAllBrandsAsync()
        {
            return await ExecuteGetAsync(ApiEndpoints.BrandsList);
        }

        public async Task<IAPIResponse> PutToBrandsListAsync()
        {
            return await ExecutePutAsync(ApiEndpoints.BrandsList);
        }
    }
}
