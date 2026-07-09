using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using Allure.NUnit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ApiTests.Config;
using ApiTests.Clients;

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
