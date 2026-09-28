using System;
using System.Text.Json;
using System.Threading.Tasks;
using AutomationExercise.RestSharp.ApiTests.Clients;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Helpers
{
    /// <summary>
    /// Deletes a test account and says whether it is really gone.
    ///
    /// deleteAccount's own answer proves nothing: the HTTP status is 200 regardless, and the body's
    /// responseCode is 404 "Account not found!" both for a missing account and for a wrong password -
    /// in which case the account stays. So the account is looked up afterwards; getUserDetailByEmail
    /// answers 404 only once it no longer exists.
    ///
    /// Never throws or asserts, because it runs in TearDown: a response that is not JSON is reported like
    /// any other problem, through the returned description, which callers pass to Assert.Warn.
    /// </summary>
    public static class AccountCleanup
    {
        /// <returns>null when the account is gone (or never existed), otherwise what went wrong.</returns>
        public static async Task<string?> DeleteAsync(AccountApiClient client, string email, string password)
        {
            try
            {
                if (ResponseCode(await client.GetUserDetailByEmailAsync(email)) == 404)
                {
                    return null;
                }

                var delete = await client.DeleteAccountAsync(email, password);
                var after = ResponseCode(await client.GetUserDetailByEmailAsync(email));
                return after == 404
                    ? null
                    // The password is included on purpose: it is a generated, single-run value, and
                    // without it nobody could delete the leftover account by hand.
                    : $"account {email} (password {password}) is still on the site; deleteAccount answered {delete.Content}";
            }
            catch (Exception ex)
            {
                return $"cleanup of {email} (password {password}) threw {ex.GetType().Name}: {ex.Message}";
            }
        }

        private static int? ResponseCode(RestResponse response)
        {
            try
            {
                using var json = JsonDocument.Parse(response.Content ?? string.Empty);
                return json.RootElement.GetProperty("responseCode").GetInt32();
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
