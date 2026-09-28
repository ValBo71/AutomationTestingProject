using System.Threading.Tasks;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Models.Responses;

namespace AutomationExercise.ApiTests.Helpers
{
    /// <summary>
    /// Deletes a test account and says whether it worked.
    ///
    /// The site answers HTTP 200 for almost everything and puts the real outcome in the body's
    /// responseCode, so a failed deletion looks successful to anything that checks the status alone.
    /// Callers report a non-null result with Assert.Warn, so an account left on the public site shows
    /// up in the test results instead of disappearing silently.
    /// </summary>
    public static class AccountCleanup
    {
        /// <returns>null when the account is gone, otherwise a description of what went wrong.</returns>
        public static async Task<string?> DeleteAsync(AccountApiClient client, string email, string password)
        {
            var response = await client.DeleteAccountAsync(email, password);
            var body = await response.TextAsync();
            var message = JsonHelper.Deserialize<ApiMessageResponse>(body);

            // 404 means there is no such account - typically because the registration it was armed for
            // never went through - so there is nothing left to clean up.
            return response.Status == 200 && (message?.ResponseCode == 200 || message?.ResponseCode == 404)
                ? null
                : $"deleteAccount for {email} answered HTTP {response.Status}, responseCode {message?.ResponseCode}: {body}";
        }
    }
}
