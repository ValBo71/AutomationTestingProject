using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using System.Threading.Tasks;
using Microsoft.Playwright;
using AutomationExercise.ApiTests.Base;
using AutomationExercise.ApiTests.Clients;
using AutomationExercise.ApiTests.Constants;
using AutomationExercise.ApiTests.Helpers;
using AutomationExercise.ApiTests.TestData;
using AutomationExercise.ApiTests.Models.Requests;
using AutomationExercise.ApiTests.Models.Responses;

namespace AutomationExercise.ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Account & Auth")]
    [AllureFeature("Authentication Operations")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "Integration", "Auth")]
    public class LoginApiTests : BaseApiTest
    {
        private AccountApiClient _accountClient = null!;

        [OneTimeSetUp]
        public async Task ClassSetUp()
        {
            // Check if default user exists, if not, create it.
            using var playwright = await Playwright.CreateAsync();
            await using var context = await playwright.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = Settings.BaseUrl
            });
            var client = new AccountApiClient(context);
            var response = await client.VerifyLoginAsync(Settings.DefaultUser.Email, Settings.DefaultUser.Password);
            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);
            
            if (messageResponse != null && messageResponse.ResponseCode == 404)
            {
                var request = new CreateUserRequest
                {
                    Name = "Valentin Bogdanov",
                    Email = Settings.DefaultUser.Email,
                    Password = Settings.DefaultUser.Password,
                    Title = "Mr",
                    BirthDate = "31",
                    BirthMonth = "May",
                    BirthYear = "1990",
                    FirstName = "Valentin",
                    LastName = "Bogdanov",
                    Company = "QA",
                    Address1 = "Sofia",
                    Address2 = "",
                    Country = "Canada",
                    Zipcode = "1000",
                    State = "Sofia",
                    City = "Sofia",
                    MobileNumber = "1234567890"
                };
                await client.CreateAccountAsync(request);
            }
        }

        [SetUp]
        public void TestSetUp()
        {
            _accountClient = new AccountApiClient(RequestContext);
        }

        [Test]
        [AllureName("API 7: POST To Verify Login with valid details")]
        [AllureDescription("Verify that POST request to /api/verifyLogin with valid details returns 200 response code and success message.")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task VerifyLogin_WithValidDetails_ShouldReturnSuccess()
        {
            // Act
            var response = await _accountClient.VerifyLoginAsync(TestUsers.DefaultEmail, TestUsers.DefaultPassword);

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(200), "API inner responseCode should be 200");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.UserExists), "Message should confirm user exists");
        }

        [Test]
        [AllureName("API 8: POST To Verify Login without email parameter")]
        [AllureDescription("Verify that POST request to /api/verifyLogin without email parameter returns 400 response code.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task VerifyLogin_WithoutEmail_ShouldReturnBadRequest()
        {
            // Act
            var response = await _accountClient.VerifyLoginWithoutEmailAsync(TestUsers.DefaultPassword);

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(400), "API inner responseCode should be 400");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.EmailMissing), "Error message should match expectation");
        }

        [Test]
        [AllureName("API 9: DELETE To Verify Login")]
        [AllureDescription("Verify that DELETE request to /api/verifyLogin is not supported and returns 405 response code.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task DeleteToVerifyLogin_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await _accountClient.DeleteToVerifyLoginAsync();

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(405), "API inner responseCode should be 405");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.MethodNotSupported), "Message should match expectation");
        }

        [Test]
        [AllureName("API 10: POST To Verify Login with invalid details")]
        [AllureDescription("Verify that POST request to /api/verifyLogin with invalid details returns 404 response code.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task VerifyLogin_WithInvalidDetails_ShouldReturnUserNotFound()
        {
            // Act
            var response = await _accountClient.VerifyLoginAsync("nonexistentuser123@invalid.com", "wrongpass");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(404), "API inner responseCode should be 404");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.UserNotFound), "Message should confirm user is not found");
        }
    }
}
