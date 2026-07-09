using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Models.Requests;

namespace AutomationExercise.ApiTests.Clients
{
    public class AccountApiClient : ApiClient
    {
        public AccountApiClient(IAPIRequestContext request) : base(request)
        {
        }

        public async Task<IAPIResponse> VerifyLoginAsync(string? email, string? password)
        {
            var request = new LoginRequest { Email = email, Password = password };
            return await ExecutePostAsync(ApiEndpoints.VerifyLogin, request.ToFormData());
        }

        public async Task<IAPIResponse> VerifyLoginWithoutEmailAsync(string? password)
        {
            var request = new LoginRequest { Password = password };
            return await ExecutePostAsync(ApiEndpoints.VerifyLogin, request.ToFormData());
        }

        public async Task<IAPIResponse> DeleteToVerifyLoginAsync()
        {
            return await ExecuteDeleteAsync(ApiEndpoints.VerifyLogin);
        }

        public async Task<IAPIResponse> CreateAccountAsync(CreateUserRequest request)
        {
            return await ExecutePostAsync(ApiEndpoints.CreateAccount, request.ToFormData());
        }

        public async Task<IAPIResponse> UpdateAccountAsync(UpdateUserRequest request)
        {
            return await ExecutePutAsync(ApiEndpoints.UpdateAccount, request.ToFormData());
        }

        public async Task<IAPIResponse> DeleteAccountAsync(string email, string password)
        {
            var data = new Dictionary<string, object>
            {
                { "email", email },
                { "password", password }
            };
            return await ExecuteDeleteAsync(ApiEndpoints.DeleteAccount, data);
        }

        public async Task<IAPIResponse> GetUserDetailByEmailAsync(string email)
        {
            var query = new Dictionary<string, object>
            {
                { "email", email }
            };
            return await ExecuteGetAsync(ApiEndpoints.GetUserDetailByEmail, query);
        }
    }
}
