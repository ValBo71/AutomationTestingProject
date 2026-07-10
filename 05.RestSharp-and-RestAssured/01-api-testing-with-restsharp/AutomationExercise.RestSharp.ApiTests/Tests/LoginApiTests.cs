using System.Net;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using AutomationExercise.RestSharp.ApiTests.Base;
using AutomationExercise.RestSharp.ApiTests.Config;
using AutomationExercise.RestSharp.ApiTests.Constants;
using AutomationExercise.RestSharp.ApiTests.Helpers;
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
        [Test]
        [Description("API 7: POST To Verify Login with valid details")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task VerifyLogin_WithValidDetails_ShouldReturnUserExists()
        {
            // Act
            var response = await AccountClient.VerifyLoginAsync(TestSettings.DefaultEmail, TestSettings.DefaultPassword);

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
            var response = await AccountClient.VerifyLoginWithoutEmailAsync(TestSettings.DefaultPassword);

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
            var response = await AccountClient.VerifyLoginWithInvalidDetailsAsync("invalid_email_test@notfound.com", "wrong_password");

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(404));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserNotFound));
        }
    }
}
