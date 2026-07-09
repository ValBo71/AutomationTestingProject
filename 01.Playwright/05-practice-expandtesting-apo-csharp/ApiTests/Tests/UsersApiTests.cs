using NUnit.Framework;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using System.Threading.Tasks;
using Microsoft.Playwright;
using ApiTests.Base;
using ApiTests.Helpers;
using ApiTests.Models.Responses;
using ApiTests.Constants;

namespace ApiTests.Tests
{
    [TestFixture]
    [AllureSuite("API Tests")]
    [AllureSubSuite("Users")]
    [AllureTag("API", "Playwright", "Users")]
    [AllureOwner("QA Automation")]
    public class UsersApiTests : BaseApiTest
    {
        [Test]
        [AllureName("Register user - Positive Scenario")]
        [AllureDescription("Verify that a new user account can be created successfully with valid data.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task RegisterUser_WithValidData_ShouldCreateAccount()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            // Send registration API request (AllureHelper hooks requests automatically inside ApiClient)
            var registerResponse = await UsersClient.RegisterAsync(name, email, password);

            // Verify response status code is 201 Created
            Assert.That(registerResponse.Status, Is.EqualTo(201));

            // Deserialize and verify response body contents
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(registerResponse);
            Assert.That(body.Success, Is.True);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserCreatedSuccess));

            // Cleanup: Login and delete account so database remains clean
            var loginResponse = await UsersClient.LoginAsync(email, password);
            if (loginResponse.Status == 200)
            {
                var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
                BaseClient.SetToken(loginData.Data.Token);
                await UsersClient.DeleteAccountAsync();
                BaseClient.ClearToken();
            }
        }

        [Test]
        [AllureName("Register user - Duplicate Email Negative Scenario")]
        [AllureDescription("Verify that registering a user with an already existing email returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task RegisterUser_WithDuplicateEmail_ShouldReturnBadRequest()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            // Register first time
            await UsersClient.RegisterAsync(name, email, password);

            // Register second time with same email
            var secondRegisterResponse = await UsersClient.RegisterAsync(name, email, password);

            // Verify status code is 409 Conflict (or 400 Bad Request)
            Assert.That(secondRegisterResponse.Status, Is.EqualTo(409).Or.EqualTo(400));

            // Verify response indicates duplicate error
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(secondRegisterResponse);
            Assert.That(body.Success, Is.False);
            Assert.That(body.Message.ToLower(), Does.Contain("already exists").Or.Contain("taken").Or.Contain("invalid"));

            // Cleanup
            var loginResponse = await UsersClient.LoginAsync(email, password);
            if (loginResponse.Status == 200)
            {
                var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
                BaseClient.SetToken(loginData.Data.Token);
                await UsersClient.DeleteAccountAsync();
                BaseClient.ClearToken();
            }
        }

        [Test]
        [AllureName("Login user - Positive Scenario")]
        [AllureDescription("Verify that an existing user can log in successfully and receive a valid auth token.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task LoginUser_WithValidCredentials_ShouldReturnToken()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            await UsersClient.RegisterAsync(name, email, password);

            // Send login API request
            var loginResponse = await UsersClient.LoginAsync(email, password);

            // Verify response status is 200 OK
            Assert.That(loginResponse.Status, Is.EqualTo(200));

            // Verify response body contains authorization token
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
            Assert.That(loginData.Success, Is.True);
            Assert.That(loginData.Message, Is.EqualTo(ExpectedMessages.LoginSuccess));
            Assert.That(loginData.Data.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(loginData.Data.Email, Is.EqualTo(email));

            // Cleanup
            BaseClient.SetToken(loginData.Data.Token);
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
        }

        [Test]
        [AllureName("Login user - Invalid Credentials Negative Scenario")]
        [AllureDescription("Verify that logging in with incorrect credentials returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task LoginUser_WithInvalidCredentials_ShouldReturnBadRequest()
        {
            var email = "nonexistent_user_expand@abv.bg";
            var password = "WrongPassword";

            // Send login API request with incorrect credentials
            var loginResponse = await UsersClient.LoginAsync(email, password);

            // Verify status code is 400 or 401
            Assert.That(loginResponse.Status, Is.EqualTo(400).Or.EqualTo(401));

            // Verify response indicates authentication failure
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(loginResponse);
            Assert.That(body.Success, Is.False);
        }
    }
}
