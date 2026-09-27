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
    /// <summary>
    /// Every account these tests create is registered through <see cref="BaseApiTest.RegisterTestUserAsync"/>
    /// (or tracked with <see cref="BaseApiTest.TrackForCleanup"/>) and deleted by the base TearDown, so
    /// no test carries its own cleanup block. The delete endpoint itself is tested once, on purpose, in
    /// <see cref="DeleteAccount_WithValidToken_ShouldRemoveAccount"/>.
    /// </summary>
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

            var registerResponse = await UsersClient.RegisterAsync(name, email, DefaultPassword);
            TrackForCleanup(email, DefaultPassword);

            Assert.That(registerResponse.Status, Is.EqualTo(201));

            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(registerResponse);
            Assert.That(body.Success, Is.True);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserCreatedSuccess));
        }

        [Test]
        [AllureName("Register user - Duplicate Email Negative Scenario")]
        [AllureDescription("Verify that registering a user with an already existing email returns 409 Conflict.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task RegisterUser_WithDuplicateEmail_ShouldReturnConflict()
        {
            // The first registration is asserted inside the helper - otherwise a refused first attempt
            // would turn the second one into an ordinary registration and the test would prove nothing.
            var user = await RegisterTestUserAsync();

            var secondRegisterResponse = await UsersClient.RegisterAsync(user.Name, user.Email, user.Password);

            Assert.That(secondRegisterResponse.Status, Is.EqualTo(409));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(secondRegisterResponse);
            Assert.That(body.Success, Is.False);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.DuplicateEmail));
        }

        [Test]
        [AllureName("Register user - Missing Name Negative Scenario")]
        [AllureDescription("Verify that registering a user without a name returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task RegisterUser_WithoutName_ShouldReturnBadRequest()
        {
            var email = RandomDataGenerator.GenerateUniqueEmail();

            var response = await UsersClient.RegisterAsync("", email, DefaultPassword);
            // Tracked although the registration should be refused: if the API ever accepts a blank
            // name - the very regression this test is here to catch - the account is still removed.
            TrackForCleanup(email, DefaultPassword);

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
            var user = await RegisterTestUserAsync();

            var loginResponse = await UsersClient.LoginAsync(user.Email, user.Password);

            Assert.That(loginResponse.Status, Is.EqualTo(200));

            var loginData = await ResponseHelper.DeserializeAsync<LoginResponse>(loginResponse);
            Assert.That(loginData.Success, Is.True);
            Assert.That(loginData.Message, Is.EqualTo(ExpectedMessages.LoginSuccess));
            Assert.That(loginData.Data.Token, Is.Not.Null.And.Not.Empty);
            Assert.That(loginData.Data.Email, Is.EqualTo(user.Email));
        }

        [Test]
        [AllureName("Login user - Invalid Credentials Negative Scenario")]
        [AllureDescription("Verify that logging in with credentials no account has returns 401 Unauthorized.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task LoginUser_WithInvalidCredentials_ShouldReturnUnauthorized()
        {
            // Generated rather than a fixed address: on a public sandbox anyone could register a
            // hard-coded "nonexistent" email, and this test would start failing for no code change.
            var email = RandomDataGenerator.GenerateUniqueEmail();

            var loginResponse = await UsersClient.LoginAsync(email, "WrongPassword");

            Assert.That(loginResponse.Status, Is.EqualTo(401));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(loginResponse);
            Assert.That(body.Success, Is.False);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.IncorrectCredentials));
        }

        [Test]
        [AllureName("Get Profile - Positive Scenario")]
        [AllureDescription("Verify that an authenticated user can retrieve their profile details successfully.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetProfile_WithValidToken_ShouldReturnProfile()
        {
            var user = await RegisterTestUserAsync();
            await LoginAsync(user.Email, user.Password);

            var profileResponse = await UsersClient.GetProfileAsync();
            Assert.That(profileResponse.Status, Is.EqualTo(200));

            var profile = await ResponseHelper.DeserializeAsync<ProfileResponse>(profileResponse);
            Assert.That(profile.Success, Is.True);
            Assert.That(profile.Data.Name, Is.EqualTo(user.Name));
            Assert.That(profile.Data.Email, Is.EqualTo(user.Email));
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
            var user = await RegisterTestUserAsync();
            await LoginAsync(user.Email, user.Password);

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
        }

        [Test]
        [AllureName("Change Password - Positive Scenario")]
        [AllureDescription("Verify that a changed password replaces the old one: the new password logs in, the old one no longer does.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task ChangePassword_WithValidCredentials_ShouldChangePassword()
        {
            var user = await RegisterTestUserAsync();
            var newPassword = "NewPassword123!";
            await LoginAsync(user.Email, user.Password);

            var changePassResponse = await UsersClient.ChangePasswordAsync(user.Password, newPassword);
            Assert.That(changePassResponse.Status, Is.EqualTo(200));
            UpdateCleanupPassword(newPassword);

            var changePassResult = await ResponseHelper.DeserializeAsync<GenericResponse>(changePassResponse);
            Assert.That(changePassResult.Success, Is.True);
            Assert.That(changePassResult.Message, Is.EqualTo(ExpectedMessages.PasswordUpdatedSuccess));

            BaseClient.ClearToken();

            var newPasswordLogin = await UsersClient.LoginAsync(user.Email, newPassword);
            Assert.That(newPasswordLogin.Status, Is.EqualTo(200), "the new password should log in");

            // The half that proves the password was *changed* rather than a second one added.
            var oldPasswordLogin = await UsersClient.LoginAsync(user.Email, user.Password);
            Assert.That(oldPasswordLogin.Status, Is.EqualTo(401), "the old password should be refused");
        }

        [Test]
        [AllureName("Change Password - Wrong Password Negative Scenario")]
        [AllureDescription("Verify that changing password with incorrect current password returns 400 Bad Request.")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task ChangePassword_WithInvalidCurrentPassword_ShouldReturnBadRequest()
        {
            var user = await RegisterTestUserAsync();
            await LoginAsync(user.Email, user.Password);

            var changePassResponse = await UsersClient.ChangePasswordAsync("WrongPassword", "NewPassword123");
            Assert.That(changePassResponse.Status, Is.EqualTo(400));

            var changePassResult = await ResponseHelper.DeserializeAsync<GenericResponse>(changePassResponse);
            Assert.That(changePassResult.Success, Is.False);
        }

        [Test]
        [AllureName("Logout - Positive Scenario")]
        [AllureDescription("Verify that logging out invalidates the token: the same token is refused afterwards.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task Logout_WithValidToken_ShouldInvalidateToken()
        {
            var user = await RegisterTestUserAsync();
            await LoginAsync(user.Email, user.Password);
            Assert.That((await UsersClient.GetProfileAsync()).Status, Is.EqualTo(200), "the token works before logout");

            var logoutResponse = await UsersClient.LogoutAsync();
            Assert.That(logoutResponse.Status, Is.EqualTo(200));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(logoutResponse);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.LogoutSuccess));

            // The client still sends the old token, so this is the actual proof of logout.
            Assert.That((await UsersClient.GetProfileAsync()).Status, Is.EqualTo(401), "the token is refused after logout");
        }

        [Test]
        [AllureName("Delete Account - Positive Scenario")]
        [AllureDescription("Verify that a deleted account can no longer log in.")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task DeleteAccount_WithValidToken_ShouldRemoveAccount()
        {
            var user = await RegisterTestUserAsync();
            await LoginAsync(user.Email, user.Password);

            var deleteResponse = await UsersClient.DeleteAccountAsync();
            Assert.That(deleteResponse.Status, Is.EqualTo(200));
            var body = await ResponseHelper.DeserializeAsync<GenericResponse>(deleteResponse);
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.AccountDeletedSuccess));

            BaseClient.ClearToken();
            var loginAfterDelete = await UsersClient.LoginAsync(user.Email, user.Password);
            Assert.That(loginAfterDelete.Status, Is.EqualTo(401), "a deleted account should not log in");

            ForgetCleanup();
        }
    }
}
