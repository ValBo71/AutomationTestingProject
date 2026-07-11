using System.Net;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using AutomationExercise.RestSharp.ApiTests.Base;
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
    [AllureSubSuite("Account management")]
    [AllureOwner("QA Automation")]
    [AllureTag("API", "RestSharp", "Account")]
    public class AccountApiTests : BaseApiTest
    {
        private string? _emailToCleanup;
        private readonly string _password = "pass1234";

        [TearDown]
        public async Task CleanupUser()
        {
            if (_emailToCleanup != null)
            {
                await AccountClient.DeleteAccountAsync(_emailToCleanup, _password);
                _emailToCleanup = null;
            }
        }

        [Test]
        [Description("API 11: POST To Create/Register User Account")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task CreateAccount_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var request = new CreateUserRequest
            {
                Name = RandomDataGenerator.GenerateName(),
                Email = email,
                Password = _password,
                FirstName = RandomDataGenerator.GenerateFirstName(),
                LastName = RandomDataGenerator.GenerateLastName(),
                Company = RandomDataGenerator.GenerateCompany(),
                Address1 = RandomDataGenerator.GenerateAddress(),
                Zipcode = RandomDataGenerator.GenerateZipcode(),
                State = RandomDataGenerator.GenerateState(),
                City = RandomDataGenerator.GenerateCity(),
                MobileNumber = RandomDataGenerator.GenerateMobileNumber()
            };

            // Register for cleanup
            _emailToCleanup = email;

            // Act
            var response = await AccountClient.CreateAccountAsync(request);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<ApiMessageResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(201));
            Assert.That(body.Message, Is.EqualTo(ExpectedMessages.UserCreated));
        }

        [Test]
        [Description("API 14: GET User Account Detail by Email")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task GetUserDetailByEmail_WithExistingUser_ShouldReturnUserDetails()
        {
            // Arrange - Create user first
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var createRequest = new CreateUserRequest
            {
                Name = RandomDataGenerator.GenerateName(),
                Email = email,
                Password = _password,
                FirstName = RandomDataGenerator.GenerateFirstName(),
                LastName = RandomDataGenerator.GenerateLastName(),
                Company = RandomDataGenerator.GenerateCompany(),
                Address1 = RandomDataGenerator.GenerateAddress(),
                Zipcode = RandomDataGenerator.GenerateZipcode(),
                State = RandomDataGenerator.GenerateState(),
                City = RandomDataGenerator.GenerateCity(),
                MobileNumber = RandomDataGenerator.GenerateMobileNumber()
            };
            _emailToCleanup = email;
            await AccountClient.CreateAccountAsync(createRequest);

            // Act - Fetch details
            var response = await AccountClient.GetUserDetailByEmailAsync(email);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var body = ResponseHelper.Deserialize<UserAccountResponse>(response);
            Assert.That(body.ResponseCode, Is.EqualTo(200));
            Assert.That(body.User, Is.Not.Null);
            Assert.That(body.User.Email, Is.EqualTo(email));
            Assert.That(body.User.Name, Is.EqualTo(createRequest.Name));
        }

        [Test]
        [Description("API 13: PUT METHOD To Update User Account")]
        [AllureSeverity(SeverityLevel.critical)]
        public async Task UpdateAccount_WithValidData_ShouldUpdateUser()
        {
            // Arrange - Create user first
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var createRequest = new CreateUserRequest
            {
                Name = RandomDataGenerator.GenerateName(),
                Email = email,
                Password = _password,
                FirstName = RandomDataGenerator.GenerateFirstName(),
                LastName = RandomDataGenerator.GenerateLastName(),
                Company = RandomDataGenerator.GenerateCompany(),
                Address1 = RandomDataGenerator.GenerateAddress(),
                Zipcode = RandomDataGenerator.GenerateZipcode(),
                State = RandomDataGenerator.GenerateState(),
                City = RandomDataGenerator.GenerateCity(),
                MobileNumber = RandomDataGenerator.GenerateMobileNumber()
            };
            _emailToCleanup = email;
            await AccountClient.CreateAccountAsync(createRequest);

            // Act - Update user details
            var updateRequest = new UpdateUserRequest
            {
                Name = createRequest.Name + "_Updated",
                Email = email,
                Password = _password,
                FirstName = createRequest.FirstName + "Updated",
                LastName = createRequest.LastName + "Updated",
                Company = createRequest.Company + "Updated",
                Address1 = createRequest.Address1 + "Updated",
                Zipcode = createRequest.Zipcode,
                State = createRequest.State,
                City = createRequest.City,
                MobileNumber = createRequest.MobileNumber
            };

            var updateResponse = await AccountClient.UpdateAccountAsync(updateRequest);
            Assert.That(updateResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var updateBody = ResponseHelper.Deserialize<ApiMessageResponse>(updateResponse);
            Assert.That(updateBody.ResponseCode, Is.EqualTo(200));
            Assert.That(updateBody.Message, Is.EqualTo(ExpectedMessages.UserUpdated));

            // Verify update with GET details
            var getResponse = await AccountClient.GetUserDetailByEmailAsync(email);
            var getBody = ResponseHelper.Deserialize<UserAccountResponse>(getResponse);
            Assert.That(getBody.User.Name, Is.EqualTo(updateRequest.Name));
            Assert.That(getBody.User.FirstName, Is.EqualTo(updateRequest.FirstName));
        }

        [Test]
        [Description("API 12: DELETE METHOD To Delete User Account")]
        [AllureSeverity(SeverityLevel.blocker)]
        public async Task DeleteAccount_WithValidCredentials_ShouldDeleteUser()
        {
            // Arrange - Create user first
            var email = RandomDataGenerator.GenerateUniqueEmail();
            var createRequest = new CreateUserRequest
            {
                Name = RandomDataGenerator.GenerateName(),
                Email = email,
                Password = _password,
                FirstName = RandomDataGenerator.GenerateFirstName(),
                LastName = RandomDataGenerator.GenerateLastName(),
                Company = RandomDataGenerator.GenerateCompany(),
                Address1 = RandomDataGenerator.GenerateAddress(),
                Zipcode = RandomDataGenerator.GenerateZipcode(),
                State = RandomDataGenerator.GenerateState(),
                City = RandomDataGenerator.GenerateCity(),
                MobileNumber = RandomDataGenerator.GenerateMobileNumber()
            };
            _emailToCleanup = email;
            await AccountClient.CreateAccountAsync(createRequest);

            // Act - Delete user
            var deleteResponse = await AccountClient.DeleteAccountAsync(email, _password);

            // Assert
            Assert.That(deleteResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            var deleteBody = ResponseHelper.Deserialize<ApiMessageResponse>(deleteResponse);
            Assert.That(deleteBody.ResponseCode, Is.EqualTo(200));
            Assert.That(deleteBody.Message, Is.EqualTo(ExpectedMessages.AccountDeleted));
            _emailToCleanup = null;

            // Verify account is gone
            var getResponse = await AccountClient.GetUserDetailByEmailAsync(email);
            var getBody = ResponseHelper.Deserialize<ApiMessageResponse>(getResponse);
            Assert.That(getBody.ResponseCode, Is.EqualTo(404));
            Assert.That(getBody.Message, Is.EqualTo(ExpectedMessages.AccountNotFound));
        }
    }
}
