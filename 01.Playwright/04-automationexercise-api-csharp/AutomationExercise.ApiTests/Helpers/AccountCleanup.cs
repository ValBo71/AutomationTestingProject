using System.Threading.Tasks;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Models.Responses;

namespace AutomationExercise.ApiTests.Helpers
{
    /// <summary>
    /// Deletes a test account and says whether it is really gone.
    ///
    /// Neither answer from deleteAccount can be taken at face value: the site replies HTTP 200 for almost
    /// everything and puts the outcome in the body's responseCode, and that responseCode is 404
    /// "Account not found!" both when there is no such account and when the password is wrong - in which
    /// case the account stays. So the result is decided by looking the account up afterwards:
    /// getUserDetailByEmail answers 404 only once the account no longer exists.
    ///
    /// Callers report a non-null result with Assert.Warn, so an account left on the public site shows up in
    /// the test results instead of disappearing silently.
    /// </summary>
    public static class AccountCleanup
    {
        /// <returns>null when the account is gone (or never existed), otherwise a description of what went wrong.</returns>
        public static async Task<string?> DeleteAsync(AccountApiClient client, string email, string password)
        {
            if (await DetailsResponseCodeAsync(client, email) == 404)
            {
                return null;
            }

            var response = await client.DeleteAccountAsync(email, password);
            var body = await response.TextAsync();

            var after = await DetailsResponseCodeAsync(client, email);
            return after == 404
                ? null
                // The password is included on purpose: it is a generated, single-run value, and without it
                // nobody could delete the leftover account by hand.
                : $"account {email} (password {password}) is still on the site; deleteAccount answered HTTP {response.Status}: {body}";
        }

        private static async Task<int?> DetailsResponseCodeAsync(AccountApiClient client, string email)
        {
            var response = await client.GetUserDetailByEmailAsync(email);
            return JsonHelper.Deserialize<ApiMessageResponse>(await response.TextAsync())?.ResponseCode;
        }
    }
}
