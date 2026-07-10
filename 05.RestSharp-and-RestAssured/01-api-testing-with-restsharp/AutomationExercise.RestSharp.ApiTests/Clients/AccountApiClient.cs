using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationExercise.RestSharp.ApiTests.Constants;
using AutomationExercise.RestSharp.ApiTests.Models.Requests;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Clients
{
    public class AccountApiClient
    {
        private readonly ApiClient _apiClient;

        public AccountApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<RestResponse> VerifyLoginAsync(string email, string password)
        {
            var formParams = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.VerifyLogin, formParams);
        }

        public async Task<RestResponse> VerifyLoginWithoutEmailAsync(string password)
        {
            var formParams = new Dictionary<string, string>
            {
                { "password", password }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.VerifyLogin, formParams);
        }

        public async Task<RestResponse> DeleteVerifyLoginAsync()
        {
            return await _apiClient.DeleteFormAsync(ApiEndpoints.VerifyLogin);
        }

        public async Task<RestResponse> VerifyLoginWithInvalidDetailsAsync(string email, string password)
        {
            return await VerifyLoginAsync(email, password);
        }

        public async Task<RestResponse> CreateAccountAsync(CreateUserRequest request)
        {
            var formParams = new Dictionary<string, string>
            {
                { "name", request.Name },
                { "email", request.Email },
                { "password", request.Password },
                { "title", request.Title },
                { "birth_date", request.BirthDate },
                { "birth_month", request.BirthMonth },
                { "birth_year", request.BirthYear },
                { "firstname", request.FirstName },
                { "lastname", request.LastName },
                { "company", request.Company },
                { "address1", request.Address1 },
                { "address2", request.Address2 },
                { "country", request.Country },
                { "zipcode", request.Zipcode },
                { "state", request.State },
                { "city", request.City },
                { "mobile_number", request.MobileNumber }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.CreateAccount, formParams);
        }

        public async Task<RestResponse> DeleteAccountAsync(string email, string password)
        {
            var formParams = new Dictionary<string, string>
            {
                { "email", email },
                { "password", password }
            };
            return await _apiClient.DeleteFormAsync(ApiEndpoints.DeleteAccount, formParams);
        }

        public async Task<RestResponse> UpdateAccountAsync(UpdateUserRequest request)
        {
            var formParams = new Dictionary<string, string>
            {
                { "name", request.Name },
                { "email", request.Email },
                { "password", request.Password },
                { "title", request.Title },
                { "birth_date", request.BirthDate },
                { "birth_month", request.BirthMonth },
                { "birth_year", request.BirthYear },
                { "firstname", request.FirstName },
                { "lastname", request.LastName },
                { "company", request.Company },
                { "address1", request.Address1 },
                { "address2", request.Address2 },
                { "country", request.Country },
                { "zipcode", request.Zipcode },
                { "state", request.State },
                { "city", request.City },
                { "mobile_number", request.MobileNumber }
            };
            return await _apiClient.PutFormAsync(ApiEndpoints.UpdateAccount, formParams);
        }

        public async Task<RestResponse> GetUserDetailByEmailAsync(string email)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "email", email }
            };
            return await _apiClient.GetAsync(ApiEndpoints.GetUserDetailByEmail, queryParams);
        }
    }
}
