using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework;

namespace AutomationExercise.Tests.Infrastructure
{
    /// <summary>
    /// Deletes, through the site's API, every account a test may have registered - whether the test got
    /// to its own "Delete Account" step or not.
    ///
    /// Every email a test generates is recorded here (see RandomDataGenerator.GenerateEmail), keyed by
    /// the running test. BaseTest.TearDown then checks each one: an address that was never registered,
    /// or that the test already deleted, is simply not there; anything still there is deleted with the
    /// run's password and checked again. This replaces a UI-based safety net that only worked while the
    /// browser session was still logged in, and only after a flag the test set once sign-up had finished.
    /// </summary>
    public static class AccountCleanup
    {
        private static readonly ConcurrentDictionary<string, ConcurrentBag<string>> EmailsByTest = new();

        public static void Track(string email)
        {
            var testId = TestContext.CurrentContext.Test.ID;
            EmailsByTest.GetOrAdd(testId, _ => new ConcurrentBag<string>()).Add(email);
        }

        public static IReadOnlyCollection<string> TakeForCurrentTest()
        {
            return EmailsByTest.TryRemove(TestContext.CurrentContext.Test.ID, out var emails)
                ? emails.ToArray()
                : System.Array.Empty<string>();
        }

        /// <returns>null when the account is gone (or never existed), otherwise what went wrong.</returns>
        public static async Task<string?> DeleteIfPresentAsync(IPlaywright playwright, string baseUrl, string email, string password)
        {
            await using var api = await playwright.APIRequest.NewContextAsync(new() { BaseURL = baseUrl });

            if (await ResponseCodeOfDetailsAsync(api, email) == 404)
            {
                return null;
            }

            await api.DeleteAsync("api/deleteAccount", new()
            {
                Form = api.CreateFormData().Set("email", email).Set("password", password)
            });

            // The delete endpoint answers 404 "Account not found!" for a wrong password too, so its own
            // answer proves nothing. Looking the account up again does.
            var after = await ResponseCodeOfDetailsAsync(api, email);
            return after == 404
                ? null
                // The password is included on purpose: it is a generated, single-run value, and without
                // it nobody could delete the leftover account by hand.
                : $"account {email} (password {password}) is still on the site after deleteAccount (details answered {after})";
        }

        private static async Task<int?> ResponseCodeOfDetailsAsync(IAPIRequestContext api, string email)
        {
            var response = await api.GetAsync("api/getUserDetailByEmail", new() { Params = new Dictionary<string, object> { ["email"] = email } });
            try
            {
                using var json = JsonDocument.Parse(await response.TextAsync());
                return json.RootElement.GetProperty("responseCode").GetInt32();
            }
            catch (System.Exception)
            {
                return null;
            }
        }
    }
}
