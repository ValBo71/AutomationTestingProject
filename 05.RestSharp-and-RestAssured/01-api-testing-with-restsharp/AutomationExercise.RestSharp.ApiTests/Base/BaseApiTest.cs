using System;
using Allure.Net.Commons;
using AutomationExercise.RestSharp.ApiTests.Clients;
using AutomationExercise.RestSharp.ApiTests.Config;
using NUnit.Framework;
using RestSharp;

namespace AutomationExercise.RestSharp.ApiTests.Base
{
    public abstract class BaseApiTest
    {
        protected RestClient RestClient { get; private set; } = null!;
        protected ApiClient ApiClient { get; private set; } = null!;
        protected ProductsApiClient ProductsClient { get; private set; } = null!;
        protected BrandsApiClient BrandsClient { get; private set; } = null!;
        protected AccountApiClient AccountClient { get; private set; } = null!;

        [SetUp]
        public void BaseSetUp()
        {
            var options = new RestClientOptions(TestSettings.BaseUrl)
            {
                Timeout = TimeSpan.FromMilliseconds(TestSettings.TimeoutMilliseconds),
                UserAgent = "RestSharp API Test Automation"
            };

            RestClient = new RestClient(options);
            ApiClient = new ApiClient(RestClient);

            ProductsClient = new ProductsApiClient(ApiClient);
            BrandsClient = new BrandsApiClient(ApiClient);
            AccountClient = new AccountApiClient(ApiClient);
        }

        [TearDown]
        public void BaseTearDown()
        {
            // Attach failure details to Allure if the test failed
            if (TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Failed)
            {
                var errorMessage = TestContext.CurrentContext.Result.Message;
                var stackTrace = TestContext.CurrentContext.Result.StackTrace;
                
                var failureInfo = $"Error Message:\n{errorMessage}\n\n" +
                                   $"Stack Trace:\n{stackTrace}";

                AllureApi.AddAttachment("Test Failure Details", "text/plain", System.Text.Encoding.UTF8.GetBytes(failureInfo), ".txt");
            }

            RestClient?.Dispose();
        }
    }
}
