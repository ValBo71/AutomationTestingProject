using Microsoft.Playwright;
using System.Collections.Generic;
using System.Threading.Tasks;
using ApiTests.Constants;

namespace ApiTests.Clients
{
    public class UsersApiClient
    {
        private readonly ApiClient _apiClient;

        public UsersApiClient(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<IAPIResponse> RegisterAsync(string name, string email, string password)
        {
            var form = new Dictionary<string, object>
            {
                { "name", name },
                { "email", email },
                { "password", password }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersRegister, form);
        }

        public async Task<IAPIResponse> LoginAsync(string email, string password)
        {
            var form = new Dictionary<string, object>
            {
                { "email", email },
                { "password", password }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersLogin, form);
        }

        public async Task<IAPIResponse> GetProfileAsync()
        {
            return await _apiClient.GetAsync(ApiEndpoints.UsersProfile);
        }

        public async Task<IAPIResponse> UpdateProfileAsync(string name, string? phone = null, string? company = null)
        {
            var form = new Dictionary<string, object>
            {
                { "name", name }
            };
            if (phone != null) form.Add("phone", phone);
            if (company != null) form.Add("company", company);

            return await _apiClient.PatchFormAsync(ApiEndpoints.UsersProfile, form);
        }

        public async Task<IAPIResponse> ForgotPasswordAsync(string email)
        {
            var form = new Dictionary<string, object>
            {
                { "email", email }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersForgotPassword, form);
        }

        public async Task<IAPIResponse> VerifyResetPasswordTokenAsync(string token)
        {
            var form = new Dictionary<string, object>
            {
                { "token", token }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersVerifyResetPasswordToken, form);
        }

        public async Task<IAPIResponse> ResetPasswordAsync(string token, string newPassword)
        {
            var form = new Dictionary<string, object>
            {
                { "token", token },
                { "newPassword", newPassword }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersResetPassword, form);
        }

        public async Task<IAPIResponse> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var form = new Dictionary<string, object>
            {
                { "currentPassword", currentPassword },
                { "newPassword", newPassword }
            };
            return await _apiClient.PostFormAsync(ApiEndpoints.UsersChangePassword, form);
        }

        public async Task<IAPIResponse> LogoutAsync()
        {
            return await _apiClient.DeleteAsync(ApiEndpoints.UsersLogout);
        }

        public async Task<IAPIResponse> DeleteAccountAsync()
        {
            return await _apiClient.DeleteAsync(ApiEndpoints.UsersDeleteAccount);
        }
    }
}
