using System.Net;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using AutomationExercise.RestSharp.ApiTests.Base;
using AutomationExercise.RestSharp.ApiTests.Config;
using AutomationExercise.RestSharp.ApiTests.Constants;
using AutomationExercise.RestSharp.ApiTests.Helpers;
using AutomationExercise.RestSharp.ApiTests.Models.Requests;
using AutomationExercise.RestSharp.ApiTests.Models.Responses;
using NUnit.Framework;

namespace AutomationExercise.RestSharp.ApiTests.Tests
{
    [TestFixture]
    [AllureNUnit]
    [AllureSuite("Automation Exercise API")]
    [AllureSubSuite("Login verification")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "RestSharp", "Login")]
    public class LoginApiTests : BaseApiTest
    {
        private string? _emailToCleanup;
        private string _passwordToCleanup = string.Empty;

        /// <summary>
        /// Deletes the throwaway account the valid-login test registers. There is deliberately no shared
        /// login account: it would need a password stored in the repo, and the permanent one this suite
        /// used to rely on sat on the public site indefinitely, readable by anyone.
        /// </summary>
        [TearDown]
        public async Task CleanupUser()
        {
            if (_emailToCleanup == null) return;

            var response = await AccountClient.DeleteAccountAsync(_emailToCleanup, _passwordToCleanup);
            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            // 404: the registration never went through, so there is nothing to delete.
            if (body.ResponseCode != 200 && body.ResponseCode != 404)
            {
                Assert.Warn($"Test account {_emailToCleanup} may be left on the site: deleteAccount answered {body.ResponseCode} {body.Message}");
            }
            _emailToCleanup = null;
        }

        [Test]
        [Description("API 7: POST To Verify Login with valid details")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task VerifyLogin_WithValidDetails_ShouldReturnUserExists()
        {
            // Arrange - an account of this test's own, with a generated password
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var password = RandomDataGenerator.GeneratePassword();
            _emailToCleanup = email;
            _passwordToCleanup = password;

            var createResponse = await AccountClient.CreateAccountAsync(new CreateUserRequest
            {
                Name = RandomDataGenerator.GenerateName(),
                Email = email,
                Password = password,
                FirstName = RandomDataGenerator.GenerateFirstName(),
                LastName = RandomDataGenerator.GenerateLastName(),
                Company = RandomDataGenerator.GenerateCompany(),
                Address1 = RandomDataGenerator.GenerateAddress(),
                Zipcode = RandomDataGenerator.GenerateZipcode(),
                State = RandomDataGenerator.GenerateState(),
                City = RandomDataGenerator.GenerateCity(),
                MobileNumber = RandomDataGenerator.GenerateMobileNumber()
            });
            Assert.That(ResponseHelper.Deserialize<ApiMessageResponse>(createResponse).ResponseCode, Is.EqualTo(201),
                "registering the login test account");

            // Act
            var response = await AccountClient.VerifyLoginAsync(email, password);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(200));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserExists));
        }

        [Test]
        [Description("API 8: POST To Verify Login without email parameter")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task VerifyLogin_WithoutEmail_ShouldReturnBadRequest()
        {
            // Act
            // Any password will do: the request is refused for the missing email before it is checked.
            var response = await AccountClient.VerifyLoginWithoutEmailAsync(RandomDataGenerator.GeneratePassword());

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(400));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.LoginParametersMissing));
        }

        [Test]
        [Description("API 9: DELETE To Verify Login")]
        [AllureSeverity(SeverityLevel.normal)]
        public async Task DeleteVerifyLogin_ShouldReturnMethodNotSupported()
        {
            // Act
            var response = await AccountClient.DeleteVerifyLoginAsync();

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(405));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.MethodNotSupported));
        }

        [Test]
        [Description("API 10: POST To Verify Login with invalid details")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task VerifyLogin_WithInvalidDetails_ShouldReturnUserNotFound()
        {
            // Act
            var response = await AccountClient.VerifyLoginWithInvalidDetailsAsync(RandomDataGenerator.GenerateUniqueEmail(), "wrong_password");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(404));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserNotFound));
        }
    }
}
