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

            var registerResponse = await UsersClient.RegisterAsync(name, email, password);
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;

            Assert.That(registerResponse.Status, Is.EqualTo(201));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(registerResponse);
            Assert.That(body.Success, Is.True);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserCreatedSuccess));

            // Cleanup
            var loginResponse = await UsersClient.LoginAsync(email, password);
            if (loginResponse.Status == 200)
            {
                var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
                BaseClient.SetToken(loginData.Data.Token);
                await UsersClient.DeleteAccountAsync();
                BaseClient.ClearToken();
                PendingCleanupEmail = null;
                PendingCleanupPassword = null;
            }
        }

        [Test]
        [AllureName("Register user - Duplicate Email Negative Scenario")]
        [AllureDescription("Verify that registering a user with an already existing email returns 409 Conflict.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task RegisterUser_WithDuplicateEmail_ShouldReturnConflict()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            await UsersClient.RegisterAsync(name, email, password);
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;

            var secondRegisterResponse = await UsersClient.RegisterAsync(name, email, password);

            Assert.That(secondRegisterResponse.Status, Is.EqualTo(409).Or.EqualTo(400));

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
                PendingCleanupEmail = null;
                PendingCleanupPassword = null;
            }
        }

        [Test]
        [AllureName("Register user - Missing Name Negative Scenario")]
        [AllureDescription("Verify that registering a user without a name returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task RegisterUser_WithoutName_ShouldReturnBadRequest()
        {
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            var response = await UsersClient.RegisterAsync("", email, password);

            Assert.That(response.Status, Is.EqualTo(400));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(response);
            Assert.That(body.Success, Is.False);
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
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;

            var loginResponse = await UsersClient.LoginAsync(email, password);

            Assert.That(loginResponse.Status, Is.EqualTo(200));

            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
            Assert.That(loginData.Success, Is.True);
            Assert.That(loginData.Message, Is.EqualTo(ExpectedMessages.LoginSuccess));
            Assert.That(loginData.Data.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(loginData.Data.Email, Is.EqualTo(email));

            // Cleanup
            BaseClient.SetToken(loginData.Data.Token);
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }

        [Test]
        [AllureName("Login user - Invalid Credentials Negative Scenario")]
        [AllureDescription("Verify that logging in with incorrect credentials returns 400 or 401.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task LoginUser_WithInvalidCredentials_ShouldReturnBadRequest()
        {
            var email = "nonexistent_user_expand@example.com";
            var password = "WrongPassword";

            var loginResponse = await UsersClient.LoginAsync(email, password);

            Assert.That(loginResponse.Status, Is.EqualTo(400).Or.EqualTo(401));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(loginResponse);
            Assert.That(body.Success, Is.False);
        }

        [Test]
        [AllureName("Get Profile - Positive Scenario")]
        [AllureDescription("Verify that an authenticated user can retrieve their profile details successfully.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetProfile_WithValidToken_ShouldReturnProfile()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            await UsersClient.RegisterAsync(name, email, password);
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;
            var loginResponse = await UsersClient.LoginAsync(email, password);
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);

            BaseClient.SetToken(loginData.Data.Token);

            var profileResponse = await UsersClient.GetProfileAsync();
            Assert.That(profileResponse.Status, Is.EqualTo(200));

            var profile = await ResponseHelper.DeserializeAsync<ProfileResponse>(profileResponse);
            Assert.That(profile.Success, Is.True);
            Assert.That(profile.Data.Name, Is.EqualTo(name));
            Assert.That(profile.Data.Email, Is.EqualTo(email));

            // Cleanup
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }

        [Test]
        [AllureName("Get Profile - Unauthorized Negative Scenario")]
        [AllureDescription("Verify that retrieving profile details without a token returns 401 Unauthorized.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetProfile_WithoutToken_ShouldReturnUnauthorized()
        {
            BaseClient.ClearToken();
            var response = await UsersClient.GetProfileAsync();
            Assert.That(response.Status, Is.EqualTo(401));
        }

        [Test]
        [AllureName("Update Profile - Positive Scenario")]
        [AllureDescription("Verify that an authenticated user can update their profile information successfully.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task UpdateProfile_WithValidData_ShouldUpdateProfile()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            await UsersClient.RegisterAsync(name, email, password);
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;
            var loginResponse = await UsersClient.LoginAsync(email, password);
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);

            BaseClient.SetToken(loginData.Data.Token);

            var newName = "Updated Name";
            var phone = "1234567890";
            var company = "Expand QA Inc";

            var updateResponse = await UsersClient.UpdateProfileAsync(newName, phone, company);
            Assert.That(updateResponse.Status, Is.EqualTo(200));

            var updatedProfile = await ResponseHelper.DeserializeAsync<ProfileResponse>(updateResponse);
            Assert.That(updatedProfile.Success, Is.True);
            Assert.That(updatedProfile.Message, Is.EqualTo(ExpectedMessages.ProfileUpdatedSuccess));
            Assert.That(updatedProfile.Data.Name, Is.EqualTo(newName));
            Assert.That(updatedProfile.Data.Phone, Is.EqualTo(phone));
            Assert.That(updatedProfile.Data.Company, Is.EqualTo(company));

            // Cleanup
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }

        [Test]
        [AllureName("Change Password - Positive Scenario")]
        [AllureDescription("Verify that an authenticated user can change their password successfully.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task ChangePassword_WithValidCredentials_ShouldChangePassword()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var currentPassword = "Password123";
            var newPassword = "NewPassword123!";

            await UsersClient.RegisterAsync(name, email, currentPassword);
            PendingCleanupEmail = email;
            PendingCleanupPassword = currentPassword;
            var loginResponse = await UsersClient.LoginAsync(email, currentPassword);
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);

            BaseClient.SetToken(loginData.Data.Token);

            var changePassResponse = await UsersClient.ChangePasswordAsync(currentPassword, newPassword);
            Assert.That(changePassResponse.Status, Is.EqualTo(200));
            PendingCleanupPassword = newPassword;

            var changePassResult = await ResponseHelper.DeserializeAsync<GenericResponse>(changePassResponse);
            Assert.That(changePassResult.Success, Is.True);

            // Log in again with the new password to verify the password is changed successfully
            BaseClient.ClearToken();
            var secondLoginResponse = await UsersClient.LoginAsync(email, newPassword);
            Assert.That(secondLoginResponse.Status, Is.EqualTo(200));

            var secondLoginData = await ResponseHelper.DeserializeAsync<LoginResponse>(secondLoginResponse);
            BaseClient.SetToken(secondLoginData.Data.Token);

            // Cleanup
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }

        [Test]
        [AllureName("Change Password - Wrong Password Negative Scenario")]
        [AllureDescription("Verify that changing password with incorrect current password returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task ChangePassword_WithInvalidCurrentPassword_ShouldReturnBadRequest()
        {
            var name = RandomDataGenerator.GenerateRandomString("User", 6);
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = "Password123";

            await UsersClient.RegisterAsync(name, email, password);
            PendingCleanupEmail = email;
            PendingCleanupPassword = password;
            var loginResponse = await UsersClient.LoginAsync(email, password);
            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);

            BaseClient.SetToken(loginData.Data.Token);

            var changePassResponse = await UsersClient.ChangePasswordAsync("WrongPassword", "NewPassword123");
            Assert.That(changePassResponse.Status, Is.EqualTo(400));

            var changePassResult = await ResponseHelper.DeserializeAsync<GenericResponse>(changePassResponse);
            Assert.That(changePassResult.Success, Is.False);

            // Cleanup
            await UsersClient.DeleteAccountAsync();
            BaseClient.ClearToken();
            PendingCleanupEmail = null;
            PendingCleanupPassword = null;
        }
    }
}
