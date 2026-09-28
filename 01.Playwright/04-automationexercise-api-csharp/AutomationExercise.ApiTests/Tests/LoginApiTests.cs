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
        private CreateUserRequest _loginUser = null!;
        private bool _loginUserRegistered;

        /// <summary>
        /// One throwaway account for the whole class, registered with generated data and deleted in
        /// ClassTearDown.
        ///
        /// It replaces a permanent shared account that this setup used to create "if it did not exist".
        /// That check was unreliable as well as leaky: verifyLogin answers 404 "User not found!" for a
        /// wrong password too, so an existing account with a different password looked missing, the
        /// re-registration failed with "Email already exists", and the valid-login test then failed with
        /// nothing pointing at the cause.
        ///
        /// Reuses the Playwright driver instance BaseApiTest.OneTimeSetUp already created (NUnit runs the
        /// base-class OneTimeSetUp before the derived class's), instead of launching a second one.
        /// </summary>
        [OneTimeSetUp]
        public async Task ClassSetUp()
        {
            _loginUser = TestUsers.GenerateRegisterUserRequest();

            await using var context = await NewClassContextAsync();
            var client = new AccountApiClient(context);
            var response = await client.CreateAccountAsync(_loginUser);
            var message = JsonHelper.Deserialize<ApiMessageResponse>(await response.TextAsync());

            if (message?.ResponseCode != 201)
            {
                // A OneTimeSetUp that fails does not get its own OneTimeTearDown, so the account - which
                // may exist despite the unexpected answer - is deleted here before failing.
                await AccountCleanup.DeleteAsync(client, _loginUser.Email, _loginUser.Password);
                Assert.Fail($"Registering the login test account {_loginUser.Email} answered responseCode {message?.ResponseCode}.");
            }

            _loginUserRegistered = true;
        }

        [OneTimeTearDown]
        public async Task ClassTearDown()
        {
            if (!_loginUserRegistered) return;

            await using var context = await NewClassContextAsync();
            var problem = await AccountCleanup.DeleteAsync(new AccountApiClient(context), _loginUser.Email, _loginUser.Password);
            if (problem != null)
            {
                Assert.Warn($"The login test account may be left on the site: {problem}");
            }
        }

        private Task<IAPIRequestContext> NewClassContextAsync() =>
            PlaywrightInstance.APIRequest.NewContextAsync(new APIRequestNewContextOptions
            {
                BaseURL = Settings.BaseUrl,
                Timeout = Settings.Api.TimeoutMilliseconds
            });

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
            var response = await _accountClient.VerifyLoginAsync(_loginUser.Email, _loginUser.Password);

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
            var response = await _accountClient.VerifyLoginWithoutEmailAsync(_loginUser.Password);

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
            // Act - a generated address, so no one on the public site can have registered it.
            var response = await _accountClient.VerifyLoginAsync(RandomDataGenerator.GenerateUniqueEmail(), "wrongpass");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var body = await response.TextAsync();
            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(body);

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(404), "API inner responseCode should be 404");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.UserNotFound), "Message should confirm user is not found");
        }

        [Test]
        [AllureName("API 10: POST To Verify Login with a wrong password")]
        [AllureDescription("Verify that a registered email with the wrong password is refused like an unknown user (404), so the API does not reveal which emails are registered.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task VerifyLogin_WithWrongPassword_ShouldReturnUserNotFound()
        {
            // Act
            var response = await _accountClient.VerifyLoginAsync(_loginUser.Email, _loginUser.Password + "_wrong");

            // Assert
            Assert.That(response.Status, Is.EqualTo(200), "HTTP Status code should be 200 OK");

            var messageResponse = JsonHelper.Deserialize<ApiMessageResponse>(await response.TextAsync());

            Assert.That(messageResponse, Is.Not.Null, "Response body should not be null");
            Assert.That(messageResponse!.ResponseCode, Is.EqualTo(404), "A wrong password should be refused with 404");
            Assert.That(messageResponse.Message, Is.EqualTo(ExpectedMessages.UserNotFound), "The message should not reveal that the email exists");
        }
    }
}
