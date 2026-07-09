using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using NUnit.Framework;
using Allure.NUnit;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AutomationExercise.ApiTests.Config;

namespace AutomationExercise.ApiTests.Base
{
    [AllureNUnit]
    public class BaseApiTest
    {
        protected static IPlaywright PlaywrightInstance = null!;
        protected IAPIRequestContext RequestContext = null!;
        protected TestSettings Settings = null!;

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            Settings = new TestSettings
            {
                BaseUrl = config["BaseUrl"] ?? "https://automationexercise.com",
                DefaultUser = new DefaultUserCredentials
                {
                    Email = config["DefaultUser:Email"] ?? "vbogdanov@abv.bg",
                    Password = config["DefaultUser:Password"] ?? "valentin"
                },
                Api = new ApiSettings
                {
                    TimeoutMilliseconds = int.TryParse(config["Api:TimeoutMilliseconds"], out var ms) ? ms : 30000
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
