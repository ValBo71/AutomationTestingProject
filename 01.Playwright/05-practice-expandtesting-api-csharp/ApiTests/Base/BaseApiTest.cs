using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using Allure.NUnit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApiTests.Config;
using ApiTests.Clients;
using ApiTests.Helpers;
using ApiTests.Models.Responses;

namespace ApiTests.Base
{
    [AllureNUnit]
    public class BaseApiTest
    {
        protected static IPlaywright PlaywrightInstance = null!;
        protected IAPIRequestContext RequestContext = null!;
        protected TestSettings Settings = null!;

        // Clients
        protected ApiClient BaseClient = null!;
        protected UsersApiClient UsersClient = null!;
        protected NotesApiClient NotesClient = null!;

        protected const string DefaultPassword = "Password123";

        /// <summary>
        /// The account this test created on the shared live sandbox, if any. Set by
        /// <see cref="RegisterTestUserAsync"/> the moment the register request returns - before its
        /// status is even asserted - and deleted by TearDown whatever happened in between: a failed
        /// assertion, a cleared token, a changed password (see <see cref="UpdateCleanupPassword"/>).
        /// Only a test that deletes the account itself, on purpose, calls <see cref="ForgetCleanup"/>.
        /// </summary>
        protected string? PendingCleanupEmail;
        protected string? PendingCleanupPassword;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("Config/appsettings.local.json", optional: true, reloadOnChange: true)
                .Build();

            Settings = new TestSettings
            {
                BaseUrl = config["BaseUrl"] ?? "https://practice.expandtesting.com/notes/api/",
                Api = new ApiSettings
                {
                    TimeoutMilliseconds = int.TryParse(config["Api:TimeoutMilliseconds"], out var ms) ? ms : 30000
                },
                Authentication = new AuthenticationSettings
                {
                    Type = config["Authentication:Type"] ?? "ApiKey",
                    HeaderName = config["Authentication:HeaderName"] ?? "x-auth-token"
                }
            };

            PlaywrightInstance = await Playwright.CreateAsync();
        }

        [SetUp]
        public async Task SetUp()
        {
            RequestContext = await PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = Settings.BaseUrl,
                Timeout = Settings.Api.TimeoutMilliseconds,
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    { "Accept", "application/json" }
                }
            });

            // Initialize Client Layer
            BaseClient = new ApiClient(RequestContext, Settings.Authentication.HeaderName);
            UsersClient = new UsersApiClient(BaseClient);
            NotesClient = new NotesApiClient(BaseClient);
        }

        /// <summary>
        /// Registers a fresh account and hands it to TearDown for deletion before asserting anything,
        /// so an unexpected status here can never leave the account behind.
        /// </summary>
        protected async Task<(string Name, string Email, string Password)> RegisterTestUserAsync(
            string? name = null, string password = DefaultPassword)
        {
            name ??= RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();

            var response = await UsersClient.RegisterAsync(name, email, password);
            TrackForCleanup(email, password);

            Assert.That(response.Status, Is.EqualTo(201), $"registering the test user {email}");
            return (name, email, password);
        }

        /// <summary>Logs in, asserts it worked, and puts the token on the client.</summary>
        protected async Task<string> LoginAsync(string email, string password)
        {
            var response = await UsersClient.LoginAsync(email, password);
            Assert.That(response.Status, Is.EqualTo(200), $"logging in as {email}");

            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(response);
            BaseClient.SetToken(loginData.Data.Token);
            return loginData.Data.Token;
        }

        /// <summary>
        /// For a test that registers an account without going through <see cref="RegisterTestUserAsync"/>,
        /// typically because the registration is expected to fail. Should it succeed after all, the
        /// account is still deleted.
        /// </summary>
        protected void TrackForCleanup(string email, string password)
        {
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;
        }

        /// <summary>After a password change, so TearDown logs in with the password that now works.</summary>
        protected void UpdateCleanupPassword(string newPassword) => PendingCleanupPassword = newPassword;

        /// <summary>Only for a test that has itself deleted the account and verified the deletion.</summary>
        protected void ForgetCleanup()
        {
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }

        [TearDown]
        public async Task TearDown()
        {
            if (PendingCleanupEmail != null && PendingCleanupPassword != null)
            {
                string? problem;
                try
                {
                    problem = await DeleteAccountAsync(PendingCleanupEmail, PendingCleanupPassword);
                }
                catch (Exception ex)
                {
                    problem = ex.Message;
                }
                finally
                {
                    ForgetCleanup();
                }

                // A warning rather than a failure: the test's own verdict stands, but a leaked
                // account on a public sandbox shows up in the results instead of in a log nobody reads.
                if (problem != null)
                {
                    Assert.Warn($"Test account was not cleaned up and may be left on the sandbox: {problem}");
                }
            }

            if (RequestContext != null)
            {
                await RequestContext.DisposeAsync();
            }
        }

        /// <summary>
        /// Logs in afresh - the test may have cleared or replaced its token - and deletes the account.
        /// Returns null when the account is gone, or a description of what went wrong.
        /// </summary>
        private async Task<string?> DeleteAccountAsync(string email, string password)
        {
            var loginResponse = await UsersClient.LoginAsync(email, password);
            if (loginResponse.Status == 401)
            {
                // "Incorrect email address or password": nothing to delete - most often because the
                // registration itself was refused, as the negative registration tests expect.
                return null;
            }
            if (loginResponse.Status != 200)
            {
                return $"login as {email} answered {loginResponse.Status}";
            }

            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
            BaseClient.SetToken(loginData.Data.Token);
            try
            {
                var deleteResponse = await UsersClient.DeleteAccountAsync();
                return deleteResponse.Status == 200
                    ? null
                    : $"delete-account for {email} answered {deleteResponse.Status}";
            }
            finally
            {
                BaseClient.ClearToken();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PlaywrightInstance?.Dispose();
        }
    }
}
