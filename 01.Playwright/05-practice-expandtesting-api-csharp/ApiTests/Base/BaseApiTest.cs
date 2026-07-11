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

        /// <summary>
        /// Set right after a test registers a user account on the shared live sandbox, and cleared once
        /// the test's own delete-account cleanup succeeds. If a test fails/throws in between, TearDown
        /// uses these to attempt a best-effort deletion so failed runs don't leak accounts.
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
                ExtraHTTPHeaders = new Dictionary<string, string>
                {
                    { "Accept", "application/json" }
                }
            });

            // Initialize Client Layer
            BaseClient = new ApiClient(RequestContext);
            UsersClient = new UsersApiClient(BaseClient);
            NotesClient = new NotesApiClient(BaseClient);
        }

        [TearDown]
        public async Task TearDown()
        {
            if (PendingCleanupEmail != null && PendingCleanupPassword != null)
            {
                try
                {
                    var loginResponse = await UsersClient.LoginAsync(PendingCleanupEmail, PendingCleanupPassword);
                    if (loginResponse.Status == 200)
                    {
                        var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
                        BaseClient.SetToken(loginData.Data.Token);
                        await UsersClient.DeleteAccountAsync();
                        BaseClient.ClearToken();
                    }
                }
                catch (Exception ex)
                {
                    TestContext.Out.WriteLine($"[WARN] TearDown safety-net account cleanup failed (account may already be gone): {ex.Message}");
                }
                finally
                {
                    PendingCleanupEmail = null;
                    PendingCleanupPassword = null;
                }
            }

            if (RequestContext != null)
            {
                await RequestContext.DisposeAsync();
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            PlaywrightInstance?.Dispose();
        }
    }
}
